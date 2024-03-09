using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartGameTest : MonoBehaviour
{
    public void StartGame()
    {
        NetworkDelegates.onGameStarted?.Invoke();
    }
}
