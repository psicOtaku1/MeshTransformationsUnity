using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereToCube : MonoBehaviour
{
    [Range(2, 256)]
    public int resolution = 10;

    [Range(0, 1)]
    public int reload = 0;

    [SerializeField, HideInInspector]
    MeshFilter[] meshFilters;
    MeshGenerator[] terrainFaces;

    private void OnValidate()
    {
        Initialize();
        GenerateMesh();
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
            GameObject obj = GameObject.Find("World");
            Transform objTrans = obj.transform;
            string meshName = "mesh " + directionsName[i];
            Transform childTrans = objTrans.Find(meshName);
            meshFilters[i] = childTrans.GetComponent<MeshFilter>();
            terrainFaces[i] = new MeshGenerator(meshFilters[i].sharedMesh, resolution, directions[i]);
        }
    }

    void GenerateMesh()
    {
        foreach (MeshGenerator face in terrainFaces)
        {
            face.CreateMeshSphereToCube();
        }
    }
}
