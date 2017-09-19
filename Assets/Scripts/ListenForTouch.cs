using System.Collections.Generic;
using UnityEngine;

public class ListenForTouch : MonoBehaviour {

    public GamePiece pieceOnTile;
    static bool pieceSelected;
    bool tileOccupied;
    TileController tileCon;
    static List<TileLocation> moves = new List<TileLocation>();

    private void Start()
    {
        tileCon = GetComponent<TileController>();
        SetPieceOnTile();
    }

    private void OnMouseUp()
    {
        if (pieceOnTile == null)
        {
            SetPieceOnTile();
        }

        if (!moves.Contains(tileCon.tileLoc))
        {
            ResetPossibleMoves();
        }

        if (tileOccupied && GameBoard.CurPlayer == pieceOnTile.playerColor)
        {
            if (pieceOnTile == null)
            {
                return;
            }

            moves = pieceOnTile.GetPossibleMoves();

            foreach (var tile in GameBoard.BoardTiles)
            {
                if (moves.Contains(tile.locationOfTile))
                {
                    tile.tileObj.GetComponent<TileController>().SetAsPossibleMove();
                    ResetSelectedPiece();
                    pieceOnTile.currentlySelected = true;
                    pieceSelected = true;
                }
            }
        }else if(!tileOccupied && pieceSelected)
        {
            MovePiece();
            ResetPossibleMoves();
            ResetSelectedPiece();
            SetPieceOnTile();
        }
    }

    private void MovePiece()
    {
        if (moves.Contains(tileCon.tileLoc))
        {
            foreach (var piece in GameBoard.blackPieces)
            {
                if (piece.currentlySelected)
                {
                    piece.MovePiece(tileCon.tileLoc);
                    ResetAllPiecesOnTiles();
                }
            }
            foreach (var piece in GameBoard.whitePieces)
            {
                if (piece.currentlySelected)
                {
                    piece.MovePiece(tileCon.tileLoc);
                    ResetAllPiecesOnTiles();
                }
            }
        }
    }

    void ResetPossibleMoves()
    {
        foreach (var tile in GameBoard.BoardTiles)
        {
            tile.tileObj.GetComponent<TileController>().RemoveAsPossibleMove();
            moves.Clear();
        }
    }

    private void SetPieceOnTile()
    {
        if (GetComponentInChildren<GamePiece>() != null)
        {   
            pieceOnTile = GetComponentInChildren<GamePiece>();
            tileOccupied = true;
        }
        else
        {
            pieceOnTile = null;
            tileOccupied = false;
        }
    }

    private void ResetSelectedPiece()
    {
        foreach (var piece in GameBoard.blackPieces)
        {
            piece.currentlySelected = false;
        }

        foreach (var piece in GameBoard.whitePieces)
        {
            piece.currentlySelected = false;
        }
        pieceSelected = false;
    }

    private void ResetAllPiecesOnTiles()
    {
        foreach (var tile in GameBoard.BoardTiles)
        {
            tile.tileObj.GetComponent<ListenForTouch>().SetPieceOnTile();
        }
    }
}
