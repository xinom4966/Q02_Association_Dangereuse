using UnityEngine;

public class NoiseBehaviour : MonoBehaviour
{
    [SerializeField] private float range = 10f;
    [SerializeField] private float secondsBeforeDestruction = 1f;
    [SerializeField] private LayerMask listenersMask;
    private float timer;
    private RaycastHit[] listeners;

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= secondsBeforeDestruction)
        {
            Destroy(gameObject);
        }
    }

    public void Activate(float modifier=1)
    {
        range *= modifier;
        listeners = Physics.SphereCastAll(transform.position, range, transform.forward, range, listenersMask);
        foreach (RaycastHit hit in listeners)
        {
            if (hit.collider.GetComponent<ListenerBehaviour>() == null)
            {
                continue;
            }
            hit.collider.GetComponent<ListenerBehaviour>().InvokeNoiseEvent(transform.position);
        }
    }
}
