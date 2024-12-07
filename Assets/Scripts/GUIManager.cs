using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class GUIManager : MonoBehaviour
{
    private Label levelLabel;
    private Label candyCounterLabel;
    
    public LevelManager levelManager;
    private float level;
    
    void Start()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        levelManager = GameObject.FindObjectOfType<LevelManager>();

        level = levelManager.levelCount;
        
        levelLabel = root.Q<Label>("level");
        candyCounterLabel = root.Q<Label>("cottonCandyCount");
        
        UpdateLevelDisplay();
        UpdateCandyCounter();
    }

    void Update()
    {
        UpdateLevelDisplay();
        UpdateCandyCounter();
    }
    
    private void UpdateLevelDisplay()
    {
        if (levelLabel != null)
        {
            level = levelManager.levelCount;
            levelLabel.text = $"Level {level}";
        }
    }

    private void UpdateCandyCounter()
    {
        if (candyCounterLabel != null && levelManager != null)
        {
            if (levelManager.cottonCandyCount <= 0)
            { 
                int roundedCandyCount = 0;
                candyCounterLabel.text = $": {roundedCandyCount}";
            }
            else
            {
                int roundedCandyCount = Mathf.CeilToInt(levelManager.cottonCandyCount) - 1;
                candyCounterLabel.text = $": {roundedCandyCount}";
            }
        }
    }
}
