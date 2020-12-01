using System;
using System.Collections;
using System.Collections.Generic;

public class MinMax
{
    private static int depth = 6;

    public Board board;

    public Color color;

    public Random rand = new Random();

    public MinMax(Board board, Color color)
    {
        this.color = color;
        this.board = board;
    }

    public Movement MinMaxAlgorithm()
    {
        List<Movement> movements = board.GetAllMovements();
        Movement selectedMove = null;
        int score = 0;
        int tmpScore = 0;

        foreach (Movement movement in movements)
        {
            if ((tmpScore = CalculateScore(movement)) > score)
            {
                score = tmpScore;
                selectedMove = movement;
            }
        }
        return selectedMove;
    }

    private int CalculateScore(Movement movement)
    {
        return rand.Next();
    }

}
