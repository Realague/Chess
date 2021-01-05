using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class GameMaster : MonoBehaviour
{

    [SerializeField]
    private GameObject highlightedMove;

    [SerializeField]
    private GameObject highlightedAttack;

    [SerializeField]
    private GameObject highlightedCastling;

    public GameObject queenDark;

    public GameObject queenLight;

    public static GameMaster instance = null;

    public Color turn = Color.Light;

    public GameObject selectedPiece;

    [SerializeField]
    private List<GameObject> lightPieces = new List<GameObject>();

    [SerializeField]
    private List<GameObject> darkPieces = new List<GameObject>();

    public Board board;

    private List<GameObject> moves;

    public Dictionary<Vector2, GameObject> piecesObject;

    void Awake()
    {
        piecesObject = new Dictionary<Vector2, GameObject>();

        int i = 0;
        foreach (GameObject piece in darkPieces) {
            piecesObject.Add(new Vector2(i % 8, i / 8), piece);
            i++;
        }

        i = 0;
        foreach (GameObject piece in lightPieces) {
            piecesObject.Add(new Vector2(i % 8, 7 - i / 8), piece);
            i++;
        }

        board = new Board(piecesObject);
        selectedPiece = null;
        moves = new List<GameObject>();
        if (instance == null)
        {
            instance = this;
        } else
        {
            Destroy(this);
        }
        DontDestroyOnLoad(this);
    }

    // Update is called once per frame
    void Update()
    {
        if (turn == Color.Dark)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            Movement movement = MinMax.PerformMinMax(board);
            stopwatch.Stop();
            UnityEngine.Debug.Log(stopwatch.ElapsedMilliseconds);
            selectedPiece = piecesObject[new Vector2(movement.piece.position.x, movement.piece.position.y)];
            movement.DoMovement(board, false);
        }
    }

    public void CreateMoves(List<Movement> movements, GameObject piece)
    {
        if (selectedPiece != piece && selectedPiece != null)
        {
            DeleteMoves();
            return;
        }
        selectedPiece = piece;
        foreach (Movement movement in movements)
        {
            if (movement.moveType == MoveType.Attack)
            {
                moves.Add(CreateMove(highlightedAttack, movement));
            } else if (movement.moveType == MoveType.Move || movement.moveType == MoveType.MovePawn)
            {
                moves.Add(CreateMove(highlightedMove, movement));
            } else if (movement.moveType == MoveType.Castling)
            {
                moves.Add(CreateMove(highlightedCastling, movement));
            }
        }
    }

    public void Move(Vector2 position, Vector2 newPosition) {
        piecesObject.Add(newPosition, piecesObject[position]);
        piecesObject.Remove(position);
        piecesObject[newPosition].transform.position = Utils.Vector2ToVector3(newPosition);
    }

    public void DeleteMoves()
    {
        foreach (GameObject move in moves)
        {
            Destroy(move);
        }
        selectedPiece = null;
    }

    public void EndTurn()
    {
        selectedPiece.GetComponent<Piece>().isFirstMove = false;
        board.pieces[(int)selectedPiece.transform.position.x, (int)selectedPiece.transform.position.z].isFirstMove = false;
        turn = turn == Color.Light ? Color.Dark : Color.Light;
        DeleteMoves();
    }

    public void DeletePiece(Vector2 position)
    {
        Destroy(piecesObject[position]);
        board.RemovePiece(position);
        piecesObject.Remove(position);
    }

    public static GameObject Instantiate(GameObject gameObject, Vector3 position, Quaternion quaternion)
    {
        return Object.Instantiate(gameObject, position, quaternion);
    }

    private GameObject CreateMove(GameObject prefab, Movement movement) {
        GameObject move = Instantiate(prefab, Utils.Vector2ToVector3(movement.position), new Quaternion(45, 0, 0, 45));
        move.GetComponent<MovementScript>().movement = movement;
        //move.transform.parent = piecesObject[new Vector2(movement.piece.position.x, movement.piece.position.y)].transform;
        return move;
    }
}
