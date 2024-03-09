using System;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;

public class ServerManager : MonoBehaviour
{
    [SerializeField] private bool debugMessages;
    [SerializeField] private TMP_Text joinCodeText;

    private Relay relay;

    //private async void Start()
    //{
    //    var options = new InitializationOptions();
    //    //string profileName = $"Test_Profile_{Random.Range(0, 10000)}";
    //    //options.SetProfile(profileName);
    //    //DebugMessage($"Profile Name: {profileName}");

    //    await UnityServices.InitializeAsync(options);
    //    if (UnityServices.State == ServicesInitializationState.Initialized)
    //        DebugMessage("Unity Services Successfully Initialized. . .");
    //    else
    //        DebugMessage("Unity Services Failed to Initialize. . .");
        
    //    //AuthenticationService.Instance.SwitchProfile(profileName);
    //    await AuthenticationService.Instance.SignInAnonymouslyAsync();
    //    DebugMessage($"{AuthenticationService.Instance.Profile}");
    //    if (AuthenticationService.Instance.IsSignedIn)
    //        DebugMessage("Signed in successfully. . .");
    //    else
    //        DebugMessage("Failed to sign in. . .");
    //}

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
        relay.onInitializeSuccessful += () => relay.JoinRelay(joinCodeText.text);
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
