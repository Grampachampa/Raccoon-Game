using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuddleController : MonoBehaviour
{
    public float growthRate = 0.1f; // Rate at which the puddle grows
    public float maxScale = 3.0f;   // Maximum scale the puddle can reach

    private Vector3 initialScale;

    void Start()
    {
        initialScale = transform.localScale;
    }

    // This method is called when the puddle enters the SprinklerCone's trigger
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Sprinkler"))
        {
            // Start growing the puddle
            Debug.Log("Growing");
            StartCoroutine(GrowPuddle());
        }
    }

    // This method is called when the puddle exits the SprinklerCone's trigger
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Sprinkler"))
        {
            // Stop growing the puddle
            StopCoroutine(GrowPuddle());
        }
    }

    IEnumerator GrowPuddle()
    {
        while (transform.localScale.x < maxScale && transform.localScale.z < maxScale)
        {
            // Grow the puddle over time
            transform.localScale += new Vector3(growthRate, 0, growthRate) * Time.deltaTime;
            yield return null;
        }
    }
}