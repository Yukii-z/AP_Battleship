using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller
{
    public static Controller Instance;
    public Game game;
    
    public void Update()
    {
        if (Input.GetMouseButtonDown(0) && Model.Instance.winingSituation == Model.win.onGoing)
        {
            var currentMouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var attachedMousePos =
                new Vector2Int((int)Mathf.Round(currentMouseWorldPos.x), (int)Mathf.Round(currentMouseWorldPos.y));
            Model.Instance.DoMapCheck(attachedMousePos);
        }
        
        if (Input.GetKeyDown(KeyCode.R))
        {
            game.Restart();
        }
    }
}
