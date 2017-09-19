using System;
using UnityEngine;

public enum TileColor { black, white };
public enum TileValue { blackPiece, whitePiece, empty };

[Serializable]
public class Tile
{
    public TileColor colorOfTile;
    public TileLocation locationOfTile;
    public TileValue valueOfTile;
    public bool tileOccupied = false;
    public Material tileMaterial;
    public GameObject tileObj;

    public Tile()
    {

    }

    public Tile(TileLocation tileLocation, Material material, TileColor tileColor, GameObject tileObject)
    {
        this.tileMaterial = material;
        this.colorOfTile = tileColor;
        this.locationOfTile = tileLocation;
        this.tileObj = tileObject;
    }
}
