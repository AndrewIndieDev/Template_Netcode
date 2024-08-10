using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class PersistentClient : NetworkBehaviour
{
    public static PersistentClient LocalInstance { get; private set; }
    public static Dictionary<ulong, PersistentClient> AllInstances { get; private set; }
    public PlayerObject PlayerObject { get { return playerObject; } }

    [Header("Debugging")]
    [SerializeField] private bool showDebugMessages;

    [Header("References")]
    [SerializeField] private NetworkObject playerObjectPrefab;
    [SerializeField] private PlayerObject playerObject;

    #region Network Variables
    private void SetupNetworkVariables(bool onDestroy = false)
    {
        if (!onDestroy)
        {
            nv_DisplayName.OnValueChanged += OnPlayerNameChanged;
        }
        else
        {
            nv_DisplayName.OnValueChanged -= OnPlayerNameChanged;
        }
    }
    private NetworkVariable<FixedString32Bytes> nv_DisplayName = new NetworkVariable<FixedString32Bytes>(
        "",
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner
    );
    public string PlayerName { get { return nv_DisplayName.Value.ToString(); } }
    private void OnPlayerNameChanged(FixedString32Bytes previousValue, FixedString32Bytes newValue)
    {

    }
    #endregion

    #region Network Spawn and Despawn
    public override void OnNetworkSpawn()
    {
        SetupNetworkVariables();

        gameObject.name = $"PersistentClient <Player {OwnerClientId}>";

        if (IsOwner)
        {
            DebugMessage("PersistentClient Initialized. . .");
            nv_DisplayName.Value = $"Player {OwnerClientId}";
            LocalInstance = this;
            if (AllInstances == null)
                AllInstances = new();
            AllInstances.Add(OwnerClientId, this);
            RPCManager.Instance.ExecuteInOrder(CreatePlayerObjectRpc);
        }
    }
    public override void OnNetworkDespawn()
    {
        DebugMessage("OnNetworkDespawn. . .");
        SetupNetworkVariables(onDestroy: true);
        if (LocalInstance == this)
            AllInstances.Clear();
    }
    #endregion

    #region RPCs
    [Rpc(SendTo.Everyone)]
    public void CreatePlayerObjectRpc()
    {
        if (IsOwner)
        {
            //playerObject = Instantiate(playerObjectPrefab).GetComponent<PlayerObject>();
            NetworkManager.Singleton.SpawnManager.InstantiateAndSpawn(playerObjectPrefab, OwnerClientId);
        }
        DebugMessage("Created a PlayerObject. . .");
    }
    #endregion

    private void DebugMessage(string message)
    {
        if (showDebugMessages)
        {
            Debug.Log($"PERSISTENTCLIENT <{OwnerClientId}> :: {message}");
        }
    }
}