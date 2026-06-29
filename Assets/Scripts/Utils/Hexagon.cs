using UnityEngine;

namespace HexagonUtils
{
    public static class Hexagon
    {
        public static MeshInfo GetMeshInfo(float width, float height)
        {
            
            float outerSide = 0.5f;
            float offset = 0.2f;
            float innerSide = outerSide - offset;

            Vector3[] vertices = new Vector3[19]
                {
                //          X                       Y   Z
                new Vector3(0,                      0,  0),                         // 0    Center

                new Vector3(0,                      0,  -innerSide * height),       // 1    Inner-Bottom
                new Vector3(-innerSide * width,     0,  -innerSide/2 * height),     // 2    Inner-Left-Bottom
                new Vector3(-innerSide * width,     0,  innerSide/2 * height),      // 3    Inner-Left-Top
                new Vector3(0,                      0,  innerSide * height),        // 4    Inner-Top
                new Vector3(innerSide * width,      0,  innerSide/2 * height),      // 5    Inner-Right-Top
                new Vector3(innerSide * width,      0,  -innerSide/2 * height),     // 6    Inner-Right-Bottom

                new Vector3(0,                      0,  -outerSide * height),       // 7    Outer-Bottom
                new Vector3(-outerSide/2 * width,   0,  -3 * outerSide/4 * height), // 8    Outer-Bottom-Left-MidPoint
                new Vector3(-outerSide * width,     0,  -outerSide/2 * height),     // 9    Outer-Left-Bottom
                new Vector3(-outerSide * width,     0,  0),                         // 10   Outer-Left-Midpoint
                new Vector3(-outerSide * width,     0,  outerSide/2 * height),      // 11   Outer-Left-Top
                new Vector3(-outerSide/2 * width,   0,  3 * outerSide/4 * height),  // 12   Outer-Top-Left-MidPoint
                new Vector3(0,                      0,  outerSide * height),        // 13   Outer-Top
                new Vector3(outerSide/2 * width,    0,  3 * outerSide/4 * height),  // 14   Outer-Top-Right-MidPoint
                new Vector3(outerSide * width,      0,  outerSide/2 * height),      // 15   Outer-Right-Top
                new Vector3(outerSide * width,      0,  0),                         // 16   Outer-Right-MidPoint
                new Vector3(outerSide * width,      0,  -outerSide/2 * height),     // 17   Outer-Right-Bottom
                new Vector3(outerSide/2 * width,    0,  -3 * outerSide/4 * height)  // 18   Outer-Bottom-Right-MidPoint
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
                2,  9,  10,
                2,  10, 3,
                10, 11, 3,
                // Top-Left
                3,  11, 12,
                3,  12, 4,
                12, 13, 4,
                // Top-Right
                4,  13, 14,
                4,  14, 5,
                14, 15, 5,
                // Right
                5,  15, 16,
                5,  16, 6,
                16, 17, 6,
                // Bottom-Right
                6,  17, 18,
                6,  18, 1,
                18, 7,  1
                };
            Vector3[] normals = new Vector3[19]
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
                Vector3.up,
                Vector3.up,
                Vector3.up,
                Vector3.up,
                Vector3.up,
                Vector3.up,
                Vector3.up
                };
            Vector2[] uv = new Vector2[19]
                {
                new Vector2(outerSide,  outerSide),                                     // Center

                new Vector2(outerSide,                  1-(outerSide+innerSide)),       // Inner-Bottom
                new Vector2(1-(outerSide+innerSide),    (1-innerSide)/2),               // Inner-Left-bottom
                new Vector2(1-(outerSide+innerSide),    1- (1-innerSide)/2),            // Inner-Left-Top
                new Vector2(outerSide,                  outerSide+innerSide),           // Inner-Top
                new Vector2(outerSide + innerSide,      1 - (1-innerSide)/2),           // Inner-Right-top
                new Vector2(outerSide + innerSide,      (1-innerSide)/2),               // Inner-Right-bottom

                new Vector2(outerSide,          0f),                                    // Outer-Bottom
                new Vector2(outerSide/2,        outerSide/4),                           // Outer-Bottom-Left-Midpoint
                new Vector2(0f,                 outerSide/2),                           // Outer-Left-bottom
                new Vector2(0f,                 outerSide),                             // Outer-Left-MidPoint
                new Vector2(0f,                 (1 - outerSide/2)),                     // Outer-Left-Top
                new Vector2((outerSide/2),      (1-outerSide/4)),                       // Outer-Top-Left-MidPoint
                new Vector2(outerSide,          1f),                                    // Outer-Top
                new Vector2((1 - outerSide),    (1-outerSide/4)),                       // Outer-Top-Right-MidPoint
                new Vector2(1f,                 (1-outerSide/2)),                       // Outer-Right-top
                new Vector2(1f,                 outerSide),                             // Outer-Right-MidPoint
                new Vector2(1f,                 outerSide/2),                           // Outer-Right-bottom
                new Vector2((1-outerSide/2),    outerSide/4),                           // Outer-Bottom-Right-MidPoint
                };
            MeshInfo meshInfo = new MeshInfo(vertices, tris, normals, uv, "Hexagon");

            return meshInfo;
        }

        public static Vector3[] GetHexVerts(int x, int y, int z, int cellSize)
        {
            HexInfo hexInfo = new HexInfo(cellSize);
            Vector3[] verts = new Vector3[6]
            {
                new Vector3(x,                          z, y - 0.5f * hexInfo.Height),
                new Vector3(x - 0.5f * hexInfo.Width,   z, y - 0.25f * hexInfo.Height),
                new Vector3(x - 0.5f * hexInfo.Width,   z, y + 0.25f * hexInfo.Height),
                new Vector3(x,                          z, y + 0.5f * hexInfo.Height),
                new Vector3(x + 0.5f * hexInfo.Width,   z, y + 0.25f * hexInfo.Height),
                new Vector3(x + 0.5f * hexInfo.Width,   z, y - 0.25f * hexInfo.Height)
            };

            return verts;
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