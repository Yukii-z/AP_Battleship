using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.WSA;
using Random = UnityEngine.Random;

public class Model
{
    public static Model Instance;
    public ApplicationIntegration applicationIntegration;
    
    private bool isPlayerTurn = true;
    
    public enum MapPiece
    {
        Empty,
        Ship,
        PossibleShip,
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

    public class Ship
    {
        public int x;
        public int y;
        public bool isBend;
        public bool isBeenSetOnBoard;
        public Vector2Int refPos;
        public Ship(int x, int y)
        {
            this.x = x;
            this.y = y;
            refPos = new Vector2Int();
        }

        public Vector2Int size
        {
            get
            {
                return isBend
                    ? new Vector2Int(Mathf.Max(x, y), Mathf.Min(x, y))
                    : new Vector2Int(Mathf.Min(x, y), Mathf.Max(x, y));
            }
        }
        
    }
    public List<Ship> playerDock = new List<Ship>{new Ship(1,2), new Ship(1,2), new Ship(1,3), new Ship(2,3)};
    public List<Ship> computerDock = new List<Ship>{new Ship(1,2), new Ship(1,2),new Ship(1,3), new Ship(2,3)};
    private Ship newShip;
    
    public GameState currentState = GameState.OnGoing;
    public static Vector2Int mapSize = new Vector2Int(9,5);

    public int placedShipNumber = 0;

    public int totalShipNumber
    {
        get
        {
            var i = 0;
            foreach (var ship in playerDock)
                i += ship.x * ship.y;
            return i;
        }
    }
    
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

        while (!_isDockSet(computerDock))
        {
            var ship = _RandomSelectShip(computerDock, computerMap);
            _PutShipDown(ship,computerMap);
        }
    }

    private void _InitPlayerBoard()
    {
        for (int x = 0; x < Model.mapSize.x; x++)
        {
            for (int y = 0; y < Model.mapSize.y; y++)
            {
                playerMap[x,y].pieceType = MapPiece.Empty;
                playerMap[x, y].hasBeenSelected = false;
            }
        }

        newShip = _PickUpRandomShipForPlayer();
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

    private bool _ShuffleBool()
    {
        return Random.Range(0, 100) > 50 ? true : false;
    }

    private bool _isGridAvaliable(Vector2Int pos,GridSpace[,] map)
    {
        if ((pos.x < 0 || pos.x > map.GetLength(0) - 1) || (pos.y < 0 || pos.y > map.GetLength(1) - 1)) return false;
        return map[pos.x, pos.y].pieceType == MapPiece.Empty;
    }
    
    private bool _isGridAvaliable(int posx, int posy,GridSpace[,] map)
    {
        return _isGridAvaliable(new Vector2Int(posx, posy), map);
    }

    private bool _isShipSetable(Ship ship, Vector2Int refPos,GridSpace[,] map)
    {
        for (int x = refPos.x; x < refPos.x + ship.size.x; x++)
        {
            for (int y = refPos.y; y < refPos.y + ship.size.y; y++)
                if (!_isGridAvaliable(x, y, map))
                    return false;
        }
        return true;
    }

    public void SetPlayerShip()
    {
        if(newShip == null) return;
        
        _PutShipDown(newShip,playerMap);
        newShip = null;
        
        if (!_isDockSet(playerDock))
        {
            newShip = _PickUpRandomShipForPlayer();
            View.Instance.DoSelectViewUpdate();
            return;
        }
        
        currentState = GameState.OnGoing;
        View.Instance.GameStartViewUpdate();
    }

    public void ChangeShip()
    {
        if (_isDockSet(playerDock)) return;
        
        _SetShipToCertainState(newShip, playerMap,MapPiece.Empty);
        newShip = _PickUpRandomShipForPlayer();
        View.Instance.DoSelectViewUpdate();
    }

    private void _PutShipDown(Ship ship, GridSpace[,] map)
    {
        _SetShipToCertainState(ship, map, MapPiece.Ship);
        ship.isBeenSetOnBoard = true;

    }
    
    private void _SetShipToCertainState(Ship ship, GridSpace[,] map, MapPiece targetType)
    {
        for (int x = ship.refPos.x; x < ship.refPos.x + ship.size.x; x++)
        {
            for (int y = ship.refPos.y; y < ship.refPos.y + ship.size.y; y++)
                map[x, y].pieceType = targetType;
        }
    }


    private Ship _RandomSelectShip(List<Ship> dock, GridSpace[,] map)
    {
        while (_Shuffle(dock)[0].isBeenSetOnBoard)
        {
        }

        dock[0].refPos = new Vector2Int(-1, -1);
        while (!_isShipSetable(dock[0], dock[0].refPos, map))
        {
            dock[0].refPos = new Vector2Int(Random.Range(0, mapSize.x), Random.Range(0, mapSize.y));
            dock[0].isBend = _ShuffleBool();
        }
        return dock[0];
    }

    private bool _isDockSet(List<Ship> dock)
    {
        foreach (var ship in dock)
        {
            if (!ship.isBeenSetOnBoard)
                return false;
        }
        return true;
    }

    private Ship _PickUpRandomShipForPlayer()
    {
        var ship = _RandomSelectShip(playerDock, playerMap);
        _SetShipToCertainState(ship,playerMap,MapPiece.PossibleShip);
        return ship;
    }
}
