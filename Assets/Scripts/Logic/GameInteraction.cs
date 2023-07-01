using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameInteraction : MonoBehaviour
{
    [Header("Board")]
    [SerializeField]
    private Grid grid;
    [SerializeField]
    private Tilemap tilemap;
    [SerializeField]
    private GameObject pickedUpFigure;                         //das Gameobject bewegt sich immer um die Maus (daran wird das Sprite angeheftet)
    [Space(10)]

    [Header("Tiles")]
    [SerializeField]
    private Tile pickedUpTile;                                 //speichert welches Tile angeklickt wurde zum aufheben
    [SerializeField]
    private Tile droppedOnTile;                                //speicher welches Tile angeklickt wurde zum ablegen
    [SerializeField]
    private Tile whiteTile;                                     //Standard wei?es Tile
    [SerializeField]
    private Tile blackTile;                                     //Standard schwarzes Tile
    [SerializeField]
    private Tile lastMovedTile;
    [Space(10)]

    private Vector3Int mousePositionInTileCoordinates;       //Position der Maus in Tile Koordinaten
    [Header("Coordinates")]
    [SerializeField]
    private Vector3Int pickedUpTilePosition;                  //Position des Tiles welches Urspruenglich aufgehoben wurde
    [SerializeField]
    private Vector3Int droppedOnTilePosition;                 //Position des Tiles wo es abgelegt wurde
    [SerializeField]
    private Vector3Int whiteKingPosition;
    [SerializeField]
    private Vector3Int blackKingPosition;

    private Vector3Int kingPosition;
    [SerializeField]
    private Vector3Int lastMovePosition;
    [SerializeField]
    private Vector3Int lastPickedUpTilePosition;
    [Space(10)]

    [Header("Scripts")]
    [SerializeField]
    private UnityLogic unityLogic;
    [SerializeField]
    private ChessLogic chessLogic;
    [SerializeField]
    private GameOver gameOver;

    [Space(10)]

    [Header("other")]
    [SerializeField]
    private SpriteRenderer spriteRenderer;
    private bool mouseOnceClicked;                               //speichert ob die Maus geklickt wurde und verfolgt somit ob ein Zug gemacht wurde
    private Dictionary<string, Sprite> spriteDictionary;        //dictionary welches alle Sprites von den Figuren enthaellt
    private Dictionary<string, Tile> tileDictionary;
    private string[] spriteNameList;
    private string[] tileNameList;
    private bool whiteKingOnceMoved;
    private bool blackKingOnceMoved;
    private bool whiteRookOnBlackBlockOnceMoved;
    private bool whiteRookOnWhiteBlockOnceMoved;
    private bool blackRookOnBlackBlockOnceMoved;
    private bool blackRookOnWhiteblockOnceMoved;
    private bool test;

    // Start is called before the first frame update
    void Start()
    {
        chessLogic.setWhitesTurn(true);
        spriteNameList = new string[12];
        tileNameList = new string[28];
        spriteDictionary = new Dictionary<string, Sprite>();
        tileDictionary = new Dictionary<string, Tile>();
        var list = Resources.LoadAll<Sprite>("Sprites/Pieces");
        int i = 0;
        foreach (Sprite sprite in list)                                     //dictionary wird mit den Sprites gef?llt
        {
            spriteDictionary.Add(sprite.name, sprite);
            spriteNameList[i] = sprite.name;
            i++;
        }
        i = 0;
        var tileList = Resources.LoadAll("Tiles/Tile parts");
        foreach (var tile in tileList)
        {
            tileDictionary.Add(tile.name, (Tile)tile);
            tileNameList[i] = tileDictionary[tile.name].name;
            i++;
        }

        spriteRenderer = pickedUpFigure.GetComponent<SpriteRenderer>();
        mouseOnceClicked = false; //überprüft und richtig
        whiteKingOnceMoved = false;
        blackKingOnceMoved = false;
        whiteRookOnBlackBlockOnceMoved = false;
        whiteRookOnWhiteBlockOnceMoved = false;
        blackRookOnBlackBlockOnceMoved = false;
        blackRookOnWhiteblockOnceMoved = false;
        whiteKingPosition = new Vector3Int(4, 0);
        blackKingPosition = new Vector3Int(4, 7);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (!mouseOnceClicked)
            {
                mousePositionInTileCoordinates = get_mouse_pos_on_Tiles();

                // Ist das angeklickte Feld nicht leer UND ist weiß am Zug UND versucht weiß aufzuheben ODER ist schwarz am zug UND versucht schwarz aufzuheben, DANN hebe Figur auf
                if (tilemap.GetTile(mousePositionInTileCoordinates) != null && tilemap.GetTile(mousePositionInTileCoordinates).name != "black_block" && tilemap.GetTile(mousePositionInTileCoordinates).name != "white_block" && ((tilemap.GetTile(mousePositionInTileCoordinates).name.StartsWith("white") && chessLogic.getWhitesTurn()) || (tilemap.GetTile(mousePositionInTileCoordinates).name.StartsWith("black") && !chessLogic.getWhitesTurn())))
                {
                    pickUpTile();
                }
            }
            else if (mouseOnceClicked)
            {
                mousePositionInTileCoordinates = get_mouse_pos_on_Tiles();

                //Ist das angeglickte Feld nicht leer
                if (tilemap.GetTile(mousePositionInTileCoordinates) != null)
                {
                    dropTile();
                    if (chessLogic.getFigureMoved())
                    {
                        chessLogic.checkIfGameOver();
                        unityLogic.prepareNextMove();
                    }
                }
            }
        }
    }


    //Kompakte getter und setter
    public bool Test { get; set; }

    //getter Methoden
    public Vector3Int get_mouse_pos_on_Tiles()
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return grid.WorldToCell(mouseWorldPos);
    }

    public Tile getDroppedOnTile()
    {
        return droppedOnTile;
    }

    public Tile getPickedUpTile()
    {
        return pickedUpTile;
    }

    public Vector3Int getDroppedOnTilePosition()
    {
        return droppedOnTilePosition;
    }

    public Vector3Int getPickedUpTilePosition()
    {
        return pickedUpTilePosition;
    }

    public Vector3Int getWhiteKingPosition()
    {
        return whiteKingPosition;
    }

    public Vector3Int getBlackKingPosition()
    {
        return blackKingPosition;
    }

    public bool getWhiteKingOnceMoved()
    {
        return whiteKingOnceMoved;
    }

    public bool getBlackKingOnceMoved()
    {
        return blackKingOnceMoved;
    }

    public bool getWhiteRookOnBlackBlockOnceMoved()
    {
        return whiteRookOnBlackBlockOnceMoved;
    }

    public bool getWhiteRookOnWhiteBlockOnceMoved()
    {
        return whiteRookOnWhiteBlockOnceMoved;
    }

    public bool getBlackRookOnBlackBlockOnceMoved()
    {
        return blackRookOnBlackBlockOnceMoved;
    }

    public bool getBlackRookOnWhiteBlockOnceMoved()
    {
        return blackRookOnWhiteblockOnceMoved;
    }

    public SpriteRenderer getSpriteRenderer()
    {
        return spriteRenderer;
    }

    public Vector3Int getLastMovePosition()
    {
        return lastMovePosition;
    }

    public Tile getLastMovedTile()
    {
        return lastMovedTile;
    }

    public Vector3Int getLastPickedUpTilePosition()
    {
        return lastPickedUpTilePosition;
    }
    
    //setter Methoden
    public void setWhiteKingOnceMoved(bool b)
    {
        whiteKingOnceMoved = b;
    }

    public void setBlackKingOnceMoved(bool b)
    {
        blackKingOnceMoved = b;
    }

    public void setWhiteRookOnBlackBlockOnceMoved(bool b)
    {
        whiteRookOnBlackBlockOnceMoved = b;
    }

    public void setWhiteRookOnWhiteBlockOnceMoved(bool b)
    {
        whiteRookOnWhiteBlockOnceMoved = b;
    }

    public void setBlackRookOnBlackOnceMoved(bool b)
    {
        blackRookOnBlackBlockOnceMoved = b;
    }

    public void setBlackRookOnWhiteBlockOnceMoved(bool b)
    {
        blackRookOnWhiteblockOnceMoved = b;
    }

    public void setWhiteKingPosition(Vector3Int pos)
    {
        whiteKingPosition = pos;
    }

    public void setBlackKingPosition(Vector3Int pos)
    {
        blackKingPosition = pos;
    }

    public void setLastMovePosition(Vector3Int pos)
    {
        lastMovePosition = pos;
    }

    public void setLastMovedTile(Tile tile)
    {
        lastMovedTile = tile;
    }

    public void setLastPickedUpTilePosition(Vector3Int pos)
    {
        lastPickedUpTilePosition = pos;
    }



    public void pickUpTile()
    {
        clipTileToMouse();
        pickedUpTile = (Tile)tilemap.GetTile(mousePositionInTileCoordinates);       // angeklickte Figur wird gespeichert

        if (pickedUpTile.name.Contains("white_block"))        // Je nach Block Farbe wird ein unterschiedlicher Block zum ersetzen gew?hlt
        {
            tilemap.SetTile(mousePositionInTileCoordinates, whiteTile);
            mouseOnceClicked = true;
        }
        else if (pickedUpTile.name.Contains("black_block"))
        {
            tilemap.SetTile(mousePositionInTileCoordinates, blackTile);
            mouseOnceClicked = true;
        }
        pickedUpTilePosition = mousePositionInTileCoordinates; //angeglickte Position wird gespeichert
    }

    private string switchWordOrder(string name)
    {
        string[] list = name.Split("_");
        return list[1] + "_" + list[0];
    }

    public void clipTileToMouse()            //es wird das richtige Sprite aus dem dictionary geholt und an die Maus geh?ngt
    {
        string name = switchWordOrder(tilemap.GetTile(mousePositionInTileCoordinates).name);

        for (int i = 0; i <= spriteNameList.Length; i++)      //Durchläuft alle Namen und überprüft, ob ein Name auf dem Feld enthalten ist
        {
            if (name.Contains(spriteNameList[i]))
            {
                Debug.Log(spriteNameList[i]);
                spriteRenderer.sprite = spriteDictionary[name];
                break;
            }
        }
    }

    public void dropTile()                     //es wird das sprite von der Maus gel?st und das Tile auf dem Schachbrett ge?ndert, falls der Zug valide war
    {       
        if (spriteRenderer.sprite != null)
        {
            droppedOnTile = (Tile)tilemap.GetTile(mousePositionInTileCoordinates);
            droppedOnTilePosition = mousePositionInTileCoordinates;
            string name = spriteRenderer.sprite.name;
            if (droppedOnTile.name.Contains("white_block") && chessLogic.isMoveLegal())
            {
                for (int i = 0; i <= spriteNameList.Length; i++)
                {
                    if (name.Contains(spriteNameList[i]))
                    {

                        chessLogic.setFigureMoved(true);
                        tilemap.SetTile(mousePositionInTileCoordinates, Resources.Load<Tile>("Tiles/Tile parts/" + switchWordOrder(spriteNameList[i]) + "_on_white_block"));
                        spriteRenderer.sprite = null;
                        mouseOnceClicked = false;
                        if (name.Contains("king_white"))
                        {
                            whiteKingPosition = droppedOnTilePosition;
                        }
                        else if (name.Contains("king_black"))
                        {
                            blackKingPosition = droppedOnTilePosition;
                        }
                        break;
                    }
                }
            }

            else if (droppedOnTile.name.Contains("black_block") && chessLogic.isMoveLegal())
            {

                for (int i = 0; i <= spriteNameList.Length; i++)
                {
                    if (name.Contains(spriteNameList[i]))
                    {
                        tilemap.SetTile(mousePositionInTileCoordinates, Resources.Load<Tile>("Tiles/Tile parts/" + switchWordOrder(spriteNameList[i]) + "_on_black_block"));
                        chessLogic.setFigureMoved(true);
                        spriteRenderer.sprite = null;
                        mouseOnceClicked = false;
                        if (name.Contains("king_white"))
                        {
                            whiteKingPosition = droppedOnTilePosition;
                        }
                        else if (name.Contains("king_black"))
                        {
                            blackKingPosition = droppedOnTilePosition;
                        }
                        break;
                    }
                }

            }

            else //setzt die Figur auf das zuvor aufgehobene Feld zurück, wenn der Zug nicht legal ist
            {
                chessLogic.setFigureMoved(false);
                tilemap.SetTile(pickedUpTilePosition, pickedUpTile);
                spriteRenderer.sprite = null;
                mouseOnceClicked = false;
            }
        }
    }
}