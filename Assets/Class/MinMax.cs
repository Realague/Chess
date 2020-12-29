using System;
using System.Collections.Generic;
using System.Threading;

public class MinMax
{

    public Board board;

    private static int depth = 5;

    private static Mutex mutex = new Mutex(true, "results");


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
        Thread thread;

        List<Movement> movements = board.GetAllMovements(GameMaster.instance.turn);
        if (movements.Count == 0)
        {
            return null;
        }
        int max = int.MinValue;
        int maxTmp;
        int alpha = int.MinValue;
        int beta = int.MaxValue;
        Movement movementToDo = null; 
        foreach (Movement movement in movements)
        {
            MinMax minMax = new MinMax(board);
            movement.DoMovement(minMax.board, true);
            thread = new Thread(() => ThreadManager(minMax, depth - 1, false, alpha, beta));
            thread.Start();
            //maxTmp = MinMaxNodes(minMax, depth - 1, false, alpha, beta);
            if (maxTmp > max)
            {
                max = maxTmp;
                movementToDo = movement;
            }
            alpha = Math.Max(alpha, max);
            if (beta <= alpha) {
                break;
            }
        }
        return movementToDo;
    }

    public static void ThreadManager(MinMax minMax, int depth, bool isMax, int alpha, int beta) {

    }

    public static int MinMaxNodes(MinMax minMax, int depth, bool isMax, int alpha, int beta)
    {
        if (depth == 0)
        {
            return minMax.CalculateScore(minMax);
        }
        
        List<Movement> movements = minMax.board.GetAllMovements(isMax ? GameMaster.instance.turn : Color.Dark == GameMaster.instance.turn ? Color.Light : Color.Dark);
        if (movements.Count == 0)
        {
            return isMax ? int.MinValue : int.MaxValue;
        }
        else if (isMax)
        {
            int max = int.MinValue;
            foreach (Movement movement in movements)
            {
                movement.DoMovement(minMax.board, true);
                max = Math.Max(max, MinMaxNodes(minMax, depth - 1, false, alpha, beta));
                alpha = Math.Max(alpha, max);
                minMax.board.UndoMove();
                if (beta <= alpha) {
                    break;
                }
            }
            return max;
        }
        else
        {
            int min = int.MaxValue;
            foreach (Movement movement in movements)
            {
                movement.DoMovement(minMax.board, true);
                min = Math.Min(min, MinMaxNodes(minMax, depth - 1, true, alpha, beta));
                beta = Math.Min(beta, min);
                minMax.board.UndoMove();
                if (beta <= alpha) {
                    break;
                }
            }
            return min;
        }
    }

    private int CalculateScore(MinMax minMax)
    {
        int value = 0;
        foreach (VirtualPiece piece in minMax.board.pieces) {
            if (piece != null && piece.side == GameMaster.instance.turn) {
                value += (int)piece.type;
                if (piece.type == PieceType.Bishop) {
                    value += 1;
                }
            } else if (piece != null) {
                value -= (int)piece.type;
                if (piece.type == PieceType.Bishop) {
                    value -= 1;
                }
            }
        }
        return value;
    }

}
