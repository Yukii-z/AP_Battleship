using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.WSA;

public class Model
{
    public static Model Instance;
    private bool isPlayerTurn = true;
    public enum mapPiece
    {
        Empty,
        Ship
    }

    public enum win
    {
        onGoing,
        player,
        computer,
        tie,
    }

    public win winingSituation = win.onGoing;
    public static Vector2Int mapSize = new Vector2Int(9,5);
    
    public int shipNumber = 15;
    
    public mapPiece[,] computerMap = new mapPiece[mapSize.x,mapSize.y], playerMap = new mapPiece[mapSize.x,mapSize.y];
    public bool[,] playerPieceCheckMap = new bool[mapSize.x,mapSize.y], computePieceCheckMap = new bool[mapSize.x,mapSize.y];
    public int catchedPlayerShip = 0, catchedComputerShip = 0;

    public void Init()
    {
        _InitCheckMap(playerPieceCheckMap);
        _InitCheckMap(computePieceCheckMap);
        //init the ship map for player
        _InitPlayerBoard();
        _InitComputerBoard();

        winingSituation = win.onGoing;
    }
    
    public void DoMapCheck(Vector2Int pos)
    {
        pos.y -= Model.mapSize.y;
        if(!_isInMap(pos) || playerPieceCheckMap[pos.x, pos.y]) return;
        
        playerPieceCheckMap[pos.x, pos.y] = true;
        if (computerMap[pos.x, pos.y] == mapPiece.Ship) catchedComputerShip++;
        
        isPlayerTurn = !isPlayerTurn;
        if (!isPlayerTurn)
        {
            _AIMove();
            isPlayerTurn = !isPlayerTurn;
        }

        if (_IsGameEnd())
        {
            //game end
            winingSituation = catchedComputerShip >= shipNumber ? win.player : win.computer;
            if (catchedComputerShip == catchedPlayerShip) winingSituation = win.tie;
        } 
        
        View.Instance.DoViewUpdate();
    }

    private bool _IsGameEnd()
    {
        if (catchedComputerShip >= shipNumber || catchedPlayerShip >= shipNumber)
        {
            return true;
        }
        return false;
    }

    private void _AIMove()
    {
        var pos = _ramdomUnCheckedPos(computePieceCheckMap);
        computePieceCheckMap[pos.x, pos.y] = true;
        if (playerMap[pos.x, pos.y] == mapPiece.Ship) catchedPlayerShip++;
    }

    private void _InitPlayerBoard()
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
        
        for (int i = 0; i < shipNumber; i++)
        {
            computerMap[posList[i].x, posList[i].y] = mapPiece.Ship;
        }
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
        
        for (int i = 0; i < shipNumber; i++)
        {
            playerMap[posList[i].x, posList[i].y] = mapPiece.Ship;
        }
    }

    private void _InitCheckMap(bool[,] map)
    {
        for (int x = 0; x < Model.mapSize.x; x++)
        {
            for (int y = 0; y < Model.mapSize.y; y++)
            {
                map[x, y] = false;
            }
        }
    }

    private bool _isInMap(Vector2Int pos)
    {
        if (pos.x < 0 || pos.x > mapSize.x-1) return false;
        if (pos.y < 0 || pos.y > mapSize.y-1) return false;
        return true;
    }
    
    private bool _isInMap(int x, int y)
    {
        if (x < 0 || x > mapSize.x-1) return false;
        if (y < 0 || y > mapSize.y-1) return false;
        return true;
    }

    private Vector2Int _ramdomUnCheckedPos(bool[,] checkMap)
    {
        var _x = Random.Range(0f, 1f);
        var x = (int) Mathf.Floor(_x * mapSize.x);
        var _y = Random.Range(0f, 1f);
        var y = (int) Mathf.Floor(_y * mapSize.y);

        if (!_isInMap(x, y) || checkMap[x, y])
        {
            var pos = _ramdomUnCheckedPos(checkMap);
            x = pos.x;
            y = pos.y;
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
