using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    private GUIManager GUI;
    private AudioSource globalAudio;

    [SerializeField] private AudioClip[] levelMusic;


    public int hp = 60 ;
    public float cottonCandyCount;
    // Start is called before the first frame update

    public float timer = 0f;
    public float levelCount = 1f;
    public static float difficulty = 1;
    void Start()
    {
       //GUI = GameObject.Find("GUI").GetComponent<GUIManager>();
        
       // Setting up the level music
       globalAudio = GameObject.Find("Main Camera").GetComponent<AudioSource>();

        globalAudio.volume = 0.17f;
        globalAudio.clip = levelMusic[Random.Range(0,levelMusic.Length)]; 
        globalAudio.loop = true;
        globalAudio.Play();
    }

    // Update is called once per frame
    void Update()
    {
        cottonCandyCount = hp / 60f;
        if (cottonCandyCount < 0)
        {
            endGame();
        }

        timer += Time.deltaTime;
        
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
        levelCount++;
        difficulty = CalculateDifficulty();
    }

    private float CalculateDifficulty()
    {
        float levelDifficultyMod = 15f;
        return Mathf.Log(((timer + (levelCount + levelDifficultyMod))/20) + 1, 2.74f);
    }

}

