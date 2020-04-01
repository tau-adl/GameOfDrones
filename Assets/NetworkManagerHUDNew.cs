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
    public class NetworkManagerHUDNew : MonoBehaviour
    {
        /// <summary>
        /// The NetworkManager associated with this HUD.
        /// </summary>
        public NetworkManager manager;
        public menu menu;
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
        public GUIStyle button_guiStyle;
        public GUIStyle box_guiStyle;
        public GUIStyle label_guiStyle;

        // update showGUI
        public void toggle_showGUI()
        {
            //Debug.Log("NetworkManagerHUDNew, toggle_showGUI");
            if (menu.menu_is_on) { showGUI = true; }
            else                 { showGUI = false; }
        }
        
        void Awake()
        {
            manager = GetComponent<NetworkManager>();
            menu = GameObject.FindObjectOfType<menu>();
            
            // change button size according to resolurion
            resolution = new Vector2(Screen.width, Screen.height);
            factor_x = (resolution.x / 1000);
            factor_y = (resolution.y / 400);

            // set default matchName
            manager.matchName = "GameOfDrones";
        }

        void Update()
        {
            // FOLLOWING CODE IS NOT NEEDED BECAUSE THERE IS ONLY LANDSCAPE MODE
            /*
            if (resolution.x != Screen.width || resolution.y != Screen.height)
            {
                resolution = new Vector2(Screen.width, Screen.height);
                factor_x = (resolution.x / 800);
                factor_y = (resolution.y / 500);
            }
            */
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

            // change font size according to resolution
            box_guiStyle = new GUIStyle(GUI.skin.box);          // copy default box style
            box_guiStyle.fontSize = (int)(12 * factor_y);       // and change only fontSize
            label_guiStyle = new GUIStyle(GUI.skin.label);      // copy default label style
            label_guiStyle.fontSize = (int)(12 * factor_y);     // and change only fontSize
            button_guiStyle = new GUIStyle(GUI.skin.button);    // copy default button style
            button_guiStyle.fontSize = (int) (12*factor_y);     // and change only fontSize

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
                        if (GUI.Button(new Rect(fact_xpos, fact_ypos, fact_xsize, fact_ysize), "LAN Host(H)", button_guiStyle))
                        {
                            manager.StartHost();
                        }
                        ypos += spacing;
                    }

                    fact_xpos = (int)(factor_x * xpos);
                    fact_ypos = (int)(factor_y * ypos);
                    fact_xsize = (int)(factor_x * 105);
                    fact_ysize = (int)(factor_y * 20);
                    if (GUI.Button(new Rect(fact_xpos, fact_ypos, fact_xsize, fact_ysize), "LAN Client(C)", button_guiStyle))
                    {
                        manager.StartClient();
                    }

                    fact_xpos = (int)(factor_x * (xpos + 100));
                    fact_ypos = (int)(factor_y * ypos);
                    fact_xsize = (int)(factor_x * 95);
                    fact_ysize = (int)(factor_y * 20);
                    manager.networkAddress = GUI.TextField(new Rect(fact_xpos, fact_ypos, fact_xsize, fact_ysize), manager.networkAddress, button_guiStyle);
                    ypos += spacing;

                    if (UnityEngine.Application.platform == RuntimePlatform.WebGLPlayer)
                    {
                        // cant be a server in webgl build
                        fact_xpos = (int)(factor_x * xpos);
                        fact_ypos = (int)(factor_y * ypos);
                        fact_xsize = (int)(factor_x * 200);
                        fact_ysize = (int)(factor_y * 25);
                        GUI.Box(new Rect(fact_xpos, fact_ypos, fact_xsize, fact_ysize), "(  WebGL cannot be server  )", box_guiStyle);
                        ypos += spacing;
                    }
                    else
                    {
                        fact_xpos = (int)(factor_x * xpos);
                        fact_ypos = (int)(factor_y * ypos);
                        fact_xsize = (int)(factor_x * 200);
                        fact_ysize = (int)(factor_y * 20);
                        if (GUI.Button(new Rect(fact_xpos, fact_ypos, fact_xsize, fact_ysize), "LAN Server Only(S)", button_guiStyle))
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
                    GUI.Label(new Rect(fact_xpos, fact_ypos, fact_xsize, fact_ysize), "Connecting to " + manager.networkAddress + ":" + manager.networkPort + "..", label_guiStyle);
                    ypos += spacing;


                    fact_xpos = (int)(factor_x * xpos);
                    fact_ypos = (int)(factor_y * ypos);
                    fact_xsize = (int)(factor_x * 200);
                    fact_ysize = (int)(factor_y * 20);
                    if (GUI.Button(new Rect(fact_xpos, fact_ypos, fact_xsize, fact_ysize), "Cancel Connection Attempt", button_guiStyle))
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
                    GUI.Label(new Rect(fact_xpos, fact_ypos, fact_xsize, fact_ysize), serverMsg, label_guiStyle);
                    ypos += spacing;
                }
                if (manager.IsClientConnected())
                {
                    fact_xpos = (int)(factor_x * xpos);
                    fact_ypos = (int)(factor_y * ypos);
                    fact_xsize = (int)(factor_x * 300);
                    fact_ysize = (int)(factor_y * 20);
                    GUI.Label(new Rect(fact_xpos, fact_ypos, fact_xsize, fact_ysize), "Client: address=" + manager.networkAddress + " port=" + manager.networkPort, label_guiStyle);
                    ypos += spacing;
                }
            }

            if (manager.IsClientConnected() && !ClientScene.ready)
            {
                fact_xpos = (int)(factor_x * xpos);
                fact_ypos = (int)(factor_y * ypos);
                fact_xsize = (int)(factor_x * 200);
                fact_ysize = (int)(factor_y * 20);
                if (GUI.Button(new Rect(fact_xpos, fact_ypos, fact_xsize, fact_ysize), "Client Ready", button_guiStyle))
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
                if (GUI.Button(new Rect(fact_xpos, fact_ypos, fact_xsize, fact_ysize), "Stop (X)", button_guiStyle))
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
                    GUI.Box(new Rect(fact_xpos, fact_ypos, fact_xsize, fact_ysize), "(WebGL cannot use Match Maker)", box_guiStyle);
                    return;
                }

                if (manager.matchMaker == null)
                {
                    fact_xpos = (int)(factor_x * xpos);
                    fact_ypos = (int)(factor_y * ypos);
                    fact_xsize = (int)(factor_x * 200);
                    fact_ysize = (int)(factor_y * 20);
                    if (GUI.Button(new Rect(fact_xpos, fact_ypos, fact_xsize, fact_ysize), "Enable Match Maker (M)", button_guiStyle))
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
                            if (GUI.Button(new Rect(fact_xpos, fact_ypos, fact_xsize, fact_ysize), "Create Internet Match", button_guiStyle))
                            {
                                manager.matchMaker.CreateMatch(manager.matchName, manager.matchSize, true, "", "", "", 0, 0, manager.OnMatchCreate);
                            }
                            ypos += spacing;

                            fact_xpos = (int)(factor_x * xpos);
                            fact_ypos = (int)(factor_y * ypos);
                            fact_xsize = (int)(factor_x * 100);
                            fact_ysize = (int)(factor_y * 20);
                            GUI.Label(new Rect(fact_xpos, fact_ypos, fact_xsize, fact_ysize), "Room Name:", label_guiStyle);
                            fact_xpos = (int)(factor_x * (xpos + 100));
                            manager.matchName = GUI.TextField(new Rect(fact_xpos, fact_ypos, fact_xsize, fact_ysize), manager.matchName, button_guiStyle);
                            ypos += spacing;

                            //ypos += 10;

                            fact_xpos = (int)(factor_x * xpos);
                            fact_ypos = (int)(factor_y * ypos);
                            fact_xsize = (int)(factor_x * 200);
                            fact_ysize = (int)(factor_y * 20);
                            if (GUI.Button(new Rect(fact_xpos, fact_ypos, fact_xsize, fact_ysize), "Find Internet Match", button_guiStyle))
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
                                if (GUI.Button(new Rect(fact_xpos, fact_ypos, fact_xsize, fact_ysize), "Join Match:" + match.name, button_guiStyle))
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
                            if (GUI.Button(new Rect(fact_xpos, fact_ypos, fact_xsize, fact_ysize), "Back to Match Menu", button_guiStyle))
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
                    if (GUI.Button(new Rect(fact_xpos, fact_ypos, fact_xsize, fact_ysize), "Change MM server", button_guiStyle))
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
                        if (GUI.Button(new Rect(fact_xpos, fact_ypos, fact_xsize, fact_ysize), "Local", button_guiStyle))
                        {
                            manager.SetMatchHost("localhost", 1337, false);
                            m_ShowServer = false;
                        }
                        ypos += spacing;
                        fact_xpos = (int)(factor_x * xpos);
                        fact_ypos = (int)(factor_y * ypos);
                        fact_xsize = (int)(factor_x * 100);
                        fact_ysize = (int)(factor_y * 20);
                        if (GUI.Button(new Rect(fact_xpos, fact_ypos, fact_xsize, fact_ysize), "Internet", button_guiStyle))
                        {
                            manager.SetMatchHost("mm.unet.unity3d.com", 443, true);
                            m_ShowServer = false;
                        }
                        ypos += spacing;
                        fact_xpos = (int)(factor_x * xpos);
                        fact_ypos = (int)(factor_y * ypos);
                        fact_xsize = (int)(factor_x * 100);
                        fact_ysize = (int)(factor_y * 20);
                        if (GUI.Button(new Rect(fact_xpos, fact_ypos, fact_xsize, fact_ysize), "Staging", button_guiStyle))
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
                    GUI.Label(new Rect(fact_xpos, fact_ypos, fact_xsize, fact_ysize), "MM Uri: " + manager.matchMaker.baseUri, label_guiStyle);
                    ypos += spacing;

                    fact_xpos = (int)(factor_x * xpos);
                    fact_ypos = (int)(factor_y * ypos);
                    fact_xsize = (int)(factor_x * 200);
                    fact_ysize = (int)(factor_y * 20);
                    if (GUI.Button(new Rect(fact_xpos, fact_ypos, fact_xsize, fact_ysize), "Disable Match Maker", button_guiStyle))
                    {
                        manager.StopMatchMaker();
                    }
                    ypos += spacing;
                }
            }
        }
    }
}
