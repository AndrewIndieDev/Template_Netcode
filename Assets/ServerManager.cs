using TMPro;
using UnityEngine;

public class ServerManager : MonoBehaviour
{
    [SerializeField] private bool debugMessages;
    [SerializeField] private TMP_Text joinCodeText;

    private Relay relay;

    #region Button Methods
    public void StartServer()
    {
        relay = new(debugMessages);
        SubToRelay();
        relay.onInitializeSuccessful += () => relay.CreateHostRelay();
    }
    public void StartHost()
    {
        relay = new(debugMessages);
        SubToRelay();
        relay.onInitializeSuccessful += () => relay.CreateHostRelay();
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
        relay.onInitializeSuccessful += () => relay.JoinRelay(joinCodeText.text.Trim());
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
        relay.onLeaveServer += OnLeaveServer;
    }
    private void OnInitializeSuccessful()
    {
        DebugMessage("Successfully initialized Relay. . .");
    }
    private void OnInitializeFail()
    {
        DebugMessage("Failed to initialize Relay. . .");
    }
    private void OnCreateSuccessful()
    {
        DebugMessage("Create Successful. . .");
    }
    private void OnCreateFail(string msg)
    {
        DebugMessage($"Create Failed. . . {msg}");
    }
    private void OnJoinSuccessful()
    {
        DebugMessage("Join Successful. . .");
    }
    private void OnJoinFail(string msg)
    {
        DebugMessage($"Join Failed. . . {msg}");
    }
    private void OnLeaveServer()
    {
        DebugMessage($"Left Server. . .");
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
