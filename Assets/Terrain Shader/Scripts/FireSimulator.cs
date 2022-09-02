using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * This is the main fire simulation class which combines all of the functionalities of the
 * Fire simulation package into one control script. Use this to do modifications such as 
 * Showing/changing maps.
 */
public class FireSimulator : MonoBehaviour
{
    //Simulation resources
    public Terrain terrain;

    //Per simulation data
    public Texture2D burnMap;
    //TODO generate vegetationMap since we already have to parse it in from the provided fuel data
    public Texture2D vegetationMap;
    public Texture2D satelliteMap;
    public Texture2D heightMap;
    public TextAsset fuelData;
    public float startTime;
    public float stepTime = 0.1f;
    public float treeBlockSize = 1.8f;
    public float treeBaseScale = 0.25f;
    public Texture2D noise;

    //settings for billboard burn data
    public float fireSize = 2.4f;
    public float firePosition = 0.21f;
    public Color billboardBurnColor; 

    //shader variables
    public Shader billboardReplacement;
    public Camera mainCamera;

    //List of trees
    public List<GameObject> treePrototypes;

    //private data
    private float elapsedTime = 0.0f;
    private Material terrainMaterial;

    // Start is called before the first frame update
    void Start()
    {
        //Trees must be initialized before initializing simulation
        //TODO get terrain size from parsed data
        InitTrees(treePrototypes, burnMap, new Vector3(906, 800, 642));
        InitSimulation(burnMap, vegetationMap, satelliteMap, fuelData, new Vector3(906, 800, 642));
    }

    void FixedUpdate()
    {
        StepSim(1);
    }

    //Initial loading
    void InitSimulation(Texture2D burnMap, Texture2D vegetationMap, Texture2D satelliteMap, TextAsset fuelData, Vector3 terrainDimensions)
    {
        //Grab the Terrain material (should be the 2018 terrain material)
        terrainMaterial = terrain.materialTemplate;

        //load in necessary maps into the simulation
        terrainMaterial.SetTexture("_BurnMap", burnMap);
        terrainMaterial.SetTexture("_VegMap", vegetationMap);
        terrainMaterial.SetTexture("_SatelliteMap", satelliteMap);

        //Set global parameters so that the billboard shader can burn properly
        Shader.SetGlobalTexture("_BurnMap", burnMap);
        Shader.SetGlobalVector("_RelativePosition", terrainDimensions);
        Shader.SetGlobalFloat("_BillboardFireSize", fireSize);
        Shader.SetGlobalFloat("_BillboardFirePosition", firePosition);
        Shader.SetGlobalColor("_BillboardBurnColor", billboardBurnColor);
        Shader.SetGlobalTexture("_PerlinNoise", noise);

        //set terrain dimensions
        terrain.terrainData.size = terrainDimensions;

        //set terrain height
        GenerateHeight.SetHeight(terrain, heightMap, new Vector2(0, 0), new Vector2(1, 1));

        //set tree instances
        PopulateTrees.GenTrees(terrain, fuelData, treeBlockSize, new Color32(1, 1, 1, 1), treeBaseScale);
    }

    //Initialize all the trees to use in the simulation
    void InitTrees(List<GameObject> treeTypes, Texture2D burnMap, Vector3 terrainDimensions)
    {
        TreePrototype[] finalTreeList = new TreePrototype[treeTypes.Count];
        //preprocess step to initialize simulation data for trees
        for (int i = 0; i < treeTypes.Count; i++)
        {
            GameObject tree = treeTypes[i];
            TreePrototype treeProto = new TreePrototype
            {
                bendFactor = 0.3f,
                prefab = treeTypes[i]
            };

            //Get tree materials
            Material[] treeMat = treeProto.prefab.GetComponent<Renderer>().sharedMaterials;

            //Load in simulation data to trees
            treeMat[0].SetTexture("_BurnMap", burnMap);
            treeMat[0].SetVector("_RelativePosition", terrainDimensions);
            treeMat[1].SetTexture("_BurnMap", burnMap);
            treeMat[1].SetVector("_RelativePosition", terrainDimensions);

            //TODO these are currently locked to the fire line burn area for consistency, but may change later on
            treeMat[0].SetFloat("_FireSize", fireSize);
            treeMat[0].SetFloat("_FirePos", firePosition);
            treeMat[1].SetFloat("_FireSize", fireSize);
            treeMat[1].SetFloat("_FirePos", firePosition);

            //Push data to tree list
            finalTreeList[i] = treeProto;
        }

        //Push generated tree list to terrain
        terrain.terrainData.treePrototypes = finalTreeList;
        terrain.terrainData.RefreshPrototypes();
    }

    //Step simulation
    public void StepSim(int stepCount)
    {
        elapsedTime += stepTime * stepCount;
        Shader.SetGlobalFloat("_BurnProgress", elapsedTime);
    }
}
