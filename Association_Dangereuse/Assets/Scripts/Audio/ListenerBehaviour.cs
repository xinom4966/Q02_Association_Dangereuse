using UnityEngine;
using UnityEngine.Events;

public class ListenerBehaviour : MonoBehaviour
{
    [SerializeField] private UnityEvent onNoiseHeard;
    private Vector3 noisePosition;

    public void InvokeNoiseEvent(Vector3 origin)
    {
        noisePosition = origin;
        onNoiseHeard.Invoke();
    }

    public Vector3 GetNoiseOrigin()
    {
        return noisePosition;
    }
}
