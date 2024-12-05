using UnityEngine;
using UnityEngine.UIElements;

public class GUIManager : MonoBehaviour
{
    private Label levelLabel;
    private Label candyCounterLabel;

    private int currentLevel = 1;
    public LevelManager levelManager;  // Reference to LevelManager

    void Start()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        levelManager = GameObject.FindObjectOfType<LevelManager>();

        // Access UI elements by their IDs
        levelLabel = root.Q<Label>("Level");
        candyCounterLabel = root.Q<Label>("CottonCandyCount");

        // Initialize the UI
        UpdateLevelDisplay();
        UpdateCandyCounter();
    }

    void Update()
    {
        // Continuously update the candy counter
        UpdateCandyCounter();
    }

    public void SetLevel(int level)
    {
        currentLevel = level;
        UpdateLevelDisplay();
    }

    private void UpdateLevelDisplay()
    {
        if (levelLabel != null)
        {
            levelLabel.text = $"Level {currentLevel}";
        }
    }

    private void UpdateCandyCounter()
    {
        if (candyCounterLabel != null && levelManager != null)
        {
            if (levelManager.cottonCandyCount <= 0)
            { 
                int roundedCandyCount = 0;
                candyCounterLabel.text = $": {roundedCandyCount}";  // Format and display as a string
               
            }
            else
            {
                int roundedCandyCount = (Mathf.CeilToInt(levelManager.cottonCandyCount))-1;  // Round to nearest int
                candyCounterLabel.text = $": {roundedCandyCount}";  // Format and display as a string
            }
        }
    }
}