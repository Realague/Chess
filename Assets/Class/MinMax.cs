using System;
using System.Collections.Generic;
using System.Threading;

public class MinMax
{

    public Board board;

    private static int depth = 5;

    public MinMax(Board board)
    {
        this.board = new Board(board);
    }

    public MinMax(MinMax minMax)
    {
        this.board = new Board(minMax.board);
    }

    public static Movement PerformMinMax(Board board)
    {
        List<Movement> movements = board.GetAllMovements(GameMaster.instance.turn);
        if (movements.Count == 0)
        {
            return null;
        }
        int bestScore = int.MinValue;
        int score;
        int alpha = int.MinValue;
        int beta = int.MaxValue;
        Movement movementToDo = null; 
        foreach (Movement movement in movements)
        {
            MinMax minMax = new MinMax(board);
            movement.DoMovement(minMax.board, true);
            score = -AlphaBeta(minMax, depth - 1, -1, -alpha, -beta);
            if (score >= beta)
            {
                return movementToDo;
            }
            if (score > bestScore) {
                bestScore = score;
                movementToDo = movement;
                if (score > alpha) {
                    alpha = bestScore;
                }
            }
        }
        return movementToDo;
    }

    public static int AlphaBeta(MinMax minMax, int depthLeft, int turn, int alpha, int beta) {
        if (depthLeft == 0) {
            return turn * minMax.CalculateScore(minMax);
        }

        List<Movement> movements = minMax.board.GetAllMovements(turn == 1 ? GameMaster.instance.turn : Color.Dark == GameMaster.instance.turn ? Color.Light : Color.Dark);
        if (movements.Count == 0) {
            return turn * int.MaxValue;
        }
        int bestScore = int.MinValue;
        foreach (Movement movement in movements) {
            movement.DoMovement(minMax.board, true);
            int score = -AlphaBeta(minMax, depthLeft - 1, -turn, -alpha, -beta);
            minMax.board.UndoMove();
            if (score >= beta) {
                return score;
            }
            if (score > bestScore) {
                bestScore = score;
                if (score > alpha) {
                    alpha = bestScore;
                }
            }
        }
        return bestScore;
    }

    private int CalculateScore(MinMax minMax)
    {
        int value = 0;
        foreach (VirtualPiece piece in minMax.board.pieces) {
            if (piece != null && piece.side == GameMaster.instance.turn) {
                value += (int)piece.type;
                value += PositionEvaluation.EvaluatePosition(piece.type, piece.side, piece.side == Color.Light ? (int)(piece.position.x + piece.position.y * 8) : (int)(piece.position.x + Math.Abs(piece.position.y - 7) * 8));
            } else if (piece != null) {
                value -= (int)piece.type;
                value -= PositionEvaluation.EvaluatePosition(piece.type, piece.side, piece.side == Color.Light ? (int)(piece.position.x + piece.position.y * 8) : (int)(piece.position.x + Math.Abs(piece.position.y - 7) * 8));
            }
        }
        return value;
    }

}
