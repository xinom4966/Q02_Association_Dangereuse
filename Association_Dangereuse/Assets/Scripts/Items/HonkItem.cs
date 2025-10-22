using UnityEngine;
using Unity.Netcode;

public class HonkItem : Item
{
    [SerializeField] private AudioSource myAudioSource;

    private void Honk()
    {
        myAudioSource.Play();
    }

    protected override void Interact()
    {
        base.Interact();
        Honk();
    }
}
