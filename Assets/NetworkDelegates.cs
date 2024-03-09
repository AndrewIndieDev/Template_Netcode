using Unity.Netcode;
using UnityEngine;

public class NetworkDelegates : NetworkBehaviour
{
    [SerializeField] private bool debugMessages;
    
    public static NetworkDelegates Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    #region Generic Network Delegates
    public delegate void OnServerStopped();
    public static OnServerStopped onServerStopped;
    public delegate void OnClientStopped();
    public static OnClientStopped onClientStopped;
    public delegate void OnClientConnected(ulong clientId);
    public static OnClientConnected onClientConnected;
    public delegate void OnClientDisconnected(ulong clientId);
    public static OnClientDisconnected onClientDisconnected;
    public delegate void OnClientStarted();
    public static OnClientStarted onClientStarted;
    #endregion

    #region Game Specific Network Delegates
    public delegate void OnGameStarted();
    public static OnGameStarted onGameStarted;
    #endregion

    #region Unity Methods
    public void Start()
    {
        NetworkManager.Singleton.OnClientStopped += (isHostServer) =>
        {
            if (!isHostServer)
            {
                onClientStopped?.Invoke();
            }
        };

        NetworkManager.Singleton.OnServerStopped += (isHostServer) =>
        {
            onServerStopped?.Invoke();
        };

        NetworkManager.Singleton.OnClientConnectedCallback += (clientId) =>
        {
            onClientConnected?.Invoke(clientId);
        };

        NetworkManager.Singleton.OnClientDisconnectCallback += (clientId) =>
        {
            onClientDisconnected?.Invoke(clientId);
        };

        NetworkManager.Singleton.OnClientStarted += () =>
        {
            onClientStarted?.Invoke();
        };
    }
    
    public new void OnDestroy()
    {
        onGameStarted = null;
    }
    #endregion

    #region RPCs
    [ServerRpc]
    public void OnGameStartedServerRpc()
    {
        DebugMessage($"OnGameStartedServerRpc. . .");
        onGameStarted?.Invoke();
        OnGameStartedClientRpc();
    }
    [ClientRpc]
    public void OnGameStartedClientRpc()
    {
        DebugMessage($"OnGameStartedClientRpc. . .");
        if (!IsHost && !IsServer)
        {
            onGameStarted?.Invoke();
        }
    }
    #endregion

    private void DebugMessage(string message)
    {
        if (debugMessages)
        {
            Debug.Log($"NETWORKDELEGATES :: {message}");
        }
    }
}
