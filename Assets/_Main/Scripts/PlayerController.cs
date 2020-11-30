using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    [Header("Piece Management")]
    public GridPiece selectedPiece;
    public GridPiece targetPiece;

    [Header("States")]
    public bool piecesSwaping;
    public bool pieceSelected;
    public bool dragging;

    private GridPiece[] pieceBuffer;
    private GridManager gridManager;
    private SessionManager sessionManager;
    private bool decidingResult;
    private bool swapback;


    #region DefaultMethods
    private void Awake()
    {
        gridManager = FindObjectOfType<GridManager>();

        sessionManager = FindObjectOfType<SessionManager>();
        pieceBuffer = new GridPiece[2];
    }

    #endregion

    #region Selection

    public void OnPieceSelected(GridPiece piece)
    {
        if (!piecesSwaping && !decidingResult)
        {
            selectedPiece = piece;
            pieceSelected = true;
            pieceBuffer[0] = piece;

            selectedPiece.SelectedStatus(true);
            AudioManager.Play_Sound(0);
        }
    }

    public void OnDragSelection(GridPiece piece)
    {   
        if (!piecesSwaping && !decidingResult && piece != selectedPiece)
        {
            targetPiece = piece;
            pieceBuffer[1] = piece;

            targetPiece.SelectedStatus(true);
            AudioManager.Play_Sound(0);
        }
    }

    private void ResetStatus()
    {
        selectedPiece = null;
        targetPiece = null;

        piecesSwaping = false;
        swapback = false;
        pieceSelected = false;
        decidingResult = false;
    }

    public void CancelOperation()
    {
        if (selectedPiece != null)
            selectedPiece.SelectedStatus(false);

        if (targetPiece != null)
            targetPiece.SelectedStatus(false);

        ResetStatus();
    }

    #endregion

    #region Movement

    public void Swap(GridPiece piece)
    {
        if (!decidingResult && piece != selectedPiece)
        {
            decidingResult = true;
            targetPiece = piece;
            pieceBuffer[1] = piece;

            if (!piecesSwaping)
            {
                piecesSwaping = true;

                selectedPiece.SelectedStatus(false);
                targetPiece.SelectedStatus(false);

                GridSpace parent1 = selectedPiece.currentGridSpace;
                GridSpace parent2 = targetPiece.currentGridSpace;

                selectedPiece.SwapTo(parent2);
                targetPiece.SwapTo(parent1);

                selectedPiece = null;
                pieceSelected = false;
            }

            AudioManager.Play_Sound(1);
            StartCoroutine(OnMovementFinished());
        }
    }

    public void SwapBack()
    {
        if (decidingResult)
        {
            if (!piecesSwaping)
            {
                GridSpace parent1 = pieceBuffer[0].currentGridSpace;
                GridSpace parent2 = pieceBuffer[1].currentGridSpace;

                pieceBuffer[0].SwapTo(parent2);
                pieceBuffer[1].SwapTo(parent1);
            }

            AudioManager.Play_Sound(1);

            ResetStatus();
        }
    }


    public IEnumerator OnMovementFinished()
    {
        yield return new WaitForSeconds(Time.deltaTime * 25f);

        yield return StartCoroutine(gridManager.CheckMatches());


        if (gridManager.matchedSpaces.Count == 0)
        {
            SwapBack();
        }
        else
        {
            StartCoroutine(OnMatch());
        }
        
    }

    public IEnumerator OnMatch()
    {
        yield return StartCoroutine(gridManager.OnMatchPlay());

        yield return StartCoroutine(gridManager.Cascade());

        yield return new WaitForSeconds(Time.deltaTime * 35f);

        yield return StartCoroutine(gridManager.CheckMatches());

        if (gridManager.matchedSpaces.Count != 0)
        {

            StartCoroutine(OnMatch());

        }
        else
        {
            decidingResult = false;
        }
    }

    #endregion

  

}
