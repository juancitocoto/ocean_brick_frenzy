using UnityEngine;

public class LooseBrick : MonoBehaviour
{
    public int brickValue = 1;
    public float attractSpeed = 5f;
    private bool collected = false;

    public void Initialize(int value)
    {
        brickValue = Mathf.Max(1, value);
        gameObject.tag = "Brick";
    }

    void OnTriggerEnter(Collider other)
    {
        if (collected) return;
        if (other.CompareTag("Player"))
        {
            var player = other.GetComponent<PlayerBrick>();
            if (player != null)
            {
                player.AddSize(brickValue);
                collected = true;
                Destroy(gameObject);
            }
        }
    }
}
