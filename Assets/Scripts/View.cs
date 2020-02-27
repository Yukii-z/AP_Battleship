using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public class View
{
    public ApplicationIntegration applicationIntegration;
    public static View Instance;
    private GameObject ExistShipCheckMark;
    private GameObject NonExistShipCheckMark;
    
    private GameObject[,] playerPieceObj = new GameObject[Model.mapSize.x,Model.mapSize.y], computerPieceObj = new GameObject[Model.mapSize.x,Model.mapSize.y];

    public void Init()
    {
        ExistShipCheckMark = Resources.Load<GameObject>("ExistShipCheckMark");
        NonExistShipCheckMark = Resources.Load<GameObject>("NonExistShipCheckMark");
        _InitLine();
        _InitBoard();

        applicationIntegration.computerTotal.text = Model.Instance.totalShipNumber.ToString();
        applicationIntegration.playerTotal.text = Model.Instance.totalShipNumber.ToString();
        applicationIntegration.totalSelect.text = Model.Instance.totalShipNumber.ToString();
        applicationIntegration.playerCatch.text = "00";
        applicationIntegration.computerCatch.text = "00";
        applicationIntegration.selected.text = "00";
        applicationIntegration.end.text = String.Empty;

        DoSelectViewUpdate();
    }

    public void GameStartViewUpdate()
    {
        DoSelectViewUpdate();
        for (int x = 0; x < Model.mapSize.x; x++)
        {
            for (int y = 0; y < Model.mapSize.y; y++)
            {
                //for PlayerWin board
                var obj = computerPieceObj[x, y];
                var color = obj.GetComponent<SpriteRenderer>().color;
                color.a = 0.1f;
                obj.GetComponent<SpriteRenderer>().color = color;

            }
        }
    }
    
    public void DoViewUpdate()
    {
        switch (Model.Instance.currentState)
        {
            case Model.GameState.PlacingPieces:
            case Model.GameState.OnGoing:
            case Model.GameState.WaitingForAITurn:
                _OngoingGameUpdate();
                break;
            case Model.GameState.Tie:
            case Model.GameState.ComputerWin:
            case Model.GameState.PlayerWin:
                _GameOverUpdate();
                break;
        }
        
    }

    private void _GameOverUpdate()
    {
        applicationIntegration.end.text = Model.Instance.currentState == Model.GameState.ComputerWin ? "You Lose!" : "You Win!";
        if (Model.Instance.currentState == Model.GameState.Tie) applicationIntegration.end.text = "It's a Tie!";

    }
    
    public void DoSelectViewUpdate()
    {
        for (int x = 0; x < Model.mapSize.x; x++)
        {
            for (int y = 0; y < Model.mapSize.y; y++)
            {
                //for PlayerWin board
                var obj = computerPieceObj[x, y];
                if(obj) GameObject.Destroy(obj);
                switch (Model.Instance.playerMap[x,y].pieceType)
                {
                    case Model.MapPiece.Ship:
                        computerPieceObj[x, y] = GameObject.Instantiate(ExistShipCheckMark);
                        break;
                    case Model.MapPiece.Empty:
                        computerPieceObj[x, y] = GameObject.Instantiate(NonExistShipCheckMark);
                        break;
                    case Model.MapPiece.PossibleShip:
                        computerPieceObj[x, y] = GameObject.Instantiate(ExistShipCheckMark);
                        var color = computerPieceObj[x, y].GetComponent<SpriteRenderer>().color;
                        color.a = 0.5f;
                        computerPieceObj[x, y].GetComponent<SpriteRenderer>().color = color;
                        break;
                }
                computerPieceObj[x, y].transform.position = new Vector3(x, y, 0);
            }
        }
        
        applicationIntegration.selected.text = Model.Instance.placedShipNumber.ToString();
    }

    private void _OngoingGameUpdate()
    {
        
        for (int x = 0; x < Model.mapSize.x; x++)
        {
            for (int y = 0; y < Model.mapSize.y; y++)
            {
                //for PlayerWin board
                var mapPiece = Model.Instance.computerMap[x, y].pieceType;
                var checkPiece = Model.Instance.computerMap[x, y].hasBeenSelected;
                if (checkPiece)
                {
                    var obj = playerPieceObj[x, y];
                    Object.Destroy(obj);
                    playerPieceObj[x,y] = Object.Instantiate(mapPiece == Model.MapPiece.Ship ? ExistShipCheckMark : NonExistShipCheckMark);
                    playerPieceObj[x,y].transform.position = new Vector3(x,y + Model.mapSize.y,0);
                    
                }
                
                //ComputerWin board
                mapPiece = Model.Instance.playerMap[x, y].pieceType;
                checkPiece = Model.Instance.playerMap[x, y].hasBeenSelected;
                if (checkPiece)
                {
                    var obj = computerPieceObj[x, y];
                    Object.Destroy(obj);
                    computerPieceObj[x, y] = Object.Instantiate(mapPiece == Model.MapPiece.Ship ? ExistShipCheckMark : NonExistShipCheckMark);
                    computerPieceObj[x, y].transform.position = new Vector3(x,y,0);
                    
                }
            }
        }

        applicationIntegration.playerCatch.text = Model.Instance.capturedComputerShip.ToString();
        applicationIntegration.computerCatch.text = Model.Instance.capturedPlayerShips.ToString();
    }

    private void _InitLine()
    {
        var obj = GameObject.Instantiate(new GameObject());
        var line =obj.AddComponent<LineRenderer>();
        line.positionCount = 2;
        line.SetPosition(0, new Vector3(-1f,(float)Model.mapSize.y-0.5f,0f));
        line.SetPosition(1, new Vector3((float)Model.mapSize.x,(float)Model.mapSize.y-0.5f,0f));
        line.startWidth = 0.2f;
    }

    private void _InitBoard()
    {
        for (int x = 0; x < Model.mapSize.x; x++)
        {
            for (int y = 0; y < Model.mapSize.y; y++)
            {
                //for PlayerWin board
                var obj = computerPieceObj[x, y];
                if(obj) GameObject.Destroy(obj);
                computerPieceObj[x, y] = GameObject.Instantiate(NonExistShipCheckMark);
                computerPieceObj[x, y].transform.position = new Vector3(x, y, 0);
                
                //for ComputerWin board
                obj = playerPieceObj[x, y];
                if (obj)
                {
                    GameObject.Destroy(obj);
                }
            }
        }
    }
}
