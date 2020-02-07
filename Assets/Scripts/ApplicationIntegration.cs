using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ApplicationIntegration : MonoBehaviour
{
    /*
     * TO DO:
     * - Make grids that are "legal", w/ ships of different size
     * - Make a webpage that changes color when you click, and makes sounds when you press keys.
     * - Download WebStorm
     *
     * - Try making a simple node.js server to show your html file
     *
     * 
     * Next Time:
     * - Make server to request AI move
     *     - Block on server request
     *     - Have server be dumb (pick randomly) 
     * 
     */
    
    
    public Text playerTotal, computerTotal, totalSelect;
    public Text playerCatch, computerCatch, selected;
    public Text end;

    private void Start()
    {
        if(Model.Instance == null) 
            Model.Instance = new Model();
        Model.Instance.applicationIntegration = this;
        Model.Instance.Init();
        
        if(InputSystem.Instance == null) InputSystem.Instance = new InputSystem();
        InputSystem.Instance.applicationIntegration = this;
        
        if(View.Instance == null) View.Instance = new View();
        View.Instance.applicationIntegration = this;
        View.Instance.Init();
    }

    private void Update()
    {
        InputSystem.Instance.Update();
    }

    public void Restart()
    {
        Start();
    }
}
