using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class CharacterInventory : NetworkBehaviour
{
    [SerializeField] private int inventorySize = 5;
    [SerializeField] private float grabRange = 1f;
    [SerializeField] private Transform camPivot;
    [SerializeField] private LayerMask grabMask;
    private RaycastHit hit;
    private Item activeItem;
    private List<Item> inventory;
    private int currentIndex = 0;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (!IsOwner)
        {
            GetComponent<CharacterInventory>().enabled = false;
        }
        inventory = new List<Item>(inventorySize);
        for (int i = 0; i < inventorySize; i++)
        {
            inventory.Add(null);
        }
    }

    public void Interact(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            if (activeItem != null)
            {
                activeItem.Interact();
                return;
            }
            if (Physics.Raycast(transform.position, camPivot.forward, out hit, grabRange, grabMask))
            {
                if (hit.transform.gameObject.GetComponent<Item>() == null)
                {
                    return;
                }
                hit.transform.gameObject.GetComponent<Item>().Interact();
                if (hit.transform.gameObject.GetComponent<Item>().GetIsGrabbable())
                {
                    activeItem = hit.transform.gameObject.GetComponent<Item>();
                    inventory[currentIndex] = activeItem;
                    hit.transform.parent = transform;
                }
            }
        }
    }

    public void NavigateInventory(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            currentIndex += Mathf.RoundToInt(ctx.ReadValue<Vector2>().y);
            currentIndex = Mathf.Clamp(currentIndex, 0, inventorySize-1);
            if (activeItem != null)
            {
                activeItem.gameObject.SetActive(false);
            }
            if (inventory[currentIndex] == null)
            {
                Debug.Log("C'est nul !");
                activeItem = null;
                return;
            }
            activeItem = inventory[currentIndex];
            activeItem.gameObject.SetActive(true);
        }
    }
}
