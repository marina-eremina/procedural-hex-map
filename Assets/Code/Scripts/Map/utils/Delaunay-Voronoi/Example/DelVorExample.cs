#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

using csDelaunay;
using System.Collections.Generic;
using System.Drawing;

public class DelVorMap : MonoBehaviour
{
    public Material mapMaterial;

    [Range(10, 100)]
	public int mapSize = 100;

    [Range(0, 0.5f)]
    public float waterThreshold = 0.4f;


    [Range(1, 10)]
    public float scale = 3;

    private DelaunayVoronoi delaunayVoronoi;

    private Dictionary<Vector2f, Site> sites;
    private List<Edge> edges;

    Rectf bounds;

    float[,] noiseMap;

    [ContextMenu("Destroy map")]
    public void DestroyMap()
    {
        List<GameObject> children = new();
        foreach (Transform child in transform) children.Add(child.gameObject);

        
        children.ForEach(child =>
            {
                #if UNITY_EDITOR
                    DestroyImmediate(child);
                #else
                    Destroy(child);
                #endif
            }
        );

        sites?.Clear();
        edges?.Clear();
    }


    [ContextMenu("Generate Map")]
    public void GenerateMap()
    {
        DestroyMap();
        bounds = new Rectf(0, 0, mapSize, mapSize);
        delaunayVoronoi = new DelaunayVoronoi(mapSize);
        noiseMap = GenerateIslandNoiseMap();
        (sites, edges) = delaunayVoronoi.GenerateVertices();
        GenerateNet();
        GenerateMeshes();
    }

    private void GenerateNet()
    {
        DrawVoronoiNet();
    }


    private void GenerateMeshes()
    {

        MeshObjectBuilder meshObjectBuilder = new(); // Create an instance


        foreach (Site site in sites.Values)
        {
            List<Vector2f> siteVertices2D = site.Region(bounds);

            // Get the noise value from the noise map
            int x = Mathf.FloorToInt(site.Coord.x);
            int y = Mathf.FloorToInt(site.Coord.y);
            float noiseValue = noiseMap[x, y];

            TileTypes tileType;
            UnityEngine.Color tileColor;
            if (noiseValue < waterThreshold)
            {
                tileType = TileTypes.WATER;
                tileColor = UnityEngine.Color.blue;
            }
            else if (noiseValue < 0.5f)
            {
                tileType = TileTypes.PLAIN;
                tileColor = UnityEngine.Color.green;
            }
            else
            {
                tileType = TileTypes.MOUNTAIN;
                tileColor = UnityEngine.Color.grey;
            }



            IMeshCreator meshCreator = new RegionMeshCreator();
            Mesh regionMesh = meshCreator.CreateMesh(siteVertices2D);

            Material material = new(mapMaterial);

            material.color = tileColor;

            GameObject regionObject = meshObjectBuilder.BuildMeshObject(regionMesh, material, "Tile");
            regionObject.transform.SetParent(transform);

            Tile tile = regionObject.AddComponent<Tile>();
            tile.Type = tileType;

            Debug.Log(site.Coord);
            // Create a sphere at the center
            GameObject centerDot = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            centerDot.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f); // Adjust the size of the sphere
            centerDot.transform.SetParent(regionObject.transform);
        }
    }

    private void DrawVoronoiNet()
{
    GameObject netObject = new GameObject("VoronoiNet");
    netObject.transform.SetParent(transform);

    foreach (Edge edge in edges)
    {
        if (edge.ClippedEnds == null)
        {
            Debug.LogError($"Edge {edge.EdgeIndex} has null ClippedEnds.");
            continue;
        }

        Vector2f? leftEnd = edge.ClippedEnds[LR.LEFT];
        Vector2f? rightEnd = edge.ClippedEnds[LR.RIGHT];

        if (leftEnd == null || rightEnd == null)
        {
            Debug.LogError($"Edge {edge.EdgeIndex} has null ends. Left: {leftEnd}, Right: {rightEnd}");
            continue;
        }

        Vector2 startPoint = new(leftEnd.Value.x, leftEnd.Value.y);
        Vector2 endPoint = new(rightEnd.Value.x, rightEnd.Value.y);

        GameObject lineObject = new GameObject($"Edge_{edge.EdgeIndex}");
        lineObject.transform.SetParent(netObject.transform);

        LineRenderer lineRenderer = lineObject.AddComponent<LineRenderer>();

        lineRenderer.positionCount = 2;
        lineRenderer.startWidth = 0.1f; // Adjust the width of the lines
        lineRenderer.endWidth = 0.1f;

        lineRenderer.material = new Material(Shader.Find("Unlit/Color"));

        float netHeight = 5.05f; // Height above the map
        Vector3[] points3D = new Vector3[] { new Vector3(startPoint.x, netHeight, startPoint.y), new Vector3(endPoint.x, netHeight, endPoint.y) };
        lineRenderer.SetPositions(points3D);
    }
}

    private float[,] GenerateIslandNoiseMap()
    {
        float[,] map = new float[mapSize, mapSize];
        Vector2 center = new Vector2(mapSize / 2, mapSize / 2);

        for (int y = 0; y < mapSize; y++)
        {
            for (int x = 0; x < mapSize; x++)
            {
                float noiseValue = Mathf.PerlinNoise(x / scale, y / scale);
                float distanceToCenter = Vector2.Distance(new Vector2(x, y), center) / (mapSize / 2);
                float gradientValue = 1 - distanceToCenter * distanceToCenter;
                map[x, y] = noiseValue * gradientValue;

                // Apply water threshold
                if (map[x, y] < waterThreshold) map[x, y] = 0;
            }
        }

        return map;
    }





}

