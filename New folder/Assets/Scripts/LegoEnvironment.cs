using UnityEngine;

public class BrickEnvironment : MonoBehaviour
{
    [Header("Environment Prefabs")]
    public GameObject[] coralPrefabs;
    public GameObject[] seaweedPrefabs;
    public GameObject[] rockPrefabs;
    public GameObject[] treasurePrefabs;
    public GameObject sandPlatePrefab;
    public GameObject waterBrickPrefab;

    [Header("Environment Settings")]
    public Vector2 environmentSize = new Vector2(50, 30);
    public int coralCount = 20;
    public int seaweedCount = 30;
    public int rockCount = 15;

    void Start()
    {
        BuildEnvironment();
    }

    void BuildEnvironment()
    {
        BuildSeaFloor();
        BuildCorals();
        BuildSeaweed();
        BuildRocks();
        BuildBubbles();
        AddDecorations();
    }

    void BuildSeaFloor()
    {
        int tilesX = Mathf.CeilToInt(environmentSize.x / 2f);
        int tilesZ = Mathf.CeilToInt(environmentSize.y / 2f);

        for (int x = -tilesX; x < tilesX; x++)
        {
            for (int z = -tilesZ; z < tilesZ; z++)
            {
                Vector3 pos = new Vector3(x * 2f, -10f, z * 2f);
                if (sandPlatePrefab == null) continue;
                GameObject plate = Instantiate(sandPlatePrefab, pos, Quaternion.identity, transform);

                Color sandColor = new Color(
                    0.9f + Random.Range(-0.1f, 0.1f),
                    0.8f + Random.Range(-0.1f, 0.1f),
                    0.6f + Random.Range(-0.1f, 0.1f)
                );
                var rend = plate.GetComponent<Renderer>();
                if (rend != null) rend.material.color = sandColor;
            }
        }
    }

    void BuildCorals()
    {
        if (coralPrefabs == null || coralPrefabs.Length == 0) return;
        for (int i = 0; i < coralCount; i++)
        {
            Vector3 pos = GetRandomFloorPosition();
            GameObject coral = Instantiate(
                coralPrefabs[Random.Range(0, coralPrefabs.Length)],
                pos,
                Quaternion.Euler(0, Random.Range(0, 360), 0),
                transform
            );

            SetCoralColor(coral);
        }
    }

    void SetCoralColor(GameObject coral)
    {
        Color[] coralColors = {
            new Color(1f, 0.4f, 0.5f),
            new Color(1f, 0.6f, 0.3f),
            new Color(0.8f, 0.4f, 0.8f),
            new Color(1f, 0.9f, 0.5f),
            new Color(0.4f, 0.9f, 0.9f),
        };

        Color chosenColor = coralColors[Random.Range(0, coralColors.Length)];

        foreach (Renderer rend in coral.GetComponentsInChildren<Renderer>())
        {
            rend.material.color = chosenColor;
        }
    }

    void BuildSeaweed()
    {
        if (seaweedPrefabs == null || seaweedPrefabs.Length == 0) return;
        for (int i = 0; i < seaweedCount; i++)
        {
            Vector3 pos = GetRandomFloorPosition();
            GameObject seaweed = Instantiate(
                seaweedPrefabs[Random.Range(0, seaweedPrefabs.Length)],
                pos,
                Quaternion.identity,
                transform
            );

            seaweed.AddComponent<SeaweedSway>();
        }
    }

    void BuildRocks()
    {
        if (rockPrefabs == null || rockPrefabs.Length == 0) return;
        for (int i = 0; i < rockCount; i++)
        {
            Vector3 pos = GetRandomFloorPosition();
            GameObject rock = Instantiate(
                rockPrefabs[Random.Range(0, rockPrefabs.Length)],
                pos,
                Quaternion.Euler(0, Random.Range(0, 360), 0),
                transform
            );

            Color rockColor = new Color(
                0.5f + Random.Range(-0.1f, 0.1f),
                0.5f + Random.Range(-0.1f, 0.1f),
                0.5f + Random.Range(-0.1f, 0.1f)
            );

            foreach (Renderer rend in rock.GetComponentsInChildren<Renderer>())
            {
                rend.material.color = rockColor;
            }
        }
    }

    void BuildBubbles()
    {
        for (int i = 0; i < 10; i++)
        {
            Vector3 pos = GetRandomFloorPosition();
            GameObject bubbleEmitter = new GameObject("BubbleEmitter");
            bubbleEmitter.transform.position = pos;
            bubbleEmitter.transform.SetParent(transform);
            bubbleEmitter.AddComponent<BubbleEmitter>();
        }
    }

    void AddDecorations()
    {
        if (treasurePrefabs == null || treasurePrefabs.Length == 0) return;
        for (int i = 0; i < 5; i++)
        {
            Vector3 pos = GetRandomFloorPosition();
            Instantiate(
                treasurePrefabs[Random.Range(0, treasurePrefabs.Length)],
                pos,
                Quaternion.Euler(0, Random.Range(0, 360), 0),
                transform
            );
        }
    }

    Vector3 GetRandomFloorPosition()
    {
        return new Vector3(
            Random.Range(-environmentSize.x / 2, environmentSize.x / 2),
            -9.5f,
            Random.Range(-environmentSize.y / 2, environmentSize.y / 2)
        );
    }
}
