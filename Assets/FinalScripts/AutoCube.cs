using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoCube : MonoBehaviour
{
    [SerializeField, HideInInspector]
    public int resolution = 10;
    MeshFilter[] meshFilters;
    MeshGenerator[] terrainFaces;

    void Update()
    {
        if(GameObject.Find("World")==null)
        {
            new GameObject("World");
            GameObject world = GameObject.Find("World");
            world.AddComponent<CubeToSphere>();
            world.AddComponent<SphereToCube>();

            Initialize();
            GenerateMesh();
        }
    }

    void Initialize()
    {
        if (meshFilters == null || meshFilters.Length == 0)
        {
            meshFilters = new MeshFilter[6];
        }
        terrainFaces = new MeshGenerator[6];

        Vector3[] directions = { Vector3.up, Vector3.down, Vector3.forward, Vector3.back, Vector3.left, Vector3.right };
        string[] directionsName = { "up", "down", "forward", "back", "left", "right" };

        for (int i = 0; i < 6; i++)
        {
            if (meshFilters[i] == null)
            {
                Transform world = GameObject.Find("World").transform;
                string meshName = "mesh " + directionsName[i];
                GameObject meshObj = new GameObject(meshName);
                meshObj.transform.parent = world;

                meshObj.AddComponent<MeshRenderer>().sharedMaterial = new Material(Shader.Find("Standard"));
                meshFilters[i] = meshObj.AddComponent<MeshFilter>();
                meshFilters[i].sharedMesh = new Mesh();
            }

            terrainFaces[i] = new MeshGenerator(meshFilters[i].sharedMesh, resolution, directions[i]);
        }
    }

    void GenerateMesh()
    {
        foreach (MeshGenerator face in terrainFaces)
        {
            face.CreateMeshCube();
        }
    }
}
