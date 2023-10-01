using UnityEngine;
using System.Collections;

public enum TileTypes
{
    WATER,
    PLAIN,
    MOUNTAIN
}

public class Tile : MonoBehaviour
{
    public TileTypes Type { get; set; }
}
