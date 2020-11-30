using System.Collections;
using UnityEngine;

public class GridSpace : MonoBehaviour
{
    [Header("Status")]
    public bool matched;
    public bool centralMatch;
    public bool hasSolution;
    public bool isEmpty;
    public GridPiece currentPiece;


    [Header("Properties")]
    public int gridSquareSize;
    public int xCoord;
    public int yCoord;

    [Header("Neighbourhood")]
    public GridSpace[] neighbours; // 0 = up, 1 = down, 2 = left, 3 = right;    
    public bool[] cornered;

    private GridManager gridManager;
    private GridSpace thisGridSpace;

    
    #region SetUp
    public void SetCoordinates(int x, int y, int size)
    {
        gridSquareSize = size;
        xCoord = x;
        yCoord = y;

        gridManager = transform.parent.GetComponent<GridManager>();


        cornered = new bool[4];
        neighbours = new GridSpace[4];
        thisGridSpace = GetComponent<GridSpace>();
        CheckCornering();

    }

    private void CheckCornering()
    {

        if (yCoord == 0)
            cornered[0] = true;

        if (yCoord == gridManager.gridSquareSize - 1)
            cornered[1] = true;

        if (xCoord == 0)
            cornered[2] = true;

        if (xCoord == gridManager.gridSquareSize - 1)
            cornered[3] = true;
    }

    #endregion

    #region Neighbouring

    public void CheckNeighbours()
    {
        for (int i = 0; i < gridManager.gridSpaces.Count; i++)
        {
            if (!cornered[0])
            {
                if (gridManager.gridSpaces[i].xCoord == xCoord  && gridManager.gridSpaces[i].yCoord == yCoord-1 )
                    neighbours[0] = gridManager.gridSpaces[i];
            }
            if (!cornered[1])
            {
                if (gridManager.gridSpaces[i].xCoord == xCoord && gridManager.gridSpaces[i].yCoord == yCoord + 1)
                    neighbours[1] = gridManager.gridSpaces[i];
            }
            if (!cornered[2])
            {
                if (gridManager.gridSpaces[i].xCoord == xCoord -1 && gridManager.gridSpaces[i].yCoord == yCoord )
                    neighbours[2] = gridManager.gridSpaces[i];
            }
            if (!cornered[3])
            {
                if (gridManager.gridSpaces[i].xCoord == xCoord +1 && gridManager.gridSpaces[i].yCoord == yCoord )
                    neighbours[3] = gridManager.gridSpaces[i];
            }
        }
    }

    #endregion

    #region Matching

    public void MatchCheck()
    {
        //Vertical
        if (!cornered[0] && !cornered[1])
        {
            if (neighbours[0].currentPiece.myCode == currentPiece.myCode && neighbours[1].currentPiece.myCode == currentPiece.myCode)
            {
                matched = true;
                centralMatch = true;
                neighbours[0].RippleMatch();
                neighbours[1].RippleMatch();

                if (!gridManager.matchedSpaces.Contains(GetComponent<GridSpace>()))
                gridManager.matchedSpaces.Add(GetComponent<GridSpace>());
            }
        }

        // Horizontal
        if (!cornered[2] && !cornered[3])
        {
            if (neighbours[2].currentPiece.myCode == currentPiece.myCode && neighbours[3].currentPiece.myCode == currentPiece.myCode)
            {
                matched = true;
                centralMatch = true;
                neighbours[2].RippleMatch();
                neighbours[3].RippleMatch();

                if (!gridManager.matchedSpaces.Contains(GetComponent<GridSpace>()))
                    gridManager.matchedSpaces.Add(GetComponent<GridSpace>());
            }
        }

    }

    public void SoftLockCheck()
    {
        // Método 1 - Vizinhos diretos
        if (!cornered[0] && !cornered[1] )
        {
            if (neighbours[0].currentPiece.myCode == neighbours[1].currentPiece.myCode)
            {
                int referenceCode = neighbours[0].currentPiece.myCode;

                if (!cornered[2])
                {
                    if (neighbours[2].currentPiece.myCode == referenceCode)
                        hasSolution = true;                                        
                }
                if (!cornered[3])
                {
                    if (neighbours[3].currentPiece.myCode == referenceCode)
                        hasSolution = true;
                }

            }
        }

        if (!cornered[2] && !cornered[3])
        {
            if (neighbours[2].currentPiece.myCode == neighbours[3].currentPiece.myCode)
            {
                int referenceCode = neighbours[2].currentPiece.myCode;

                if (!cornered[0])
                {
                    if (neighbours[0].currentPiece.myCode == referenceCode)
                        hasSolution = true;
                }
                if (!cornered[1])
                {
                    if (neighbours[1].currentPiece.myCode == referenceCode)
                        hasSolution = true;
                }

            }
        }

        // Método 2 - vizinhos dos vizinhos
        if (!cornered[0] && !cornered[1])
        {
            if (neighbours[0].currentPiece.myCode == currentPiece.myCode)
            { 
                if(!neighbours[1].cornered[2])
                {
                    if (neighbours[1].neighbours[2].currentPiece.myCode == currentPiece.myCode)
                        hasSolution = true;
                }
                if (!neighbours[1].cornered[3])
                {
                    if (neighbours[1].neighbours[3].currentPiece.myCode == currentPiece.myCode)
                        hasSolution = true;
                }
            }
            if (neighbours[1].currentPiece.myCode == currentPiece.myCode)
            {
                if (!neighbours[0].cornered[2])
                {
                    if (neighbours[0].neighbours[2].currentPiece.myCode == currentPiece.myCode)
                        hasSolution = true;
                }
                if (!neighbours[0].cornered[3])
                {
                    if (neighbours[0].neighbours[3].currentPiece.myCode == currentPiece.myCode)
                        hasSolution = true;
                }
            }
        }

        if (!cornered[2] && !cornered[3])
        {
            if (neighbours[2].currentPiece.myCode == currentPiece.myCode)
            {
                if (!neighbours[3].cornered[0])
                {
                    if (neighbours[3].neighbours[0].currentPiece.myCode == currentPiece.myCode)
                        hasSolution = true;
                }
                if (!neighbours[3].cornered[1])
                {
                    if (neighbours[3].neighbours[1].currentPiece.myCode == currentPiece.myCode)
                        hasSolution = true;
                }
            }
            if (neighbours[3].currentPiece.myCode == currentPiece.myCode)
            {
                if (!neighbours[2].cornered[0])
                {
                    if (neighbours[2].neighbours[0].currentPiece.myCode == currentPiece.myCode)
                        hasSolution = true;
                }
                if (!neighbours[2].cornered[1])
                {
                    if (neighbours[2].neighbours[1].currentPiece.myCode == currentPiece.myCode)
                        hasSolution = true;
                }
            }
        }

        if (hasSolution)
            gridManager.softLocked = false;

    }

    public void RippleMatch()
    {
        matched = true;

        if (!gridManager.matchedSpaces.Contains(GetComponent<GridSpace>()))
            gridManager.matchedSpaces.Add(GetComponent<GridSpace>());
    }

    public void Reroll()
    {
        if (centralMatch)
        {
            currentPiece.SetPieceProperties(Random.Range(0, gridManager.pieceVariety));
        }
    }

    public bool IsNeighbour(GridSpace compare)
    {
        bool n = false;

        for (int i = 0; i < neighbours.Length; i++)
        {
            if (neighbours[i] != null)
            {
                if (neighbours[i].gameObject == compare.gameObject)
                    n = true;
            }
        }

        return n;
    }

 
    #endregion

    #region Cascading

    public IEnumerator CascadePiece()
    {
        if (!cornered[0])
        {
            if (neighbours[0].currentPiece != null)
            {
                neighbours[0].currentPiece.SwapTo(thisGridSpace);
                neighbours[0].isEmpty = true;
                neighbours[0].currentPiece = null;
            }
            else
            {
                neighbours[0].isEmpty = true;
            }
        }
       
        yield return null;

    }



    #endregion

}
