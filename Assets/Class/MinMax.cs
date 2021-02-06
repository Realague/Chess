using System;
using System.Collections.Generic;
using System.Diagnostics;

public class MinMax
{

    public static int nodes = 0;

    public static int MAX_PLY = 64;

    public static int depth = 2;

    public static GameStage gameStage = GameStage.Early;

    public MinMax(Board board)
    {
        this.board = new Board(board);
    }

    public MinMax(MinMax minMax)
    {
        this.board = minMax.board;
    }

    public Board board;

    public int ply = 0;

    public Movement[,] killerMoves = new Movement[2, MAX_PLY];

    public int[] pvLenght = new int[MAX_PLY];

    public Movement[,] pvTable = new Movement[MAX_PLY, MAX_PLY];

    public Dictionary<KeyValuePair<VirtualPiece, int>, int> historyMoves = new Dictionary<KeyValuePair<VirtualPiece, int>, int>();

    public static Movement[,] SearchBestMove() {
        Stopwatch stopwatch = new Stopwatch();
        nodes = 0;
        stopwatch.Start();
        MinMax minMax = new MinMax(GameMaster.instance.board);
        for (int i = 1; i <= depth; i++) {
            NegaMax(minMax, i, 1, int.MinValue + 100, int.MaxValue - 100);
        }
        stopwatch.Stop();
        UnityEngine.Debug.Log(stopwatch.ElapsedMilliseconds);
        UnityEngine.Debug.Log("Nodes: " + MinMax.nodes);
        return minMax.pvTable;
    }

    public static int NegaMax(MinMax minMax, int depthLeft, int turn, int alpha, int beta) {

        nodes++;
        if (depthLeft == 0) {
            return MinMax.QuiescenceSearch(minMax, alpha, beta, turn);
        }

        if (minMax.ply > MAX_PLY - 1) {
            return turn * minMax.CalculateScore(minMax);
        }
        minMax.pvLenght[minMax.ply] = minMax.ply;

        List<Movement> movements = minMax.board.GetAllMovements(turn == 1 ? GameMaster.instance.turn : Color.Dark == GameMaster.instance.turn ? Color.Light : Color.Dark);
        if (movements.Count == 0) {
            if (minMax.board.Check(turn == 1 ? GameMaster.instance.turn : Color.Dark == GameMaster.instance.turn ? Color.Light : Color.Dark)) {
                return int.MinValue + 100 + minMax.ply;
            }
            return 0;
        }

        if (minMax.board.Check(turn == 1 ? GameMaster.instance.turn : Color.Dark == GameMaster.instance.turn ? Color.Light : Color.Dark)) {
            depthLeft++;
        }

        movements = SortMoves.SortMove(movements, minMax);

        foreach (Movement movement in movements) {
            minMax.ply++;
            movement.DoMovement(minMax.board, true);
            int score = -NegaMax(minMax, depthLeft - 1, -turn, -beta, -alpha);
            minMax.board.UndoMove();
            minMax.ply--;
            if (score >= beta) {
                if (movement.moveType != MoveType.Attack) {
                    minMax.killerMoves[1, minMax.ply] = minMax.killerMoves[0, minMax.ply];
                    minMax.killerMoves[0, minMax.ply] = movement;
                }

                return beta;
            }
            if (score > alpha) {
                if (movement.moveType != MoveType.Attack) {
                    if (minMax.historyMoves.ContainsKey(new KeyValuePair<VirtualPiece, int>(movement.piece, (int)(movement.position.x + movement.position.y * 8)))) {
                        minMax.historyMoves[new KeyValuePair<VirtualPiece, int>(movement.piece, (int)(movement.position.x + movement.position.y * 8))] += depth;
                    } else {
                        minMax.historyMoves.Add(new KeyValuePair<VirtualPiece, int>(movement.piece, (int)(movement.position.x + movement.position.y * 8)), depth);
                    }
                }

                minMax.pvTable[minMax.ply, minMax.ply] = movement;

                for (int i = minMax.ply + 1; i < minMax.pvLenght[minMax.ply + 1]; i++) {
                    minMax.pvTable[minMax.ply, i] = minMax.pvTable[minMax.ply + 1, i];
                }

                minMax.pvLenght[minMax.ply] = minMax.pvLenght[minMax.ply + 1];
                
                alpha = score;
            }
        }

        return alpha;
    }

    private static int QuiescenceSearch(MinMax minMax, int alpha, int beta, int turn) {
        nodes++;
        int score = turn * minMax.CalculateScore(minMax);

        if (score >= beta) {
            return beta;
        }

        if (score > alpha) {
            alpha = score;
        }

        List<Movement> movements = minMax.board.GetAllMovements(turn == 1 ? GameMaster.instance.turn : Color.Dark == GameMaster.instance.turn ? Color.Light : Color.Dark);

        movements = SortMoves.SortMove(movements, minMax);

        foreach (Movement movement in movements) {
            if (movement.moveType == MoveType.Attack) {
                minMax.ply++;
                movement.DoMovement(minMax.board, true);
                score = -QuiescenceSearch(minMax, -beta, -alpha, -turn);
                minMax.board.UndoMove();
                minMax.ply--;
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
        int nbLightPiece = 0;
        int nbDarkPiece = 0;
        if (gameStage == GameStage.Early) {
            foreach (VirtualPiece piece in minMax.board.pieces) {
                int nbToAdd = 0;
                if (piece != null && piece.type == PieceType.Queen) {
                    nbToAdd = 2;
                } else if (piece != null && piece.type != PieceType.Pawn && piece.type != PieceType.King) {
                    nbToAdd = 1;
                }

                if (piece != null && piece.side == Color.Light) {
                    nbLightPiece += nbToAdd;
                } else {
                    nbDarkPiece += nbToAdd;
                }

                if (nbDarkPiece > 3 || nbLightPiece > 3) {
                    break;
                }
            }
        }

        if (nbLightPiece <= 3 && nbDarkPiece <= 3) {
            gameStage = GameStage.Late;
        }

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

        value += 10 * (minMax.board.GetAllMovements(GameMaster.instance.turn).Count - minMax.board.GetAllMovements(Color.Dark != GameMaster.instance.turn ? Color.Dark : Color.Light).Count);
        return value;
    }

}
