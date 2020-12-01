using System.Collections;
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

    public static GameObject queenDark;

    public static GameObject queenLight;

    public static GameMaster instance = null;

    private bool isPlayerTurn = true;

    public Color turn = Color.Light;

    public GameObject selectedPiece;

    [SerializeField]
    private List<GameObject> lightPieces = new List<GameObject>();

    [SerializeField]
    private List<GameObject> darkPieces = new List<GameObject>();

    public Board board;

    private List<GameObject> moves;

    private MinMax minMax;


    void Awake()
    {
        minMax = new MinMax(board, Color.Dark);
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
        board = new Board(lightPieces, darkPieces);
        selectedPiece = null;
        moves = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        if (turn == Color.Dark)
        {
            minMax.board = board;
            Movement movement = minMax.MinMaxAlgorithm();
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
                moves.Add(Instantiate(highlightedAttack, movement.position, new Quaternion(45, 0, 0, 45)));
            }
            else if (movement.moveType == MoveType.Move || movement.moveType == MoveType.MovePawn)
            {
                moves.Add(Instantiate(highlightedMove, movement.position, new Quaternion(45, 0, 0, 45)));
            }
            else if (movement.moveType == MoveType.Castling)
            {
                moves.Add(Instantiate(highlightedCastling, movement.position, new Quaternion(45, 0, 0, 45)));
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

    public void Move(Vector3 position)
    {

    }

    public void EndTurn()
    {
        selectedPiece.GetComponent<Piece>().isFirstMove = false;
        selectedPiece = null;
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
}
