using UnityEngine;
using Unity.Netcode;

public class Item : NetworkBehaviour
{
    [SerializeField] protected string itemName;
    [SerializeField] protected int id;
    [SerializeField] protected int value;
    [SerializeField] protected bool isGrabbable;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        /*if (NetworkManager.Singleton.IsClient)
        {
            Debug.Log("tspmo");
            NetworkObject.Despawn();
        }*/
    }

    public virtual void Interact()
    {
        if (isGrabbable)
        {
            GetComponent<BoxCollider>().enabled = false;
            GetComponent<Rigidbody>().isKinematic = true;
        }
    }

    public bool GetIsGrabbable()
    {
        return isGrabbable;
    }
}
