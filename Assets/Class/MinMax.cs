using System;
using System.Collections.Generic;

public class MinMax
{

    public Board board;

    public static int depth = 3;

    public MinMax(Board board)
    {
        this.board = new Board(board);
    }

    public MinMax(MinMax minMax)
    {
        this.board = new Board(minMax.board);
    }

    public static Movement bestMove = null;

    public static int ply = 0;

    public static int NegaMax(MinMax minMax, int depthLeft, int turn, int alpha, int beta) {
        if (depthLeft == 0) {
            return MinMax.QuiescenceSearch(minMax, alpha, beta, turn);
        }

        int oldAlpha = alpha;

        List<Movement> movements = minMax.board.GetAllMovements(turn == 1 ? GameMaster.instance.turn : Color.Dark == GameMaster.instance.turn ? Color.Light : Color.Dark);
        if (movements.Count == 0) {
            if (minMax.board.Check(turn == 1 ? GameMaster.instance.turn : Color.Dark == GameMaster.instance.turn ? Color.Light : Color.Dark)) {
                return int.MinValue + 100 + ply;
            }
            return 0;
        }
        Movement bestSoFar = null;
        foreach (Movement movement in movements) {
            ply++;
            movement.DoMovement(minMax.board, true);
            int score = -NegaMax(new MinMax(minMax), depthLeft - 1, -turn, -beta, -alpha);
            minMax.board.UndoMove();
            ply--;
            if (score >= beta) {
                return beta;
            }
            if (score > alpha) {
                alpha = score;
                if (depthLeft == MinMax.depth) {
                    bestSoFar = movement;
                }
            }
        }
        if (alpha != oldAlpha) {
            bestMove = bestSoFar;
        }

        return alpha;
    }

    private static int QuiescenceSearch(MinMax minMax, int alpha, int beta, int turn) {

        int score = turn * minMax.CalculateScore(minMax);

        if (score >= beta) {
            return beta;
        }

        if (score > alpha) {
            alpha = score;
        }

        List<Movement> movements = minMax.board.GetAllMovements(turn == 1 ? GameMaster.instance.turn : Color.Dark == GameMaster.instance.turn ? Color.Light : Color.Dark);
        /*if (movements.Count == 0) {
            if (minMax.board.Check(turn == 1 ? GameMaster.instance.turn : Color.Dark == GameMaster.instance.turn ? Color.Light : Color.Dark)) {
                return turn * (int.MaxValue - 100);
            }
            return 0;
        }*/
        foreach (Movement movement in movements) {
            if (movement.moveType == MoveType.Attack) {
                ply++;
                movement.DoMovement(minMax.board, true);
                score = -QuiescenceSearch(minMax, -beta, -alpha, -turn);
                minMax.board.UndoMove();
                ply--;
                if (score >= beta) {
                    return beta;
                }
                if (score > alpha) {
                    alpha = score;
                }
            }
        }

        return alpha;
    }

    private int CalculateScore(MinMax minMax)
    {
        int value = 0;
        foreach (VirtualPiece piece in minMax.board.pieces) {
            if (piece != null && piece.side == GameMaster.instance.turn) {
                value += (int)piece.type;
                value += PositionEvaluation.EvaluatePosition(piece.type, piece.side == Color.Light ? (int)(piece.position.x + piece.position.y * 8) : (int)(piece.position.x + Math.Abs(piece.position.y - 7) * 8));
            } else if (piece != null) {
                value -= (int)piece.type;
                value -= PositionEvaluation.EvaluatePosition(piece.type, piece.side == Color.Light ? (int)(piece.position.x + piece.position.y * 8) : (int)(piece.position.x + Math.Abs(piece.position.y - 7) * 8));
            }
        }
        return value;
    }

}
