// vis2k: GUILayout instead of spacey += ...; removed Update hotkeys to avoid
// confusion if someone accidentally presses one.
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Mirror
{
    /// <summary>Shows NetworkManager controls in a GUI at runtime.</summary>
    [DisallowMultipleComponent]
    [AddComponentMenu("Network/Network Manager HUD")]
    [RequireComponent(typeof(NetworkManager))]
    [HelpURL("https://mirror-networking.gitbook.io/docs/components/network-manager-hud")]
    public class NetworkManagerHUD : MonoBehaviour
    {
        NetworkManager manager;
        [SerializeField] GameObject uiPlayer;
        [SerializeField] GameObject newGameButton;
        public int offsetX;
        public int offsetY;
        public string playerName;
        public string message;

        public event EventHandler<bool> onButtonClick;

        void Awake()
        {
            manager = GetComponent<NetworkManager>();
        }

        void OnGUI()
        {
            GUILayout.BeginArea(new Rect(10 + offsetX, 40 + offsetY, 215, 9999));
            if (!NetworkClient.isConnected && !NetworkServer.active)
            {
                StartButtons();
               

            }
            else
            {
                StatusLabels();
            }

            // client ready
            if (NetworkClient.isConnected && !NetworkClient.ready)
            {
              
                if (GUILayout.Button("Client Ready"))
                {
                    NetworkClient.Ready();
                    if (NetworkClient.localPlayer == null)
                    {
                        NetworkClient.AddPlayer();
                       

                    }
                }
            }

            StopButtons();

            GUILayout.EndArea();
        }

        void StartButtons()
        {
            if (!NetworkClient.active)
            {
                uiPlayer.SetActive(false);
                newGameButton.SetActive(false);
                GUILayout.Label("Enter player name before starting");
                playerName = GUILayout.TextField(playerName);

                // Server + Client
                if (Application.platform != RuntimePlatform.WebGLPlayer)
                {
                    if (GUILayout.Button("Host (Server + Client)"))
                    {


                        onButtonClick?.Invoke(this, false);
                        if (playerName != "")
                        {
                            uiPlayer.SetActive(true);
                            newGameButton.SetActive(true);
                            manager.StartHost();
                
                        }
                        else message = "Please enter player name";


                    }
                }

                // Client + IP
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Client"))
                {
                    onButtonClick?.Invoke(this, false);

                    if (playerName != "")
                    {
                        manager.StartClient();
                        uiPlayer.SetActive(true);
                        newGameButton.SetActive(true);


                    }
                    else if (playerName != "") message = "Please enter player name";

                }
                // This updates networkAddress every frame from the TextField
                manager.networkAddress = GUILayout.TextField(manager.networkAddress);
                GUILayout.EndHorizontal();

                // Server Only
                if (Application.platform == RuntimePlatform.WebGLPlayer)
                {
                    // cant be a server in webgl build
                    GUILayout.Box("(  WebGL cannot be server  )");

                }
                else
                {

                    if (GUILayout.Button("Server Only")) manager.StartServer();
                }


                if (GUILayout.Button("Spectator mode"))
                {
             
                    onButtonClick?.Invoke(this, true);
                    manager.StartClient();
             

                }

            }
            else
            {
                // Connecting
                GUILayout.Label($"Connecting to {manager.networkAddress}..");
                if (GUILayout.Button("Cancel Connection Attempt"))
                {
                    manager.StopClient();
                    uiPlayer.SetActive(false);
                    newGameButton.SetActive(false);
                }
            }
            GUIStyle red = new GUIStyle(EditorStyles.label);
            red.normal.textColor = Color.red;
            GUILayout.Label(message, red);

        }



        void StatusLabels()
        {
            // host mode
            // display separately because this always confused people:
            //   Server: ...
            //   Client: ...
            if (NetworkServer.active && NetworkClient.active)
            {
                GUILayout.Label($"<b>Host</b>: running via {Transport.activeTransport}");
            }
            // server only
            else if (NetworkServer.active)
            {
                GUILayout.Label($"<b>Server</b>: running via {Transport.activeTransport}");
            }
            // client only
            else if (NetworkClient.isConnected)
            {
                GUILayout.Label($"<b>Client</b>: connected to {manager.networkAddress} via {Transport.activeTransport}");
            }
        }

        void StopButtons()
        {
            // stop host if host mode
            if (NetworkServer.active && NetworkClient.isConnected)
            {
                if (GUILayout.Button("Stop Host"))
                {

                    manager.StopHost();
                    uiPlayer.SetActive(false);
                    newGameButton.SetActive(false);
                    message = "";
                }
            }
            // stop client if client-only
            else if (NetworkClient.isConnected)
            {
                if (GUILayout.Button("Stop Client"))
                {

                    manager.StopClient();
                    uiPlayer.SetActive(false);
                    newGameButton.SetActive(false);
                    message = "";
                }
            }
            // stop server if server-only
            else if (NetworkServer.active)
            {
                if (GUILayout.Button("Stop Server"))
                {
                    manager.StopServer();
                    uiPlayer.SetActive(false);
                    newGameButton.SetActive(false);
                    message = "";
                }
            }
        }
    }
}
