using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopulateTrees : MonoBehaviour
{
    public static void GenTrees(Terrain target, TextAsset rawFuelData, float treeBlockMultiplier, Color32 lightColor, float baseScale)
    {
        //TODO probably make this react to the vegetation map we generated before
        Dictionary<int, float> fuelDensity = new Dictionary<int, float>
        {
            { 99, 0.0f },
            { 102, 15.0f },
            { 165, 100.0f },
            { 6, 30.0f },
            { 141, 22.0f },
            { 142, 82.0f },
            { 98, 0.0f },
            { 103, 15.0f }
        };

        Dictionary<int, int> treeType = new Dictionary<int, int>
        {
            { 99, 0 },
            { 102, 0 },
            { 165, 2 },
            { 6, 1 },
            { 141, 0 },
            { 142, 0 },
            { 98, 0 },
            { 103, 0 }
        };

        //Clear all tree instances
        target.terrainData.treeInstances = new List<TreeInstance>().ToArray();

        //parse in raw fuel data and generate the fuel load index map
        Debug.Log("Parsing " + rawFuelData.name + "...");
        FuelData parsedData = GenerateMaps.ParseFuelData(rawFuelData.text);
        Debug.Log(rawFuelData.name + " parsing complete.");
        Debug.Log("Generating index map...");
        Texture2D indexMap = GenerateMaps.GenerateIndexMap(parsedData.width, parsedData.height, parsedData.data);
        Debug.Log("Fuel load index map generation complete.");

        //get terrain info
        TerrainData info = target.terrainData;
        Vector3 size = info.size;

        //populate each cell in the index map
        for (float y = 0; y < size.z; y += treeBlockMultiplier)
        {
            for (float x = 0; x < size.x; x += treeBlockMultiplier)
            {

                //get block spawning range
                Vector3 min = new Vector3(x / size.x, 0, y / size.z);
                Vector3 max = new Vector3((x + treeBlockMultiplier) / size.x, 0, (y + treeBlockMultiplier) / size.z);
                Vector3 worldPos = new Vector3(Random.Range(min.x, max.x), 0, Random.Range(min.z, max.z));

                //get spawn rate based on index
                //Debug.Log(indexMap.GetPixel(x, y).r);
                Vector2 pixelSample = WorldToPixel(parsedData, worldPos);
                int pixelIndex = Mathf.RoundToInt(indexMap.GetPixel(Mathf.RoundToInt(pixelSample.x), Mathf.RoundToInt(pixelSample.y)).r);
                float chance = fuelDensity[pixelIndex];

                if (ShouldISpawn(chance))
                {
                    TreeInstance newTree = new TreeInstance()
                    {
                        position = worldPos,
                        color = Color.white,
                        prototypeIndex = treeType[pixelIndex],
                        heightScale = RandDev((chance / 100.0f + 0.1f) * baseScale, 0.2f * baseScale),
                        widthScale = RandDev((chance / 100.0f + 0.1f) * baseScale, 0.2f * baseScale),
                        lightmapColor = Color.white
                    };

                    target.AddTreeInstance(newTree);
                }
            }
        }
        target.Flush();

        //place trees based on index map and terrain UV coordinates
    }

    //helper function to convert from Texture pixels to terrain cells (top left position)
    static Vector3 PixeltoWorld(FuelData texture, Terrain terrain, float x, float y)
    {
        //get position in UV
        Vector2 terrainPos = new Vector2(x / texture.width, y / texture.height);
        return new Vector3(terrainPos.x, 0.0f, terrainPos.y);
    }

    static bool ShouldISpawn(float chance)
    {
        float roll = Random.Range(0.0f, 100.0f);
        if (roll <= chance)
        {
            return true;
        } else
        {
            return false;
        }
    }

    static float RandDev(float point, float deviation)
    {
        return Random.Range(point - deviation, point + deviation);
    }

    //Converts a UV world coordinate to an X,Y position in the index data
    //the vector3 is stored from 0.0f to 1.0f
    static Vector2 WorldToPixel(FuelData texture, Vector3 position)
    {
        return new Vector2(position.x * texture.width, position.z * texture.height);
    }

}
