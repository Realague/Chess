using System;
using System.Collections.Generic;
using System.Diagnostics;

public class MinMax
{

    public static int fullDepthMoves = 4;

    public static int reductionLimit = 1;

    public static int nodes = 0;

    public static int MAX_PLY = 64;

    public static int depth = 3;

    public static bool scorePv = false;

    public static GameStage gameStage = GameStage.Early;

    public MinMax(Board board)
    {
        this.board = new Board(board);
    }

    public MinMax(MinMax minMax)
    {
        this.board = new Board(minMax.board);
    }

    public Board board;

    public static int ply = 0;

    public static Movement[,] killerMoves = new Movement[2, MAX_PLY];

    public static int[] pvLenght = new int[MAX_PLY];

    public static Movement[,] pvTable = new Movement[MAX_PLY, MAX_PLY];

    public static Dictionary<KeyValuePair<VirtualPiece, int>, int> historyMoves = new Dictionary<KeyValuePair<VirtualPiece, int>, int>();

    public static bool followPv = false;

    public static List<int> repetitionTable = new List<int>();

    public static Movement[,] SearchBestMove() {
        Stopwatch stopwatch = new Stopwatch();

        int alpha = int.MinValue + 100;
        int beta = int.MaxValue - 100;
        int score;
        followPv = false;
        scorePv = false;
        killerMoves = new Movement[2, MAX_PLY];
        historyMoves = new Dictionary<KeyValuePair<VirtualPiece, int>, int>();
        pvLenght = new int[MAX_PLY];
        pvTable = new Movement[MAX_PLY, MAX_PLY];
        nodes = 0;

        stopwatch.Start();
        MinMax minMax = new MinMax(GameMaster.instance.board);
        for (int i = 1; i <= depth; i++) {
            followPv = true;
            score = NegaMax(minMax, i, 1, alpha, beta);
            if (score <= alpha || score >= beta) {
                alpha = int.MinValue + 100;
                beta = int.MaxValue - 100;
                continue;
            }
            alpha = score - 50;
            beta = score + 50;
            String str = "Moves";
            for (int j = 0; j < pvLenght[0]; j++) {
                str += " " + pvTable[0, j].position;
            }
            UnityEngine.Debug.Log(str);
        }
        stopwatch.Stop();
        UnityEngine.Debug.Log(stopwatch.ElapsedMilliseconds);
        UnityEngine.Debug.Log("Nodes: " + MinMax.nodes);
        return pvTable;
    }
    public static int NegaMax(MinMax minMax, int depthLeft, int turn, int alpha, int beta) {
        nodes++;

        if (ply > 0 && IsRepetition(minMax.board.GetHashCode())) {
            return 0;
        }

        if (depthLeft == 0) {
            return MinMax.QuiescenceSearch(minMax, alpha, beta, turn);
        }

        if (ply > MAX_PLY - 1) {
            return turn * minMax.CalculateScore(minMax);
        }
        pvLenght[ply] = ply;

        List<Movement> movements = minMax.board.GetAllMovements(turn == 1 ? GameMaster.instance.turn : Color.Dark == GameMaster.instance.turn ? Color.Light : Color.Dark);
        if (movements.Count == 0) {
            if (minMax.board.Check(turn == 1 ? GameMaster.instance.turn : Color.Dark == GameMaster.instance.turn ? Color.Light : Color.Dark)) {
                return int.MinValue + 100 + ply;
            }
            return 0;
        }

        if (followPv == true) {
            EnablePvScoring(movements);
        }

        bool inCheck = minMax.board.Check(turn == 1 ? GameMaster.instance.turn : Color.Dark == GameMaster.instance.turn ? Color.Light : Color.Dark);

        if (inCheck) {
            depthLeft++;
        }
        int score;
        if (depthLeft >= 3 && !inCheck && ply > 0) {
            MinMax minMaxtmp = new MinMax(minMax);
            ply++;
            score = -NegaMax(minMaxtmp, depthLeft - 1 - 2, -turn, -beta, -beta + 1);
            ply--;
            if (score >= beta) {
                return beta;
            }
        }

        movements = SortMoves.SortMove(movements);

        int movesSearched = 0;

        foreach (Movement movement in movements) {
            ply++;
            MinMax minMaxtmp = new MinMax(minMax);
            movement.DoMovement(minMaxtmp.board, true);

            if (movesSearched == 0) {
                score = -NegaMax(minMaxtmp, depthLeft - 1, -turn, -beta, -alpha);
            } else {
                if (movesSearched >= fullDepthMoves && depthLeft >= reductionLimit && !inCheck && movement.moveType != MoveType.Attack) {
                    score = -NegaMax(minMaxtmp, depthLeft - reductionLimit, -turn, -alpha - 1, -alpha);
                } else {
                    score = alpha + 1;
                }

                if (score > alpha) {
                    score = -NegaMax(minMaxtmp, depthLeft - 1, -turn, -alpha - 1, -alpha);
                    if (score > alpha && score < beta) {
                        score = -NegaMax(minMaxtmp, depthLeft - 1, -turn, -beta, -alpha);
                    }
                }
            }

            movesSearched++;
            ply--;
            if (score > alpha) {
                if (movement.moveType != MoveType.Attack) {
                    if (historyMoves.ContainsKey(new KeyValuePair<VirtualPiece, int>(movement.piece, (int)(movement.position.x + movement.position.y * 8)))) {
                        historyMoves[new KeyValuePair<VirtualPiece, int>(movement.piece, (int)(movement.position.x + movement.position.y * 8))] += depth;
                    } else {
                        historyMoves.Add(new KeyValuePair<VirtualPiece, int>(movement.piece, (int)(movement.position.x + movement.position.y * 8)), depth);
                    }
                }

                alpha = score;

                pvTable[ply, ply] = movement;

                for (int nextPly = ply + 1; nextPly < pvLenght[ply + 1]; nextPly++) {
                    pvTable[ply, nextPly] = pvTable[ply + 1, nextPly];
                }
                pvLenght[ply] = pvLenght[ply + 1];

                if (score >= beta) {
                    if (movement.moveType != MoveType.Attack) {
                        killerMoves[1, ply] = killerMoves[0, ply];
                        killerMoves[0, ply] = movement;
                    }

                    return beta;
                }
            }
        }

        return alpha;
    }

    private static int QuiescenceSearch(MinMax minMax, int alpha, int beta, int turn) {
        nodes++;
        int score = turn * minMax.CalculateScore(minMax);

        if (ply > MAX_PLY - 1) {
            return score;
        }

        if (score >= beta) {
            return beta;
        }

        if (score > alpha) {
            alpha = score;
        }

        List<Movement> movements = minMax.board.GetAllMovements(turn == 1 ? GameMaster.instance.turn : Color.Dark == GameMaster.instance.turn ? Color.Light : Color.Dark);

        movements = SortMoves.SortMove(movements);

        foreach (Movement movement in movements) {
            if (movement.moveType == MoveType.Attack) {
                ply++;
                MinMax minMaxtmp = new MinMax(minMax);
                movement.DoMovement(minMaxtmp.board, true);
                score = -QuiescenceSearch(minMaxtmp, -beta, -alpha, -turn);
                //minMax.board.UndoMove();
                ply--;

                if (score > alpha) {
                    alpha = score;

                    if (score >= beta) {
                        return beta;
                    }

                }
            }
        }

        return alpha;
    }

    public static bool IsRepetition(int hashKey) {
        foreach (int move in repetitionTable) {
            if (move == hashKey) {
                return true;
            }
        }
        return false;
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
                value += PositionEvaluation.EvaluatePosition(minMax.board, piece.side == Color.Light ? (int)(piece.position.x + piece.position.y * 8) : (int)(piece.position.x + Math.Abs(piece.position.y - 7) * 8), piece);
            } else if (piece != null) {
                value -= (int)piece.type;
                value -= PositionEvaluation.EvaluatePosition(minMax.board, piece.side == Color.Light ? (int)(piece.position.x + piece.position.y * 8) : (int)(piece.position.x + Math.Abs(piece.position.y - 7) * 8), piece);
            }
        }

        //value += 10 * (minMax.board.GetAllMovements(GameMaster.instance.turn).Count - minMax.board.GetAllMovements(Color.Dark != GameMaster.instance.turn ? Color.Dark : Color.Light).Count);
        return value;
    }

    private static void EnablePvScoring(List<Movement> movememts) {
        followPv = false;

        foreach (Movement movement in movememts) {
            if (pvTable[0, ply] == movement) {
                scorePv = true;
                followPv = true;
                break;
            }
        }
    }

}
