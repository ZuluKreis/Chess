using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ChessLogic : MonoBehaviour
{
    [Header("Components")]
    [SerializeField]
    private Tilemap tilemap;
    [Space(10)]

    [Header("Scripts")]
    [SerializeField]
    private GameInteraction gameInteraction;
    [SerializeField]
    private UnityLogic unityLogic;

    private int yCoordinateDifference;
    private int yCoordinateStart;
    private int yCoordinateEnd;
    private int xCoordinateDifference;
    private int xCoordianteStart;
    private int xCoordinateEnd;
    [SerializeField]
    private Vector3Int figureThatAttacksFieldPosition;
    private bool whitesTurn;
    private bool figureMoved;

    private void Update()
    {
        //Debug.Log(figureThatAttacksFieldPosition);
    }

    public Vector3Int getFigureThatAttacksFieldPosition()
    {
        return figureThatAttacksFieldPosition;
    }

    public void setFigureThatAttacksFieldPosition(Vector3Int pos)
    {
        figureThatAttacksFieldPosition = pos;
    }

    public bool getWhitesTurn()
    {
        return whitesTurn;
    }

    public void setWhitesTurn(bool turn)
    {
        whitesTurn = turn;
    }

    public bool getFigureMoved()
    {
        return figureMoved;
    }

    public void setFigureMoved(bool figure)
    {
        figureMoved = figure;
    }

    public bool isDroppedOnFieldEmpty()
    {
        if (gameInteraction.getDroppedOnTile().name == "black_block" || gameInteraction.getDroppedOnTile().name == "white_block")        //Wenn das angeklickte Feld zum ablegen leer ist dann return true
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool isFieldEmpty(Vector3Int pos)       //Zur Überprüfung ob ein Feld mit bestimmten Koordinaten leer ist
    {
        if (tilemap.GetTile(pos).name == "black_block" || tilemap.GetTile(pos).name == "white_block")
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool isDroppedOnEnemy()
    {
        if (!unityLogic.whites_turn() && !isDroppedOnFieldEmpty() && gameInteraction.getDroppedOnTile().name.StartsWith("white"))           //Wenn schwaz dran ist und das angeklickte Feld zum ablegen eine weiße Figur enthällt, return true
        {
            return true;
        }
        else if (unityLogic.whites_turn() && !isDroppedOnFieldEmpty() && gameInteraction.getDroppedOnTile().name.StartsWith("black"))      //Wenn weiß dran ist und das angeklickte Feld zum ablegen eine schwarze Figur enthällt, return true
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool didMove()
    {
        if (gameInteraction.getPickedUpTilePosition() == gameInteraction.getDroppedOnTilePosition())
        {
            figureMoved = false;
            return false;
        }
        else
        {
            figureMoved = true;
            return true;
        }
    }

    public bool isMoveLegal()
    {
        yCoordinateStart = gameInteraction.getPickedUpTilePosition().y;
        xCoordianteStart = gameInteraction.getPickedUpTilePosition().x;
        yCoordinateEnd = gameInteraction.getDroppedOnTilePosition().y;
        xCoordinateEnd = gameInteraction.getDroppedOnTilePosition().x;
        xCoordinateDifference = xCoordinateEnd - xCoordianteStart;
        yCoordinateDifference = yCoordinateEnd - yCoordinateStart;
        Debug.Log("das nächste ist von moveLegal, also egal");
        if (kingIsInCheck(unityLogic.getMoveCount()))
        {
            return false;
        }
        else
        {
            if (didMove())
            {
                if (gameInteraction.getSpriteRenderer().sprite.name.Contains("pawn"))
                {
                    return is_pawn_move_legal();
                }
                else if (gameInteraction.getSpriteRenderer().sprite.name.Contains("rook"))
                {
                    return isRookMoveLegal();
                }
                else if (gameInteraction.getSpriteRenderer().sprite.name.Contains("knight"))
                {
                    return is_knight_move_legal();
                }
                else if (gameInteraction.getSpriteRenderer().sprite.name.Contains("bishop"))
                {
                    return isBishopMoveLegal();
                }
                else if (gameInteraction.getSpriteRenderer().sprite.name.Contains("queen"))
                {
                    return isQueenMoveLegal();
                }
                else if (gameInteraction.getSpriteRenderer().sprite.name.Contains("king"))
                {
                    return is_king_move_legal();
                }
                Debug.LogWarning("Bug: Die aufgehobene Figur ist keiner Kategorie zuzuordnen, deshalb ist is_move_legal() == false"); // Bug Meldung
                return false;
            }
            else
            {
                return false;
            }
        }
    }

    private bool isRookMoveLegal()
    {
        //Wenn sich der rook nur auf einer Achsen-Richtung bewegt hat und das abgelegte Feld leer oder ein Gegner ist, dann überprüfe, ob alle Felder dazwischen leer sind
        if (((yCoordinateDifference != 0 && xCoordinateDifference == 0) || (yCoordinateDifference == 0 && xCoordinateDifference != 0)) && (isDroppedOnFieldEmpty() || isDroppedOnEnemy()))
        {
            if (xCoordinateDifference == 0 && yCoordinateEnd > yCoordinateStart)   //Überprüft ob sich in positve y-Richtung bewegt wurde
            {
                for (int y_coordinate = yCoordinateStart; y_coordinate < yCoordinateEnd; y_coordinate++)
                {
                    if (!isFieldEmpty(new Vector3Int(xCoordinateEnd, y_coordinate)))
                    {
                        return false;
                    }
                }
            }

            else if (xCoordinateDifference == 0 && yCoordinateEnd < yCoordinateStart) // Überprüft ob sich in negative y-Richtung bewegt wurde
            {
                for (int y_coordinate = yCoordinateStart; y_coordinate > yCoordinateEnd; y_coordinate--)    //durchläuft alle y-Koordinaten und prüft ob Figuren den Weg blockieren
                {
                    if (!isFieldEmpty(new Vector3Int(xCoordinateEnd, y_coordinate)))
                    {
                        return false;
                    }
                }
            }

            else if (yCoordinateDifference == 0 && xCoordinateEnd > xCoordianteStart)       // Überprüft ob sich in positve x-Richtung bewegt wurde
            {
                for (int x_coordinate = xCoordianteStart; x_coordinate < xCoordinateEnd; x_coordinate++)    //durchläuft alle x-Koordinaten und prüft ob eine Figur den Weg blockiert
                {
                    if (!isFieldEmpty(new Vector3Int(x_coordinate, yCoordinateEnd)))
                    {
                        return false;
                    }
                }
            }

            else if (yCoordinateDifference == 0 && xCoordinateEnd < xCoordianteStart)       // Überprüft ob sich in negative x-Richtung bewegt wurde
            {
                for (int x_coordinate = xCoordianteStart; x_coordinate > xCoordinateEnd; x_coordinate--)    //durchläuft alle x-Koordinaten und prüft ob eine Figur den Weg blockiert
                {
                    if (!isFieldEmpty(new Vector3Int(x_coordinate, yCoordinateEnd)))
                    {
                        return false;
                    }
                }
            }


            //Rochade und die dazugehörigen Überprüfungen
            if (gameInteraction.getPickedUpTile().name.Contains("white") && gameInteraction.getPickedUpTilePosition() == new Vector3Int(0, 0) && !gameInteraction.getWhiteRookOnBlackBlockOnceMoved())
            {
                gameInteraction.setWhiteRookOnBlackBlockOnceMoved(true);
            }

            else if (gameInteraction.getPickedUpTile().name.Contains("white") && gameInteraction.getPickedUpTilePosition() == new Vector3Int(7, 0) && !gameInteraction.getWhiteRookOnWhiteBlockOnceMoved())
            {
                gameInteraction.setWhiteRookOnWhiteBlockOnceMoved(true);
            }

            else if (gameInteraction.getPickedUpTile().name.Contains("black") && gameInteraction.getPickedUpTilePosition() == new Vector3Int(7, 7) && !gameInteraction.getBlackRookOnBlackBlockOnceMoved())
            {
                gameInteraction.setBlackRookOnBlackOnceMoved(true);
            }

            else if (gameInteraction.getPickedUpTile().name.Contains("black") && gameInteraction.getPickedUpTilePosition() == new Vector3Int(0, 7) && !gameInteraction.getBlackRookOnWhiteBlockOnceMoved())
            {
                gameInteraction.setBlackRookOnWhiteBlockOnceMoved(true);
            }
            return true;
        }
        else
        {
            return false;
        }
    }

    private bool is_knight_move_legal()
    {
        return ((Math.Abs(xCoordinateDifference) == 1 && Math.Abs(yCoordinateDifference) == 2) || (Math.Abs(xCoordinateDifference) == 2 && Math.Abs(yCoordinateDifference) == 1)) && (isDroppedOnFieldEmpty() || isDroppedOnEnemy());
    }

    private bool isBishopMoveLegal()
    {
        //Überprüft, ob der bishop diagonal gezogen wurde
        if(Math.Abs(xCoordinateDifference) == Math.Abs(yCoordinateDifference) && (isDroppedOnFieldEmpty() || isDroppedOnEnemy()))
        {
            if (xCoordinateEnd > xCoordianteStart && yCoordinateEnd > yCoordinateStart) //Überprüft die Diagonale rechts nach oben
            {
                for (int i = 0; i < xCoordinateDifference; i++)
                {
                    if (!isFieldEmpty(new Vector3Int(xCoordianteStart+i, yCoordinateStart+i)))
                    {
                        return false;
                    }
                }
            }
            else if (xCoordinateEnd > xCoordianteStart && yCoordinateEnd < yCoordinateStart) //Überprüft die Diagonale rechts nach unten
            {
                for (int i = 0; i < xCoordinateDifference; i++)
                {
                    if (!isFieldEmpty(new Vector3Int(xCoordianteStart + i, yCoordinateStart - i)))
                    {
                        return false;
                    }
                }
            }
            else if (xCoordinateEnd < xCoordianteStart && yCoordinateEnd > yCoordinateStart) //Überprüft die Diagonale links nach oben
            {
                for (int i = 0; i > xCoordinateDifference; i--)
                {
                    if (!isFieldEmpty(new Vector3Int(xCoordianteStart + i, yCoordinateStart - i)))
                    {
                        return false;
                    }
                }
            }
            else if (xCoordinateEnd < xCoordianteStart && yCoordinateEnd < yCoordinateStart) //Überprüft die Diagonale links nach unten
            {
                for (int i = 0; i > xCoordinateDifference; i--)
                {
                    if (!isFieldEmpty(new Vector3Int(xCoordianteStart + i, yCoordinateStart + i)))
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        return false;
    }

    private bool is_pawn_move_legal()
    {
        if (!unityLogic.whites_turn()) //berechnet im Folgenden die Differenz abhängig davon ob schwarz oder weiß dran ist -> in diesem Fall schwarz
        {
            yCoordinateDifference = yCoordinateStart - yCoordinateEnd;
        }
        else //berechnet im Folgenden die Differenz abhängig davon ob schwarz oder weiß dran ist -> in diesem Fall weiß
        {
            yCoordinateDifference = yCoordinateEnd - yCoordinateStart;
        }

        if (yCoordinateDifference == 1 && xCoordinateDifference == 0 && isDroppedOnFieldEmpty())
        {
            return true;
        }
        else if (yCoordinateDifference == 2 && xCoordinateDifference == 0 && (yCoordinateStart == 1 || yCoordinateStart == 6) && isDroppedOnFieldEmpty())
        {
            return true;
        }
        else if (yCoordinateDifference == 1 && Math.Abs(xCoordinateDifference) == 1 && isDroppedOnEnemy())
        {
            return true;
        }
        else if (yCoordinateDifference == 1 && Math.Abs(xCoordinateDifference) == 1 && gameInteraction.getLastMovedTile().name.Contains("pawn") && (gameInteraction.getPickedUpTilePosition() + new Vector3Int(1, 0) == gameInteraction.getLastMovePosition() || gameInteraction.getPickedUpTilePosition() + new Vector3Int(-1, 0) == gameInteraction.getLastMovePosition()) && (gameInteraction.getLastPickedUpTilePosition().y == 6 || gameInteraction.getLastPickedUpTilePosition().y == 1) && gameInteraction.getDroppedOnTilePosition().x == gameInteraction.getLastMovePosition().x)
        {
            if (gameInteraction.getLastMovedTile().name.Contains("white_block"))
            {
                tilemap.SetTile(gameInteraction.getLastMovePosition(), Resources.Load<Tile>("Tiles/Tile parts/white_block"));
            }
            if (gameInteraction.getLastMovedTile().name.Contains("black_block"))
            {
                tilemap.SetTile(gameInteraction.getLastMovePosition(), Resources.Load<Tile>("Tiles/Tile parts/black_block"));
            }
            return true;
        }
        else
        {
            return false;
        }
    }

    private bool isQueenMoveLegal()
    {
        return isBishopMoveLegal() || isRookMoveLegal();
    }

    private bool is_king_move_legal()
    {
        //Überprüft, ob der King sich wie eine Dame bewegt, aber auf eine Feld Abstand beschränkt
        if (isQueenMoveLegal() && (Math.Abs(xCoordinateDifference) == 0 || Math.Abs(xCoordinateDifference) == 1) && (Math.Abs(yCoordinateDifference) == 0 || Math.Abs(yCoordinateDifference) == 1))
        {
            changeKingProperties();
            return true;
        }

        //Überprüft, ob der King schon mal bewegt wurde und ob dadurch noch eine Rochade zulässig ist (jeweils für die kurze und lange Rochade)
        if (!gameInteraction.getWhiteKingOnceMoved() && !gameInteraction.getWhiteRookOnWhiteBlockOnceMoved() && unityLogic.getMoveCount() % 2 == 0 && gameInteraction.getDroppedOnTilePosition() == new Vector3Int(6, 0))
        {
            if (isFieldEmpty(new Vector3Int(5, 0)) && isFieldEmpty(new Vector3Int(6, 0)) && !isFieldAttacked(unityLogic.getMoveCount(), new Vector3Int(5, 0)) && !isFieldAttacked(unityLogic.getMoveCount(), new Vector3Int(6, 0)))
            {
                tilemap.SetTile(new Vector3Int(7, 0), Resources.Load<Tile>("Tiles/Tile parts/white_block"));
                tilemap.SetTile(new Vector3Int(5, 0), Resources.Load<Tile>("Tiles/Tile parts/white_rook_on_white_block"));
                changeKingProperties();
                return true;
            }
        }
        else if (!gameInteraction.getWhiteKingOnceMoved() && !gameInteraction.getWhiteRookOnBlackBlockOnceMoved() && unityLogic.getMoveCount() % 2 == 0 && gameInteraction.getDroppedOnTilePosition() == new Vector3Int(2, 0))
        {
            if (isFieldEmpty(new Vector3Int(1, 0)) && isFieldEmpty(new Vector3Int(2, 0)) && isFieldEmpty(new Vector3Int(3, 0)) && !isFieldAttacked(unityLogic.getMoveCount(), new Vector3Int(2, 0)) && !isFieldAttacked(unityLogic.getMoveCount(), new Vector3Int(3, 0)))
            {
                tilemap.SetTile(new Vector3Int(0, 0), Resources.Load<Tile>("Tiles/Tile parts/black_block"));
                tilemap.SetTile(new Vector3Int(3, 0), Resources.Load<Tile>("Tiles/Tile parts/white_rook_on_white_block"));
                changeKingProperties();
                return true;
            }
        }
        else if (!gameInteraction.getBlackKingOnceMoved() && !gameInteraction.getBlackRookOnBlackBlockOnceMoved() && unityLogic.getMoveCount() % 2 == 1 && gameInteraction.getDroppedOnTilePosition() == new Vector3Int(6, 7))
        {
            if (isFieldEmpty(new Vector3Int(5, 7)) && isFieldEmpty(new Vector3Int(6, 7)) && !isFieldAttacked(unityLogic.getMoveCount(), new Vector3Int(5, 7)) && !isFieldAttacked(unityLogic.getMoveCount(), new Vector3Int(6, 7)))
            {
                tilemap.SetTile(new Vector3Int(7, 7), Resources.Load<Tile>("Tiles/Tile parts/black_block"));
                tilemap.SetTile(new Vector3Int(5, 7), Resources.Load<Tile>("Tiles/Tile parts/black_rook_on_black_block"));
                changeKingProperties();
                return true;
            }
        }
        else if (!gameInteraction.getBlackKingOnceMoved() && !gameInteraction.getBlackRookOnWhiteBlockOnceMoved() && unityLogic.getMoveCount() % 2 == 1 && gameInteraction.getDroppedOnTilePosition() == new Vector3Int(2, 7))
        {
            if (isFieldEmpty(new Vector3Int(1, 7)) && isFieldEmpty(new Vector3Int(2, 7)) && isFieldEmpty(new Vector3Int(3, 7)) && !isFieldAttacked(unityLogic.getMoveCount(), new Vector3Int(2, 7)) && !isFieldAttacked(unityLogic.getMoveCount(), new Vector3Int(3, 7)))
            {
                tilemap.SetTile(new Vector3Int(0, 7), Resources.Load<Tile>("Tiles/Tile parts/white_block"));
                tilemap.SetTile(new Vector3Int(3, 7), Resources.Load<Tile>("Tiles/Tile parts/black_rook_on_black_block"));
                changeKingProperties();
                return true;
            }
        }
        return false;
    }

    private void changeKingProperties()
    {
        //Je nach Farbe des Königs wird die aktuelle Position aktualisiert und, dass er bewegt wurde
        if (gameInteraction.getPickedUpTile().name.Contains("white_king"))
        {
            if (!gameInteraction.getWhiteKingOnceMoved())
            {
                gameInteraction.setWhiteKingOnceMoved(true);
            }
            gameInteraction.setWhiteKingPosition(gameInteraction.getDroppedOnTilePosition());
        }
        else if (gameInteraction.getPickedUpTile().name.Contains("black_king"))
        {
            if (!gameInteraction.getBlackKingOnceMoved())
            {
                gameInteraction.setBlackKingOnceMoved(true);
            }
            gameInteraction.setBlackKingPosition(gameInteraction.getDroppedOnTilePosition());
        }
    }

    public bool kingIsInCheck(int movecount)
    {
        bool whitesTurn = movecount % 2 == 0;
        Vector3Int kingPosition = whitesTurn ? gameInteraction.getWhiteKingPosition() : gameInteraction.getBlackKingPosition();
        Debug.Log(kingPosition);
        Debug.Log("whitesTurn nach Pos: " + whitesTurn);
        if (pawnAttacksField(whitesTurn, kingPosition) || knightAttacksField(whitesTurn, kingPosition))
        {
            return true;
        }
        if (queenAttacksField(whitesTurn, kingPosition))
        {
            Debug.Log("Queen setzt Schach");
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool isFieldAttacked(int movecount, Vector3Int position)
    {
        bool whites_turn = movecount % 2 == 0;
        if (tilemap.GetTile(position) != null)
        {
            if (pawnAttacksField(whites_turn, position) || knightAttacksField(whites_turn, position) || queenAttacksField(whites_turn, position))
            {
                return true;
            }
        }
        return false;
    }

    public bool isKingCheckmated(int moveCount, Vector3Int kingPosition)
    {
        if (!kingIsInCheck(moveCount))
        {
            Debug.Log("Kein Schach -> Kein Schachmatt");
            return false;
        }
        else
        {
            bool whites_turn = moveCount % 2 == 0;
            Debug.Log("Schach -> vielleicht Schachmatt");
            Debug.Log(kingPosition);
            return !(canKingMove(whites_turn, kingPosition) || canCheckingFigureBeCaptured() || canFigurePreventCheck());
        }
    }

    private bool canKingMove(bool whites_turn, Vector3Int kingPosition)
    {
        
        List<Vector3Int> surroundingFields = new List<Vector3Int>
        {
            kingPosition + new Vector3Int(0, 1),    //0     N
            kingPosition + new Vector3Int(1, 1),    //1     NE
            kingPosition + new Vector3Int(1, 0),    //2     E
            kingPosition + new Vector3Int(1, -1),   //3     SE
            kingPosition + new Vector3Int(0, -1),   //4     S
            kingPosition + new Vector3Int(-1, -1),  //5     SW
            kingPosition + new Vector3Int(-1, 0),   //6     W
            kingPosition + new Vector3Int(-1, 1)    //7     NW
        };

        for (int i = 0; i < surroundingFields.Count; i++)      //Iteriere durch die Liste und entferne alle "ungültigen" Koordinaten, welche auf der tilemap leer sind
        {
            if (tilemap.GetTile(surroundingFields[i]) == null)
            {
                surroundingFields.RemoveAt(i);
                i--;
            }
        }

        foreach (Vector3Int pos in surroundingFields)           //Für alle übrigen Felder wird überprüft, ob der König sich dort hin bewegen kann
        {
            if (!isFieldAttacked(unityLogic.getMoveCount(), pos) && isFieldEmpty(pos))
            {
                Debug.Log("König kann sich aus dem Schach raus bewegen");
                return true;
            }
        }
        Debug.Log("König kann sich nicht mehr aus dem Schach raus bewegen");
        return false;
    }

    private bool canCheckingFigureBeCaptured()
    {
        
        Vector3Int checkingFigurePositon = figureThatAttacksFieldPosition;
        Debug.Log("Diese Figur gibt Schach: " + checkingFigurePositon);
        if (isFieldAttacked(unityLogic.getMoveCount() + 1, figureThatAttacksFieldPosition))
        {
            Tile checkingFigureTile = (Tile)tilemap.GetTile(checkingFigurePositon);
            tilemap.SetTile(checkingFigurePositon, Resources.Load<Tile>("Tiles/Tile parts/black_block"));
            Tile figureThatAttacksCheckingFigureTile = (Tile)tilemap.GetTile(figureThatAttacksFieldPosition);
            tilemap.SetTile(figureThatAttacksFieldPosition, Resources.Load<Tile>("Tiles/Tile parts/black_block"));
            if (!isFieldAttacked(unityLogic.getMoveCount(), unityLogic.whites_turn() ? gameInteraction.getWhiteKingPosition() : gameInteraction.getBlackKingPosition()))        //Darf nicht isKingInCheck() verwenden sonst wird checkmate aufgerufen und auch canFigureBeCaptured() -> rekursion
            {
                tilemap.SetTile(checkingFigurePositon, checkingFigureTile);
                tilemap.SetTile(figureThatAttacksFieldPosition, figureThatAttacksCheckingFigureTile);
                Debug.Log("Die Schachgebende Figur kann geschlagen werden");
                return true;
            }
            else
            {
                tilemap.SetTile(checkingFigurePositon, checkingFigureTile);
                tilemap.SetTile(figureThatAttacksFieldPosition, figureThatAttacksCheckingFigureTile);
                Debug.Log("Die Schachgebende Figur kann nicht geschlagen werden, da sie gefesselt ist");
                return false;
            }

        }
        else
        {
            Debug.Log("Die Schachgebende Figur kann nicht geschlagen werden, da sie außer Reichweite ist");
            return false;
        }
    }

    private bool canFigurePreventCheck()
    {
        return false;
    }

    private bool knightAttacksField(bool whites_turn, Vector3Int position)
    {
        string enemyKnight = whites_turn ? "black_knight" : "white_knight";

        List<Vector3Int> possibleKnightPositions = new List<Vector3Int>
        {
            position + new Vector3Int(1, 2),
            position + new Vector3Int(2, 1),
            position + new Vector3Int(2, -1),
            position + new Vector3Int(1, -2),
            position + new Vector3Int(-1, -2),
            position + new Vector3Int(-2, -1),
            position + new Vector3Int(-2, 1),
            position + new Vector3Int(-1, 2),
        };
        for (int i = 0; i < possibleKnightPositions.Count; i++)      //Iteriere durch die Liste und entferne alle "ungültigen" Koordinaten, welche auf der tilemap leer sind
        {
            if (tilemap.GetTile(possibleKnightPositions[i]) == null)
            {
                possibleKnightPositions.RemoveAt(i);
                i--;
            }
        }
        foreach (Vector3Int possibleKnightPosition in possibleKnightPositions)
        {
            if (tilemap.GetTile(possibleKnightPosition).name.Contains(enemyKnight))
            {
                figureThatAttacksFieldPosition = possibleKnightPosition;
                return true;
            }
        }
        return false;
    }

    private bool queenAttacksField(bool whites_turn, Vector3Int position)
    {
        return bishopAttacksField(whites_turn, position) || rookAttacksField(whites_turn, position);
    }

    private bool bishopAttacksField(bool whites_turn, Vector3Int position)
    {
        string enemyColor = whites_turn ? "black" : "white";

        for(int x = 1; position.x + x <= 7 && position.y + x <=7; x++)
        {
            if (tilemap.GetTile(position + new Vector3Int(x, x)).name.Contains(enemyColor + "_bishop") || tilemap.GetTile(position + new Vector3Int(x, x)).name.Contains(enemyColor + "_queen"))
            {
                figureThatAttacksFieldPosition = position + new Vector3Int(x, x);
                return true;
            }
            else if (tilemap.GetTile(position + new Vector3Int(x, x)).name != "black_block" && tilemap.GetTile(position + new Vector3Int(x, x)).name != "white_block")
            {
                break;
            }
        }

        for (int x = 1; position.x + x <= 7 && position.y - x >= 0; x++)
        {
            if (tilemap.GetTile(position + new Vector3Int(x, -x)).name.Contains(enemyColor + "_bishop") || tilemap.GetTile(position + new Vector3Int(x, -x)).name.Contains(enemyColor + "_queen"))
            {
                figureThatAttacksFieldPosition = position + new Vector3Int(x, -x);
                return true;
            }
            else if (tilemap.GetTile(position + new Vector3Int(x, -x)).name != "black_block" && tilemap.GetTile(position + new Vector3Int(x, -x)).name != "white_block")
            {
                break;
            }
        }

        for (int x = 1; position.x - x >= 0 && position.y + x <= 7; x++)
        {
            if (tilemap.GetTile(position + new Vector3Int(-x, x)).name.Contains(enemyColor + "_bishop") || tilemap.GetTile(position + new Vector3Int(-x, x)).name.Contains(enemyColor + "_queen"))
            {
                figureThatAttacksFieldPosition = position + new Vector3Int(-x, x);
                return true;
            }
            else if (tilemap.GetTile(position + new Vector3Int(-x, x)).name != "black_block" && tilemap.GetTile(position + new Vector3Int(-x, x)).name != "white_block")
            {
                break;
            }
        }

        for (int x = 1; position.x - x >= 0 && position.y - x >= 0; x++)
        {
            if (tilemap.GetTile(position + new Vector3Int(-x, -x)).name.Contains(enemyColor + "_bishop") || tilemap.GetTile(position + new Vector3Int(-x, -x)).name.Contains(enemyColor + "_queen"))
            {
                figureThatAttacksFieldPosition = position + new Vector3Int(-x, -x);
                return true;
            }
            else if (tilemap.GetTile(position + new Vector3Int(-x, -x)).name != "black_block" && tilemap.GetTile(position + new Vector3Int(-x, -x)).name != "white_block")
            {
                break;
            }
        }
        return false;
    }

    private bool rookAttacksField(bool whites_turn, Vector3Int position)
    {
        string enemyColor = whites_turn ? "black" : "white";

        for (int x = position.x + 1; x <= 7; x++)                        
        {
            if (tilemap.GetTile(new Vector3Int(x, position.y)).name.Contains(enemyColor + "_rook") || tilemap.GetTile(new Vector3Int(x, position.y)).name.Contains(enemyColor + "_queen"))
            {
                figureThatAttacksFieldPosition = new Vector3Int(x, position.y);
                return true;
            }
            else if (tilemap.GetTile(new Vector3Int(x, position.y)).name != "black_block" && tilemap.GetTile(new Vector3Int(x, position.y)).name != "white_block")
            {
                break;
            }
        }       //Überprüft Schach in positve x-Richtung       

        for (int x = position.x - 1; x >= 0; x--)
        {
            if (tilemap.GetTile(new Vector3Int(x, position.y)).name.Contains(enemyColor + "_rook") || tilemap.GetTile(new Vector3Int(x, position.y)).name.Contains(enemyColor + "_queen"))
            {
                figureThatAttacksFieldPosition = new Vector3Int(x, position.y);
                return true;
            }
            else if (tilemap.GetTile(new Vector3Int(x, position.y)).name != "black_block" && tilemap.GetTile(new Vector3Int(x, position.y)).name != "white_block")
            {
                break;
            }
        }       //Überprüft Schach in negative x-Richtung

        for (int y = position.y + 1; y <= 7; y++)
        {
            if (tilemap.GetTile(new Vector3Int(position.x, y)).name.Contains(enemyColor + "_rook") || tilemap.GetTile(new Vector3Int(position.x, y)).name.Contains(enemyColor + "_queen"))
            {
                figureThatAttacksFieldPosition = new Vector3Int(position.x, y);
                return true;
            }
            else if (tilemap.GetTile(new Vector3Int(position.x, y)).name != "black_block" && tilemap.GetTile(new Vector3Int(position.x, y)).name != "white_block")
            {
                break;
            }
        }       //Überprüft Schach in positive y-Richtung

        for (int y = position.y - 1; y >= 0; y--)
        {
            if (tilemap.GetTile(new Vector3Int(position.x, y)).name.Contains(enemyColor + "_rook") || tilemap.GetTile(new Vector3Int(position.x, y)).name.Contains(enemyColor + "_queen"))
            {
                figureThatAttacksFieldPosition = new Vector3Int(position.x, y);
                return true;
            }
            else if (tilemap.GetTile(new Vector3Int(position.x, y)).name != "black_block" && tilemap.GetTile(new Vector3Int(position.x, y)).name != "white_block")
            {
                break;
            }
        }       //Überprüft Schach in negative y-Richtung
        return false;
    }

    private bool pawnAttacksField(bool whites_turn, Vector3Int position)
    {
        string enemyPawn = whites_turn ? "black_pawn" : "white_pawn";
        Debug.LogWarning(position);
        Vector3Int possible_pawn_position = position + new Vector3Int(1, 1);
        if (tilemap.GetTile(possible_pawn_position) != null && tilemap.GetTile(possible_pawn_position).name.Contains(enemyPawn))
        {
            figureThatAttacksFieldPosition = possible_pawn_position;
            Debug.LogError(enemyPawn + " attackiert " + position + " von " + possible_pawn_position);
            return true;
        }
        else
        {
            possible_pawn_position = position + new Vector3Int(-1, 1);
            if (tilemap.GetTile(possible_pawn_position) != null && tilemap.GetTile(possible_pawn_position).name.Contains(enemyPawn))
            {
                figureThatAttacksFieldPosition = possible_pawn_position;
                Debug.LogWarning(enemyPawn);
                return true;
            }
        }
        return false;
    }

    public void checkIfGameOver()
    {
        /*
         * Tile wurde nun abgelegt
         * Es muss überprüft werden, ob Der Gegner durch den Zug Schachmatt ist.
         * Es muss überprüft werden, ob es ein Patt ist
         */
        if (isKingCheckmated(unityLogic.getMoveCount() + 1, unityLogic.getMoveCount() % 2 == 0 ? gameInteraction.getBlackKingPosition() : gameInteraction.getWhiteKingPosition()))
        {
            unityLogic.declareWinner();
        }
        // Patt...
    }
}