using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BrickCoralBuilder : MonoBehaviour
{
    public GameObject brick1x1;
    public GameObject brick1x2;
    public GameObject cylinder1x1;
    public GameObject cone1x1;

    public enum CoralType
    {
        Brain,
        Branch,
        Tube,
        Fan,
        Mushroom
    }

    public void BuildCoral(CoralType type)
    {
        switch (type)
        {
            case CoralType.Brain:
                BuildBrainCoral();
                break;
            case CoralType.Branch:
                BuildBranchCoral();
                break;
            case CoralType.Tube:
                BuildTubeCoral();
                break;
            case CoralType.Fan:
                BuildFanCoral();
                break;
            case CoralType.Mushroom:
                BuildMushroomCoral();
                break;
        }
    }

    void BuildBrainCoral()
    {
        int layers = 4;
        float baseRadius = 2f;

        for (int layer = 0; layer < layers; layer++)
        {
            float radius = baseRadius * (1 - (float)layer / layers);
            int bricksInLayer = Mathf.Max(1, Mathf.RoundToInt(radius * 4));

            for (int i = 0; i < bricksInLayer; i++)
            {
                float angle = (360f / bricksInLayer) * i;
                Vector3 pos = new Vector3(
                    Mathf.Cos(angle * Mathf.Deg2Rad) * radius,
                    layer * 0.3f,
                    Mathf.Sin(angle * Mathf.Deg2Rad) * radius
                );

                if (brick1x1 != null) Instantiate(brick1x1, transform.position + pos, Quaternion.identity, transform);
            }
        }
    }

    void BuildBranchCoral()
    {
        BuildBranch(Vector3.zero, Vector3.up, 4, 0);
    }

    void BuildBranch(Vector3 position, Vector3 direction, int length, int depth)
    {
        if (depth > 3 || length <= 0) return;

        for (int i = 0; i < length; i++)
        {
            Vector3 pos = position + direction * i * 0.5f;
            if (cylinder1x1 != null) Instantiate(cylinder1x1, transform.position + pos, Quaternion.identity, transform);

            if (Random.value > 0.6f && depth < 3)
            {
                Vector3 branchDir = Quaternion.Euler(
                    Random.Range(-45, 45),
                    Random.Range(0, 360),
                    0
                ) * direction;

                BuildBranch(pos, branchDir, length - 1, depth + 1);
            }
        }
    }

    void BuildTubeCoral()
    {
        int tubeCount = Random.Range(3, 7);

        for (int t = 0; t < tubeCount; t++)
        {
            Vector3 basePos = Random.insideUnitSphere * 1f;
            basePos.y = 0;
            int height = Random.Range(3, 8);

            for (int h = 0; h < height; h++)
            {
                Vector3 pos = basePos + Vector3.up * h * 0.4f;
                if (cylinder1x1 != null) Instantiate(cylinder1x1, transform.position + pos, Quaternion.identity, transform);
            }

            Vector3 topPos = basePos + Vector3.up * height * 0.4f;
            if (cylinder1x1 != null)
            {
                GameObject top = Instantiate(cylinder1x1, transform.position + topPos, Quaternion.identity, transform);
                top.transform.localScale = new Vector3(1.2f, 0.3f, 1.2f);
            }
        }
    }

    void BuildFanCoral()
    {
        int width = 5;
        int height = 4;

        for (int x = 0; x < width; x++)
        {
            int columnHeight = height - Mathf.Abs(x - width / 2);

            for (int y = 0; y < columnHeight; y++)
            {
                Vector3 pos = new Vector3((x - width / 2) * 0.5f, y * 0.3f, 0);
                if (brick1x1 != null) Instantiate(brick1x1, transform.position + pos, Quaternion.identity, transform);
            }
        }
    }

    void BuildMushroomCoral()
    {
        for (int i = 0; i < 3; i++)
        {
            Vector3 pos = Vector3.up * i * 0.4f;
            if (cylinder1x1 != null) Instantiate(cylinder1x1, transform.position + pos, Quaternion.identity, transform);
        }

        float capRadius = 1.5f;
        for (int x = -2; x <= 2; x++)
        {
            for (int z = -2; z <= 2; z++)
            {
                if (x * x + z * z <= 4)
                {
                    Vector3 pos = new Vector3(x * 0.4f, 1.2f, z * 0.4f);
                    if (brick1x1 != null) Instantiate(brick1x1, transform.position + pos, Quaternion.identity, transform);
                }
            }
        }
    }
}

public class SeaweedSway : MonoBehaviour
{
    public float swaySpeed = 1f;
    public float swayAmount = 10f;
    public float randomOffset;

    private Transform[] segments;

    void Start()
    {
        randomOffset = Random.Range(0f, 100f);
        segments = GetComponentsInChildren<Transform>();
    }

    void Update()
    {
        for (int i = 1; i < segments.Length; i++)
        {
            float sway = Mathf.Sin((Time.time + randomOffset) * swaySpeed + i * 0.5f) * swayAmount * (i * 0.3f);
            segments[i].localRotation = Quaternion.Euler(sway, 0, sway * 0.5f);
        }
    }
}

public class BubbleEmitter : MonoBehaviour
{
    public GameObject bubblePrefab;
    public float emitInterval = 2f;
    public int bubblesPerEmit = 3;

    void Start()
    {
        StartCoroutine(EmitBubbles());
    }

    IEnumerator EmitBubbles()
    {
        while (true)
        {
            yield return new WaitForSeconds(emitInterval + Random.Range(-0.5f, 0.5f));

            for (int i = 0; i < bubblesPerEmit; i++)
            {
                SpawnBubble();
            }
        }
    }

    void SpawnBubble()
    {
        Vector3 offset = new Vector3(
            Random.Range(-0.5f, 0.5f),
            0,
            Random.Range(-0.5f, 0.5f)
        );

        if (bubblePrefab == null)
        {
            GameObject b = new GameObject("Bubble");
            b.transform.position = transform.position + offset;
            b.AddComponent<BrickBubble>();
            return;
        }

        GameObject bubble = Instantiate(bubblePrefab, transform.position + offset, Quaternion.identity);
        bubble.AddComponent<BrickBubble>();
    }
}

public class BrickBubble : MonoBehaviour
{
    public float riseSpeed = 2f;
    public float wobbleSpeed = 3f;
    public float wobbleAmount = 0.3f;
    public float lifetime = 10f;

    private float startX;
    private float startZ;
    private float randomOffset;

    void Start()
    {
        startX = transform.position.x;
        startZ = transform.position.z;
        randomOffset = Random.Range(0f, 100f);

        float scale = Random.Range(0.3f, 1f);
        transform.localScale = Vector3.one * scale;

        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        transform.position += Vector3.up * riseSpeed * Time.deltaTime;

        float wobbleX = Mathf.Sin((Time.time + randomOffset) * wobbleSpeed) * wobbleAmount;
        float wobbleZ = Mathf.Cos((Time.time + randomOffset) * wobbleSpeed * 0.7f) * wobbleAmount;

        transform.position = new Vector3(
            startX + wobbleX,
            transform.position.y,
            startZ + wobbleZ
        );

        transform.localScale += Vector3.one * 0.01f * Time.deltaTime;
    }
}

public class AIBrickFish : BrickFish
{
    [Header("AI Settings")]
    public float detectionRange = 15f;
    public float fleeRange = 8f;
    public float wanderRadius = 10f;
    public float stateChangeInterval = 3f;

    [Header("Behavior Weights")]
    [Range(0, 1)] public float aggressiveness = 0.5f;
    [Range(0, 1)] public float schoolingTendency = 0.3f;

    private Transform player;
    private PlayerBrickFish playerFish;
    private Vector3 wanderTarget;
    private AIState currentState;
    private float stateTimer;

    public enum AIState
    {
        Wandering,
        ChasingPrey,
        FleeingPredator,
        Schooling,
        Eating
    }

    void Start()
    {
        base.AssembleFish();

        player = GameObject.FindWithTag("Player")?.transform;
        playerFish = player?.GetComponent<PlayerBrickFish>();

        SetNewWanderTarget();
        currentState = AIState.Wandering;
    }

    void Update()
    {
        UpdateState();
        ExecuteState();
        AnimateSwim();
    }

    void UpdateState()
    {
        stateTimer -= Time.deltaTime;

        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (playerFish != null)
        {
            if (playerFish.fishSize > fishSize)
            {
                if (distanceToPlayer < fleeRange)
                {
                    currentState = AIState.FleeingPredator;
                    return;
                }
            }
            else if (playerFish.fishSize < fishSize && distanceToPlayer < detectionRange)
            {
                if (Random.value < aggressiveness)
                {
                    currentState = AIState.ChasingPrey;
                    return;
                }
            }
        }

        BrickFish nearbyPrey = FindNearbyPrey();
        if (nearbyPrey != null && Random.value < aggressiveness)
        {
            currentState = AIState.ChasingPrey;
            return;
        }

        if (Random.value < schoolingTendency)
        {
            BrickFish[] school = FindSchoolMates();
            if (school.Length > 2)
            {
                currentState = AIState.Schooling;
                return;
            }
        }

        if (stateTimer <= 0)
        {
            currentState = AIState.Wandering;
            SetNewWanderTarget();
            stateTimer = stateChangeInterval;
        }
    }

    void ExecuteState()
    {
        switch (currentState)
        {
            case AIState.Wandering:
                Wander();
                break;
            case AIState.ChasingPrey:
                ChasePrey();
                break;
            case AIState.FleeingPredator:
                Flee();
                break;
            case AIState.Schooling:
                School();
                break;
        }
    }

    void Wander()
    {
        MoveTowards(wanderTarget, swimSpeed * 0.7f);

        if (Vector3.Distance(transform.position, wanderTarget) < 1f)
        {
            SetNewWanderTarget();
        }
    }

    void ChasePrey()
    {
        if (player != null && playerFish != null && playerFish.fishSize < fishSize)
        {
            MoveTowards(player.position, swimSpeed * 1.2f);
        }
        else
        {
            BrickFish prey = FindNearbyPrey();
            if (prey != null)
            {
                MoveTowards(prey.transform.position, swimSpeed * 1.2f);

                if (Vector3.Distance(transform.position, prey.transform.position) < 1f)
                {
                    EatPrey(prey);
                }
            }
            else
            {
                currentState = AIState.Wandering;
            }
        }
    }

    void Flee()
    {
        if (player != null)
        {
            Vector3 fleeDirection = (transform.position - player.position).normalized;
            Vector3 fleeTarget = transform.position + fleeDirection * 5f;

            MoveTowards(fleeTarget, swimSpeed * 1.5f);

            if (Vector3.Distance(transform.position, player.position) > fleeRange * 1.5f)
            {
                currentState = AIState.Wandering;
            }
        }
    }

    void School()
    {
        BrickFish[] schoolMates = FindSchoolMates();

        if (schoolMates.Length < 2)
        {
            currentState = AIState.Wandering;
            return;
        }

        Vector3 schoolCenter = Vector3.zero;
        Vector3 averageDirection = Vector3.zero;

            foreach (BrickFish mate in schoolMates)
        {
            schoolCenter += mate.transform.position;
            averageDirection += mate.transform.right;
        }

        schoolCenter /= schoolMates.Length;
        averageDirection /= schoolMates.Length;

        Vector3 cohesion = (schoolCenter - transform.position) * 0.5f;
        Vector3 alignment = averageDirection * 0.3f;

        Vector3 separation = Vector3.zero;
        foreach (BrickFish mate in schoolMates)
        {
            float dist = Vector3.Distance(transform.position, mate.transform.position);
            if (dist < 2f)
            {
                separation += (transform.position - mate.transform.position) / dist;
            }
        }

        Vector3 schoolTarget = transform.position + cohesion + alignment + separation;
        MoveTowards(schoolTarget, swimSpeed);
    }

    void MoveTowards(Vector3 target, float speed)
    {
        Vector3 direction = (target - transform.position).normalized;

        transform.position += direction * speed * Time.deltaTime;

        if (direction.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 5f * Time.deltaTime);
        }
    }

    void SetNewWanderTarget()
    {
        wanderTarget = transform.position + Random.insideUnitSphere * wanderRadius;
        wanderTarget.y = Mathf.Clamp(wanderTarget.y, -8f, 8f);
    }

    BrickFish FindNearbyPrey()
    {
        Collider[] nearby = Physics.OverlapSphere(transform.position, detectionRange);

        foreach (Collider col in nearby)
        {
            BrickFish fish = col.GetComponent<BrickFish>();
            if (fish != null && fish != this && fish.fishSize < fishSize)
            {
                return fish;
            }
        }

        return null;
    }

    BrickFish[] FindSchoolMates()
    {
        List<BrickFish> schoolMates = new List<BrickFish>();
        Collider[] nearby = Physics.OverlapSphere(transform.position, detectionRange);

        foreach (Collider col in nearby)
        {
            AIBrickFish fish = col.GetComponent<AIBrickFish>();
            if (fish != null && fish != this && fish.fishType == fishType)
            {
                schoolMates.Add(fish);
            }
        }

        return schoolMates.ToArray();
    }

    void EatPrey(BrickFish prey)
    {
        Vector3 eatForce = transform.right * 5f;
        prey.BreakApart(eatForce);

        fishSize += prey.fishSize / 2;
        transform.localScale *= 1.05f;
    }

    void AnimateSwim()
    {
        if (assembledBricks.Count > 2)
        {
            Transform tail = assembledBricks[assembledBricks.Count - 1].transform;
            float wag = Mathf.Sin(Time.time * 5f) * 15f;
            tail.localRotation = Quaternion.Euler(0, wag, 0);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        BrickFish otherFish = other.GetComponent<BrickFish>();

        if (otherFish != null && otherFish != this)
        {
            if (otherFish.fishSize < fishSize)
            {
                EatPrey(otherFish);
            }
        }
    }
}
