using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunSimulation : MonoBehaviour
{
    public Material terrainMaterial;
    public float startTime = 0.0f;
    public float stepTime = 0.1f;
    private float elapsedTime = 0.0f;
    // Start is called before the first frame update
    void Start()
    {
        elapsedTime = startTime;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        elapsedTime += stepTime;
        terrainMaterial.SetFloat("_BurnProgress", elapsedTime);
    }
}
