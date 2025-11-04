using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;

public class CharacterHealth : NetworkBehaviour
{
    [SerializeField] private int baseHealth = 10;
    [SerializeField] private Image healthbar;
    [SerializeField] private Gradient hpGradient;
    [SerializeField] private CharacterMovement playerMovement;
    [SerializeField] private CharacterInventory playerInventory;
    [SerializeField] private GameObject characterCamera;
    [SerializeField] private GameObject spectatorCamera;
    private int currentHealth;
    private float ratio = 0f;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        currentHealth = baseHealth;
        UpdateHealthBar();
    }

    public void TakeDamage(int ammount)
    {
        currentHealth -= ammount;
        currentHealth = Mathf.FloorToInt(Mathf.Clamp(currentHealth, 0.0f, baseHealth));
        UpdateHealthBar();
        if (currentHealth == 0)
        {
            Die();
        }
    }

    private void UpdateHealthBar()
    {
        ratio = (float)currentHealth / (float)baseHealth;
        healthbar.fillAmount = ratio;
        healthbar.color = hpGradient.Evaluate(ratio);
    }

    private void Die()
    {
        playerMovement.enabled = false;
        playerInventory.ReleaseAllItems();
        playerInventory.enabled = false;
        characterCamera.SetActive(false);
        spectatorCamera.SetActive(true);
    }
}
