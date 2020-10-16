using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMaster : MonoBehaviour
{

    [SerializeField]
    private GameObject highlightedMove;

    [SerializeField]
    private GameObject highlightedAttack;

    public static GameMaster instance = null;

    private bool isPlayerTurn = true;

    private bool isPlayerStarting = true;

    public GameObject selectedPiece;

    [SerializeField]
    private List<GameObject> lightPieces;

    [SerializeField]
    private List<GameObject> darkPieces;

    public Dictionary<Vector3, GameObject> pieces;

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
        selectedPiece = null;
        moves = new List<GameObject>();
        pieces = new Dictionary<Vector3, GameObject>();
        int i = 0;
        foreach (GameObject piece in darkPieces)
        {
            pieces.Add(new Vector3(i % 8, 0, i / 8), piece);
            i++;
        }

        i = 0;
        foreach (GameObject piece in lightPieces)
        {
            pieces.Add(new Vector3(i % 8, 0, 7 - i / 8), piece);
            i++;
        }

        if (!isPlayerStarting)
        {
            isPlayerTurn = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isPlayerTurn)
        {

        }
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
            moves.Add(Instantiate(valuePair.Value == MoveType.Move ? highlightedMove : highlightedAttack, valuePair.Key, new Quaternion(45, 0, 0, 45)));
        }
    }

    /*public bool CreateMovePawn(Vector3 position, GameObject piece)
    {
        if (selectedPiece != piece && selectedPiece != null)
        {
            DeleteMoves();
        }
        if (PieceMovementHelper.CheckCase(position) == null && position.x >= 0 && position.x < 8 && position.z >= 0 && position.z < 8)
        {
            selectedPiece = piece;
            moves.Add(Instantiate(highlightedMove, position, new Quaternion(45, 0, 0, 45)));
            return true;
        }
        return false;
    }
    public bool CreateAttackMovePawn(Vector3 position, GameObject piece)
    {
        if (selectedPiece != piece && selectedPiece != null)
        {
            DeleteMoves();
        }

        GameObject foundPiece = PieceMovementHelper.CheckCase(position, pieces);
        if (foundPiece != null)
        {
            if (foundPiece.GetComponent<Piece>().side != piece.GetComponent<Piece>().side)
            {
                selectedPiece = piece;
                moves.Add(Instantiate(highlightedAttack, position, new Quaternion(45, 0, 0, 45)));
            }
            return true;
        }
        return false;
    }*/


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
        Destroy(PieceMovementHelper.CheckCase(position, pieces));
        MovePiece(position);
    }

    public void MovePiece(Vector3 position)
    {
        pieces[position] = pieces[selectedPiece.transform.position];
        pieces.Remove(selectedPiece.transform.position);
    }

    private bool SimulateKing(GameObject piece, Vector3 position)
    {
        Dictionary<Vector3, GameObject> piecesCopy = pieces;
        piecesCopy[position] = piecesCopy[piece.transform.position];
        piecesCopy.Remove(piece.transform.position);


    }
}
