using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Cursor = UnityEngine.Cursor;

public class StartScreenManager : MonoBehaviour
{
    private Label startText;
    private Label title;
    private float bounceSpeed = 15.1f; 
    private float bounceHeight = 8f; 
    private Vector3 originalPosition; 

    private float pulseSpeed = 5f;  // Speed of the pulse
    private float pulseAmount = 0.03f;  // Amount to scale
    void Start()
    {
        Cursor.visible = false;
        var root = GetComponent<UIDocument>().rootVisualElement;
        startText = root.Q<Label>("startText");
        title = root.Q<Label>("title");

        if (startText != null)
        {
            StartCoroutine(PulseText());
        }
    }

    void Update()
    {
        if (title != null)
        {
            float newY = originalPosition.y + Mathf.Sin(Time.time * bounceSpeed) * bounceHeight;
            title.transform.position = new Vector3(originalPosition.x, newY, originalPosition.z);
        }

        if (Input.anyKey)
        {
            SceneManager.LoadScene("Scenes/SampleScene");
        }
    }
    
    private IEnumerator PulseText()
    {
        while (true)
        {
            // Calculate the scale factor
            float scale = 1 + Mathf.Sin(Time.time * pulseSpeed) * pulseAmount;

            // Apply the scale transformation to the button
            startText.style.scale = new Scale(new Vector3(scale, scale, 1));

            // Wait until the next frame
            yield return null;
        }
    }
}