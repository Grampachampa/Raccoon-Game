using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Cursor = UnityEngine.Cursor;

public class EndScreenManager : MonoBehaviour
{
    private Label titleLabel;
    private Label line1Label;
    private Label line2Label;
    private Button playAgainButton;
    private Button exitGameButton;

    private float pulseSpeed = 5f;  // Speed of the pulse
    private float pulseAmount = 0.03f;  // Amount to scale
    void Start()
    {
        Cursor.visible = true;
        var root = GetComponent<UIDocument>().rootVisualElement;
        
        line2Label = root.Q<Label>("line2");
        playAgainButton = root.Q<Button>("playAgain");
        exitGameButton = root.Q<Button>("exitGame");
        
        if (playAgainButton != null)
            StartCoroutine(PulseButton());
        
        line2Label.text = $"{PlayerPrefs.GetFloat("FinalScore", 0)}";
        
        playAgainButton.clicked += playAgainButtonPressed;
        exitGameButton.clicked += exitGameButtonPressed;
    }
    
    private IEnumerator PulseButton()
    {
        while (true)
        {
            float scale = 1 + Mathf.Sin(Time.time * pulseSpeed) * pulseAmount;

            playAgainButton.style.scale = new Scale(new Vector3(scale, scale, 1));
            
            yield return null;
        }
    }

    private void playAgainButtonPressed()
    {
        PlayerPrefs.SetFloat("FinalScore", 1);
        SceneManager.LoadScene("SampleScene");
    }

    private void exitGameButtonPressed()
    {
        Application.Quit();
    }
}