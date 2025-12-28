using UnityEngine;

public class EnemyBrickStructure : MonoBehaviour
{
    public int structureSize = 5;
    public float moveSpeed = 3f;
    public float detectionRange = 10f;

    private Transform player;
    private PlayerBrick playerScript;

    private Vector3 wanderDirection;
    private float wanderTimer = 0f;
    private float wanderChangeIntervalMin = 1.5f;
    private float wanderChangeIntervalMax = 3.5f;

    void Start()
    {
        var p = GameObject.FindWithTag("Player");
        if (p != null)
        {
            player = p.transform;
            playerScript = p.GetComponent<PlayerBrick>();
        }
        PickNewWanderDirection();
    }

    void Update()
    {
        if (player == null || playerScript == null)
        {
            var p = GameObject.FindWithTag("Player");
            if (p != null)
            {
                player = p.transform;
                playerScript = p.GetComponent<PlayerBrick>();
            }
        }

        if (player == null || playerScript == null)
        {
            Wander();
            return;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer < detectionRange)
        {
            if (structureSize > playerScript.currentSize)
            {
                ChasePlayer();
            }
            else
            {
                FleeFromPlayer();
            }
        }
        else
        {
            Wander();
        }
    }

    void ChasePlayer()
    {
        MoveTowards(player.position, moveSpeed);
    }

    void FleeFromPlayer()
    {
        Vector3 dir = (transform.position - player.position).normalized;
        Vector3 fleeTarget = transform.position + dir * 10f;
        MoveTowards(fleeTarget, moveSpeed * 1.2f);
    }

    void Wander()
    {
        wanderTimer -= Time.deltaTime;
        if (wanderTimer <= 0f) PickNewWanderDirection();

        Vector3 target = transform.position + wanderDirection * 5f;
        MoveTowards(target, moveSpeed * 0.6f);
    }

    void PickNewWanderDirection()
    {
        wanderDirection = Random.insideUnitSphere;
        wanderDirection.y = 0f;
        if (wanderDirection.sqrMagnitude < 0.01f) wanderDirection = Vector3.forward;
        wanderDirection.Normalize();
        wanderTimer = Random.Range(wanderChangeIntervalMin, wanderChangeIntervalMax);
    }

    void MoveTowards(Vector3 target, float speed)
    {
        Vector3 dir = target - transform.position;
        dir.y = 0f;
        if (dir.sqrMagnitude < 0.0001f) return;

        Vector3 step = dir.normalized * speed * Time.deltaTime;
        transform.position += step;

        if (step != Vector3.zero)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir.normalized), 10f * Time.deltaTime);
        }
    }
}
