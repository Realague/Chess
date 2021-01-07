using System;
using System.Collections.Generic;
using UnityEngine;

public class MinMax
{

    public Board board;

    public static int depth = 1;

    public MinMax(Board board)
    {
        this.board = new Board(board);
    }

    public MinMax(MinMax minMax)
    {
        this.board = new Board(minMax.board);
    }

    public static Movement bestMove = null;

    public static int NegaMax(MinMax minMax, int depthLeft, int turn, int alpha, int beta) {
        if (depthLeft == 0) {
            //return minMax.CalculateScore(minMax);
            return MinMax.QuiescenceSearch(minMax, alpha, beta, turn);
        }

        int oldAlpha = alpha;

        List<Movement> movements = minMax.board.GetAllMovements(turn == 1 ? GameMaster.instance.turn : Color.Dark == GameMaster.instance.turn ? Color.Light : Color.Dark);
        if (movements.Count == 0) {
            return turn * int.MaxValue;
        }
        Movement bestSoFar = null;
        foreach (Movement movement in movements) {
            movement.DoMovement(minMax.board, true);
            int score = -NegaMax(new MinMax(minMax), depthLeft - 1, -turn, -beta, -alpha);
            minMax.board.UndoMove();
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
        if (movements.Count == 0) {
            return turn * int.MaxValue;
        }
        foreach (Movement movement in movements) {
            if (movement.moveType == MoveType.Attack) {
                movement.DoMovement(minMax.board, true);
                score = -QuiescenceSearch(minMax, -beta, -alpha, -turn);
                minMax.board.UndoMove();
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
