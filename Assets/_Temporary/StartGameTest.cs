using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class StartGameTest : MonoBehaviour
{
    public void StartGame()
    {
        if (NetworkManager.Singleton.IsServer)
        {
            NetworkDelegates.Instance.OnGameStartedServerRpc();
        }
    }
}
