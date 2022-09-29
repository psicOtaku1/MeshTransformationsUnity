using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshGenerator : MonoBehaviour
{
    Mesh mesh;
    int resolution;
    Vector3 localUp;
    Vector3 axisA;
    Vector3 axisB;

    public MeshGenerator(Mesh mesh, int resolution, Vector3 localUp)
    {
        this.mesh = mesh;
        this.resolution = resolution;
        this.localUp = localUp;

        axisA = new Vector3(localUp.y, localUp.z, localUp.x);
        axisB = Vector3.Cross(localUp, axisA);
    }

    public void CreateMeshCube()
    {
        Vector3[] vertices = new Vector3[resolution * resolution];
        int[] triangles = new int[(resolution - 1) * (resolution - 1) * 6];
        int triIndex = 0;

        for (int y = 0; y < resolution; y++)
        {
            for (int x = 0; x < resolution; x++)
            {
                int i = x + y * resolution;
                Vector2 percent = new Vector2(x, y) / (resolution - 1);
                Vector3 pointOnCube = localUp + (percent.x - .5f) * 2 * axisA + (percent.y - .5f) * 2 * axisB;
                vertices[i] = pointOnCube;

                if (x != resolution - 1 && y != resolution - 1)
                {
                    triangles[triIndex] = i;
                    triangles[triIndex + 1] = i + resolution + 1;
                    triangles[triIndex + 2] = i + resolution;

                    triangles[triIndex + 3] = i;
                    triangles[triIndex + 4] = i + 1;
                    triangles[triIndex + 5] = i + resolution + 1;
                    triIndex += 6;
                }
            }
        }
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateBounds();
    }
    public void CreateMeshCubeToSphere()
    {
        Vector3[] vertices = new Vector3[resolution * resolution];
        int[] triangles = new int[(resolution - 1) * (resolution - 1) * 6];
        int triIndex = 0;

        for (int y = 0; y < resolution; y++)
        {
            for (int x = 0; x < resolution; x++)
            {
                int i = x + y * resolution;
                Vector2 percent = new Vector2(x, y) / (resolution - 1);
                Vector3 pointOnCube = localUp + (percent.x - .5f) * 2 * axisA + (percent.y - .5f) * 2 * axisB;
                Vector3 pointOnUnitSphere = TransformCubePointToSpherePoint(pointOnCube);
                vertices[i] = pointOnUnitSphere;


                if (x != resolution - 1 && y != resolution - 1)
                {
                    triangles[triIndex] = i;
                    triangles[triIndex + 1] = i + resolution + 1;
                    triangles[triIndex + 2] = i + resolution;

                    triangles[triIndex + 3] = i;
                    triangles[triIndex + 4] = i + 1;
                    triangles[triIndex + 5] = i + resolution + 1;
                    triIndex += 6;
                }
            }
        }
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateBounds();
    }

    public void CreateMeshSphereToCube()
    {
        Vector3[] vertices = new Vector3[resolution * resolution];
        int[] triangles = new int[(resolution - 1) * (resolution - 1) * 6];
        int triIndex = 0;

        for (int y = 0; y < resolution; y++)
        {
            for (int x = 0; x < resolution; x++)
            {
                int i = x + y * resolution;
                Vector2 percent = new Vector2(x, y) / (resolution - 1);
                Vector3 pointOnCube = localUp + (percent.x - .5f) * 2 * axisA + (percent.y - .5f) * 2 * axisB;
                Vector3 pointOnSphere = TransformCubePointToSpherePoint(pointOnCube);
                Vector3 pointOnUnitCube = TransformSpherePointToCubePoint(pointOnSphere);
                vertices[i] = pointOnUnitCube;


                if (x != resolution - 1 && y != resolution - 1)
                {
                    triangles[triIndex] = i;
                    triangles[triIndex + 1] = i + resolution + 1;
                    triangles[triIndex + 2] = i + resolution;

                    triangles[triIndex + 3] = i;
                    triangles[triIndex + 4] = i + 1;
                    triangles[triIndex + 5] = i + resolution + 1;
                    triIndex += 6;
                }
            }
        }
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateBounds();
    }

    public static Vector3 TransformCubePointToSpherePoint(Vector3 p)
    {
        float x2 = p.x * p.x / 2;
        float y2 = p.y * p.y / 2;
        float z2 = p.z * p.z / 2;
        float x = p.x * Mathf.Sqrt(1 - y2 - z2 + (p.y * p.y * p.z * p.z) / 3);
        float y = p.y * Mathf.Sqrt(1 - z2 - x2 + (p.x * p.x * p.z * p.z) / 3);
        float z = p.z * Mathf.Sqrt(1 - x2 - y2 + (p.x * p.x * p.y * p.y) / 3);
        return new Vector3(x, y, z);
    }

    public static Vector3 TransformSpherePointToCubePoint(Vector3 p)
    {
        float absX = Mathf.Abs(p.x);
        float absY = Mathf.Abs(p.y);
        float absZ = Mathf.Abs(p.z);

        if (absY >= absX && absY >= absZ)
        {
            return CubifyFace(p);
        }
        else if (absX >= absY && absX >= absZ)
        {
            p = CubifyFace(new Vector3(p.y, p.x, p.z));
            return new Vector3(p.y, p.x, p.z);
        }
        else
        {
            p = CubifyFace(new Vector3(p.x, p.z, p.y));
            return new Vector3(p.x, p.z, p.y);
        }
    }
    static Vector3 CubifyFace(Vector3 p)
    {
        const float inverseSqrt2 = 0.70710676908493042f;

        float a2 = p.x * p.x * 2.0f;
        float b2 = p.z * p.z * 2.0f;
        float inner = -a2 + b2 - 3;
        float innersqrt = -Mathf.Sqrt((inner * inner) - 12.0f * a2);

        if (p.x != 0)
        {
            p.x = Mathf.Min(1, Mathf.Sqrt(innersqrt + a2 - b2 + 3.0f) * inverseSqrt2) * Mathf.Sign(p.x);
        }

        if (p.z != 0)
        {
            p.z = Mathf.Min(1, Mathf.Sqrt(innersqrt - a2 + b2 + 3.0f) * inverseSqrt2) * Mathf.Sign(p.z);
        }

        // Top/bottom face
        p.y = Mathf.Sign(p.y);

        return p;
    }
}
