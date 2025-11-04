using UnityEngine;

public class HonkItem : Item
{
    [SerializeField] private AudioSource myAudioSource;

    private void Honk()
    {
        myAudioSource.Play();
    }

    public override void Interact()
    {
        base.Interact();
        Honk();
    }
}
