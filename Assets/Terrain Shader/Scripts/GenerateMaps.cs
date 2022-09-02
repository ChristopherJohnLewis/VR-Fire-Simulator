using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public struct BurnData
{
    public float topLeftLat;
    public float topleftLong;
    public float bottomRightLat;
    public float bottomRightLong;
    public int width;
    public int height;
    public float maxVal;
    public float unburnableVal;
    public float[] data;
}

public struct FuelData
{
    public float xllcorner;
    public float yllcorner;
    public int width;
    public int height;
    public float cellSize;
    public int noDataVal;
    public int[] data;
}

public struct HeightData
{
    public float xllcorner;
    public float yllcorner;
    public int width;
    public int height;
    public float cellSize;
    public float[] data;
}

public class GenerateMaps : MonoBehaviour
{
    //TODO TEMPORARY: move value to a different script
    public TextAsset burnData;

    // Start is called before the first frame update
    void Start()
    {
        //TODO TEMPORARY: move to a different script
        //BurnData parsedData = ParseBurnData(burnData.text);
        //Debug.Log("Burn data parse completed!");
        //Texture2D burnMap = GenerateBurnMap(parsedData.width, parsedData.height, parsedData.data);
        //Debug.Log("Burn texture generated!");

        //WARNING: only maps the data for kyle canyon, not all values
        Dictionary<int, float> fuelDensity = new Dictionary<int, float>();
        fuelDensity.Add(99, 0.0f);
        fuelDensity.Add(102, 15.0f);
        fuelDensity.Add(165, 100.0f);
        fuelDensity.Add(6, 30.0f);
        fuelDensity.Add(141, 22.0f);
        fuelDensity.Add(142, 32.0f);
        fuelDensity.Add(98, 0.0f);
        fuelDensity.Add(103, 15.0f);

        Dictionary<int, float> fuelHumidity = new Dictionary<int, float>();
        fuelHumidity.Add(99, 0.0f);
        fuelHumidity.Add(102, 15.0f);
        fuelHumidity.Add(165, 30.0f);
        fuelHumidity.Add(6, 65.0f);
        fuelHumidity.Add(141, 30.0f);
        fuelHumidity.Add(142, 30.0f);
        fuelHumidity.Add(98, 100.0f);
        fuelHumidity.Add(103, 80.0f);
        //FuelData parsedData = ParseFuelData(burnData.text);
        //Debug.Log("Vegetation data parse completed!");
        //Texture2D indexMap = GenerateIndexMap(parsedData.width, parsedData.height, parsedData.data);
        //Debug.Log("Vegetation texture generated!");
        //SaveMap(indexMap, "vegetation", "Terrain Shader/Scripts/testData");

        HeightData parsedData = ParseHeightData(burnData.text);
        Debug.Log("height data parse completed!");
        Texture2D heightMap = GenerateHeightMap(parsedData.width, parsedData.height, parsedData.data);
        Debug.Log("Burn texture generated!");
        SaveMap(heightMap, "KyleHeightMap", "");
    }

    // Create a burn map as a red channel float texture
    public static Texture2D GenerateBurnMap(int width, int height, float[] data)
    {
        return GenerateMap(width, height, data, TextureFormat.RFloat);
    }

    // Create a float height map as a red channel float texture
    public static Texture2D GenerateHeightMap(int width, int height, float[] data)
    {
        return GenerateMap(width, height, data, TextureFormat.RFloat);
    }

    // Create an index map as a red channel int texture
    public static Texture2D GenerateIndexMap(int width, int height, int[] data)
    {
        Texture2D map = new Texture2D(width, height, TextureFormat.RFloat, false);
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                map.SetPixel(x, y, new Color(data[x + y * width], 0.0f, 0.0f));
            }
        }
        // Apply all SetPixel calls
        map.Apply();

        return map;
    }

    //  Create a wind map as a red green channel float texture
    public static Texture2D GenerateWindMap(int width, int height, float[] data)
    {
        return GenerateMap(width, height, data, TextureFormat.RGFloat);
    }

    //  Create a vegetation map as an RGB texture
    public static Texture2D GenerateVegMap(int width, int height, int[] data, Dictionary<int, float> fuelDensity, Dictionary<int, float> fuelHumidity)
    {
        Texture2D map = new Texture2D(width, height, TextureFormat.RGBA32, false);
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int treeIndex = data[x + y * width];
                //set rocky areas
                if (data[x + y * width] == 99)
                {
                    map.SetPixel(x, y, new Color(fuelHumidity[treeIndex] / 100.0f, fuelDensity[treeIndex] / 100.0f, 1.0f, 1.0f));
                } else
                {
                    map.SetPixel(x, y, new Color(fuelHumidity[treeIndex] / 100.0f, fuelDensity[treeIndex] / 100.0f, 0.0f, 1.0f));
                }
                
            }
        }
        // Apply all SetPixel calls
        map.Apply();

        return map;
    }

    //TODO we should probably parallelize this
    // Generate a map to be used for the terrain shader
    public static Texture2D GenerateMap(int width, int height, float[] data, TextureFormat format)
    {
        Texture2D map = new Texture2D(width, height, format, false);
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                map.SetPixel(x, y, new Color(data[x + y * width], 0.0f, 0.0f));
            }
        }
        // Apply all SetPixel calls
        map.Apply();

        return map;
    }

    public static BurnData ParseBurnData(string filename)
    {
        BurnData data = new BurnData();

        //WARNING: this is specifically made to the (jank) specifications of VFireLib
        //changing the format will completely break this function
        //Parse in all metadata
        string[,] splitData = CSVReader.SplitCsvGrid(filename);
        data.topLeftLat = float.Parse(splitData[0, 0].Remove(0, 14));
        data.topleftLong = float.Parse(splitData[0, 1].Remove(0, 15));
        data.bottomRightLat = float.Parse(splitData[0, 2].Remove(0, 18));
        data.bottomRightLong = float.Parse(splitData[0, 3].Remove(0, 19));
        data.height = int.Parse(splitData[0, 4].Remove(0, 9));
        data.width = int.Parse(splitData[0, 5].Remove(0, 9));
        data.maxVal = float.Parse(splitData[0, 6].Remove(0, 9));
        data.unburnableVal = float.Parse(splitData[0, 7].Remove(0, 12));
        data.data = new float[data.width * data.height];

        //parse in time of arrival
        for (int y = 0; y < data.height; y++)
        {
            for (int x = 0; x < data.width; x++)
            {
                //skip in case we accidentally go out of bounds
                if (splitData[x, y + 8] != null)
                {
                    data.data[x + y * data.width] = float.Parse(splitData[x, y + 8]);
                }
            }
        }

        return data;
    }

    public static FuelData ParseFuelData(string filename)
    {
        FuelData data = new FuelData();

        //WARNING: this is specifically made to the (jank) specifications of VFireLib
        //changing the format will completely break this function
        //Parse in all metadata
        string[,] splitData = CSVReader.SplitCsvGrid(filename, true);
        data.width = int.Parse(splitData[1, 0]);
        data.height = int.Parse(splitData[1, 1]);
        data.xllcorner = float.Parse(splitData[1, 2]);
        data.yllcorner = float.Parse(splitData[1, 3]);
        data.cellSize = float.Parse(splitData[1, 4]);
        data.noDataVal = int.Parse(splitData[1, 5]);
        data.data = new int[data.width * data.height];
        //parse in allData
        for (int y = 0; y < data.height; y++)
        {
            for (int x = 0; x < data.width; x++)
            {
                //skip in case we accidentally go out of bounds
                if (splitData[x, y + 6] != null)
                {
                    //parse in as an int to get tree index
                    data.data[x + y * data.width] = int.Parse(splitData[x, y + 6]);
                }
            }
        }
        return data;
    }

    public static HeightData ParseHeightData(string filename)
    {
        HeightData data = new HeightData();

        //WARNING: this is specifically made to the (jank) specifications of VFireLib
        //changing the format will completely break this function
        //Parse in all metadata
        
        string[,] splitData = CSVReader.SplitCsvGrid(filename, true);
        string[] dataData = CSVReader.ReadHeightData(filename);
        data.width = int.Parse(splitData[1, 0]);
        data.height = int.Parse(splitData[1, 1]);
        data.xllcorner = float.Parse(splitData[1, 2]);
        data.yllcorner = float.Parse(splitData[1, 3]);
        data.cellSize = float.Parse(splitData[1, 4]);
        data.data = new float[data.width * data.height];
        //parse in allData
        Debug.Log(dataData[1].Substring(0, dataData[1].Length - 5));
        for (int i = 1, j = 1; i < data.height * data.width; i++, j++)
        {
            try
            {
                if (dataData[i].Length > 0)
                {
                    if (dataData[i].Length < 8)
                    {
                        data.data[j] = float.Parse(dataData[i]);
                    }
                    else
                    {
                        data.data[j] = float.Parse(dataData[i]);
                    }
                } else
                {
                    Debug.Log("Empty entry at index " + i + ", skipping");
                    j--;
                }
            } catch (Exception e)
            {
                Debug.LogError(e);
                Debug.LogError("Value: " + dataData[i] + ", " + i);
                throw new Exception();
            }
        }

        return data;
    }

    //Saves the generated burn map to a file
    public static void SaveMap(Texture2D burnMap, string filename, string directory)
    {
        byte[] bytes = burnMap.EncodeToEXR(Texture2D.EXRFlags.OutputAsFloat);
        File.WriteAllBytes(Application.dataPath + "/" + directory + "/" + filename + ".exr", bytes);
        Debug.Log(filename + ".exr Saved to " + Application.dataPath + directory);
    }

    void SaveMapPng(Texture2D map, string filename, string directory)
    {
        byte[] bytes = map.EncodeToPNG();
        File.WriteAllBytes(Application.dataPath + "/" + directory + "/" + filename + ".png", bytes);
        Debug.Log(filename + ".png Saved to " + Application.dataPath + directory);
    }
}
