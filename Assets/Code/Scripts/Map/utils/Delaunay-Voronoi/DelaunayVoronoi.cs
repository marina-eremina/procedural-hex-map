using csDelaunay;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class DelaunayVoronoi
{
    private const int DefaultMapDimension = 100;
    private const float DefaultPointSize = 1f;
    private const float DefaultMinDistanceMultiplier = 5f;

    private float pointSpacing;
    private Rectf mapBounds;

    public DelaunayVoronoi() : this(DefaultMapDimension) { }

    public DelaunayVoronoi(int mapSize)
    {
        pointSpacing = DefaultPointSize * DefaultMinDistanceMultiplier;
        mapBounds = new Rectf(0, 0, mapSize, mapSize);
    }

    public (Dictionary<Vector2f, Site> sites, List<Edge> edges) GenerateVertices()
    {
        Vector2 bottomLeftCorner = Vector2.zero;
        Vector2 topRightCorner = new(mapBounds.width, mapBounds.height);

        List<Vector2> samplePoints = PoissonDisk.Sampling(bottomLeftCorner, topRightCorner, pointSpacing);
        Voronoi voronoiDiagram = new(PoissonDisk.ConvertToListVector2f(samplePoints), mapBounds, 5);

        Dictionary<Vector2f, Site> sites = voronoiDiagram.SitesIndexedByLocation;
        List<Edge> edges = voronoiDiagram.Edges;


        return (sites, edges);
    }
}