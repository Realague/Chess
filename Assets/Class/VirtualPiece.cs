using UnityEngine;

public class VirtualPiece
{
    public PieceType type;

    public Color side;

    public Vector2 position;

    public bool isFirstMove = true;

    public VirtualPiece(PieceType type, Color side, Vector2 position, bool isFirstMove) {
        this.type = type;
        this.side = side;
        this.position = position;
        this.isFirstMove = isFirstMove;
    }

    public VirtualPiece(Piece piece, Vector2 position) {
        this.type = piece.type;
        this.side = piece.side;
        this.position = position;
        this.isFirstMove = piece.isFirstMove;
    }

    public VirtualPiece(VirtualPiece piece) {
        //if (piece != null) {
            this.type = piece.type;
            this.side = piece.side;
            this.position = piece.position;
            this.isFirstMove = piece.isFirstMove;
        //}
    }

}
