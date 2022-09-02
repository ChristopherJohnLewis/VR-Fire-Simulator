using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateHeight : MonoBehaviour
{
    public static void SetHeight(Terrain terrain, Texture2D heightData, Vector2 UVTopLeft, Vector2 UVBottomRight)
    {
        int widthResolution = terrain.terrainData.heightmapResolution - 1; //Power of 2 - 1
        int heightResolution = terrain.terrainData.heightmapResolution - 1; //Power of 2 - 1

        //format into what unity wants it to be
        Debug.Log("Reading in Height data...");
        float[,] formattedData = new float[widthResolution, heightResolution];
        Debug.Log("Reformatting Heightmap data...");

        //y/x to get ratio (with a check to prevent NaN errors)
        float ratio = (UVBottomRight.y - UVTopLeft.y) / (UVTopLeft.x == UVBottomRight.x ? 0.0001f : (UVBottomRight.x - UVTopLeft.x));
        float xTexture = 0.0f;
        float yTexture = 0.0f;
        for (int y = 0; y < heightResolution; y++)
        {
            for (int x = 0; x < widthResolution; x++)
            {
                //sample based on location
                xTexture = (x / (float)widthResolution) * (UVBottomRight.x - UVTopLeft.x) + UVTopLeft.x;
                yTexture = (y / (float)heightResolution) * (UVBottomRight.y - UVTopLeft.y) + UVTopLeft.y;
                formattedData[x, y] = (heightData.GetPixelBilinear(yTexture, xTexture).r - 20.0f) / 50.0f;
            }
        }

        //load
        Debug.Log("Applying heightmap data...");
        terrain.terrainData.SetHeights(0, 0, formattedData);
        Debug.Log("Height map application complete!");
    }
}
