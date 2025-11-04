using System.Collections;
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
    [SerializeField] protected float chaseMaxSpeed;
    [SerializeField] protected float chaseAcceleration;
    [SerializeField] protected float maxTravelDistance;
    [SerializeField] protected float killDistance;
    [SerializeField] protected int damage;
    [SerializeField] protected float damageInterval;
    protected Collider[] targetsInFov;
    protected bool playerInFov = false;
    protected EnemyState state;
    protected float aggressionLevel;
    protected GameObject target;
    protected float baseSpeed;
    protected float currentSpeed;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        baseSpeed = agent.speed;
    }

    protected void Update()
    {
        //Patrol routine
        if (state == EnemyState.Neutral && agent.remainingDistance == 0)
        {
            Vector3 randomPoint = Random.insideUnitSphere * maxTravelDistance;
            SetDestinationServerRpc(randomPoint);
        }

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
                SetDestinationServerRpc(target.transform.position);
                state = EnemyState.Curious;
            }
            else
            {
                playerInFov = false;
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
        }
    }

    protected virtual void Pursuit()
    {
        currentSpeed = agent.speed;
        if (currentSpeed < chaseMaxSpeed)
        {
            agent.speed *= chaseAcceleration;
        }
        if (Vector3.Distance(target.transform.position, transform.position) <= killDistance)
        {
            StartCoroutine(DamageCoroutine());
        }
        else
        {
            StopAllCoroutines();
        }
    }

    public void OnNoiseEvent()
    {
        if (state == EnemyState.Aggresive)
        {
            return;
        }
        SetDestinationServerRpc(listener.GetNoiseOrigin());
    }

    [ServerRpc (RequireOwnership = false)]
    private void SetDestinationServerRpc(Vector3 destination)
    {
        agent.SetDestination(destination);
        SetDestinationClientRpc(destination);
    }

    [ClientRpc (RequireOwnership = false)]
    private void SetDestinationClientRpc(Vector3 destination)
    {
        agent.SetDestination(destination);
    }

    IEnumerator DamageCoroutine()
    {
        while (true)
        {
            target.GetComponent<CharacterHealth>().TakeDamage(damage);
            yield return new WaitForSeconds(damageInterval);
        }
    }
}

public enum EnemyState
{
    Neutral,
    Curious,
    Aggresive
}
