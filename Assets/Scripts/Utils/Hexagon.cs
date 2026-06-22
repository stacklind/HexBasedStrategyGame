using UnityEngine;

namespace HexagonUtils
{
    public static class Hexagon
    {
        public static MeshInfo GetMeshInfo(float width, float height, int elevation, int[] elevationDiffs)
        {
            
            float outerSide = 0.5f;
            float offset = 0.05f;
            float innerSide = outerSide - offset;

            Vector3[] vertices = new Vector3[19]
                {
                new Vector3(0,                  0, 0),                                          // 0    Center

                new Vector3(0,                  elevation, -innerSide * height),                // 1    Inner-Bottom
                new Vector3(-innerSide * width, elevation, -innerSide/2 * height),              // 2    Inner-Left-Bottom
                new Vector3(-innerSide * width, elevation, innerSide/2 * height),               // 3    Inner-Left-Top
                new Vector3(0,                  elevation, innerSide * height),                 // 4    Inner-Top
                new Vector3(innerSide * width,  elevation, innerSide/2 * height),               // 5    Inner-Right-Top
                new Vector3(innerSide * width,  elevation, -innerSide/2 * height),              // 6    Inner-Right-Bottom

                new Vector3(0,                  elevationDiffs[0],  -outerSide * height),       // 7    Outer-Bottom
                new Vector3(-outerSide/2,       elevationDiffs[1],  -3 * outerSide/4),          // 8    Outer-Bottom-Left-MidPoint
                new Vector3(-outerSide * width, elevationDiffs[2],  -outerSide/2 * height),     // 9    Outer-Left-Bottom
                new Vector3(-outerSide,         elevationDiffs[3],  0),                         // 10   Outer-Left-Midpoint
                new Vector3(-outerSide * width, elevationDiffs[4],  outerSide/2 * height),      // 11   Outer-Left-Top
                new Vector3(-outerSide/2,       elevationDiffs[5],  3 * outerSide/4),           // 12   Outer-Top-Left-MidPoint
                new Vector3(0,                  elevationDiffs[6],  outerSide * height),        // 13   Outer-Top
                new Vector3(outerSide/2,        elevationDiffs[7],  3 * outerSide/4),           // 14   Outer-Top-Right-MidPoint
                new Vector3(outerSide * width,  elevationDiffs[8],  outerSide/2 * height),      // 15   Outer-Right-Top
                new Vector3(outerSide,          elevationDiffs[9],  0),                         // 16   Outer-Right-MidPoint
                new Vector3(outerSide * width,  elevationDiffs[10], -outerSide/2 * height),     // 17   Outer-Right-Bottom
                new Vector3(outerSide/2,        elevationDiffs[11], -3 * outerSide/4)           // 18   Outer-Bottom-Right-MidPoint
                };
            int[] tris = new int[72]
                {
                // Inner triangles
                0,  1,  2,
                0,  2,  3,
                0,  3,  4,
                0,  4,  5,
                0,  5,  6,
                0,  6,  1,

                // Outer triangles
                // Bottom-Left
                1,  7,  8,
                1,  8,  2,
                8,  9,  2,
                // Left
                8,  9,  3,
                3,  9,  4,
                9,  10, 4,
                // Top-Left
                4,  10, 5,
                10, 11, 5,
                5,  11, 6,
                // Top-Right
                11, 12, 6,
                6,  12, 1,
                12, 7,  1,
                // Right
                4,  10, 5,
                10, 11, 5,
                5,  11, 6,
                // Bottom-Right
                11, 12, 6,
                6,  12, 1,
                12, 7,  1
                };
            Vector3[] normals = new Vector3[13]
                {
                Vector3.up,
                Vector3.up,
                Vector3.up,
                Vector3.up,
                Vector3.up,
                Vector3.up,
                Vector3.up,
                Vector3.up,
                Vector3.up,
                Vector3.up,
                Vector3.up,
                Vector3.up,
                Vector3.up
                };
            Vector2[] uv = new Vector2[13]
                {
                new Vector2(outerSide,  outerSide),                                     // Center

                new Vector2(outerSide,                  1-(outerSide+innerSide)),       // Inner-Bottom
                new Vector2(1-(outerSide+innerSide),    (1-innerSide)/2),               // Inner-Left-bottom
                new Vector2(1-(outerSide+innerSide),    1- (1-innerSide)/2),            // Inner-Left-Top
                new Vector2(outerSide,                  outerSide+innerSide),           // Inner-Top
                new Vector2(outerSide + innerSide,      1 - (1-innerSide)/2),           // Inner-Right-top
                new Vector2(outerSide + innerSide,      (1-innerSide)/2),               // Inner-Right-bottom

                new Vector2(0.5f,       0f),                                            // Outer-Bottom
                new Vector2(0f,         0.25f),                                         // Outer-Left-bottom
                new Vector2(0f,         0.75f),                                         // Outer-Left-Top
                new Vector2(0.5f,       1f),                                            // Outer-Top
                new Vector2(1f,         0.75f),                                         // Outer-Right-top
                new Vector2(1f,         0.25f)                                          // Outer-Right-bottom
                };
            MeshInfo meshInfo = new MeshInfo(vertices, tris, normals, uv, "Hexagon");

            return meshInfo;
        }
    }

    public struct HexInfo
    {
        public float Width;
        public float Height;

        public HexInfo(int size)
        {
            Height = size;
            Width = size * 0.866f;
        }
    }
}