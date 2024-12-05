using UnityEngine;
using UnityEngine.UIElements;

public class GUIManager : MonoBehaviour
{
    private Label levelLabel;
    private Label candyCounterLabel;

    private int currentLevel = 1;
    private RaccoonController raccoonController;

    void Start()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        // Access UI elements by their IDs
        levelLabel = root.Q<Label>("Level");
        candyCounterLabel = root.Q<Label>("HealthAmount");

        // Find RaccoonController in the scene
        raccoonController = FindObjectOfType<RaccoonController>();
     
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
        if (candyCounterLabel != null && raccoonController != null)
        {
            int candyCount = raccoonController.ateCandy;
            string candyCountString = candyCount.ToString();
            candyCounterLabel.text = $": {candyCountString}";
        }
    }
}