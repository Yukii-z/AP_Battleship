using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Game : MonoBehaviour
{
    public Text playerTotal, computerTotal;
    public Text playerCatch, computerCatch;
    public Text end;
    public bool isPlayerSelecting;
    
    // Start is called before the first frame update
    void Start()
    {
        Model.Instance = new Model();
        Model.Instance.Init();
        
        Controller.Instance = new Controller();
        Controller.Instance.game = this;
        
        View.Instance = new View();
        View.Instance.game = this;
        View.Instance.Init();

        isPlayerSelecting = true;
    }

    // Update is called once per frame
    void Update()
    {
        Controller.Instance.Update();
    }

    public void Restart()
    {
        Start();
    }
}
