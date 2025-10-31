using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : NetworkBehaviour
{
    [SerializeField] protected NavMeshAgent agent;
    //[SerializeField] protected LayerMask noiseMask;
    [SerializeField] protected ListenerBehaviour listener;
    [SerializeField] protected float fov;
    [SerializeField] [Range(0,360)] protected float fovAngle;
    [SerializeField] protected float secondsBeforeAggression;
    [SerializeField] protected float chaseSpeed;
    protected Collider[] targetsInFov;
    protected bool playerInFov = false;
    protected EnemyState state;
    protected float aggressionLevel;
    protected GameObject target;

    protected void Update()
    {
        targetsInFov = Physics.OverlapSphere(transform.position, fov);
        foreach (Collider collider in targetsInFov)
        {
            if (!collider.CompareTag("Player"))
            {
                continue;
            }
            float signedAngle = Vector3.Angle(transform.forward, collider.transform.position-transform.position);
            if (Mathf.Abs(signedAngle) < fovAngle / 2)
            {
                //Player is in cone of vision
                playerInFov = true;
                target = collider.gameObject;
                agent.SetDestination(target.transform.position);
                state = EnemyState.Curious;
            }
        }
        UpdateState();
    }

    /*protected void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.gameObject.name);
        if (collision.gameObject.layer == noiseMask)
        {
            agent.SetDestination(collision.transform.position);
            Debug.Log("Heard");
        }
    }*/

    protected void UpdateState()
    {
        switch (state)
        {
            case EnemyState.Neutral:
                if (playerInFov)
                {
                    state = EnemyState.Curious;
                }
                break;
            case EnemyState.Curious:
                if (playerInFov)
                {
                    aggressionLevel += Time.deltaTime;
                    if (aggressionLevel >= secondsBeforeAggression)
                    {
                        state = EnemyState.Aggresive;
                    }
                }
                else
                {
                    aggressionLevel -= Time.deltaTime;
                    if (aggressionLevel >= 0)
                    {
                        state = EnemyState.Neutral;
                        aggressionLevel = 0;
                    }
                }
                break;
            case EnemyState.Aggresive:
                if (playerInFov)
                {
                    Pursuit();
                }
                break;
        }
    }

    protected virtual void Pursuit()
    {
        agent.speed = chaseSpeed;
    }

    public void OnNoiseEvent()
    {
        if (state == EnemyState.Aggresive)
        {
            Debug.Log("AggroHeard");
            return;
        }
        Debug.Log("heard");
        agent.SetDestination(listener.GetNoiseOrigin());
    }
}

public enum EnemyState
{
    Neutral,
    Curious,
    Aggresive
}
