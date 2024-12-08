using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private GUIManager GUI;
    [SerializeField] private AudioSource globalAudio;

    [SerializeField] private AudioClip[] levelMusic;
    [SerializeField] private GameObject generator;
    private GameObject currentGenerator;


    public int hp = 60 ;
    public float cottonCandyCount;

    public float timer = 0f;
    public float levelCount = 0f;
    public static float difficulty = 1;
    void Start()
    {
        // LoadStartScene();
        enterNewLevel();
        //GUI = GameObject.Find("GUI").GetComponent<GUIManager>();
        
        // Setting up the level music

        globalAudio.volume = 0.25f;
        globalAudio.clip = levelMusic[Random.Range(0,levelMusic.Length)]; 
        globalAudio.loop = true;
        globalAudio.Play();
    }

    // Update is called once per frame
    void Update()
    {
        cottonCandyCount = hp / 60f;
        if (cottonCandyCount <= 0)
        {
            endGame();
        }

        timer += Time.deltaTime;
        
        //Debug.Log(cottonCandyCount);
    }
    public void endGame()
    {
        //Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        
        //GUI.ReportToPlayer("You died!", "Score:", levelCount);
        Debug.Log(levelCount);
        
        PlayerPrefs.SetFloat("FinalScore", levelCount);
    }
    
    public void enterNewLevel()
    {
        //GUI.ReportToPlayer("Your raccoon kids are proud of you", "Keep going!");
        
        levelCount++;
        difficulty = CalculateDifficulty();
        if (currentGenerator != null)
        {
            DestroyOldLevel();
        }
        currentGenerator = Instantiate(generator, new Vector3(0, 0, 0), Quaternion.identity);
        
        globalAudio.clip = levelMusic[Random.Range(0,levelMusic.Length)]; 
        globalAudio.loop = true;
        globalAudio.Play();
        
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
    
    static void LoadStartScene()
    {
        if (SceneManager.GetActiveScene().name != "Start")
        {
            SceneManager.LoadScene("Start");
        }
    }


}

