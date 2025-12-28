using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class BrickPiece : MonoBehaviour
{
    public int sizeX = 1;
    public int sizeY = 1;
    public BrickType brickType = BrickType.Single_1x1;
    public Color brickColor = Color.white;

    public enum BrickType
    {
        Single_1x1,
        Double_2x1,
        Quad_2x2,
        Long_4x1,
        Large_4x2,
        Mega_4x4
    }

    void Awake()
    {
        SetSizeFromType(brickType);
        ApplyRandomColor();
    }

    void OnValidate()
    {
        SetSizeFromType(brickType);
    }

    void SetSizeFromType(BrickType type)
    {
        switch (type)
        {
            case BrickType.Single_1x1: sizeX = 1; sizeY = 1; break;
            case BrickType.Double_2x1: sizeX = 2; sizeY = 1; break;
            case BrickType.Quad_2x2:   sizeX = 2; sizeY = 2; break;
            case BrickType.Long_4x1:   sizeX = 4; sizeY = 1; break;
            case BrickType.Large_4x2:  sizeX = 4; sizeY = 2; break;
            case BrickType.Mega_4x4:   sizeX = 4; sizeY = 4; break;
            default: sizeX = 1; sizeY = 1; break;
        }
    }

    void ApplyRandomColor()
    {
        brickColor = GetRandomBrickColor();
        var rend = GetComponent<Renderer>();
        if (rend != null && rend.material != null)
        {
            rend.material.color = brickColor;
        }
    }

    static Color GetRandomBrickColor()
    {
        Color[] brickColors = {
            Color.red,
            Color.blue,
            Color.yellow,
            Color.green,
            new Color(1f, 0.5f, 0f), // Orange
            Color.white,
            Color.black
        };
        return brickColors[Random.Range(0, brickColors.Length)];
    }
}
