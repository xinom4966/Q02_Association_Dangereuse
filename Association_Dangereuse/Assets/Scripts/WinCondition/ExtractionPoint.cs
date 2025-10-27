using Unity.Netcode;
using UnityEngine;

public class ExtractionPoint : NetworkBehaviour
{
    [SerializeField] private int baseQuota = 100;
    private int quotaToReach;
    private int currentSum;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        quotaToReach = baseQuota;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Item>() == null)
        {
            return;
        }
        Item itemEntering = other.GetComponent<Item>();
        currentSum += itemEntering.GetValue();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<Item>() == null)
        {
            return;
        }
        Item itemExiting = other.GetComponent<Item>();
        currentSum -= itemExiting.GetValue();
    }
}
