using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePiece : MonoBehaviour {

    public static readonly TileLocation[] captureLocations = new TileLocation[4] { new TileLocation(-1, 0), new TileLocation(1, 0), new TileLocation(0, 1), new TileLocation(0, -1) };
    public static readonly TileLocation[] potentialSingleMoves = new TileLocation[4] {new TileLocation(-1, -1), new TileLocation(-1, 1), new TileLocation(1, -1), new TileLocation(1, 1) };
    public static readonly TileLocation[] potentialDoubleMoves = new TileLocation[4] {new TileLocation(-2, -2), new TileLocation(-2, 2), new TileLocation(2, -2), new TileLocation(2, 2) };

    public bool usedDoubleMove;
    public TileLocation currentLocation;
    public Material mat;
    public Player playerColor;
    public GameObject pieceObj;
    public bool currentlySelected;
    private GameObject twoMoveIndicator;

    private void Start()
    {
        twoMoveIndicator = transform.GetChild(0).gameObject;
        SetDoubleMove(true);
    }

    public void SetDoubleMove(bool canMove)
    {
        usedDoubleMove = !canMove;
        twoMoveIndicator.SetActive(canMove);
    }

    public void SetPieceColor(TileColor tileCol)
    {
        Renderer rend = GetComponent<Renderer>();
        string prefabPath;
        switch (tileCol)
        {
            case TileColor.black:
                prefabPath = "Materials/Black";
                break;
            case TileColor.white:
                prefabPath = "Materials/White";
                break;
            default:
                Debug.LogWarning("Tile color was invalid");
                prefabPath = "invalid";
                break;
        }
        mat = (Material)Resources.Load(prefabPath);
        rend.material = mat;
    }

    public void MovePiece(TileLocation targetLocation)
    {
        foreach (var move in potentialDoubleMoves)
        {
            if(targetLocation == move + currentLocation)
            {
                SetDoubleMove(false);
                break;
            }
        }
        foreach (var tile in GameBoard.BoardTiles)
        {
            if(tile.locationOfTile == targetLocation)
            {
                transform.SetParent(tile.tileObj.transform);
            }
        }
        pieceObj.transform.localPosition = new Vector3(0, 0.5f, 0);
        GameBoard.BoardTiles[currentLocation.x, currentLocation.y].valueOfTile = TileValue.empty;
        GameBoard.BoardTiles[targetLocation.x, targetLocation.y].valueOfTile = playerColor == Player.black ? TileValue.blackPiece : TileValue.whitePiece;
        currentLocation = targetLocation;
        currentlySelected = false;
        GameBoard.EndTurn();
    }

    public List<TileLocation> GetPossibleMoves()
    {
        List <TileLocation> potentialMoves = new List<TileLocation>();
        if(usedDoubleMove == false)
        {
            foreach (var move in potentialDoubleMoves)
            {
                TileLocation moveLocation = move + currentLocation;
                if (CheckValidLocation(moveLocation))
                {
                    if (GameBoard.BoardTiles[moveLocation.x, moveLocation.y].valueOfTile == TileValue.empty)
                    {
                        potentialMoves.Add(moveLocation);
                    }
                }
            }
        }

        foreach (var move in potentialSingleMoves)
        {
            TileLocation moveLocation = move + currentLocation;
            if (CheckValidLocation(moveLocation))
            {
                if (GameBoard.BoardTiles[moveLocation.x, moveLocation.y].valueOfTile == TileValue.empty)
                {
                    potentialMoves.Add(moveLocation);
                }
            }
        }
        return potentialMoves;
    }

    private bool CheckValidLocation(TileLocation moveLocation)
    {
        bool validMove = true;
        if (moveLocation.x < 0 || moveLocation.x > GameBoard.BoardSize - 1)
        {
            validMove = false;
        }else if(moveLocation.y < 0 || moveLocation.y > GameBoard.BoardSize - 1)
        {
            validMove = false;
        }
        return validMove;
    }

    public bool CheckForCapture()
    {
        TileValue captureValue = playerColor == Player.black ? TileValue.whitePiece : TileValue.blackPiece;
        bool captured = false;
        TileLocation captureLocation = captureLocations[0] + currentLocation;
        if (CheckValidLocation(captureLocation))
        {
            if (GameBoard.BoardTiles[captureLocation.x, captureLocation.y].valueOfTile == captureValue)
            {
                captureLocation = captureLocations[1] + currentLocation;
                if (CheckValidLocation(captureLocation))
                {
                    if (GameBoard.BoardTiles[captureLocation.x, captureLocation.y].valueOfTile == captureValue)
                    {
                        captured = true;
                    }
                }
            }
        }

        if (!captured)
        {
            captureLocation = captureLocations[2] + currentLocation;
            if (CheckValidLocation(captureLocation))
            {
                if (GameBoard.BoardTiles[captureLocation.x, captureLocation.y].valueOfTile == captureValue)
                {
                    captureLocation = captureLocations[3] + currentLocation;
                    if (CheckValidLocation(captureLocation))
                    {
                        if (GameBoard.BoardTiles[captureLocation.x, captureLocation.y].valueOfTile == captureValue)
                        {
                            captured = true;
                        }
                    }
                }
            }
        }
        if (captured)
        {
            Debug.Log("Piece captured at: " + currentLocation);
        }
        return captured;
    }

    public void Capture()
    {
        GameBoard.BoardTiles[currentLocation.x, currentLocation.y].valueOfTile = TileValue.empty;
        Destroy(this.gameObject);
    }
}
