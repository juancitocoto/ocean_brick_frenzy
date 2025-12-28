using UnityEngine;

public class CollectableStud : MonoBehaviour
{
    public int value = 1;
    public float lifeTime = 10f;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var player = other.GetComponent<PlayerBrick>();
            if (player != null)
            {
                player.AddSize(value);
                Destroy(gameObject);
            }
        }
    }
}
