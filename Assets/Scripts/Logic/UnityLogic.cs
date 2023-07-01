using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
public class UnityLogic : MonoBehaviour
{
    [Header("Camera")]
    [SerializeField]
    private new Camera camera;
    [Space(10)]

    [Header("Components")]
    [SerializeField]
    private Tilemap tilemap;
    [SerializeField]
    private SpriteRenderer spriteRendererOfPickedUpFigure;
    [Space(10)]

    [Header("Scripts")]
    [SerializeField]
    private GameInteraction gameInteraction;
    [SerializeField]
    private GameOver gameOver;
    [SerializeField]
    private ChessLogic chessLogic;
    [Space(10)]

    [Header("Vectors")]
    [SerializeField]
    private Vector3 cameraRotation;
    [SerializeField]
    private Vector3Int coordinates;
    [Space(10)]

    [Header("int numbers")]
    [SerializeField]
    private int rotation;
    [SerializeField]
    private int moveCount;

    // Start is called before the first frame update
    void Start()
    {
        tilemap.CompressBounds();
        camera = Camera.main;
        cameraRotation = new Vector3(0, 0, 180);
        rotation = 180;
        moveCount = 0;
    }


    public int getMoveCount()
    {
        return moveCount;
    }

    public void setMoveCount(int x)
    {
        moveCount = x;
    }

    public void rotateBoard()       //dreht die Kamera und die Tiles auf dem Feld, nachdem ein Spieler gezogen hat
    {
        TileBase tile;
        camera.transform.rotation *= Quaternion.Euler(cameraRotation);                     //dreht die Kamera um 180?
        for (int x = 0; x <= 7; x++)
        {
            for (int y = 0; y <= 7; y++)
            {                                                                               //doppelte for Schleife durchl?uft alle Tiles auf der Tilemap und rotiert sie mit rotateTile()
                coordinates =  new Vector3Int(x, y);
                tile = tilemap.GetTile(coordinates);
                rotateTile(tile);
            }
        }
        spriteRendererOfPickedUpFigure.flipY = !spriteRendererOfPickedUpFigure.flipY;
        spriteRendererOfPickedUpFigure.flipX = !spriteRendererOfPickedUpFigure.flipX;
        rotation += 180;
        gameInteraction.setLastMovePosition(gameInteraction.getDroppedOnTilePosition());
        gameInteraction.setLastMovedTile(gameInteraction.getPickedUpTile());
        gameInteraction.setLastPickedUpTilePosition(gameInteraction.getPickedUpTilePosition());
    }

    public void rotateTile(TileBase tile)       //rotiert ein Tile um 180?
    {
        //Debug.Log(tile.name);
        Matrix4x4 transformMatrix = Matrix4x4.Translate(new Vector3(0, 0, 0)) * Matrix4x4.Rotate(Quaternion.Euler(0, 0, rotation));               //Die Position der Tiles kann nur ueber eine 4x4 Matrix veraendert werden
        TileChangeData changeData = new TileChangeData                                                                                            //es werden fuer jedes Tile in der Tilemap neue Daten generiert
        {
            tile = tile,
            transform = transformMatrix,
            position = coordinates
        };
        tilemap.SetTile(changeData, false);                                                                                                 //Tiles werden mit den neuen Daten plaziert
         
    }

    public bool whites_turn()
    {
        if (getMoveCount() % 2 == 0)
        {
            return true;
        }
        else if (getMoveCount() % 2 == 1)
        {
            return false;
        }
        Debug.LogWarning("Bug: Spiel weiß nicht mehr wer dran ist"); //Bug Meldung
        return false;
    }

    public void declareWinner()
    {
        gameOver.setup(whites_turn());
    }

    public void terminateGame()
    {
        declareWinner();
    }

    public void prepareNextMove()
    {
        rotateBoard();
        moveCount++;
        chessLogic.setWhitesTurn(!chessLogic.getWhitesTurn());
    }
}