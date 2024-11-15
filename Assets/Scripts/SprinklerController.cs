using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class SprinklerController : MonoBehaviour
{
    public float rotationSpeed = 36.0f;
    
    [SerializeField] 
    private ParticleSystem waterParticles;
    
    [SerializeField] 
    private GameObject[] puddlePrefabs;
    
    private GameObject player;
    
    [SerializeField] private AudioClip[] sprinklerSounds;
    private AudioSource sprinklerAudio;

    private bool didOnce = false;

    [SerializeField] private int puddleSpawnChance = 100;
    
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
   
        // Rotate the sprinkler around the y-axis
        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);

        // Spawning puddles
        if (didOnce != true)
        {
            didOnce = true;
            Invoke("spawnPuddle", Random.Range(5f,10f));
        }
    }
    
    void spawnPuddle()
    {
        Renderer coneRenderer = transform.GetChild(0).GetComponent<Renderer>();
        if (coneRenderer != null)
        {
            Bounds coneBounds = coneRenderer.bounds;
            
            //Random x and y coordinates in the cone
            float randomX = Random.Range(coneBounds.min.x, coneBounds.max.x);
            float randomZ = Random.Range(coneBounds.min.z, coneBounds.max.z);
            
            Vector3 randomPos = new Vector3(randomX, 0.01f, randomZ);
            
            // Create the desired rotation
            float randomYRotation = Random.Range(0f, 360f); 
            Quaternion spawnRotation = Quaternion.Euler(-90f, randomYRotation, 0f);

            
            Instantiate(puddlePrefabs[Random.Range(0, puddlePrefabs.Length)], randomPos, spawnRotation);
        }
        else
        {
            Debug.Log("No renderer found");
        }
        
        didOnce = false;
    }
    
}
