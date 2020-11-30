
using UnityEngine;

public class GridPiece : MonoBehaviour
{
    /// <summary>
    /// Peça de jogo
    /// </summary>    
    /// 
    [Header("Grid Piece Properties")]
    public int myCode;
    public Sprite[] pieceSprites;
    public Color[] pieceColors;
    public SpriteRenderer mySpriteRenderer;
    public GridSpace currentGridSpace;

    public Transform spriteTransform;

    private PlayerController controller;
    private GridPiece thisGridPiece;
    private bool swapping;
    private bool selected;
    public ParticleSystem onDestroyParticle;

    #region DefaultMethods
    private void Awake()
    {
        thisGridPiece = GetComponent<GridPiece>();
        onDestroyParticle = GetComponent<ParticleSystem>();
    }

    private void Update()
    {
        if(swapping)
        PieceMovement();
    }

    #endregion

    #region PieceManagement
    public void OnPieceMoved()
    {
        transform.parent.GetComponent<GridSpace>().currentPiece = thisGridPiece;
    }

    public void OnPieceSelected()
    {
        GetController();

        if (!controller.pieceSelected)
        {
            controller.OnPieceSelected(thisGridPiece);            
        }
        else
        {
            if (currentGridSpace.IsNeighbour(controller.selectedPiece.currentGridSpace))
            {
                controller.Swap(thisGridPiece);                
            }
            else
            {
                controller.CancelOperation();
            }


        }
    }

    public void OnClickUp()
    {
        GetController();

        if (controller.pieceSelected && controller.selectedPiece != null)
        {
            if (currentGridSpace.IsNeighbour(controller.selectedPiece.currentGridSpace))
            {
                controller.Swap(thisGridPiece);                
            }
            else
            {
                controller.CancelOperation();
            }
        }
    }

    public void OnDragOver()
    {

        GetController();
        if (controller.pieceSelected && controller.selectedPiece != null)
        {
            if (currentGridSpace.IsNeighbour(controller.selectedPiece.currentGridSpace))
            {
                controller.OnDragSelection(thisGridPiece);                
            }
            else
            {
                controller.CancelOperation();
            }
        }
    }

    public void SwapTo(GridSpace gridSpace)
    {
        swapping = true;
        transform.parent = gridSpace.transform;
    }

    private void PieceMovement()
    {
        if (Vector3.Distance(transform.localPosition, Vector3.zero) != 0)
        {
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, Vector3.zero, Time.deltaTime * 15f);
        }
        else
        {
            PieceDestinationReached();
        }
    }

    public void PieceDestinationReached()
    {
        GetController();

        swapping = false;
        controller.piecesSwaping = false;

        currentGridSpace = transform.parent.GetComponent<GridSpace>();
        currentGridSpace.currentPiece = thisGridPiece;


    }

    public void MatchDestroy()
    {
        GetComponent<BoxCollider2D>().enabled = false;

        Animator myAnim = GetComponent<Animator>();
        myAnim.enabled = true;
        myAnim.SetTrigger("Activate");

        AudioManager.Play_Sound(2);

        currentGridSpace.currentPiece = null;
        currentGridSpace.isEmpty = true;
        transform.parent = null;

        onDestroyParticle.Play();


    }

    #endregion

    #region Visual

    public void SelectedStatus(bool selected)
    {
        if (selected)
        {
            spriteTransform.localScale = new Vector3(1f, 1f, 1f);
        }
        else
        {
            spriteTransform.localScale = new Vector3(0.7f, 0.7f, 0.7f);
        }
    }

    #endregion

    #region Utility
    private void GetController()
    {
        if (controller == null)
            controller = FindObjectOfType<PlayerController>();

        if (currentGridSpace == null)
            currentGridSpace = transform.parent.GetComponent<GridSpace>();
    }

    public void SetPieceProperties(int code)
    {
        myCode = code;

        mySpriteRenderer.sprite = pieceSprites[code];
        mySpriteRenderer.color = pieceColors[code];

        gameObject.name = code.ToString();

        spriteTransform = mySpriteRenderer.transform;

    }
    #endregion
}
