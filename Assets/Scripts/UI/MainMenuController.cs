using MLAPI;
using MLAPI.Spawning;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    private const int PlayerPrefabHashIndex = 0;
    [SerializeField] private TMP_InputField IpAddressInput;
    [SerializeField] private Button JoinButton;


    public void EnableJoin()
    {
        JoinButton.interactable = false;
        if (!string.IsNullOrEmpty(IpAddressInput.text) && IpAddressInput.text.Count(c => c == '.') == 3)
        {
            //check if it is a valid IP Address
            if (IPAddress.TryParse(IpAddressInput.text, out IPAddress address) &&
                address.AddressFamily.Equals(AddressFamily.InterNetwork))
            {
                JoinButton.interactable = true;
            }
        }
    }

    public void Join()
    {
        Debug.Log("Join");
        NetworkManager.Singleton.StartClient();
    }

    public void Host()
    {

        //NetworkManager.Singleton.ConnectionApprovalCallback += ClientConnectionApproval;
        NetworkManager.Singleton.StartHost(SpawnLocationManager.GetRandomSpawn(), Quaternion.identity);
    }

    //private static void ClientConnectionApproval(byte[] connectionData, ulong clientId, NetworkManager.ConnectionApprovedDelegate callback)
    //{
    //    ulong? prefabHash = NetworkSpawnManager.GetPrefabHashFromIndex(PlayerPrefabHashIndex);
    //    callback(true, prefabHash, true, SpawnLocationManager.GetRandomSpawn(), Quaternion.identity);
    //}
}
