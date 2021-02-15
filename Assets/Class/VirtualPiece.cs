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
        this.type = piece.type;
        this.side = piece.side;
        this.position = piece.position;
        this.isFirstMove = piece.isFirstMove;
    }

    public override bool Equals(object obj) {
        return obj is VirtualPiece piece &&
               type == piece.type &&
               side == piece.side &&
               position.Equals(piece.position) &&
               isFirstMove == piece.isFirstMove;
    }

    public override int GetHashCode() {
        int hashCode = -1072877299;
        hashCode = hashCode * -1521134295 + type.GetHashCode();
        hashCode = hashCode * -1521134295 + side.GetHashCode();
        hashCode = hashCode * -1521134295 + position.GetHashCode();
        hashCode = hashCode * -1521134295 + isFirstMove.GetHashCode();
        return hashCode;
    }
}
