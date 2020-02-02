using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class View
{
    public Game game;
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

        game.computerTotal.text = Model.Instance.shipNumber.ToString();
        game.playerTotal.text = Model.Instance.shipNumber.ToString();
        game.totalSelect.text = Model.Instance.shipNumber.ToString();
        game.playerCatch.text = "00";
        game.computerCatch.text = "00";
        game.selected.text = "00";
        game.end.text = String.Empty;
    }

    public void GameStartViewUpdate()
    {
        for (int x = 0; x < Model.mapSize.x; x++)
        {
            for (int y = 0; y < Model.mapSize.y; y++)
            {
                //for player board
                var obj = computerPieceObj[x, y];
                var color = obj.GetComponent<SpriteRenderer>().color;
                color.a = 0.1f;
                obj.GetComponent<SpriteRenderer>().color = color;

            }
        }
    }
    public void DoViewUpdate()
    {
        for (int x = 0; x < Model.mapSize.x; x++)
        {
            for (int y = 0; y < Model.mapSize.y; y++)
            {
                //for player board
                var mapPiece = Model.Instance.computerMap[x, y];
                var checkPiece = Model.Instance.playerPieceCheckMap[x, y];
                if (checkPiece)
                {
                    var obj = playerPieceObj[x, y];
                    GameObject.Destroy(obj);
                    playerPieceObj[x,y] = GameObject.Instantiate(mapPiece == Model.mapPiece.Ship ? ExistShipCheckMark : NonExistShipCheckMark);
                    playerPieceObj[x,y].transform.position = new Vector3(x,y + Model.mapSize.y,0);
                    
                }
                
                //computer board
                mapPiece = Model.Instance.playerMap[x, y];
                checkPiece = Model.Instance.computePieceCheckMap[x, y];
                if (checkPiece)
                {
                    var obj = computerPieceObj[x, y];
                    GameObject.Destroy(obj);
                    computerPieceObj[x, y] = GameObject.Instantiate(mapPiece == Model.mapPiece.Ship ? ExistShipCheckMark : NonExistShipCheckMark);
                    computerPieceObj[x, y].transform.position = new Vector3(x,y,0);
                    
                }
            }
        }

        game.playerCatch.text = Model.Instance.catchedComputerShip.ToString();
        game.computerCatch.text = Model.Instance.catchedPlayerShip.ToString();

        if (Model.Instance.winingSituation != Model.win.onGoing)
        {
            game.end.text = Model.Instance.winingSituation == Model.win.computer ? "You Lose!" : "You Win!";
            if (Model.Instance.winingSituation == Model.win.tie) game.end.text = "It's a tie!";
        }
    }

    public void DoSelectViewUpdate()
    {
        for (int x = 0; x < Model.mapSize.x; x++)
        {
            for (int y = 0; y < Model.mapSize.y; y++)
            {
                //for player board
                var obj = computerPieceObj[x, y];
                if(obj) GameObject.Destroy(obj);
                computerPieceObj[x, y] = GameObject.Instantiate(Model.Instance.playerMap[x,y]== Model.mapPiece.Ship ? ExistShipCheckMark : NonExistShipCheckMark);;
                computerPieceObj[x, y].transform.position = new Vector3(x, y, 0);
            }
        }
        
        game.selected.text = Model.Instance.selectedShipNum.ToString();
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
                //for player board
                var obj = computerPieceObj[x, y];
                if(obj) GameObject.Destroy(obj);
                computerPieceObj[x, y] = GameObject.Instantiate(NonExistShipCheckMark);
                computerPieceObj[x, y].transform.position = new Vector3(x, y, 0);
                
                //for computer board
                obj = playerPieceObj[x, y];
                if (obj)
                {
                    GameObject.Destroy(obj);
                }
            }
        }
    }
}
