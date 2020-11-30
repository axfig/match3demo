using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridPieceProperties : MonoBehaviour
{

    [Header("Grid Piece Properties")]
    public int myCode;
    public Sprite[] pieceSprites;
    public Color[] pieceColors;
    public SpriteRenderer mySpriteRenderer;
    public GridSpace currentGridSpace;

    public Transform spriteTransform;

    
    public void SetPieceProperties(int code)
    {
        myCode = code;

        mySpriteRenderer.sprite = pieceSprites[code];
        mySpriteRenderer.color = pieceColors[code];

        gameObject.name = code.ToString();

        spriteTransform = mySpriteRenderer.transform;

    }

  

   


}
