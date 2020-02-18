using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class TelloSdkClient : IDisposable
{
    #region Defaults

    public const int DefaultControlUdpPort = 8889;
    public const int DefaultTelemetryUdpPort = 8890;
    public const string DefaultHostName = "192.168.10.1";
    public const double DefaultStickDataIntervalMilliseconds = 20;
    public const string DefaultRegion = "US";

    #endregion Defaults

    #region Private Fields

    private readonly UdpClient _commandChannel;
    private readonly CancellationTokenSource _cts = new CancellationTokenSource();
    private readonly System.Timers.Timer _stickDataTimer = new System.Timers.Timer();
    private TelloSdkStickData _stickData = new TelloSdkStickData();
    private TelloSdkTelemetry _telemetry;
    private bool _enableMissionMode;

    public TelloSdkStickData StickData
    {
        get => _stickData;
        set => _stickData = value ?? throw new ArgumentNullException(nameof(value));
    }

    #endregion Private Fields

    #region Public Properties
    public byte? BatteryPercent
    {
        get { return _telemetry?.BatteryPercent; }
    }

    public TelloSdkTelemetry Telemetry { get => _telemetry; }

    public string HostName { get; private set; }

    public int ControlUdpPort { get; private set; } = DefaultControlUdpPort;

    public int TelemetryUdpPort { get; private set; } = DefaultTelemetryUdpPort;

    public bool IsDisposed { get; private set; }

    public bool IsStarted { get; private set; }

    public double StickDataIntervalMilliseconds { get; set; } = DefaultStickDataIntervalMilliseconds;

    public bool EnableMissionMode { get => _enableMissionMode;
        set
        {
            if (_enableMissionMode != value)
            {
                if (IsStarted)
                    throw new InvalidOperationException();
                _enableMissionMode = value;
            }
        }
    }

    #endregion Public Properties

    #region Construction & Destruction

    public TelloSdkClient(string hostname = DefaultHostName, int controlUdpPort = DefaultControlUdpPort, int telemetryUdpPort = DefaultTelemetryUdpPort)
    {
        if (hostname == null)
            throw new ArgumentNullException(nameof(hostname));
        if (controlUdpPort <= 0 || controlUdpPort > System.Net.IPEndPoint.MaxPort)
            throw new ArgumentOutOfRangeException(nameof(controlUdpPort), controlUdpPort,
                $"Argument '{nameof(controlUdpPort)}' is out of range.");
        if (telemetryUdpPort <= 0 || telemetryUdpPort > System.Net.IPEndPoint.MaxPort)
            throw new ArgumentOutOfRangeException(nameof(telemetryUdpPort), telemetryUdpPort,
                $"Argument '{nameof(telemetryUdpPort)}' is out of range.");
        HostName = hostname;
        ControlUdpPort = controlUdpPort;
        TelemetryUdpPort = telemetryUdpPort;
        // resolve Tello hostname and determine the local IP address for communicating with it:
        var hostResolver = new UdpClient(hostname, ControlUdpPort);
        var localIPAddress = ((IPEndPoint)hostResolver.Client.LocalEndPoint).Address;
        var telloIPAddress = ((IPEndPoint)hostResolver.Client.RemoteEndPoint).Address;
        hostResolver.Close();
        // create contorl and telemetry channels:
        _commandChannel = new UdpClient() { ExclusiveAddressUse = false };
        //_telemetryChannel = new UdpClient() { ExclusiveAddressUse = false };
        _commandChannel.Client.Bind(new IPEndPoint(localIPAddress, TelemetryUdpPort));
        //_telemetryChannel.Client.Bind(new IPEndPoint(localIPAddress, TelemetryUdpPort));
        var telloUdpPort = ControlUdpPort;
        _commandChannel.Connect(new IPEndPoint(telloIPAddress, telloUdpPort));
        //_telemetryChannel.Connect(new IPEndPoint(telloIPAddress, telloUdpPort));
    }

    ~TelloSdkClient()
    {
        Dispose(false);
    }

    private void Dispose(bool disposing)
    {
        if (!IsDisposed)
        {
            if (_stickDataTimer != null)
                _stickDataTimer.Dispose();
            //if (_telemetryChannel != null)
            //    _telemetryChannel.Dispose();
            if (_commandChannel != null)
                _commandChannel.Dispose();
            if (_cts != null)
                _cts.Dispose();
            IsStarted = false;
            IsDisposed = true;
            if (disposing)
                GC.SuppressFinalize(this);
        }
    }

    #endregion Construction & Destruction

    #region Closed API

    private async Task<int> SendCommandAsync(string command)
    {
        var bytes = Encoding.ASCII.GetBytes(command);
        return await _commandChannel.SendAsync(bytes, bytes.Length);
        //return await ReceiveCommandResultAsync();
    }

    private async Task<string> ReceiveCommandResultAsync()
    {
        var result = await _commandChannel.ReceiveAsync();
        return Encoding.ASCII.GetString(result.Buffer);
    }

    private void TelemetryChannelLoop()
    {
        while (true)
        {
            try
            {
                _cts.Token.ThrowIfCancellationRequested();
                IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Parse("192.168.10.1"), 8889);
                var buffer = _commandChannel.Receive(ref remoteEndPoint);
                var text = Encoding.ASCII.GetString(buffer);
                bool ok = TelloSdkTelemetry.TryParse(text, out _telemetry);
                if (!ok)
                    System.Diagnostics.Debug.Print("Failed to parse Tello telemetry.");
            }
            catch (ObjectDisposedException)
            {
                return;
            }
            catch (ThreadAbortException)
            {
                return;
            }
            catch (OperationCanceledException)
            {
                return;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Print(ex.ToString());
            }
        }
    }

    public async Task<bool> StartAsync()
    {
        var tcs = new TaskCompletionSource<int>();
        using (_cts.Token.Register(s => ((TaskCompletionSource<int>)s).TrySetResult(0), tcs)) {
            // start SDK mode:
            var startSdkTask = SendCommandAsync("command");
            var setMissionMode = SendCommandAsync(
                _enableMissionMode ? "mon" : "moff");
            await Task.WhenAny(startSdkTask, setMissionMode, tcs.Task);
            _cts.Token.ThrowIfCancellationRequested();
            IsStarted = true;
        }
        var telemetryThread = new Thread(TelemetryChannelLoop) 
        {
            IsBackground = true,
            Priority= System.Threading.ThreadPriority.Lowest 
        };
        telemetryThread.Start();
        if (StickDataIntervalMilliseconds > 0)
        {
            _stickDataTimer.Interval = StickDataIntervalMilliseconds;
            _stickDataTimer.Elapsed += StickDataTimer_Elapsed;
            _stickDataTimer.Start();
        }
        return true;
    }

    #endregion Closed API

    #region Event Handlers

    private void StickDataTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
    {
        bool restart = true;
        try
        {
            var stickData = _stickData;
            var stickCommand = $"rc {stickData.Roll} {stickData.Pitch} {stickData.Throttle} {stickData.Yaw}";
            var task = SendCommandAsync(stickCommand);
            task.Wait(_cts.Token);
        }
        catch (ObjectDisposedException)
        {
            restart = false;
            return;
        }
        catch (OperationCanceledException)
        {
            restart = false;
            return;
        }
        catch (AggregateException ex)
        {
            restart = ex.InnerExceptions.Count != 1 ||
                      !(ex.InnerExceptions[0] is ObjectDisposedException);
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.ToString());
        }
        finally
        {
            if (restart)
                _stickDataTimer.Start();
        }
    }

    #endregion Event Handlers

    #region Public Methods

    public void Close()
    {
        try
        {
            _cts.Cancel();
        }
        catch (ObjectDisposedException)
        {
            // suppress exception.
        }
        catch (AggregateException)
        {
            // suppress exception.
        }
        //if (_telemetryChannel != null)
        //    _telemetryChannel.Close();
        if (_commandChannel != null)
            _commandChannel.Close();
        IsStarted = false;
    }
    
    public async Task<bool> TakeOffAsync()
    {
        await SendCommandAsync("takeoff");
        return true;
    }

    public async Task<bool> LandAsync()
    {
        await SendCommandAsync("land");
        return true;
    }

    /// <summary>
    /// Configure the drone to operate as the wifi station.
    /// i.e. the drone will connect to the specified wifi network.
    /// </summary>
    /// <remarks>
    /// This is the swarm mode of the drone.
    /// In this mode only the SDK API can be used.
    /// Video streaming from the drone (streamon) is not supported in this mode.
    /// In order to reset the drone to the default (access-point) mode, press the power
    /// button for five seconds while the drone is turned-on.
    /// </remarks>
    /// <param name="ssid">Drone wifi network SSID</param>
    /// <param name="password">Drone wifi network password</param>
    /// <returns>the number of bytes transmitted.</returns>
    public async Task<int> EnableWifiStationMode(string ssid, string password)
    {
        var command = $"ap {ssid} {password}";
        return await SendCommandAsync(command);
    }

    /// <summary>
    /// Configure the drone to operate as the wifi access-point.
    /// i.e. the drone will create a wifi network of its own to which the phone should connect.
    /// </summary>
    /// <remarks>
    /// This is the default operation mode of the drone.
    /// In this mode both the closed-API and the SDK can be used.
    /// Video streaming from the drone is also supported in this mode.
    /// </remarks>
    /// <param name="ssid">Drone wifi network SSID</param>
    /// <param name="password">Drone wifi network password</param>
    /// <returns>the number of bytes transmitted.</returns>
    public async Task<int> EnableWifiAccessPointMode(string ssid, string password)
    {
        var command = $"ap {ssid} {password}";
        return await SendCommandAsync(command);
    }

    public float MoveSpeedFactor
    {
        get => StickData.MoveSpeedFactor;
        set => StickData.MoveSpeedFactor = value;
    }

    public bool MoveLeft
    {
        get => StickData.MoveLeft;
        set => StickData.MoveLeft = value;
    }

    public bool MoveRight
    {
        get => StickData.MoveRight;
        set => StickData.MoveRight = value;
    }


    public bool MoveForward
    {
        get => StickData.MoveForward;
        set => StickData.MoveForward = value;
    }
    public bool MoveBackward
    {
        get => StickData.MoveBackward;
        set => StickData.MoveBackward = value;
    }

    public bool MoveDown
    {
        get => StickData.MoveDown;
        set => StickData.MoveDown = value;
    }

    public bool MoveUp
    {
        get => StickData.MoveUp;
        set => StickData.MoveUp = value;
    }

    public bool TurnLeft
    {
        get => StickData.TurnLeft;
        set => StickData.TurnLeft = value;
    }

    public bool TurnRight
    {
        get => StickData.TurnRight;
        set => StickData.TurnRight = value;
    }

    public bool HoldingPosition
    {
        get => StickData.HoldingPosition;
    }

    public void HoldPosition()
    {
        StickData.HoldPosition();
    }

    #region IDisposable Members

    public void Dispose()
    {
        Dispose(true);
    }

    #endregion IDisposable Members

    #endregion Public Methods
}
