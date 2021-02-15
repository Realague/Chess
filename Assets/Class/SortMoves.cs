using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SortMoves {

    private static int[,] mvvLva = new int[6, 6] {
        { 105, 205, 305, 405, 505, 605 },
        { 104, 204, 304, 404, 504, 604 },
        { 103, 203, 303, 403, 503, 603 },
        { 102, 202, 302, 402, 502, 602 },
        { 101, 201, 301, 401, 501, 601 },
        { 100, 200, 300, 400, 500, 600 }
    };

    private static int ScoreMove(Movement movement) {
        if (MinMax.scorePv) {
            // make sure we are dealing with PV move
            if (MinMax.pvTable[0, MinMax.ply] == movement) {
                // disable score PV flag
                MinMax.scorePv = false;

                return 20000;
            }
        }

        if (movement.moveType == MoveType.Attack) {
            return mvvLva[TypeToId(movement.piece.type), TypeToId(movement.pieceAttacked.type)] + 10000;
        } else if (MinMax.ply < MinMax.MAX_PLY) {
            if (MinMax.killerMoves[0, MinMax.ply] == movement) {
                return 9000;
            } else if (MinMax.killerMoves[1, MinMax.ply] == movement) {
                return 8000;
            } else if (MinMax.historyMoves.ContainsKey(new KeyValuePair<VirtualPiece, int>(movement.piece, (int)(movement.position.x + movement.position.y * 8)))) {
                return MinMax.historyMoves[new KeyValuePair<VirtualPiece, int>(movement.piece, (int)(movement.position.x + movement.position.y * 8))];
            } else {
                return 0;
            }
        }
        return 0;
    }

    public static List<Movement> SortMove(List<Movement> movements) {
        List<int> scores = new List<int>();

        foreach (Movement movement in movements) {
            scores.Add(ScoreMove(movement));
        }

        for (int i = 0; i != movements.Count; i++) {
            for (int o = i + 1; o != movements.Count; o++) {
                if (scores[i] < scores[o]) {
                    int tmp = scores[o];
                    scores[o] = scores[i];
                    scores[i] = tmp;

                    Movement tmpMove = movements[o];
                    movements[o] = movements[i];
                    movements[i] = tmpMove;
                }
            }
        }
        return movements;
    }

    public static void PrintMoves(List<Movement> movements) {
        Debug.Log("Print move score");
        foreach (Movement movement in movements) {
            Debug.Log(ScoreMove(movement));
        }
    }

    private static int TypeToId(PieceType pieceType) {
        switch (pieceType) {
            case PieceType.Pawn:
                return 0;
            case PieceType.Knight:
                return 1;
            case PieceType.Bishop:
                return 2;
            case PieceType.Rook:
                return 3;
            case PieceType.Queen:
                return 4;
            case PieceType.King:
                return 5;
            default:
                return 0;
        }
    }
}
