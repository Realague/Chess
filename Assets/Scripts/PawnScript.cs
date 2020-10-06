using UnityEngine;

public class PawnScript : MonoBehaviour
{

    public GameObject highlightedMove;

    public GameObject highlightedMoveKill;

    private bool isFirstMove = true;

    private bool isSelected = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnMouseDown()
    {
        isSelected = true;
        GameObject obj1 = null;
        if (isFirstMove)
        {
            obj1 = Instantiate(highlightedMove, transform.position + new Vector3(0, 0, 2), new Quaternion(45, 0, 0, 45));
        }
        GameObject obj = Instantiate(highlightedMove, transform.position + new Vector3(0, 0, 1), new Quaternion(45, 0, 0, 45));

        /*if (isFirstMove)
        {
            Destroy(obj1);
        }
        Destroy(obj);
        isFirstMove = false;*/
    }

}
