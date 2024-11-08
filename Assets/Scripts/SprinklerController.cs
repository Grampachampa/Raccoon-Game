using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SprinklerController : MonoBehaviour
{
    public float rotationSpeed = 36.0f;
    
    [SerializeField] 
    private ParticleSystem waterParticles;
    
    private GameObject player;
    
    [SerializeField] private AudioClip[] sprinklerSounds;
    private AudioSource sprinklerAudio;
    
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        
        sprinklerAudio = GetComponent<AudioSource>();
        sprinklerAudio.volume = 1.0f;
        sprinklerAudio.spatialBlend = 0f; 
        sprinklerAudio.maxDistance = 5.0f;
        
    }

    void Update()
    {
        // Transforms
        Transform sprinklerTransform = transform;
        
        // Rotate the sprinkler around the y-axis
        sprinklerTransform.Rotate(0, 0, rotationSpeed * Time.deltaTime);

        // Spawning water
    }
}
