using UnityEngine;

public class PositionEvaluation {

   private static int[] pawn = new int[8 * 8] {
      900, 900, 900, 900, 900, 900, 900, 900,
      50, 50, 50, 50, 50, 50, 50, 50,
      10, 10, 20, 30, 30, 20, 10, 10,
      5,  5, 10, 25, 25, 10,  5,  5 ,
      0,  0,  0, 20, 20,  0,  0,  0,
      5, -5,-10,  10,  10,-10, -5,  5,
      5, 10, 10,-20,-20, 10, 10,  5,
      0,  0,  0,  0,  0,  0,  0,  0
    };

    private static int[] knight = new int[8 * 8] {
      -50,-40,-30,-30,-30,-30,-40,-50,
      -40,-20,  0,  0,  0,  0,-20,-40,
      -30,  0, 10, 15, 15, 10,  0,-30,
      -30,  5, 15, 20, 20, 15,  5,-30,
      -30,  0, 15, 20, 20, 15,  0,-30,
      -30,  5, 10, 15, 15, 10,  5,-30,
      -40,-20,  0,  5,  5,  0,-20,-40,
      -50,-40,-30,-30,-30,-30,-40,-50
    };

    private static int[] bishop = new int[8 * 8] {
      -20,-10,-10,-10,-10,-10,-10,-20,
      -10,  0,  0,  0,  0,  0,  0,-10,
      -10,  0,  5, 10, 10,  5,  0,-10,
      -10,  5,  5, 10, 10,  5,  5,-10,
      -10,  0, 10, 10, 10, 10,  0,-10,
      -10, 10, 10, 10, 10, 10, 10,-10,
      -10,  5,  0,  0,  0,  0,  5,-10,
      -20,-10,-10,-10,-10,-10,-10,-20,
    };

    private static int[] rook = new int[8 * 8] {
      0,  0,  0,  0,  0,  0,  0,  0,
      5, 10, 10, 10, 10, 10, 10,  5,
      -5,  0,  0,  0,  0,  0,  0, -5,
      -5,  0,  0,  0,  0,  0,  0, -5,
      -5,  0,  0,  0,  0,  0,  0, -5,
      -5,  0,  0,  0,  0,  0,  0, -5,
      -5,  0,  0,  0,  0,  0,  0, -5,
      0,  0,  0,  5,  5,  0,  0,  0
    };

    private static int[] queen = new int[8 * 8] {
      -20,-10,-10, -5, -5,-10,-10,-20,
      -10,  0,  0,  0,  0,  0,  0,-10,
      -10,  0,  5,  5,  5,  5,  0,-10,
       -5,  0,  5,  5,  5,  5,  0, -5,
       0,  0,  5,  5,  5,  5,  0, -5,
      -10,  5,  5,  5,  5,  5,  0,-10,
      -10,  0,  5,  0,  0,  0,  0,-10,
      -20,-10,-10, -5, -5,-10,-10,-20
    };

    private static int[] kingMidleGame = new int[8 * 8] {
      -30,-40,-40,-50,-50,-40,-40,-30,
      -30,-40,-40,-50,-50,-40,-40,-30,
      -30,-40,-40,-50,-50,-40,-40,-30,
      -30,-40,-40,-50,-50,-40,-40,-30,
      -20,-30,-30,-40,-40,-30,-30,-20,
      -10,-20,-20,-20,-20,-20,-20,-10,
      20, 20,  0,  0,  0,  0, 20, 20,
      20, 30, 10,  0,  0, 10, 30, 20
    };

    private static int[] kingEndGame = new int[8 * 8] {
      -50,-40,-30,-20,-20,-30,-40,-50,
      -30,-20,-10,  0,  0,-10,-20,-30,
      -30,-10, 20, 30, 30, 20,-10,-30,
      -30,-10, 30, 40, 40, 30,-10,-30,
      -30,-10, 30, 40, 40, 30,-10,-30,
      -30,-10, 20, 30, 30, 20,-10,-30,
      -30,-30,  0,  0,  0,  0,-30,-30,
      -50,-30,-30,-30,-30,-30,-30,-50
    };

    private static int doublePawnPenalty = -10;

    // isolated pawn penalty
    private static int isolatedPawnPenalty = -10;

    // passed pawn bonus
    private static int[] passedPawnBonus = new int[8] { 0, 10, 30, 50, 75, 100, 150, 200 };

    public static int EvaluatePosition(Board board, int position, VirtualPiece piece) {
        int score = 0;

        switch (piece.type) {
            case PieceType.Pawn:
                score = pawn[position];
                score += PawnBonusesOrPenalty(board, piece);
                break;
            case PieceType.Knight:
                score = knight[position];
                break;
            case PieceType.Bishop:
                score = bishop[position];
                break;
            case PieceType.Rook:
                score = rook[position];
                break;
            case PieceType.Queen:
                score = queen[position];
                break;
            case PieceType.King:
                if (MinMax.gameStage == GameStage.Late) {
                    score = kingEndGame[position];
                } else {
                    score = kingMidleGame[position];
                }
                break;
            default:
                break;
        }
        return score;
    }

    private static int PawnBonusesOrPenalty(Board board, VirtualPiece pawn) {
        Vector2 pos = pawn.position;
        Color side = board.CheckCase(pos).side;
        int doublePawns = 0;
        bool isolatedPawn = true;
        int score = 0;

        for (int i = 0; i != 8; i++) {
            VirtualPiece piece = board.CheckCase(new Vector2(pos.x, i));
            VirtualPiece piece2 = board.CheckCase(new Vector2(pos.x + 1, i));
            VirtualPiece piece3 = board.CheckCase(new Vector2(pos.x - 1, i));
            if (piece != null && piece.type == PieceType.Pawn && piece.side == side && i != pos.y) {
                doublePawns++;
            }
            if ((piece3 != null && piece3.type == PieceType.Pawn && piece3.side == side) || (piece2 != null && piece2.type == PieceType.Pawn && piece2.side == side)) {
                isolatedPawn = false;
            }
        }

        if (doublePawns != 0) {
            score += doublePawns * doublePawnPenalty;
        }

        if (isolatedPawn) {
            score = isolatedPawnPenalty;
        }

        if (side == Color.Dark) {
            for (int i = (int)pos.y + 1; i != 8; i++) {
                VirtualPiece piece = board.CheckCase(new Vector2(pos.x, i));
                VirtualPiece piece2 = board.CheckCase(new Vector2(pos.x + 1, i));
                VirtualPiece piece3 = board.CheckCase(new Vector2(pos.x - 1, i));
                if ((piece3 != null && piece3.type == PieceType.Pawn && piece3.side != side) ||
                    (piece2 != null && piece2.type == PieceType.Pawn && piece2.side != side) ||
                    (piece != null && piece.type == PieceType.Pawn && piece.side != side)) {
                    return score;
                }
            }
            score += passedPawnBonus[(int)pos.y];
        } else {
            for (int i = (int)pos.y - 1; i >= 0; i--) {
                VirtualPiece piece = board.CheckCase(new Vector2(pos.x, i));
                VirtualPiece piece2 = board.CheckCase(new Vector2(pos.x + 1, i));
                VirtualPiece piece3 = board.CheckCase(new Vector2(pos.x - 1, i));
                if ((piece3 != null && piece3.type == PieceType.Pawn && piece3.side != side) ||
                    (piece2 != null && piece2.type == PieceType.Pawn && piece2.side != side) ||
                    (piece != null && piece.type == PieceType.Pawn && piece.side != side)) {
                    return score;
                }
            }
            score += passedPawnBonus[7 - (int)pos.y];
        }
        return score;
    }

}
