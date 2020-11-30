using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    /// <summary>
    /// Responsável pela criação, instância e organização da grid de jogo
    /// </summary>

    [Header("Grid size")]
    public int gridSquareSize = 8;
    public int pieceVariety = 5;
    public float gridSpacing = 2f;


    [Header("Grid Management")]
    public List<GridSpace> gridSpaces;

    [Header("Components and prefabs")]
    public GameObject gridPrefab;
    public GameObject piecePrefab;

    [Header("Matching")]
    public List<GridSpace> matchedSpaces;
    public bool softLocked = true;
    public bool empitySpaces = false;
    public bool empitySpacesBellow = false;

    private SessionManager sessionManager;
    private int previousGeneratedPiece = -1;
    
    #region DefaultMethods
    private void Awake()
    {
        sessionManager = FindObjectOfType<SessionManager>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(EmpityCheck());
        }
    }

    #endregion

    #region PieceMovement


    #endregion
    
    #region Instance

    public void CreateGrid()
    {
        for (int y = 0; y < gridSquareSize; y++)
        {
            for (int x = 0; x < gridSquareSize; x++)
            {
                GameObject gridSpace = Instantiate(gridPrefab) as GameObject;
                Transform gridSpaceTR = gridSpace.GetComponent<Transform>();
                GridSpace gridSpaceScript = gridSpace.GetComponent<GridSpace>();

                gridSpaceTR.parent = transform;
                gridSpaceTR.localPosition = new Vector3(gridSpacing * x, gridSpacing * -y, 0);
                gridSpaceScript.SetCoordinates(x, y,gridSquareSize);
                
                gridSpaces.Add(gridSpaceScript);
                gridSpace.name = y + "/" + x;
            }
        }
    }

    public void GeneratePieces()
    {
        for (int i = 0; i < gridSpaces.Count; i++)
        {
            GameObject gamePiece = Instantiate(piecePrefab) as GameObject;
            Transform gamePieceTR = gamePiece.GetComponent<Transform>();
            GridPiece gamePieceScript = gamePiece.GetComponent<GridPiece>();

            gamePieceTR.parent = gridSpaces[i].transform;
            gamePieceTR.localPosition = Vector3.zero;
            gamePieceScript.SetPieceProperties(Random.Range(0, 5));
            gamePieceScript.currentGridSpace = gridSpaces[i];
            gamePieceScript.PieceDestinationReached();

        }
    }

    public IEnumerator ReplenishPiece(GridSpace space)
    {
        int pieceSeed = Random.Range(0, pieceVariety);

        while (pieceSeed == previousGeneratedPiece )
        {
            pieceSeed = Random.Range(0, pieceVariety);
        }

        GameObject gamePiece = Instantiate(piecePrefab) as GameObject;
        Transform gamePieceTR = gamePiece.GetComponent<Transform>();
        GridPiece gamePieceScript = gamePiece.GetComponent<GridPiece>();

        gamePieceTR.parent = space.transform;
        gamePieceTR.localPosition = Vector3.zero;
        gamePieceScript.SetPieceProperties(pieceSeed);
        gamePieceScript.currentGridSpace = space;
        gamePieceScript.PieceDestinationReached();

        previousGeneratedPiece = pieceSeed;
        yield return null;
    }

    #endregion


    #region Matching

    public IEnumerator OnMatchPlay()
    {
        for (int i = 0; i < matchedSpaces.Count; i++)
        {
            matchedSpaces[i].currentPiece.MatchDestroy();
          
        }
        EraseMatches();
        yield return null;
    }

    public IEnumerator NeighbourhoodCheck()
    {

        for (int i = 0; i < gridSpaces.Count; i++)
        {
            gridSpaces[i].CheckNeighbours();
        }
        yield return null;
    }

    public IEnumerator CheckMatches()
    {
        for (int i = 0; i < gridSpaces.Count; i++)
        {
            gridSpaces[i].MatchCheck();            
        }

        yield return null;
    }

    public IEnumerator CheckSoftLock()
    {
        softLocked = true;

        for (int i = 0; i < gridSpaces.Count; i++)
        {
            gridSpaces[i].SoftLockCheck();
        }

        yield return null;
    }

    public IEnumerator ClearPrematureMatches()
    {
        for (int i = 0; i < matchedSpaces.Count; i++)
        {
            matchedSpaces[i].Reroll();
        }

        yield return null;
    }

    public void EraseMatches()
    {
        if(sessionManager.gameIsRunning)
        sessionManager.AddScore(matchedSpaces.Count);
        

        matchedSpaces.Clear();
        
    }
    #endregion

    #region Cascading

    public IEnumerator Cascade()
    {

        yield return new WaitForEndOfFrame();

        for (int i = 0; i < gridSpaces.Count; i++)
        {
            if (gridSpaces[i].isEmpty)
            {
                yield return StartCoroutine(gridSpaces[i].CascadePiece());
            }
        }
        yield return StartCoroutine(EmpityCheck());        
    }

    public IEnumerator EmpityCheck()
    {
        empitySpaces = false;
        empitySpacesBellow = false;
        for (int i = 0 ; i < gridSpaces.Count; i++)
        {

            if (gridSpaces[i].transform.childCount == 0)
            {
                gridSpaces[i].isEmpty = true;

                empitySpaces = true;
            }
            else
            {
                gridSpaces[i].isEmpty = false;
            }

            if (!gridSpaces[i].cornered[1] && !gridSpaces[i].isEmpty)
            {
                if (gridSpaces[i].neighbours[1].isEmpty)
                    empitySpacesBellow = true;
            }
        }

        if (empitySpacesBellow)
        {
            yield return StartCoroutine(Cascade());
        }
        else
        {

            if (!empitySpacesBellow && empitySpaces)
            {
                yield return StartCoroutine(ReplenishCheck());
            }

        }
    }

    public IEnumerator ReplenishCheck()
    {
        for (int i = 0; i < gridSquareSize ; i++)
        {
            if (gridSpaces[i].isEmpty)
            {
                yield return StartCoroutine(ReplenishPiece(gridSpaces[i]));
            }
        }
        yield return StartCoroutine(EmpityCheck());
    }
    

    #endregion


}
