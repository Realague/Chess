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

    public GameObject queenDark;

    public GameObject queenLight;

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

    void Awake()
    {
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
    }

    public void CreateMoves(List<KeyValuePair<Vector3, MoveType>> movements, GameObject piece)
    {
        if (selectedPiece != piece && selectedPiece != null)
        {
            DeleteMoves();
            return;
        }
        selectedPiece = piece;
        foreach (KeyValuePair<Vector3, MoveType> valuePair in movements)
        {
            if (valuePair.Value == MoveType.Attack)
            {
                moves.Add(Instantiate(highlightedAttack, valuePair.Key, new Quaternion(45, 0, 0, 45)));
            }
            else if (valuePair.Value == MoveType.Move || valuePair.Value == MoveType.MovePawn)
            {
                moves.Add(Instantiate(highlightedMove, valuePair.Key, new Quaternion(45, 0, 0, 45)));
            }
            else if (valuePair.Value == MoveType.Castling)
            {
                moves.Add(Instantiate(highlightedCastling, valuePair.Key, new Quaternion(45, 0, 0, 45)));
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
        board.MovePiece(position, selectedPiece);
        selectedPiece.transform.position = position;
        selectedPiece.GetComponent<Piece>().isFirstMove = false;
        turn = turn == Color.Light ? Color.Dark : Color.Light;
        if (selectedPiece.GetComponent<Piece>().type == PieceType.Pawn)
        {
            HandlePawnFinish(position);
        }
        DeleteMoves();
    }

    private void HandlePawnFinish(Vector3 position)
    {
        if (selectedPiece.GetComponent<Piece>().side == Color.Light && position.z == 0)
        {
            DeletePiece(position);
            board.AddPiece(Instantiate(queenLight, position, new Quaternion(0, 180, 0, 0)));
        } else if (selectedPiece.GetComponent<Piece>().side == Color.Dark && position.z == 7)
        {
            DeletePiece(position);
            board.AddPiece(Instantiate(queenDark, position, new Quaternion(0, 0, 0, 0)));
        }
    }

    public void DeletePiece(Vector3 position)
    {
        Destroy(board.CheckCase(position));
        board.RemovePiece(position);
    }
}
