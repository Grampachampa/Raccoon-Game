using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuddleController : MonoBehaviour
{
    public bool inCone = false;
    public float growthRate = 10f; // Rate at which the puddle grows
    public float maxScale = 2.0f;   // Maximum scale the puddle can reach

    private Vector3 initialScale;
    private Vector3 maxPuddleScale;

    void Start()
    {
        initialScale = transform.localScale;
        maxPuddleScale = initialScale * maxScale;
    }

    private void Update()
    {
        if (transform.localScale.x < maxPuddleScale.x && transform.localScale.z < maxPuddleScale.z && inCone)
        {
            Debug.Log("GROWING");
            // Grow the puddle over time
            Vector3 newSize = transform.localScale + Vector3.one * growthRate * Time.deltaTime;
            transform.localScale = newSize;
        }
    }

    // This method is called when the puddle enters the SprinklerCone's trigger
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Sprinkler"))
        {
            // Start growing the puddle
            inCone = true;
        }
    }

    // This method is called when the puddle exits the SprinklerCone's trigger
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Sprinkler"))
        {
            inCone = false;
        }
    }
}