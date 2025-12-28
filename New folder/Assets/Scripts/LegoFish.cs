using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class FishBodyPart
{
    public GameObject brickPrefab;
    public Vector3 localPosition;
    public Vector3 localRotation;
    public Color brickColor;
}

public class LegoFish : MonoBehaviour
{
    [Header("Fish Stats")]
    public int fishSize = 1;
    public float swimSpeed = 3f;
    public FishType fishType;

    [Header("LEGO Components")]
    public List<FishBodyPart> bodyParts = new List<FishBodyPart>();
    public List<GameObject> assembledBricks = new List<GameObject>();

    [Header("Brick Prefabs")]
    public GameObject stud1x1Prefab;
    public GameObject brick1x2Prefab;
    public GameObject brick2x2Prefab;
    public GameObject brick2x4Prefab;
    public GameObject platePrefab;
    public GameObject slopeBrickPrefab;

    [Header("Effects")]
    public GameObject breakApartEffect;
    public AudioClip buildSound;
    public AudioClip breakSound;

    public enum FishType
    {
        Tiny,       // 3-5 bricks
        Small,      // 6-10 bricks
        Medium,     // 11-20 bricks
        Large,      // 21-40 bricks
        Giant,      // 41-60 bricks
        Boss        // 60+ bricks
    }

    void Start()
    {
        AssembleFish();
    }

    public virtual void AssembleFish()
    {
        switch (fishType)
        {
            case FishType.Tiny:
                BuildTinyFish();
                break;
            case FishType.Small:
                BuildSmallFish();
                break;
            case FishType.Medium:
                BuildMediumFish();
                break;
            case FishType.Large:
                BuildLargeFish();
                break;
            case FishType.Giant:
                BuildGiantFish();
                break;
        }

        if (buildSound != null)
        {
            AudioSource.PlayClipAtPoint(buildSound, transform.position);
        }
    }

    void BuildTinyFish()
    {
        AddBrick(brick1x2Prefab, Vector3.zero, Vector3.zero, GetRandomFishColor());
        AddBrick(slopeBrickPrefab, new Vector3(-0.5f, 0, 0), new Vector3(0, 180, 0), GetRandomFishColor());
        AddBrick(stud1x1Prefab, new Vector3(0.3f, 0.25f, 0), Vector3.zero, Color.white);
        fishSize = 3;
    }

    void BuildSmallFish()
    {
        Color mainColor = GetRandomFishColor();
        Color accentColor = GetRandomFishColor();

        AddBrick(brick2x2Prefab, Vector3.zero, Vector3.zero, mainColor);
        AddBrick(brick1x2Prefab, new Vector3(0.75f, 0, 0), Vector3.zero, mainColor);
        AddBrick(slopeBrickPrefab, new Vector3(-1f, 0, 0), new Vector3(0, 180, 0), accentColor);
        AddBrick(slopeBrickPrefab, new Vector3(-1f, 0.3f, 0), new Vector3(0, 180, 180), accentColor);
        AddBrick(platePrefab, new Vector3(0, 0.5f, 0), new Vector3(0, 0, 45), accentColor);
        AddBrick(stud1x1Prefab, new Vector3(1f, 0.25f, 0.3f), Vector3.zero, Color.white);
        AddBrick(stud1x1Prefab, new Vector3(1f, 0.25f, -0.3f), Vector3.zero, Color.white);
        AddBrick(stud1x1Prefab, new Vector3(1.05f, 0.25f, 0.3f), Vector3.zero, Color.black);
        AddBrick(stud1x1Prefab, new Vector3(1.05f, 0.25f, -0.3f), Vector3.zero, Color.black);
        fishSize = 8;
    }

    void BuildMediumFish()
    {
        Color mainColor = GetRandomFishColor();
        Color bellyColor = Color.white;
        Color finColor = GetRandomFishColor();

        AddBrick(brick2x4Prefab, Vector3.zero, Vector3.zero, mainColor);
        AddBrick(brick2x4Prefab, new Vector3(0, -0.3f, 0), Vector3.zero, bellyColor);
        AddBrick(brick2x2Prefab, new Vector3(1.25f, 0, 0), Vector3.zero, mainColor);
        AddBrick(brick2x2Prefab, new Vector3(1.25f, -0.3f, 0), Vector3.zero, bellyColor);

        AddBrick(brick1x2Prefab, new Vector3(-1.5f, 0, 0), Vector3.zero, mainColor);
        AddBrick(slopeBrickPrefab, new Vector3(-2f, 0.25f, 0), new Vector3(0, 180, 0), finColor);
        AddBrick(slopeBrickPrefab, new Vector3(-2f, -0.25f, 0), new Vector3(0, 180, 180), finColor);

        AddBrick(platePrefab, new Vector3(0, 0.6f, 0), new Vector3(0, 0, 0), finColor);
        AddBrick(platePrefab, new Vector3(-0.5f, -0.6f, 0), new Vector3(0, 0, 0), finColor);
        AddBrick(platePrefab, new Vector3(0.5f, -0.6f, 0), new Vector3(0, 0, 0), finColor);

        AddBrick(stud1x1Prefab, new Vector3(1.75f, 0.15f, 0.5f), Vector3.zero, Color.white);
        AddBrick(stud1x1Prefab, new Vector3(1.75f, 0.15f, -0.5f), Vector3.zero, Color.white);

        fishSize = 15;
    }

    void BuildLargeFish()
    {
        fishSize = 30;
    }

    void BuildGiantFish()
    {
        fishSize = 50;
    }

    GameObject AddBrick(GameObject prefab, Vector3 localPos, Vector3 localRot, Color color)
    {
        if (prefab == null) return null;
        GameObject brick = Instantiate(prefab, transform);
        brick.transform.localPosition = localPos;
        brick.transform.localEulerAngles = localRot;

        Renderer rend = brick.GetComponent<Renderer>();
        if (rend != null)
        {
            Material mat = new Material(rend.material);
            mat.color = color;
            rend.material = mat;
        }

        assembledBricks.Add(brick);
        return brick;
    }

    Color GetRandomFishColor()
    {
        Color[] fishColors = {
            new Color(1f, 0.4f, 0.4f),
            new Color(1f, 0.6f, 0.2f),
            new Color(1f, 0.9f, 0.3f),
            new Color(0.4f, 0.8f, 0.4f),
            new Color(0.3f, 0.6f, 1f),
            new Color(0.6f, 0.3f, 0.8f),
            new Color(1f, 0.5f, 0.7f),
            new Color(0f, 0.8f, 0.8f),
        };
        return fishColors[Random.Range(0, fishColors.Length)];
    }

    public void BreakApart(Vector3 force)
    {
        if (breakSound != null) AudioSource.PlayClipAtPoint(breakSound, transform.position);

        foreach (GameObject brick in assembledBricks)
        {
            if (brick == null) continue;
            brick.transform.SetParent(null);

            Rigidbody rb = brick.GetComponent<Rigidbody>();
            if (rb == null) rb = brick.AddComponent<Rigidbody>();
            rb.AddForce(force + Random.insideUnitSphere * 3f, ForceMode.Impulse);
            rb.AddTorque(Random.insideUnitSphere * 5f, ForceMode.Impulse);

            var loose = brick.AddComponent<LooseBrick>();
            loose.Initialize(1);

            Destroy(brick, 5f);
        }

        SpawnStuds();

        Destroy(gameObject);
    }

    void SpawnStuds()
    {
        int studCount = Mathf.Max(1, fishSize / 2);
        for (int i = 0; i < studCount; i++)
        {
            Vector3 spawnPos = transform.position + Random.insideUnitSphere * 1.5f;
            if (stud1x1Prefab == null) continue;
            GameObject stud = Instantiate(stud1x1Prefab, spawnPos, Random.rotation);
            var cs = stud.AddComponent<CollectableStud>();
            cs.value = 1;

            Rigidbody rb = stud.GetComponent<Rigidbody>();
            if (rb == null) rb = stud.AddComponent<Rigidbody>();
            rb.AddForce(Random.insideUnitSphere * 5f, ForceMode.Impulse);
        }
    }
}
