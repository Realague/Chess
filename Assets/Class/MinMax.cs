using System;
using System.Collections.Generic;

public class MinMax
{

    public Board board;

    private static int depth = 1;

    public Random rand = new Random();

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
        MinMax minMax = new MinMax(board);
        List<Movement> movements = minMax.board.GetAllMovements(GameMaster.instance.turn);
        if (movements.Count == 0)
        {
            return null;
        }
        int max = int.MinValue;
        int maxTmp;
        Movement movementToDo = null; 
        foreach (Movement movement in movements)
        {
            MinMax minMaxtmp = new MinMax(minMax);
            movement.DoMovement(minMaxtmp.board, true);
            maxTmp = MinMaxNodes(minMaxtmp, depth - 1, false);
            if (maxTmp > max)
            {
                max = maxTmp;
                movementToDo = movement;
            }
        }
        return movementToDo;
    }

    public static int MinMaxNodes(MinMax minMax, int depth, bool isMax)
    {
        if (depth == 0)
        {
            return minMax.CalculateScore();
        }
        
        List<Movement> movements = minMax.board.GetAllMovements(GameMaster.instance.turn);
        if (movements.Count == 0)
        {
            return isMax ? int.MinValue : int.MaxValue;
        }
        else if (isMax)
        {
            int max = int.MinValue;
            foreach (Movement movement in movements)
            {
                MinMax minMaxtmp = new MinMax(minMax);
                movement.DoMovement(minMaxtmp.board, true);
                max = Math.Max(max, MinMaxNodes(minMaxtmp, depth - 1, false));
            }
            return max;
        }
        else
        {
            int min = int.MaxValue;
            foreach (Movement movement in movements)
            {
                MinMax minMaxtmp = new MinMax(minMax);
                movement.DoMovement(minMaxtmp.board, true);
                min = Math.Min(min, MinMaxNodes(minMaxtmp, depth - 1, true));
            }
            return min;
        }
    }

    private int CalculateScore()
    {
        return rand.Next();
    }

}
