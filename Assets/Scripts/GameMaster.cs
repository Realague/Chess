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

    public static GameMaster instance = null;

    private bool isPlayerTurn = true;

    public Color turn = Color.Light;

    public GameObject selectedPiece;

    [SerializeField]
    private List<GameObject> lightPieces;

    [SerializeField]
    private List<GameObject> darkPieces;

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

    public void DeletePiece(Vector3 position)
    {
        Destroy(board.CheckCase(position));
        board.MovePiece(position, selectedPiece);
    }

    /*private bool SimulateKing(GameObject piece, Vector3 position)
    {
        Dictionary<Vector3, GameObject> piecesCopy = pieces;
        piecesCopy[position] = piecesCopy[piece.transform.position];
        piecesCopy.Remove(piece.transform.position);


    }*/
}
