using Steamworks;
using Steamworks.Data;
using Unity.Netcode;

public class MyNetworkManager : NetworkManager
{
    public void Host()
    {
        StartHost();
    }

    public void Leave()
    {
        Shutdown();
    }

    public async void Join()
    {
        Lobby[] lobbies = await SteamMatchmaking.LobbyList.RequestAsync();
        if (lobbies.Length > 0)
            await SteamMatchmaking.JoinLobbyAsync(lobbies[0].Id);
    }
}
