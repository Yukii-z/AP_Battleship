using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputSystem
{
    public static InputSystem Instance;
    public ApplicationIntegration applicationIntegration;

    public void Update()
    {
        switch (Model.Instance.currentState)
        {
            case Model.GameState.Tie:
            case Model.GameState.ComputerWin:
            case Model.GameState.PlayerWin:
                GameOverUpdate();
                break;
            case Model.GameState.PlacingPieces:
                SelectUpdate();
                break;
            case Model.GameState.OnGoing when Model.Instance.currentState != Model.GameState.WaitingForAITurn:
                GameUpdate();
                break;
            case Model.GameState.OnGoing when Model.Instance.currentState == Model.GameState.WaitingForAITurn:
                AIUpdate();
                break;
        }
    }
    
    public void SelectUpdate()
    {
        if (!Input.GetMouseButtonDown(0)) return;
        
        var attachedMousePos = _attachedMousePos(Input.mousePosition);
        Model.Instance.PlacingShipAtPosition(attachedMousePos);
    }

    public void GameOverUpdate()
    {
        
    }

    public void AIUpdate()
    {
        
    }

    public void GameUpdate()
    {
        if (Input.GetMouseButtonDown(0) && Model.Instance.currentState == Model.GameState.OnGoing)
        {
            var attachedMousePos = _attachedMousePos(Input.mousePosition);
            Model.Instance.DoMapCheck(attachedMousePos);
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            applicationIntegration.Restart();
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
