using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;


public class Item : NetworkBehaviour
{
    [SerializeField] protected string itemName;
    [SerializeField] protected int id;
    [SerializeField] protected int value;
    [SerializeField] protected bool isGrabbable;
    [SerializeField] protected Sprite sprite;
    private Transform myParent;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (!NetworkManager.Singleton.IsHost)
        {
            NetworkObject.Despawn();
        }
    }

    public virtual void Interact()
    {
        
    }

    public bool GetIsGrabbable()
    {
        return isGrabbable;
    }

    public void Grab(Transform newParent)
    {
        if (isGrabbable)
        {
            myParent = newParent;
            //ReparentItemClientRpc();
            ItemDisapearServerRpc();
            //Interact();

            //NetworkObject.Despawn();
            //gameObject.SetActive(false);
        }
    }

    [ServerRpc (RequireOwnership = false)]
    public void ItemDisapearServerRpc()
    {
        Interact();
        gameObject.SetActive(false);
        ItemDisapearClientRpc();
        //NetworkObject.Despawn(true);
        //ReparentItemClientRpc();
    }

    [ClientRpc (RequireOwnership = false)]
    public void ItemDisapearClientRpc()
    {
        Interact();
        gameObject.SetActive(false);
        //NetworkObject.TrySetParent(myParent, false);
    }

    public void Release()
    {
        ReleaseServerRpc(myParent.position);
        gameObject.SetActive(true);
    }

    [ServerRpc(RequireOwnership = false)]
    public void ReleaseServerRpc(Vector3 parentPos)
    {
        transform.position = parentPos;
        GetComponent<NetworkTransform>().Teleport(parentPos, Quaternion.identity, transform.localScale);
        gameObject.SetActive(true);
        ReleaseClientRpc(parentPos);
    }

    [ClientRpc(RequireOwnership = false)]
    public void ReleaseClientRpc(Vector3 parentPos)
    {
        transform.position = parentPos;
        GetComponent<NetworkTransform>().Teleport(parentPos, Quaternion.identity, transform.localScale);
        gameObject.SetActive(true);
    }

    public Sprite GetItemSprite()
    {
        return sprite;
    }

    public int GetValue()
    {
        return value;
    }
}
