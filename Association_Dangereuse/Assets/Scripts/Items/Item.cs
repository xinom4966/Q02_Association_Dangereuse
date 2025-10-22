using UnityEngine;
using Unity.Netcode;

public class Item : NetworkBehaviour
{
    [SerializeField] protected string itemName;
    [SerializeField] protected int id;
    [SerializeField] protected int value;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
    }

    protected virtual void Interact()
    {
        
    }
}
