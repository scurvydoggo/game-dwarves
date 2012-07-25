using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Script for a terrain chunk.
/// </summary>
public class ChunkScript : MonoBehaviour
{
    /// <summary>
    /// The mesh filter for this chunk.
    /// </summary>
    private MeshFilter meshFilter;

    /// <summary>
    /// Gets or sets the chunk.
    /// </summary>
    public Chunk Chunk { get; set; }

    /// <summary>
    /// Initialises the component.
    /// </summary>
    public void Start()
    {
        this.meshFilter = this.gameObject.GetComponent<MeshFilter>();
    }

    /// <summary>
    /// Called once per frame.
    /// </summary>
    public void Update()
    {
        const int z = 0;
        var vertices = new List<Vector3>();
        var triangles = new List<int>();
        var uv = new List<Vector2>();
        int index = 0;
        for (int x = 0; x < Chunk.SizeX; x++)
        {
            for (int y = 0; y < Chunk.SizeY; y++)
            {
                if (Random.value > 0.75f)
                {
                    continue;
                }

                Vector3 bottomLeft = new Vector3(x, y, z);
                Vector3 topLeft = new Vector3(x, y + 1, z);
                Vector3 topRight = new Vector3(x + 1, y + 1, z);
                Vector3 bottomRight = new Vector3(x + 1, y, z);

                // Vertices
                vertices.Add(bottomLeft);
                vertices.Add(topLeft);
                vertices.Add(topRight);
                vertices.Add(bottomRight);

                // UV coordinates
                uv.Add(new Vector2(bottomLeft.x, bottomLeft.y));
                uv.Add(new Vector2(topLeft.x, topLeft.y));
                uv.Add(new Vector2(topRight.x, topRight.y));
                uv.Add(new Vector2(bottomRight.x, bottomRight.y));

                // Bottom-left triangle
                triangles.Add(index);
                triangles.Add(index + 1);
                triangles.Add(index + 2);

                // Top-right triangle
                triangles.Add(index + 2);
                triangles.Add(index + 3);
                triangles.Add(index);

                index += 4;
            }
        }

        this.meshFilter.mesh.Clear();
        this.meshFilter.mesh.vertices = vertices.ToArray();
        this.meshFilter.mesh.triangles = triangles.ToArray();
        this.meshFilter.mesh.uv = uv.ToArray();

        this.meshFilter.mesh.RecalculateNormals();
    }
}