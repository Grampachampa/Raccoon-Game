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
    [SerializeField] private GameObject generator;
    private GameObject currentGenerator;


    public int hp = 60 ;
    public float cottonCandyCount;
    // Start is called before the first frame update

    public float timer = 0f;
    public float levelCount = 1f;
    public static float difficulty = 1;
    void Start()
    {
        enterNewLevel();
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
        
        Debug.Log(cottonCandyCount);
    }
    private void endGame()
    {
        //Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        
        //GUI.ReportToPlayer("Oh no, you died! Your raccoon family’s fate remains uncertain.");

    }
    
    public void enterNewLevel()
    {
        //GUI.ReportToPlayer("Your raccoon kids are proud of you! Keep going!");
        levelCount++;
        difficulty = CalculateDifficulty();
        if (currentGenerator != null)
        {
            DestroyOldLevel();
        }
        currentGenerator = Instantiate(generator, new Vector3(0, 0, 0), Quaternion.identity);

        InvokeNextFrame(SpawnPlayer);
    }

    public void SpawnPlayer()
    {
        GameObject raccoon = GameObject.Find("Raccoon");
        raccoon.transform.position = currentGenerator.GetComponent<BoundsGenerator>().playerSpawnLocation; 
    }

    private void DestroyOldLevel(){
        BoundsGenerator bounds = currentGenerator.GetComponent<BoundsGenerator>();
        bounds.Terminate();
        
    }

    private float CalculateDifficulty()
    {
        float levelDifficultyMod = 15f;
        return Mathf.Log(((timer + (levelCount + levelDifficultyMod))/20) + 1, 2.74f);
    }
    public void InvokeNextFrame(System.Action function)
    {
        try
        {
            StartCoroutine(InvokeNextFrameCoroutine(function));	
        }
        catch
        {
            Debug.Log("Trying to invoke " + function.ToString() + " but it doesn't seem to exist");	
        }			
    }
        
    private IEnumerator InvokeNextFrameCoroutine(System.Action function)
    {
        yield return null;
        function();
    }


}

