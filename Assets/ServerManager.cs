using TMPro;
using Unity.Netcode;
using UnityEngine;

public class ServerManager : MonoBehaviour
{
    [SerializeField] private bool debugMessages;
    [SerializeField] private TMP_Text joinCodeText;
    [SerializeField] private TMP_Text joinCodeDisplay;

    private Relay relay;

    [SerializeField] private GameObject persistentClientPrefab;

    private void Start()
    {
        NetworkDelegates.onClientConnected += (clientId) =>
        {
            if (clientId == 0)
                return;
            TrySpawnPersistentClient(clientId);
        };
    }

    #region Button Methods
    public void StartServer()
    {
        relay = new(debugMessages);
        SubToRelay();
        relay.onInitializeSuccessful += () => relay.CreateHostRelay();
        relay.Initialize();
    }
    public void StartHost()
    {
        relay = new(debugMessages);
        SubToRelay();
        relay.onInitializeSuccessful += () => relay.CreateHostRelay();
        relay.Initialize();
    }
    public void LeaveServer()
    {
        relay?.CloseRelay();
        relay = null;
    }
    public void StartClient()
    {
        relay = new(debugMessages);
        SubToRelay();
        relay.onInitializeSuccessful += () => relay.JoinRelay(joinCodeText.text.Substring(0, 6));
        relay.Initialize();
    }
    #endregion

    #region Relay Event Subscriptions
    private void SubToRelay()
    {
        relay.onInitializeSuccessful += OnInitializeSuccessful;
        relay.onInitializeFail += OnInitializeFail;
        relay.onCreateSuccessful += OnCreateSuccessful;
        relay.onCreateFail += OnCreateFail;
        relay.onJoinSuccessful += OnJoinSuccessful;
        relay.onJoinFail += OnJoinFail;
        relay.onLeftServer += OnLeaveServer;
    }
    private void OnInitializeSuccessful()
    {
        DebugMessage("Successfully initialized Relay. . .");
    }
    private void OnInitializeFail()
    {
        DebugMessage("Failed to initialize Relay. . .");
    }
    private void OnCreateSuccessful(string joinCode)
    {
        DebugMessage("Create Successful. . .");
        joinCodeDisplay.text = $"Successfully created server with join code: {joinCode}";
        TrySpawnPersistentClient(0);
    }
    private void OnCreateFail(string msg)
    {
        DebugMessage($"Create Failed. . . {msg}");
        joinCodeDisplay.text = $"Create Failed. . . {msg}";
    }
    private void OnJoinSuccessful(string joinCode)
    {
        DebugMessage("Join Successful. . .");
        joinCodeDisplay.text = $"Successfully joined server with join code: {joinCode}";
    }
    private void OnJoinFail(string msg)
    {
        DebugMessage($"Join Failed. . . {msg}");
        joinCodeDisplay.text = $"Join Failed. . . {msg}";
    }
    private void OnLeaveServer()
    {
        DebugMessage($"Left Server. . .");
        joinCodeDisplay.text = "";
    }
    #endregion

    #region Server Methods
    private void TrySpawnPersistentClient(ulong clientId)
    {
        if (NetworkManager.Singleton.IsServer)
        {
            NetworkManager.Singleton.ConnectedClients.TryGetValue(clientId, out var networkClient);
            if (PersistentClient.AllInstances == null || networkClient != null && !PersistentClient.AllInstances.ContainsKey(clientId))
            {
                var persistentClient = Instantiate(persistentClientPrefab).GetComponent<PersistentClient>();
                persistentClient.NetworkObject.SpawnAsPlayerObject(clientId);
            }
        }
    }
    #endregion

    private void DebugMessage(string message)
    {
        if (debugMessages)
        {
            Debug.Log($"SERVERMANAGER :: {message}");
        }
    }
}
