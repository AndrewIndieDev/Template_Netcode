using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

public class Relay
{
    public delegate void OnInitializeSuccessful();
    public event OnInitializeSuccessful onInitializeSuccessful;
    public delegate void OnInitializeFail();
    public event OnInitializeFail onInitializeFail;
    public delegate void OnCreateSuccessful(string joinCode);
    public event OnCreateSuccessful onCreateSuccessful;
    public delegate void OnCreateFail(string message);
    public event OnCreateFail onCreateFail;
    public delegate void OnJoinSuccessful(string joinCode);
    public event OnJoinSuccessful onJoinSuccessful;
    public delegate void OnJoinFail(string message);
    public event OnJoinFail onJoinFail;
    public delegate void OnLeftServer();
    public event OnLeftServer onLeftServer;

    private bool debugMessages;

    #region Initialization
    public Relay(bool debugMode = false)
    {
        debugMessages = debugMode;
    }
    public async void Initialize()
    {
        if (UnityServices.State == ServicesInitializationState.Uninitialized)
        {
            try
            {
                DebugMessage("Initializing Unity Services. . .");
                var options = new InitializationOptions();
#if UNITY_EDITOR
                if (ParrelSync.ClonesManager.IsClone())
                {
                    string customArgument = ParrelSync.ClonesManager.GetArgument();
                    options.SetProfile(customArgument);
                }
#endif
                await UnityServices.InitializeAsync(options);
                DebugMessage("Completed Successfully. . .");
            }
            catch (System.Exception e)
            {
                DebugMessage($"Failed to initialize Unity Services. . . {e.Message}");
                onInitializeFail?.Invoke();
                return;
            }

            try
            {
                DebugMessage("Signing in annonymously. . .");
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
                DebugMessage("Signed in successfully. . .");
                DebugMessage($"ID: {AuthenticationService.Instance.PlayerId}");
            }
            catch (System.Exception e)
            {
                DebugMessage($"Failed to sign in. . . {e.Message}");
                onInitializeFail?.Invoke();
                return;
            }
        }

        NetworkManager.Singleton.OnClientStopped += (isHostServer) =>
        {
            if (!isHostServer)
            {
                DebugMessage("Server Stopped. . .");
                NetworkDelegates.onServerStopped?.Invoke();
            }
        };

        NetworkManager.Singleton.OnServerStopped += (isHostServer) =>
        {
            DebugMessage("Server Stopped. . .");
            NetworkDelegates.onServerStopped?.Invoke();
        };

        onInitializeSuccessful?.Invoke();
    }
    #endregion

    #region Relay-specific Methods
    public async void CreateHostRelay()
    {
        try
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(5);

            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

            DebugMessage($"Join Code created: {joinCode}. . .");

            RelayServerData serverData = new RelayServerData(allocation, "dtls");

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(serverData);

            NetworkManager.Singleton.StartHost();

            NetworkManager.Singleton.OnClientConnectedCallback += (clientId) =>
            {
                DebugMessage($"Client {clientId} connected. . .");
            };
            NetworkManager.Singleton.OnClientDisconnectCallback += (clientId) =>
            {
                DebugMessage($"Client {clientId} disconnected. . .");
            };

            onCreateSuccessful?.Invoke(joinCode);
        }
        catch (RelayServiceException e)
        {
            DebugMessage($"[{e.ErrorCode}] :: " + e.Message);
            onCreateFail?.Invoke($"[{e.ErrorCode}] :: " + e.Message);
        }
    }
    public async void CreateServerRelay()
    {
        try
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(5);

            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

            DebugMessage($"Join Code created: {joinCode}. . .");

            RelayServerData serverData = new RelayServerData(allocation, "dtls");

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(serverData);

            NetworkManager.Singleton.StartServer();

            NetworkManager.Singleton.OnClientConnectedCallback += (clientId) =>
            {
                DebugMessage($"Client {clientId} connected. . .");
            };
            NetworkManager.Singleton.OnClientDisconnectCallback += (clientId) =>
            {
                DebugMessage($"Client {clientId} disconnected. . .");
            };

            onCreateSuccessful?.Invoke(joinCode);
        }
        catch (RelayServiceException e)
        {
            DebugMessage($"[{e.ErrorCode}] :: " + e.Message);
            onCreateFail?.Invoke($"[{e.ErrorCode}] :: " + e.Message);
        }
    }
    public async void JoinRelay(string joinCode)
    {
        try
        {
            DebugMessage($"Joining lobby with joincode: {joinCode}. . .");
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

            RelayServerData serverData = new RelayServerData(joinAllocation, "dtls");

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(serverData);

            NetworkManager.Singleton.StartClient();

            onJoinSuccessful?.Invoke(joinCode);
        }
        catch (RelayServiceException e)
        {
            DebugMessage($"[{e.ErrorCode}] :: " + e.Message);
            onJoinFail?.Invoke($"[{e.ErrorCode}] :: " + e.Message);
        }
    }
    public void CloseRelay()
    {
        // TODO: Check if this is a server, host, or client. May be able to migrate the host to another client.
        NetworkManager.Singleton.Shutdown();
        onLeftServer?.Invoke();

        onInitializeSuccessful = null;
        onInitializeFail = null;
        onCreateSuccessful = null;
        onCreateFail = null;
        onJoinSuccessful = null;
        onJoinFail = null;
        onLeftServer = null;
    }
    #endregion

    private void DebugMessage(string message)
    {
        if (debugMessages)
        {
            Debug.Log($"RELAY :: {message}");
        }
    }
}
