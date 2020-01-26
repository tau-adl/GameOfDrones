using System;
using System.ComponentModel;

namespace UnityEngine.Networking
{
    /// <summary>
    /// An extension for the NetworkManager that displays a default HUD for controlling the network state of the game.
    /// <para>This component also shows useful internal state for the networking system in the inspector window of the editor. It allows users to view connections, networked objects, message handlers, and packet statistics. This information can be helpful when debugging networked games.</para>
    /// </summary>
    [AddComponentMenu("Network/NetworkManagerHUD")]
    [RequireComponent(typeof(NetworkManager))]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("The high level API classes are deprecated and will be removed in the future.")]
    public class NetworkManagerHUD : MonoBehaviour
    {
        /// <summary>
        /// The NetworkManager associated with this HUD.
        /// </summary>
        public NetworkManager manager;
        /// <summary>
        /// Whether to show the default control HUD at runtime.
        /// </summary>
        [SerializeField] public bool showGUI = true;
        /// <summary>
        /// The horizontal offset in pixels to draw the HUD runtime GUI at.
        /// </summary>
        [SerializeField] public int offsetX;
        /// <summary>
        /// The vertical offset in pixels to draw the HUD runtime GUI at.
        /// </summary>
        [SerializeField] public int offsetY;

        // Runtime variable
        bool m_ShowServer;

        // factor
        Vector2 resolution;
        float factor_x, factor_y;
        int fact_xpos, fact_ypos, fact_xsize, fact_ysize;

        void Awake()
        {
            manager = GetComponent<NetworkManager>();
            resolution = new Vector2(Screen.width, Screen.height);
            factor_x = (resolution.x / 1000);
            factor_y = (resolution.y / 500);
        }

        void Update()
        {
            if (resolution.x != Screen.width || resolution.y != Screen.height)
            {
                resolution = new Vector2(Screen.width, Screen.height);
                factor_x = (resolution.x / 800);
                factor_y = (resolution.y / 600);
            }

            if (!showGUI)
                return;

            if (!manager.IsClientConnected() && !NetworkServer.active && manager.matchMaker == null)
            {
                if (UnityEngine.Application.platform != RuntimePlatform.WebGLPlayer)
                {
                    if (Input.GetKeyDown(KeyCode.S))
                    {
                        manager.StartServer();
                    }
                    if (Input.GetKeyDown(KeyCode.H))
                    {
                        manager.StartHost();
                    }
                }
                if (Input.GetKeyDown(KeyCode.C))
                {
                    manager.StartClient();
                }
            }
            if (NetworkServer.active)
            {
                if (manager.IsClientConnected())
                {
                    if (Input.GetKeyDown(KeyCode.X))
                    {
                        manager.StopHost();
                    }
                }
                else
                {
                    if (Input.GetKeyDown(KeyCode.X))
                    {
                        manager.StopServer();
                    }
                }
            }
        }

        void OnGUI()
        {
            if (!showGUI)
                return;

            int xpos = 10 + offsetX;
            int ypos = 10 + offsetY;
            const int spacing = 24;

            bool noConnection = (manager.client == null || manager.client.connection == null ||
                manager.client.connection.connectionId == -1);

            if (!manager.IsClientConnected() && !NetworkServer.active && manager.matchMaker == null)
            {
                if (noConnection)
                {
                    if (UnityEngine.Application.platform != RuntimePlatform.WebGLPlayer)
                    {
                        fact_xpos = (int)(factor_x * xpos);
                        fact_ypos = (int)(factor_y * ypos);
                        fact_xsize = (int)(factor_x * 200);
                        fact_ysize = (int)(factor_y * 20);
                        if (GUI.Button(new Rect(fact_xpos, fact_ypos, fact_xsize, fact_ysize), "LAN Host(H)"))
                        {
                            manager.StartHost();
                        }
                        ypos += spacing;
                    }

                    fact_xpos = (int)(factor_x * xpos);
                    fact_ypos = (int)(factor_y * ypos);
                    fact_xsize = (int)(factor_x * 105);
                    fact_ysize = (int)(factor_y * 20);
                    if (GUI.Button(new Rect(fact_xpos, fact_ypos, fact_xsize, fact_ysize), "LAN Client(C)"))
                    {
                        manager.StartClient();
                    }

                    fact_xpos = (int)(factor_x * (xpos + 100));
                    fact_ypos = (int)(factor_y * ypos);
                    fact_xsize = (int)(factor_x * 95);
                    fact_ysize = (int)(factor_y * 20);
                    manager.networkAddress = GUI.TextField(new Rect(fact_xpos, fact_ypos, fact_xsize, fact_ysize), manager.networkAddress);
                    ypos += spacing;

                    if (UnityEngine.Application.platform == RuntimePlatform.WebGLPlayer)
                    {
                        // cant be a server in webgl build
                        fact_xpos = (int)(factor_x * xpos);
                        fact_ypos = (int)(factor_y * ypos);
                        fact_xsize = (int)(factor_x * 200);
                        fact_ysize = (int)(factor_y * 25);
                        GUI.Box(new Rect(fact_xpos, fact_ypos, fact_xsize, fact_ysize), "(  WebGL cannot be server  )");
                        ypos += spacing;
                    }
                    else
                    {
                        fact_xpos = (int)(factor_x * xpos);
                        fact_ypos = (int)(factor_y * ypos);
                        fact_xsize = (int)(factor_x * 200);
                        fact_ysize = (int)(factor_y * 20);
                        if (GUI.Button(new Rect(fact_xpos, fact_ypos, fact_xsize, fact_ysize), "LAN Server Only(S)"))
                        {
                            manager.StartServer();
                        }
                        ypos += spacing;
                    }
                }
                else
                {
                    fact_xpos = (int)(factor_x * xpos);
                    fact_ypos = (int)(factor_y * ypos);
                    fact_xsize = (int)(factor_x * 200);
                    fact_ysize = (int)(factor_y * 20);
                    GUI.Label(new Rect(fact_xpos, fact_ypos, fact_xsize, fact_ysize), "Connecting to " + manager.networkAddress + ":" + manager.networkPort + "..");
                    ypos += spacing;


                    fact_xpos = (int)(factor_x * xpos);
                    fact_ypos = (int)(factor_y * ypos);
                    fact_xsize = (int)(factor_x * 200);
                    fact_ysize = (int)(factor_y * 20);
                    if (GUI.Button(new Rect(fact_xpos, fact_ypos, fact_xsize, fact_ysize), "Cancel Connection Attempt"))
                    {
                        manager.StopClient();
                    }
                }
            }
            else
            {
                if (NetworkServer.active)
                {
                    string serverMsg = "Server: port=" + manager.networkPort;
                    if (manager.useWebSockets)
                    {
                        serverMsg += " (Using WebSockets)";
                    }
                    fact_xpos = (int)(factor_x * xpos);
                    fact_ypos = (int)(factor_y * ypos);
                    fact_xsize = (int)(factor_x * 200);
                    fact_ysize = (int)(factor_y * 20);
                    GUI.Label(new Rect(fact_xpos, fact_ypos, fact_xsize, fact_ysize), serverMsg);
                    ypos += spacing;
                }
                if (manager.IsClientConnected())
                {
                    fact_xpos = (int)(factor_x * xpos);
                    fact_ypos = (int)(factor_y * ypos);
                    fact_xsize = (int)(factor_x * 300);
                    fact_ysize = (int)(factor_y * 20);
                    GUI.Label(new Rect(fact_xpos, fact_ypos, fact_xsize, fact_ysize), "Client: address=" + manager.networkAddress + " port=" + manager.networkPort);
                    ypos += spacing;
                }
            }

            if (manager.IsClientConnected() && !ClientScene.ready)
            {
                fact_xpos = (int)(factor_x * xpos);
                fact_ypos = (int)(factor_y * ypos);
                fact_xsize = (int)(factor_x * 200);
                fact_ysize = (int)(factor_y * 20);
                if (GUI.Button(new Rect(fact_xpos, fact_ypos, fact_xsize, fact_ysize), "Client Ready"))
                {
                    ClientScene.Ready(manager.client.connection);

                    if (ClientScene.localPlayers.Count == 0)
                    {
                        ClientScene.AddPlayer(0);
                    }
                }
                ypos += spacing;
            }

            if (NetworkServer.active || manager.IsClientConnected())
            {
                fact_xpos = (int)(factor_x * xpos);
                fact_ypos = (int)(factor_y * ypos);
                fact_xsize = (int)(factor_x * 200);
                fact_ysize = (int)(factor_y * 20);
                if (GUI.Button(new Rect(fact_xpos, fact_ypos, fact_xsize, fact_ysize), "Stop (X)"))
                {
                    manager.StopHost();
                }
                ypos += spacing;
            }

            if (!NetworkServer.active && !manager.IsClientConnected() && noConnection)
            {
                //ypos += 10;

                if (UnityEngine.Application.platform == RuntimePlatform.WebGLPlayer)
                {
                    fact_xpos = (int)(factor_x * (xpos - 5));
                    fact_ypos = (int)(factor_y * ypos);
                    fact_xsize = (int)(factor_x * 220);
                    fact_ysize = (int)(factor_y * 25);
                    GUI.Box(new Rect(fact_xpos, fact_ypos, fact_xsize, fact_ysize), "(WebGL cannot use Match Maker)");
                    return;
                }

                if (manager.matchMaker == null)
                {
                    fact_xpos = (int)(factor_x * xpos);
                    fact_ypos = (int)(factor_y * ypos);
                    fact_xsize = (int)(factor_x * 200);
                    fact_ysize = (int)(factor_y * 20);
                    if (GUI.Button(new Rect(fact_xpos, fact_ypos, fact_xsize, fact_ysize), "Enable Match Maker (M)"))
                    {
                        manager.StartMatchMaker();
                    }
                    ypos += spacing;
                }
                else
                {
                    if (manager.matchInfo == null)
                    {
                        if (manager.matches == null)
                        {
                            fact_xpos = (int)(factor_x * xpos);
                            fact_ypos = (int)(factor_y * ypos);
                            fact_xsize = (int)(factor_x * 200);
                            fact_ysize = (int)(factor_y * 20);
                            if (GUI.Button(new Rect(fact_xpos, fact_ypos, fact_xsize, fact_ysize), "Create Internet Match"))
                            {
                                manager.matchMaker.CreateMatch(manager.matchName, manager.matchSize, true, "", "", "", 0, 0, manager.OnMatchCreate);
                            }
                            ypos += spacing;

                            fact_xpos = (int)(factor_x * xpos);
                            fact_ypos = (int)(factor_y * ypos);
                            fact_xsize = (int)(factor_x * 100);
                            fact_ysize = (int)(factor_y * 20);
                            GUI.Label(new Rect(fact_xpos, fact_ypos, fact_xsize, fact_ysize), "Room Name:");
                            fact_xpos = (int)(factor_x * (xpos + 100));
                            manager.matchName = GUI.TextField(new Rect(fact_xpos, fact_ypos, fact_xsize, fact_ysize), manager.matchName);
                            ypos += spacing;

                            //ypos += 10;

                            fact_xpos = (int)(factor_x * xpos);
                            fact_ypos = (int)(factor_y * ypos);
                            fact_xsize = (int)(factor_x * 200);
                            fact_ysize = (int)(factor_y * 20);
                            if (GUI.Button(new Rect(fact_xpos, fact_ypos, fact_xsize, fact_ysize), "Find Internet Match"))
                            {
                                manager.matchMaker.ListMatches(0, 20, "", false, 0, 0, manager.OnMatchList);
                            }
                            ypos += spacing;
                        }
                        else
                        {
                            for (int i = 0; i < manager.matches.Count; i++)
                            {
                                var match = manager.matches[i];
                                fact_xpos = (int)(factor_x * xpos);
                                fact_ypos = (int)(factor_y * ypos);
                                fact_xsize = (int)(factor_x * 200);
                                fact_ysize = (int)(factor_y * 20);
                                if (GUI.Button(new Rect(fact_xpos, fact_ypos, fact_xsize, fact_ysize), "Join Match:" + match.name))
                                {
                                    manager.matchName = match.name;
                                    manager.matchMaker.JoinMatch(match.networkId, "", "", "", 0, 0, manager.OnMatchJoined);
                                }
                                ypos += spacing;
                            }

                            fact_xpos = (int)(factor_x * xpos);
                            fact_ypos = (int)(factor_y * ypos);
                            fact_xsize = (int)(factor_x * 200);
                            fact_ysize = (int)(factor_y * 20);
                            if (GUI.Button(new Rect(fact_xpos, fact_ypos, fact_xsize, fact_ysize), "Back to Match Menu"))
                            {
                                manager.matches = null;
                            }
                            ypos += spacing;
                        }
                    }

                    fact_xpos = (int)(factor_x * xpos);
                    fact_ypos = (int)(factor_y * ypos);
                    fact_xsize = (int)(factor_x * 200);
                    fact_ysize = (int)(factor_y * 20);
                    if (GUI.Button(new Rect(fact_xpos, fact_ypos, fact_xsize, fact_ysize), "Change MM server"))
                    {
                        m_ShowServer = !m_ShowServer;
                    }
                    if (m_ShowServer)
                    {
                        ypos += spacing;
                        fact_xpos = (int)(factor_x * xpos);
                        fact_ypos = (int)(factor_y * ypos);
                        fact_xsize = (int)(factor_x * 100);
                        fact_ysize = (int)(factor_y * 20);
                        if (GUI.Button(new Rect(fact_xpos, fact_ypos, fact_xsize, fact_ysize), "Local"))
                        {
                            manager.SetMatchHost("localhost", 1337, false);
                            m_ShowServer = false;
                        }
                        ypos += spacing;
                        fact_xpos = (int)(factor_x * xpos);
                        fact_ypos = (int)(factor_y * ypos);
                        fact_xsize = (int)(factor_x * 100);
                        fact_ysize = (int)(factor_y * 20);
                        if (GUI.Button(new Rect(fact_xpos, fact_ypos, fact_xsize, fact_ysize), "Internet"))
                        {
                            manager.SetMatchHost("mm.unet.unity3d.com", 443, true);
                            m_ShowServer = false;
                        }
                        ypos += spacing;
                        fact_xpos = (int)(factor_x * xpos);
                        fact_ypos = (int)(factor_y * ypos);
                        fact_xsize = (int)(factor_x * 100);
                        fact_ysize = (int)(factor_y * 20);
                        if (GUI.Button(new Rect(fact_xpos, fact_ypos, fact_xsize, fact_ysize), "Staging"))
                        {
                            manager.SetMatchHost("staging-mm.unet.unity3d.com", 443, true);
                            m_ShowServer = false;
                        }
                    }

                    ypos += spacing;

                    fact_xpos = (int)(factor_x * xpos);
                    fact_ypos = (int)(factor_y * ypos);
                    fact_xsize = (int)(factor_x * 300);
                    fact_ysize = (int)(factor_y * 20);
                    GUI.Label(new Rect(fact_xpos, fact_ypos, fact_xsize, fact_ysize), "MM Uri: " + manager.matchMaker.baseUri);
                    ypos += spacing;

                    fact_xpos = (int)(factor_x * xpos);
                    fact_ypos = (int)(factor_y * ypos);
                    fact_xsize = (int)(factor_x * 200);
                    fact_ysize = (int)(factor_y * 20);
                    if (GUI.Button(new Rect(fact_xpos, fact_ypos, fact_xsize, fact_ysize), "Disable Match Maker"))
                    {
                        manager.StopMatchMaker();
                    }
                    ypos += spacing;
                }
            }
        }
    }
}