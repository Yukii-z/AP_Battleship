using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Game : MonoBehaviour
{
    public Text playerTotal, computerTotal, totalSelect;
    public Text playerCatch, computerCatch, selected;
    public Text end;
    public bool isPlayerSelecting;
    
    // Start is called before the first frame update
    void Start()
    {
        if(Model.Instance == null) Model.Instance = new Model();
        Model.Instance.game = this;
        Model.Instance.Init();
        
        if(Controller.Instance == null) Controller.Instance = new Controller();
        Controller.Instance.game = this;
        
        if(View.Instance == null) View.Instance = new View();
        View.Instance.game = this;
        View.Instance.Init();

        isPlayerSelecting = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(isPlayerSelecting) Controller.Instance.SelectUpdate(); 
        else Controller.Instance.GameUpdate();
    }

    public void Restart()
    {
        Start();
    }
}
