using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    #region Variables

    public Texture2D map;

    public ColorToPrefab[] colorMappings;

    #endregion
    
    // Start is called before the first frame update
    void Start()
    {
        GenerateLevel();
    }

    private void GenerateLevel()
    {
        for (int x = 0; x < map.width; x++)
        {
            for (int y = 0; y < map.height; y++)
            {
                GenerateTile(x, y);
            }
        }
    }

    private void GenerateTile(int x, int y)
    {
        Color pixelColor = map.GetPixel(x, y);
        
        if (pixelColor.a == 0) return;

        foreach (ColorToPrefab colorMapping in colorMappings)
        {
            if (colorMapping.color.Equals(pixelColor))
            {
                Vector2 position = new Vector2(x, y);
                Instantiate(colorMapping.prefab, position, quaternion.identity, transform);
            }
        }
    }
}
