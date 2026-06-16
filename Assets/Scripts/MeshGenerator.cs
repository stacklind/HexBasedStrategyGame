using UnityEditor;
using UnityEngine;

public class MeshGenerator
{
    public static void GenerateMesh(MeshInfo meshInfo, string path)
    {
        Mesh mesh = new Mesh();
        mesh.vertices = meshInfo.Vertices;
        mesh.triangles = meshInfo.Tris;
        mesh.normals = meshInfo.Normals;
        mesh.uv = meshInfo.UV;
        AssetDatabase.CreateAsset(mesh, $"{path}{meshInfo.Name}.mesh");
    }
}

public struct MeshInfo
{
    public Vector3[] Vertices { get; }
    public int[] Tris { get; }
    public Vector3[] Normals { get; }
    public Vector2[] UV { get; }

    public string Name { get; }

    public MeshInfo(Vector3[] vertices, int[] tris, Vector3[] normals, Vector2[] uv, string name)
    {
        Vertices = vertices;
        Tris = tris;
        Normals = normals;
        UV = uv;
        Name = name;
    }
}