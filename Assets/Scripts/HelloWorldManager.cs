
using MLAPI;
using UnityEngine;

namespace HelloWorld
{
    public class HelloWorldManager : MonoBehaviour
    {
        void OnGUI()
        {
            GUILayout.BeginArea(new Rect(10, 10, 300, 300));
            if (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer)
            {
                StartButtons();
            }
            else
            {
                StatusLabels();

                Respawn();

                Leave();
            }

            GUILayout.EndArea();
        }

        static void StartButtons()
        {
            if (GUILayout.Button("Host")) NetworkManager.Singleton.StartHost();
            if (GUILayout.Button("Client")) NetworkManager.Singleton.StartClient();
            if (GUILayout.Button("Server")) NetworkManager.Singleton.StartServer();
        }

        static void StatusLabels()
        {
            var mode = NetworkManager.Singleton.IsHost ?
                "Host" : NetworkManager.Singleton.IsServer ? "Server" : "Client";

            GUILayout.Label("Transport: " +
                NetworkManager.Singleton.NetworkConfig.NetworkTransport.GetType().Name);
            GUILayout.Label("Mode: " + mode);
        }

        static void Leave()
        {
            if (GUILayout.Button("Leave"))
            {
                Disconnect();
            }
        }

        static void Respawn()
        {
            if (GUILayout.Button(NetworkManager.Singleton.IsServer ? "Respawn" : "Request Respawn"))
            {
                if (NetworkManager.Singleton.ConnectedClients.TryGetValue(NetworkManager.Singleton.LocalClientId,
                    out var networkedClient))
                {
                    //TODO: Change to player respawner
                    var player = networkedClient.PlayerObject.GetComponent<PlayerManager>();
                    if (player)
                    {
                        player.Respawn();
                    }
                }
            }
        }

        static void Disconnect()
        {
            if (NetworkManager.Singleton.IsHost)
            {
                NetworkManager.Singleton.StopHost();
            }
            else if (NetworkManager.Singleton.IsClient)
            {
                NetworkManager.Singleton.StopClient();
            }
            else if (NetworkManager.Singleton.IsServer)
            {
                NetworkManager.Singleton.StopServer();
            }

            //UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
        }
    }
}