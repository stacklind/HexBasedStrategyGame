using UnityEngine;

namespace HexagonUtils
{
    public static class Hexagon
    {
        public static void GenerateAndSaveMesh(float width, float height, string path)
        {
            Vector3[] vertices = new Vector3[7]
                {
                new Vector3(0               , 0, 0),                // Center
                new Vector3(0               , 0, -0.5f * height),   // Bottom
                new Vector3(-0.5f * width   , 0, -0.25f * height),  // Left-bottom
                new Vector3(-0.5f * width   , 0, 0.25f * height),   // Left-Top
                new Vector3(0               , 0, 0.5f * height),    // Top
                new Vector3(0.5f * width    , 0, 0.25f * height),   // Right-top
                new Vector3(0.5f * width    , 0, -0.25f * height)   // Right-bottom
                };
            int[] tris = new int[18]
                {
                0, 1, 2,
                0, 2, 3,
                0, 3, 4,
                0, 4, 5,
                0, 5, 6,
                0, 6, 1
                };
            Vector3[] normals = new Vector3[7]
                {
                Vector3.up,
                Vector3.up,
                Vector3.up,
                Vector3.up,
                Vector3.up,
                Vector3.up,
                Vector3.up
                };
            Vector2[] uv = new Vector2[7]
                {
                new Vector2(0.5f    , 0.5f),    // Center
                new Vector2(0.5f    , 0f),      // Bottom
                new Vector2(0f      , 0.25f),   // Left-bottom
                new Vector2(0f      , 0.75f),   // Left-Top
                new Vector2(0.5f    , 1f),      // Top
                new Vector2(1f      , 0.75f),   // Right-top
                new Vector2(1f      , 0.25f)    // Right-bottom
                };
            MeshInfo meshInfo = new MeshInfo(vertices, tris, normals, uv, "Hexagon");
            MeshGenerator.GenerateMesh(meshInfo, path);
        }
    }

    public struct HexInfo
    {
        public float Width;
        public float Height;

        public HexInfo(int size)
        {
            Height = size;
            Width = 2 * Mathf.Sqrt(Mathf.Pow(size / 2f, 2) - Mathf.Pow(size / 4f, 2));
        }
    }
}