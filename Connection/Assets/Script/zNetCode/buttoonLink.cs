using Unity.Netcode;
using UnityEngine;

public class buttoonLink : MonoBehaviour
{
    public NetworkManager networkManager;

    private void Start()
    {
        networkManager = GetComponent<NetworkManager>();
    }

    public void Host()
    {
        networkManager.StartHost();
    }
    public void Client()
    {
        networkManager.StartClient();
    }
    
    public void Server()
    {
            networkManager.StartServer();
    }
}
