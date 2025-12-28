using UnityEngine;

public class PlayerBrick : MonoBehaviour
{
    [Tooltip("Aggregated size value representing the player's current brick mass or level.")]
    public int currentSize = 1;

    [Tooltip("Player movement speed used by powerups and movement code.")]
    public float moveSpeed = 5f;

    [HideInInspector]
    public bool isShielded = false;

    void Awake()
    {
        if (gameObject.tag != "Player") gameObject.tag = "Player";
    }

    public void AddSize(int amount)
    {
        currentSize = Mathf.Max(0, currentSize + amount);
    }

    public void SetSize(int size)
    {
        currentSize = Mathf.Max(0, size);
    }
}
