using UnityEngine;

public class VisualBrickBuilder : MonoBehaviour
{
    // Dynamically build player visual based on absorbed bricks

    public void AddBrickToStructure(BrickPiece brick, Transform playerTransform)
    {
        Vector3 attachPoint = FindNearestAttachPoint(playerTransform);
        brick.transform.position = SnapToGrid(attachPoint);
        brick.transform.SetParent(playerTransform);
        CreateStudConnection(brick.transform.position);
    }

    Vector3 FindNearestAttachPoint(Transform playerTransform)
    {
        // Simplified: attach on top of player
        return playerTransform.position + Vector3.up * 0.5f;
    }

    Vector3 SnapToGrid(Vector3 position)
    {
        float gridSize = 0.5f; // BRICK-like grid
        return new Vector3(
            Mathf.Round(position.x / gridSize) * gridSize,
            Mathf.Round(position.y / gridSize) * gridSize,
            Mathf.Round(position.z / gridSize) * gridSize
        );
    }

    void CreateStudConnection(Vector3 pos)
    {
        // Optional visual effect: spawn particle or small gizmo.
    }
}
