using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using UnityEngine.UI;

public class CharacterInventory : NetworkBehaviour
{
    [SerializeField] private int inventorySize = 5;
    [SerializeField] private float grabRange = 1f;
    [SerializeField] private Transform camPivot;
    [SerializeField] private LayerMask grabMask;
    [SerializeField] private List<Image> inventoryImages = new List<Image>();
    [SerializeField] private Sprite inventoryPlaceHolder;
    [SerializeField] private Color semiTransparent;
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
                    inventoryImages[currentIndex].sprite = activeItem.GetItemSprite();
                    inventoryImages[currentIndex].color += semiTransparent;
                    activeItem.Grab(transform);
                }
            }
        }
    }

    public void NavigateInventory(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            inventoryImages[currentIndex].color -= semiTransparent;
            currentIndex += Mathf.RoundToInt(ctx.ReadValue<Vector2>().y);
            currentIndex = Mathf.Clamp(currentIndex, 0, inventorySize-1);
            if (inventory[currentIndex] == null)
            {
                activeItem = null;
                inventoryImages[currentIndex].sprite = inventoryPlaceHolder;
                inventoryImages[currentIndex].color += semiTransparent;
                return;
            }
            activeItem = inventory[currentIndex];
            inventoryImages[currentIndex].sprite = activeItem.GetItemSprite();
            inventoryImages[currentIndex].color += semiTransparent;
        }
    }

    public void ReleaseItem(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            if (activeItem != null)
            {
                activeItem.Release();
                inventory[currentIndex] = null;
                inventoryImages[currentIndex].sprite = inventoryPlaceHolder;
                inventoryImages[currentIndex].color -= semiTransparent;
                activeItem = null;
            }
        }
    }
}
