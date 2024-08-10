using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

/*
 * Documentation for RPC calls
 * https://docs-multiplayer.unity3d.com/netcode/current/advanced-topics/message-system/rpc/
 */

public class RPCManager : NetworkBehaviour
{
    public static RPCManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<RPCManager>();
            }
            return _instance;
        }
    }
    private static RPCManager _instance;

    private const float RPC_FREQUENCY = 1f / 15f;

    private Queue<Action> rpcQueue = new Queue<Action>();

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (!IsServer)
            return;

        StartCoroutine(ProcessQueue());
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        if (!IsServer)
            return;
    }
    
    public void ExecuteInOrder(Action action)
    {
        rpcQueue.Enqueue(action);
    }

    public void ExecuteImmediately(Action action)
    {
        rpcQueue.Reverse();
        rpcQueue.Enqueue(action);
        rpcQueue.Reverse();
    }

    private IEnumerator ProcessQueue()
    {
        while (true)
        {
            if (rpcQueue.Count > 0)
            {
                Action action = rpcQueue.Dequeue();
                if (action != null)
                    action.Invoke();
            }
            yield return new WaitForSeconds(RPC_FREQUENCY);
        }
    }
}