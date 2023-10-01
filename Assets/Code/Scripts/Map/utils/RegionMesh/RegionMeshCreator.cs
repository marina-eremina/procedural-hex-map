using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public interface IMeshCreator
{
    Mesh CreateMesh(List<Vector2f> vertices);
}

public class RegionMeshCreator : IMeshCreator
{
    public Mesh CreateMesh(List<Vector2f> vertices)
    {
        int vertexCount = vertices.Count;
        Vector3[] meshVertices = new Vector3[vertexCount * 2];
        Vector3[] normals = new Vector3[vertexCount * 2];

        for (int i = 0; i < vertexCount; i++)
        {
            // Top face
            meshVertices[i] = new Vector3(vertices[i].x, 5f, vertices[i].y); // top vertex with thickness
            normals[i] = Vector3.up; // top face normal

            // Bottom face
            meshVertices[i + vertexCount] = new Vector3(vertices[i].x, 0f, vertices[i].y); // bottom vertex without thickness
            normals[i + vertexCount] = Vector3.down; // bottom face normal
        }

        int[] topTriangles = Triangulate(meshVertices.Take(vertexCount).ToArray());
        int[] bottomTriangles = Triangulate(meshVertices.Skip(vertexCount).ToArray())
            .Select(index => index + vertexCount).ToArray();

        List<int> sideTriangles = new List<int>();
        for (int i = 0; i < vertexCount; i++)
        {
            int next = (i + 1) % vertexCount;
            int current = i;

            // Side triangles
            sideTriangles.Add(current);
            sideTriangles.Add(vertexCount + current);
            sideTriangles.Add(vertexCount + next);
            sideTriangles.Add(current);
            sideTriangles.Add(vertexCount + next);
            sideTriangles.Add(next);
        }

        int[] triangles = topTriangles.Concat(bottomTriangles).Concat(sideTriangles).ToArray();

        Mesh regionMesh = new Mesh
        {
            vertices = meshVertices,
            triangles = triangles,
            normals = normals
        };

        return regionMesh;
    }

    public static int[] Triangulate(Vector3[] vertices)
    {
        List<int> triangles = new();
        for (int i = 1; i < vertices.Length - 1; i++)
        {
            triangles.Add(0);
            triangles.Add(i);
            triangles.Add(i + 1);
        }
        return triangles.ToArray();
    }
}
