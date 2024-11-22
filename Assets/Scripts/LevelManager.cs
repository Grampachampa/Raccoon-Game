using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    private GUIManager GUI;
    public int cottonCandyCount;
    // Start is called before the first frame update
    void Start()
    {
       //GUI = GameObject.Find("GUI").GetComponent<GUIManager>();
        
    }

    // Update is called once per frame
    void Update()
    {
        if (cottonCandyCount <= 0)
        {
            endGame();
        }
        
    }
    private void endGame()
    {
        //Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        
        //GUI.ReportToPlayer("Oh no, you died! Your raccoon family’s fate remains uncertain.");

    }
    private void enterNewLevel()
    {
        //GUI.ReportToPlayer("Your raccoon kids are proud of you! Keep going!");

    }
}
