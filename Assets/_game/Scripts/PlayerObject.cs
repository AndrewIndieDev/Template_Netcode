using Unity.Netcode;
using UnityEngine;

public class PlayerObject : NetworkBehaviour
{
    [SerializeField] private bool showDebugMessages;

    #region Network Methods
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        
        if (IsOwner)
        {
            transform.SetParent(PersistentClient.LocalInstance.transform);
        }
    }
    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
    }
    #endregion

    protected void DebugMessage(string message)
    {
        if (showDebugMessages)
        {
            Debug.Log($"PLAYEROBJECT :: {message}");
        }
    }
}
