using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.WSA;

public class Model
{
    public static Model Instance;
    public ApplicationIntegration applicationIntegration;
    
    private bool isPlayerTurn = true;
    
    public enum MapPiece
    {
        Empty,
        Ship
    }

    public struct GridSpace
    {
        public MapPiece pieceType;
        public bool hasBeenSelected;
    }

    public enum GameState
    {
        WaitingForAITurn,
        PlacingPieces,
        OnGoing,
        PlayerWin,
        ComputerWin,
        Tie,
    }

    public GameState currentState = GameState.OnGoing;
    public static Vector2Int mapSize = new Vector2Int(9,5);

    public int placedShipNumber = 0;
    public int totalShipNumber = 15;
    
    public GridSpace[,] computerMap = new GridSpace[mapSize.x,mapSize.y], playerMap = new GridSpace[mapSize.x,mapSize.y];
    
    public int capturedPlayerShips = 0, capturedComputerShip = 0;

    public void Init()
    {
        //init the ship map for PlayerWin
        _InitComputerBoard();
        _InitPlayerBoard();

        currentState = GameState.PlacingPieces;
        placedShipNumber = 0;
    }

    public void PlacingShipAtPosition(Vector2Int pos)
    {
        if(!_isInMap(pos)) return;
        
        if(playerMap[pos.x,pos.y].pieceType == MapPiece.Ship) return;

        playerMap[pos.x, pos.y].pieceType = MapPiece.Ship;
        placedShipNumber++;

        View.Instance.DoSelectViewUpdate();

        if (placedShipNumber != totalShipNumber) return;
        
        currentState = GameState.OnGoing;
        View.Instance.GameStartViewUpdate();
    }
    public void DoMapCheck(Vector2Int pos)
    {
        pos.y -= Model.mapSize.y;
        if(!_isInMap(pos) || playerMap[pos.x, pos.y].hasBeenSelected) return;
        
        playerMap[pos.x, pos.y].hasBeenSelected = true;
        if (computerMap[pos.x, pos.y].pieceType == MapPiece.Ship) 
            capturedComputerShip++;
        
        isPlayerTurn = !isPlayerTurn;
        if (!isPlayerTurn)
        {
            _AIMove();
            isPlayerTurn = !isPlayerTurn;
        }

        if (_IsGameEnd())
        {
            //applicationIntegration end
            currentState = capturedComputerShip >= totalShipNumber ? GameState.PlayerWin : GameState.ComputerWin;
            if (capturedComputerShip == capturedPlayerShips) currentState = GameState.Tie;
        } 
        
        View.Instance.DoViewUpdate();
    }

    private bool _IsGameEnd()
    {
        if (capturedComputerShip >= totalShipNumber || capturedPlayerShips >= totalShipNumber)
        {
            return true;
        }
        return false;
    }

    private void _AIMove()
    {
        var pos = _ramdomUnCheckedPos(computerMap);
        computerMap[pos.x, pos.y].hasBeenSelected = true;
        if (playerMap[pos.x, pos.y].pieceType == MapPiece.Ship) capturedPlayerShips++;
    }

    private void _InitComputerBoard()
    {
        List<Vector2Int> posList = new List<Vector2Int>();
        for (int x = 0; x < Model.mapSize.x; x++)
        {
            for (int y = 0; y < Model.mapSize.y; y++)
            {
                posList.Add(new Vector2Int(x,y));
            }
        }
        
        posList = _Shuffle(posList);
        
        for (int i = 0; i < totalShipNumber; i++)
        {
            computerMap[posList[i].x, posList[i].y].pieceType = MapPiece.Ship;
        }
    }

    private void _InitPlayerBoard()
    {
        List<Vector2Int> posList = new List<Vector2Int>();
        for (int x = 0; x < Model.mapSize.x; x++)
        {
            for (int y = 0; y < Model.mapSize.y; y++)
            {
                playerMap[x,y].pieceType = MapPiece.Empty;
                playerMap[x, y].hasBeenSelected = false;
            }
        }
    }

    private bool _isInMap(Vector2Int pos)
    {
        return _isInMap(pos.x, pos.y);
    }
    
    private bool _isInMap(int x, int y)
    {
        if (x < 0 || x > mapSize.x-1) return false;
        if (y < 0 || y > mapSize.y-1) return false;
        return true;
    }

    private Vector2Int _ramdomUnCheckedPos(GridSpace[,] checkMap)
    {
        var x = -1;
        var y = -1;

        while (!(_isInMap(x, y) && !checkMap[x, y].hasBeenSelected))
        {
            var _x = Random.Range(0f, 1f);
            x = (int) Mathf.Floor(_x * mapSize.x);
            var _y = Random.Range(0f, 1f);
            y = (int) Mathf.Floor(_y * mapSize.y);
        }

        return new Vector2Int(x,y);
    }

    private List<T> _Shuffle<T>(List<T> list)
    {
        Random rand = new Random();
        int iTarget = 0;
        T iTemp;
        for (int i = 0; i < list.Count; i++)
        {
            iTarget = Random.Range(0, list.Count);
            iTemp = list[i];
            list[i] = list[iTarget];
            list[iTarget] = iTemp;
        }
        return list;
    }
}
