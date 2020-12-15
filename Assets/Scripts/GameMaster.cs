using System.Collections.Generic;
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

    void Awake()
    {
        board = new Board(lightPieces, darkPieces);
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

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (turn == Color.Dark)
        {
            Movement movement = MinMax.PerformMinMax(board);
            selectedPiece = movement.piece;
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
        turn = turn == Color.Light ? Color.Dark : Color.Light;
        DeleteMoves();
    }

    public void DeletePiece(Vector3 position)
    {
        Destroy(board.CheckCase(position));
        board.RemovePiece(position);
    }

    public static GameObject Instantiate(GameObject gameObject, Vector3 position, Quaternion quaternion)
    {
        return Object.Instantiate(gameObject, position, quaternion);
    }

    private GameObject CreateMove(GameObject prefab, Movement movement) {
        GameObject move = Instantiate(prefab, movement.position, new Quaternion(45, 0, 0, 45));
        move.GetComponent<MovementScript>().movement = movement;
        move.transform.parent = movement.piece.transform;
        return move;
    }
}
