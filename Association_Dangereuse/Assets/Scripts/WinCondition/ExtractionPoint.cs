using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ExtractionPoint : NetworkBehaviour
{
    [SerializeField] private int baseQuota = 100;
    [SerializeField] private float secondsToWaitBeforeWinning = 4f;
    [SerializeField] private GameObject warningLight;
    private int quotaToReach;
    private int currentSum;
    private List<Item> items = new List<Item>();

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        quotaToReach = baseQuota;
    }

    private void Update()
    {
        CheckForGrabbedItem();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Item>() == null)
        {
            return;
        }
        Item itemEntering = other.GetComponent<Item>();
        currentSum += itemEntering.GetValue();
        items.Add(itemEntering);
        if (currentSum >= quotaToReach)
        {
            //warningLight.SetActive(true);
            SwitchWarningLightClientRpc(true);
            StartCoroutine(LoadBeforeWinning());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<Item>() == null)
        {
            return;
        }
        Item itemExiting = other.GetComponent<Item>();
        currentSum -= itemExiting.GetValue();
        items.Remove(itemExiting);
        if (currentSum < quotaToReach)
        {
            //warningLight.SetActive(false);
            SwitchWarningLightClientRpc(false);
            StopAllCoroutines();
        }
    }

    private void CheckForGrabbedItem()
    {
        foreach (Item item in items)
        {
            if (!item.gameObject.activeSelf)
            {
                currentSum -= item.GetValue();
                items.Remove(item);
                if (currentSum < quotaToReach)
                {
                    //warningLight.SetActive(false);
                    SwitchWarningLightClientRpc(false);
                    StopAllCoroutines();
                }
            }
        }
    }

    IEnumerator LoadBeforeWinning()
    {
        yield return new WaitForSeconds(secondsToWaitBeforeWinning);
        Debug.Log("yipee");
    }

    [ClientRpc]
    public void SwitchWarningLightClientRpc(bool OnOff)
    {
        warningLight.SetActive(OnOff);
    }
}
