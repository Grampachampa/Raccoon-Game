using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class StartScreenManager : MonoBehaviour
{
    private Label startText;
    private float bounceSpeed = 15.1f; 
    private float bounceHeight = 8f; 
    private Vector3 originalPosition; 

    void Start()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        startText = root.Q<Label>("startText");

        if (startText != null)
        {
            originalPosition = startText.transform.position;
        }
    }

    void Update()
    {
        if (startText != null)
        {
            float newY = originalPosition.y + Mathf.Sin(Time.time * bounceSpeed) * bounceHeight;
            startText.transform.position = new Vector3(originalPosition.x, newY, originalPosition.z);
        }

        if (Input.anyKey)
        {
            SceneManager.LoadScene("Scenes/SampleScene");
        }
    }
}