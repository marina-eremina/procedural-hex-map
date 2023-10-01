using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class PoissonExample : MonoBehaviour
{
    [Range(1,5)]
    public int dotSize = 2;

    [Range(5, 30)]
    public int minimumDistanceMultiplier = 30;

    public Color backgroundColor = Color.white;
    public Color dotColor = Color.black;

    readonly int imageSize = 512;
    readonly int iterationPerPoint = 0;

    float minimumDistance;

    void Start()
    {
        GenerateImage();
    }

    public void DeleteImage()
    {
        Sprite sprite = GetComponent<SpriteRenderer>().sprite;

        if (sprite != null)
        {
            DestroyImmediate(sprite.texture);
            DestroyImmediate(sprite);
            GetComponent<SpriteRenderer>().sprite = null;
        }
    }

    [ContextMenu("Generate Image")]
    public void GenerateImage()
    {
        DeleteImage();
        minimumDistance = dotSize * minimumDistanceMultiplier;

        Texture2D texture = new Texture2D(imageSize, imageSize);
        texture.filterMode = FilterMode.Point;

        // Fill the texture with the background color
        for (int y = 0; y < texture.height; y++)
        {
            for (int x = 0; x < texture.width; x++)
            {
                texture.SetPixel(x, y, backgroundColor);
            }
        }

        // Get the sample points using the Fast Poisson Disk Sampling algorithm
        Vector2 bottomLeft = Vector2.zero;
        Vector2 topRight = new(imageSize, imageSize);
        List<Vector2> samplePoints = PoissonDisk.Sampling(bottomLeft, topRight, minimumDistance, iterationPerPoint);

        // Draw the dots
        foreach (Vector2 point in samplePoints)
        {
            DrawDot(texture, (int)point.x, (int)point.y, dotSize, dotColor);
        }

        // Apply the changes to the texture and create a sprite
        texture.Apply();
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, imageSize, imageSize), new Vector2(0.5f, 0.5f));

        // Assign the sprite to a SpriteRenderer
        GetComponent<SpriteRenderer>().sprite = sprite;
    }

    // Draw a dot at a specific position in the texture
    void DrawDot(Texture2D texture, int x, int y, int size, Color color)
    {
        for (int i = -size; i <= size; i++)
        {
            for (int j = -size; j <= size; j++)
            {
                int px = x + i;
                int py = y + j;
                if (px >= 0 && px < texture.width && py >= 0 && py < texture.height)
                {
                    texture.SetPixel(px, py, color);
                }
            }
        }
    }
}
