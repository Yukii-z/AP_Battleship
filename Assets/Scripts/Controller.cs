using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller
{
    public static Controller Instance;
    public Game game;
    
    public void SelectUpdate()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var attachedMousePos = _attachedMousePos(Input.mousePosition);
            Model.Instance.DoMapSelect(attachedMousePos);
        }
    }

    public void GameUpdate()
    {
        if (Input.GetMouseButtonDown(0) && Model.Instance.winingSituation == Model.win.onGoing)
        {
            var attachedMousePos = _attachedMousePos(Input.mousePosition);
            Model.Instance.DoMapCheck(attachedMousePos);
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            game.Restart();
        }
    }
    private Vector2Int _attachedMousePos(Vector2 oriPos)
    {
        var currentMouseWorldPos = Camera.main.ScreenToWorldPoint(oriPos);
        var attachedMousePos =
            new Vector2Int((int)Mathf.Round(currentMouseWorldPos.x), (int)Mathf.Round(currentMouseWorldPos.y));
        return attachedMousePos;
    }
}
