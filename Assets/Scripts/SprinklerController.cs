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
        sprinklerTransform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
        
        // Checking if player is in spray
        // if (inSprinkler(player, 45, 3))
        // {
        //     
        // }


        // Spawning water
    }

    // bool inSprinkler(GameObject player,  float angle, float maxDistance)
    // {
    //     Vector3 heading = Vector3.Normalize(player.transform.position - transform.position);
    //     float dot = Vector3.Dot(heading, transform.forward);
    //     float cone = 1.0f - angle;
    //     float distance =  (transform.position - player.transform.position).magnitude;
    //     
    //     if (dot >= cone && distance < maxDistance)
    //     {
    //         return true;
    //     }
    //     return false;
    // }
}
