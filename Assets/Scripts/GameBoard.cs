﻿using UnityEngine;
using System.Collections.Generic;
using TMPro;
using System;

public class GameBoard : MonoBehaviour
{
    public static int BoardSize = 8;
    public static Tile[,] BoardTiles = new Tile[BoardSize, BoardSize];
    public GameObject tilePrefab, piecePrefab;
    public static GameObject playerUI;
    public static Player CurPlayer { get; set; }

    public static readonly TileLocation[] captureLocations = new TileLocation[4] { new TileLocation(-1, 0), new TileLocation(1, 0), new TileLocation(0, 1), new TileLocation(0, -1) };

    public static List<GamePiece> blackPieces = new List<GamePiece>();
    public static List<GamePiece> whitePieces = new List<GamePiece>();

    public enum GameType { standard, centered, corners };

    private GameObject boardObjects;
    private static GameObject winPanel;

    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
        if (GameObject.Find("BoardObjects") == null)
        {
            boardObjects = (GameObject)Instantiate(Resources.Load("Prefabs/BoardObjects"));
        }
        else
        {
            boardObjects = GameObject.Find("BoardObjects");
        }
        CurPlayer = Player.white;
    }

    public void CreateBoard(GameType gameType)
    {
        ResetGame();
        Transform[] t = GetComponentsInChildren<Transform>();
        foreach (var transform in t)
        {
            if (transform.gameObject != gameObject)
            {
                Destroy(transform.gameObject);
            }
        }
        bool isBlack = true;
        for (int i = 0; i < BoardSize; i++)
        {
            for (int j = 0; j < BoardSize; j++)
            {
                TileColor tc;
                if (j % 2 == 0)
                {
                    tc = isBlack ? TileColor.black : TileColor.white;
                }
                else
                {
                    tc = isBlack ? TileColor.white : TileColor.black;
                }
                AddTile(new TileLocation(i, j), tc);
            }
            isBlack = !isBlack;
        }
        boardObjects.GetComponent<BoardController>().PositionChildren();
        switch (gameType)
        {
            case GameType.standard:
                PlacePiecesOnBoard(GameSetups.standardSetup);
                break;
            case GameType.corners:
                PlacePiecesOnBoard(GameSetups.corners);
                break;
            case GameType.centered:
                PlacePiecesOnBoard(GameSetups.centered);
                break;
            default:
                PlacePiecesOnBoardAtStart();
                break;
        }
    }

    //custom board size with standard piece layout
    void PlacePiecesOnBoardAtStart()
    {
        for (int row = 2; row < BoardSize - 2; row++)
        {
            if(row % 2 == 0)
            {
                PlacePiece(Player.black, new TileLocation(row, 1));
                PlacePiece(Player.black, new TileLocation(row, BoardSize - 1));
            }
            else
            {
                PlacePiece(Player.black, new TileLocation(row, 0));
                PlacePiece(Player.black, new TileLocation(row, BoardSize - 2));
            }
        }

        for (int col = 2; col < BoardSize - 2; col++)
        {
            if (col % 2 == 0)
            {
                PlacePiece(Player.white, new TileLocation(0, col));
                PlacePiece(Player.white, new TileLocation(BoardSize - 2, col));
            }
            else
            {
                PlacePiece(Player.white, new TileLocation(1, col));
                PlacePiece(Player.white, new TileLocation(BoardSize - 1, col));
            }
        }
    }

    //Preset boards as defined in GameSetups
    void PlacePiecesOnBoard(Dictionary<TileLocation, TileValue> boardSetup)
    {
        foreach (var item in boardSetup)
        {
            if(item.Value != TileValue.empty)
            {
                PlacePiece(item.Value, item.Key);
            }
        }
    }

    void AddTile(TileLocation tileLocation, TileColor tileColor)
    {
        GameObject tileObj = Instantiate(tilePrefab, boardObjects.transform);
        TileController tileCon = tileObj.GetComponent<TileController>();
        tileCon.tileColor = tileColor;
        tileCon.SetTileLocation(tileLocation);
        tileCon.SetMaterial(tileColor);
        Tile newTile = new Tile();
        newTile.tileObj = tileObj;
        newTile.locationOfTile = tileLocation;
        newTile.colorOfTile = tileColor;
        newTile.valueOfTile = TileValue.empty;
        BoardTiles[tileLocation.x, tileLocation.y] = newTile;
    }

    static void SetUIText()
    {
        if (playerUI == null && GameObject.Find("CurrentPlayerText") != null)
        {
            playerUI = GameObject.Find("CurrentPlayerText");
        }

        if(playerUI == null)
        {
            return;
        }
        playerUI.GetComponent<CurrentPlayerUI>().SetUIText();
    }

    public static void ChangePlayer()
    {
        CurPlayer = CurPlayer == Player.white ? Player.black : Player.white;
        SetUIText();
    }

    public static void EndTurn()
    {
        CheckForCaptures();
        CheckForWin();
        ChangePlayer();
    }

    private static void CheckForWin()
    {
        if(blackPieces.Count < 2)
        {
            DisplayWinPanel("White");
        }else if(whitePieces.Count < 2)
        {
            DisplayWinPanel("Black");
        }
    }

    private static void FindWinPanel()
    {
        Transform boardCanvas = GameObject.Find("BoardCanvas").transform;
        if (GameObject.Find("WinPanel") == null)
        {
            winPanel = (GameObject)Instantiate(Resources.Load("Prefabs/WinPanel"), boardCanvas);
        }
        else
        {
            Debug.LogWarning("Couldn't find win panel");
        }
        winPanel.SetActive(false);
    }

    private static void DisplayWinPanel(string winner)
    {
        FindWinPanel();
        winPanel.GetComponentInChildren<TextMeshProUGUI>().text = winner + " Wins!";
        winPanel.SetActive(true);
    }

    private static void CheckForCaptures()
    {
        List<GamePiece> piecesToRemove = new List<GamePiece>();
        if (CurPlayer == Player.black)
        {
            foreach (var whitePiece in whitePieces)
            {
                if (whitePiece.CheckForCapture())
                {
                    piecesToRemove.Add(whitePiece);
                }
            }
            if (piecesToRemove.Count >= 1)
            {
                foreach (var piece in piecesToRemove)
                {
                    piece.Capture();
                    whitePieces.Remove(piece);
                }
            }
        }
        else
        {
            foreach (var blackPiece in blackPieces)
            {
                if (blackPiece.CheckForCapture())
                {
                    piecesToRemove.Add(blackPiece);
                }
            }
            if (piecesToRemove.Count >= 1)
            {
                foreach (var piece in piecesToRemove)
                {
                    piece.Capture();
                    BoardTiles[piece.currentLocation.x, piece.currentLocation.y].valueOfTile = TileValue.empty;
                    blackPieces.Remove(piece);
                }
            }
        }
    }

    void PlacePiece(Player pieceColor, TileLocation tileLocation)
    {
        Transform parent = BoardTiles[tileLocation.x, tileLocation.y].tileObj.transform;
        GameObject newPiece = Instantiate(piecePrefab, parent);
        newPiece.transform.localPosition = new Vector3(newPiece.transform.localPosition.x, newPiece.transform.localPosition.y + 0.5f, newPiece.transform.localPosition.z);
        GamePiece piece = newPiece.GetComponent<GamePiece>();
        piece.pieceObj = newPiece;
        piece.currentLocation = tileLocation;
        piece.playerColor = pieceColor;
        if(pieceColor == Player.black)
        {
            BoardTiles[tileLocation.x, tileLocation.y].valueOfTile = TileValue.blackPiece;
            blackPieces.Add(piece);
            piece.SetPieceColor(TileColor.black);
        }
        else
        {
            BoardTiles[tileLocation.x, tileLocation.y].valueOfTile = TileValue.whitePiece;
            whitePieces.Add(piece);
            piece.SetPieceColor(TileColor.white);
        }
    }

    //Overload accepting TileValue in lieu of Player color
    void PlacePiece(TileValue tValue, TileLocation tileLocation)
    {
        Transform parent = BoardTiles[tileLocation.x, tileLocation.y].tileObj.transform;
        GameObject newPiece = Instantiate(piecePrefab, parent);
        newPiece.transform.localPosition = new Vector3(newPiece.transform.localPosition.x, newPiece.transform.localPosition.y + 0.5f, newPiece.transform.localPosition.z);
        GamePiece piece = newPiece.GetComponent<GamePiece>();
        piece.pieceObj = newPiece;
        piece.currentLocation = tileLocation;
        piece.playerColor = tValue == TileValue.blackPiece ? Player.black : Player.white;
        if (tValue == TileValue.blackPiece)
        {
            BoardTiles[tileLocation.x, tileLocation.y].valueOfTile = TileValue.blackPiece;
            blackPieces.Add(piece);
            piece.SetPieceColor(TileColor.black);
        }
        else
        {
            BoardTiles[tileLocation.x, tileLocation.y].valueOfTile = TileValue.whitePiece;
            whitePieces.Add(piece);
            piece.SetPieceColor(TileColor.white);
        }
    }

    void ResetGame()
    {
        for (int i = 0; i < BoardSize; i++)
        {
            for (int j = 0; j < BoardSize; j++)
            {
                if(BoardTiles[i, j] != null)
                {
                    Destroy(BoardTiles[i, j].tileObj);
                    BoardTiles[i, j] = new Tile();
                }
            }
        }

        for (int i = blackPieces.Count -1; i >= 0; i--)
        {
            Destroy(blackPieces[i].pieceObj);
            blackPieces.RemoveAt(i);
        }

        for (int i = whitePieces.Count - 1; i >= 0; i--)
        {
            Destroy(whitePieces[i].pieceObj);
            whitePieces.RemoveAt(i);
        }
    }
}