using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class GUIManager : MonoBehaviour
{
    private Label levelLabel;
    private Label candyCounterLabel;
    
    public LevelManager levelManager;
    private float level;

    private GroupBox reportGb;
    private VisualElement info;
    private Label titleLabel;
    private Label line1Label;
    private Label line2Label;
    private Button playAgainButton;
    private Button exitGameButton;
    
    void Start()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        levelManager = GameObject.FindObjectOfType<LevelManager>();

        level = levelManager.levelCount;
        
        levelLabel = root.Q<Label>("level");
        candyCounterLabel = root.Q<Label>("cottonCandyCount");
        
        reportGb = root.Q<GroupBox>("report");
        info = root.Q<VisualElement>("info");
        titleLabel = root.Q<Label>("title");
        line1Label = root.Q<Label>("line1");
        line2Label = root.Q<Label>("line2");
        playAgainButton = root.Q<Button>("playAgain");
        exitGameButton = root.Q<Button>("exitGame");
        
        playAgainButton.clicked += playAgainButtonPressed;
        exitGameButton.clicked += exitGameButtonPressed;
        
        setDisplay(reportGb, false);
        setDisplay(info, true);
        
        UpdateLevelDisplay();
        UpdateCandyCounter();
    }

    void Update()
    {
        UpdateLevelDisplay();
        UpdateCandyCounter();
    }
    
    public void ReportToPlayer(string line1, string line2)
    {
        if (getDisplay(reportGb))
        {
            CancelInvoke(nameof(HideReport));
        }
        setDisplay(info, false);
        setDisplay(reportGb, true);
        line1Label.text = line1;
        line2Label.text = line2;
        line1Label.visible = true;
        line2Label.visible = true;
        playAgainButton.visible = false;
        exitGameButton.visible = false;
    }
    
    public void ReportToPlayer(string title, string line1, float levelCount)
    {
        if (getDisplay(info))
        {
            CancelInvoke(nameof(HideReport));
        }
        setDisplay(info, false);
        setDisplay(reportGb, true);
        titleLabel.text = title;
        line1Label.text = line1;
        titleLabel.visible = true;
        line1Label.visible = true;
        line2Label.visible = true;
        playAgainButton.visible = true;
        exitGameButton.visible = true;
        
        Invoke(nameof(HideReport), levelCount);
    }    
    
    private void HideReport()
    {        
        setDisplay(info, true);
        setDisplay(reportGb, false);
    }
    
    private void playAgainButtonPressed()
    {
        SceneManager.LoadScene("Scenes/Start");
    }

    private void exitGameButtonPressed()
    {
        Application.Quit();
    }

    private void setDisplay(GroupBox gb, bool display)
    {
        gb.style.display = display ? DisplayStyle.Flex : DisplayStyle.None;
    }
    
    private void setDisplay(VisualElement ve, bool display)
    {
        ve.style.display = display ? DisplayStyle.Flex : DisplayStyle.None;
    }


    private bool getDisplay(GroupBox gb)
    {
        if (gb.style.display == DisplayStyle.Flex)
        {
            return true;
        }
        return false;
    }
    
    private bool getDisplay(VisualElement ve)
    {
        if (ve.style.display == DisplayStyle.Flex)
        {
            return true;
        }
        return false;
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
