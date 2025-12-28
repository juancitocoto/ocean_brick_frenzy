using UnityEngine;
using System.Collections;

public class PlayerBrickFish : BrickFish
{
    [Header("Player Settings")]
    public float rotationSpeed = 200f;
    public int currentStuds = 0;
    public int bricksCollected = 0;

    [Header("Growth")]
    public int bricksToGrow = 10;
    private int growthProgress = 0;

    [Header("Animation")]
    public float tailWagSpeed = 5f;
    public float tailWagAmount = 15f;
    private Transform tailBone;

    [Header("Mouth")]
    public Transform mouthPosition;
    public float eatRange = 1f;

    private Camera mainCamera;
    private Rigidbody rb;

    void Start()
    {
        base.AssembleFish();
        mainCamera = Camera.main;
        rb = GetComponent<Rigidbody>();

        if (assembledBricks.Count > 0)
        {
            tailBone = assembledBricks[assembledBricks.Count - 1].transform;
        }
    }

    void Update()
    {
        HandleMovement();
        AnimateTail();
        CheckForFood();
    }

    void HandleMovement()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(horizontal, vertical, 0).normalized;

        if (movement.magnitude > 0.1f)
        {
            transform.Translate(Vector3.right * swimSpeed * Time.deltaTime, Space.World);

            float targetAngle = Mathf.Atan2(movement.y, movement.x) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.Euler(0, 0, targetAngle);
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }

        if (Input.GetKey(KeyCode.Space))
        {
            transform.Translate(Vector3.right * swimSpeed * 0.5f * Time.deltaTime, Space.World);
        }
    }

    void AnimateTail()
    {
        if (tailBone != null)
        {
            float wagAngle = Mathf.Sin(Time.time * tailWagSpeed) * tailWagAmount;
            tailBone.localRotation = Quaternion.Euler(0, wagAngle, 0);
        }
    }

    void CheckForFood()
    {
        Vector3 origin = mouthPosition != null ? mouthPosition.position : transform.position + transform.right;
        Collider[] nearbyObjects = Physics.OverlapSphere(origin, eatRange);

        foreach (Collider col in nearbyObjects)
        {
            BrickFish otherFish = col.GetComponent<BrickFish>();
            if (otherFish != null && otherFish != this)
            {
                if (otherFish.fishSize < fishSize)
                {
                    EatFish(otherFish);
                }
            }

            LooseBrick looseBrick = col.GetComponent<LooseBrick>();
            if (looseBrick != null)
            {
                CollectBrick(looseBrick);
            }

            CollectableStud stud = col.GetComponent<CollectableStud>();
            if (stud != null)
            {
                CollectStud(stud);
            }
        }
    }

    void EatFish(BrickFish prey)
    {
        Vector3 eatForce = transform.right * 5f;
        prey.BreakApart(eatForce);

        growthProgress += prey.fishSize;
        bricksCollected += prey.assembledBricks.Count;

        StartCoroutine(EatAnimation());
        CheckGrowth();
        CameraShake.Instance?.Shake(0.1f, 0.2f);
        GameManager.Instance?.AddScore(prey.fishSize * 10);
    }

    void CollectBrick(LooseBrick brick)
    {
        growthProgress += brick.brickValue;
        bricksCollected++;

        StartCoroutine(BrickCollectAnimation(brick.gameObject));

        CheckGrowth();
    }

    void CollectStud(CollectableStud stud)
    {
        currentStuds += stud.value;
        UIManager.Instance?.ShowStudPopup(stud.transform.position, stud.value);
        Destroy(stud.gameObject);
    }

    IEnumerator BrickCollectAnimation(GameObject brick)
    {
        float duration = 0.3f;
        float elapsed = 0f;
        Vector3 startPos = brick.transform.position;

        Collider col = brick.GetComponent<Collider>();
        if (col != null) col.enabled = false;

        Rigidbody brickRb = brick.GetComponent<Rigidbody>();
        if (brickRb != null) brickRb.isKinematic = true;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            brick.transform.position = Vector3.Lerp(startPos, transform.position, t);
            brick.transform.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, t);

            yield return null;
        }

        Destroy(brick);
    }

    IEnumerator EatAnimation()
    {
        Vector3 originalScale = transform.localScale;

        transform.localScale = originalScale * 1.1f;
        yield return new WaitForSeconds(0.1f);
        transform.localScale = originalScale;
    }

    void CheckGrowth()
    {
        if (growthProgress >= bricksToGrow)
        {
            GrowFish();
            growthProgress = 0;
            bricksToGrow = Mathf.RoundToInt(bricksToGrow * 1.5f);
        }
    }

    void GrowFish()
    {
        fishSize += 5;
        StartCoroutine(GrowAnimation());
        AddGrowthBricks();
        swimSpeed = Mathf.Max(2f, swimSpeed - 0.1f);
        UpdateFishType();
        AudioSource.PlayClipAtPoint(buildSound, transform.position);
        if (breakApartEffect != null) Instantiate(breakApartEffect, transform.position, Quaternion.identity);
    }

    IEnumerator GrowAnimation()
    {
        Vector3 targetScale = transform.localScale * 1.15f;
        Vector3 startScale = transform.localScale;
        float duration = 0.5f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            float bounce = 1f + Mathf.Sin(t * Mathf.PI) * 0.2f;
            transform.localScale = Vector3.Lerp(startScale, targetScale, t) * bounce;
            yield return null;
        }

        transform.localScale = targetScale;
    }

    void AddGrowthBricks()
    {
        if (assembledBricks.Count == 0) return;
        Color bodyColor = Color.white;
        var rend = assembledBricks[0].GetComponent<Renderer>();
        if (rend != null) bodyColor = rend.material.color;

        Vector3 randomOffset = new Vector3(
            Random.Range(-0.3f, 0.3f),
            Random.Range(-0.2f, 0.2f),
            0
        );

        AddBrick(brick1x2Prefab, randomOffset, Vector3.zero, bodyColor);
    }

    void UpdateFishType()
    {
        if (fishSize >= 60) fishType = FishType.Boss;
        else if (fishSize >= 40) fishType = FishType.Giant;
        else if (fishSize >= 20) fishType = FishType.Large;
        else if (fishSize >= 10) fishType = FishType.Medium;
        else if (fishSize >= 5) fishType = FishType.Small;
        else fishType = FishType.Tiny;
    }

    void OnTriggerEnter(Collider other)
    {
        BrickFish otherFish = other.GetComponent<BrickFish>();

        if (otherFish != null && otherFish.fishSize > fishSize)
        {
            TakeDamage(otherFish);
        }
    }

    void TakeDamage(BrickFish predator)
    {
        int bricksToLose = Mathf.Min(3, assembledBricks.Count - 3);

        for (int i = 0; i < bricksToLose; i++)
        {
            if (assembledBricks.Count > 3)
            {
                int lastIndex = assembledBricks.Count - 1;
                GameObject brick = assembledBricks[lastIndex];
                assembledBricks.RemoveAt(lastIndex);

                brick.transform.SetParent(null);
                Rigidbody brickRb = brick.AddComponent<Rigidbody>();
                Vector3 ejectDir = (transform.position - predator.transform.position).normalized;
                brickRb.AddForce(ejectDir * 10f, ForceMode.Impulse);

                brick.AddComponent<LooseBrick>().Initialize(1);

                fishSize--;
            }
        }

        Vector3 knockback = (transform.position - predator.transform.position).normalized * 5f;
        if (rb != null) rb.AddForce(knockback, ForceMode.Impulse);

        StartCoroutine(InvincibilityFrames());

        if (fishSize <= 0)
        {
            Die();
        }
    }

    IEnumerator InvincibilityFrames()
    {
        float duration = 2f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            SetFishVisible(false);
            yield return new WaitForSeconds(0.1f);
            SetFishVisible(true);
            yield return new WaitForSeconds(0.1f);
            elapsed += 0.2f;
        }
    }

    void SetFishVisible(bool visible)
    {
        foreach (GameObject brick in assembledBricks)
        {
            if (brick == null) continue;
            Renderer rend = brick.GetComponent<Renderer>();
            if (rend != null) rend.enabled = visible;
        }
    }

    void Die()
    {
        BreakApart(Vector3.up * 5f);
        GameManager.Instance?.GameOver();
    }
}
