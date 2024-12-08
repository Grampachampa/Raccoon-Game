using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class EndScreenManager : MonoBehaviour
{
    private Label titleLabel;
    private Label line1Label;
    private Label line2Label;
    private Button playAgainButton;
    private Button exitGameButton;

    void Start()
    {
        // Initialize UI Elements
        var root = GetComponent<UIDocument>().rootVisualElement;
        
        line2Label = root.Q<Label>("line2");
        playAgainButton = root.Q<Button>("playAgain");
        exitGameButton = root.Q<Button>("exitGame");

        // Set up the UI
        line2Label.text = $"{PlayerPrefs.GetFloat("FinalScore", 0)}"; // Retrieve the score

        // Add button functionality
        playAgainButton.clicked += playAgainButtonPressed;
        exitGameButton.clicked += exitGameButtonPressed;
    }

    private void playAgainButtonPressed()
    {
        SceneManager.LoadScene("Start");
    }

    private void exitGameButtonPressed()
    {
        Application.Quit();
    }
}