using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class RaccoonController : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Insert Main Camera")]
    private Camera mainCamera;
    
    [SerializeField]
    [Tooltip("Insert Character Controller")]
    private CharacterController controller;
    
    [SerializeField]
    [Tooltip("Insert Animator Controller")]
    private Animator playerAnimator;
    
    [SerializeField]
    [Tooltip("Insert Trail Renderer")]
    private TrailRenderer trailRenderer;
    
    private Vector3 velocity;
    private float gravity = -9.81f;
    
    private bool canDash = true;
    private bool isDashing = false;
    private float dashingPower = 10f;
    private float dashingTime = 0.2f;
    private float dashingCooldown = 2f;
    private bool isEating = false;
    private bool isDeath = false;
    
    
    public float speed = 2f;
    public float slowSpeed = 1f;
    public float currentSpeed;
    public float rotationSpeed = 800f;
    
    public LevelManager levelManager;
    
    [SerializeField] private AudioClip[] dashSounds;
    [SerializeField] private AudioClip[] grassSounds;
    [SerializeField] private AudioClip deathSound;
    [SerializeField] private AudioClip dropSound;
    [SerializeField] private AudioClip eatingSound;
    
    private AudioSource movementAudio;
    private AudioSource raccoonAudio;
    
    private static readonly int IsSwimming = Animator.StringToHash("IsSwimming");
    private static readonly int IsEating = Animator.StringToHash("IsEating");
    private static readonly int IsWalking = Animator.StringToHash("IsWalking");
    private static readonly int IsDead = Animator.StringToHash("IsDead");
    
    float tiltAngle = 60.0f;

    void Start()
    {
        movementAudio = GetComponents<AudioSource>()[0];
        raccoonAudio = GetComponents<AudioSource>()[1];
    }
    
    void Update()
    {
        if (isDashing)
        {
            return;
        }

        if (isDeath)
        {
            return;
           
        }
        
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        // Get camera-relative directions for movement
        Vector3 camForward = mainCamera.transform.forward;
        camForward.y = 0;
        Vector3 camRight = mainCamera.transform.right;
        camRight.y = 0;

        // Calculate movement based on camera orientation
        Vector3 movement = (camForward * z) + (camRight * x);
        movement.Normalize();

        // Set walking animation
        if (movement.magnitude > 0)
        {
            playerAnimator.SetBool(IsWalking, true);
        }
        else
        {
            playerAnimator.SetBool(IsWalking, false);
        }
        

        if ((Input.GetMouseButton(0) | Input.GetKey(KeyCode.Space)) && canDash)
        {
            playerAnimator.SetBool(IsSwimming, true);
            StartCoroutine(Dash());
        }
        else
        {
            playerAnimator.SetBool(IsSwimming, false);
        }

        currentSpeed = speed;

        // Move the character
        controller.Move(movement * (currentSpeed * Time.deltaTime));
        
        Vector3 movementDirection = new Vector3(movement.x, 0, movement.z);
        movementDirection.Normalize();
        
        transform.Translate(movementDirection * speed * Time.deltaTime, Space.World);

        if (movementDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(movementDirection, Vector3.up);
            
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
        

        // Trigger eating animation on right-click
        if (Input.GetKey(KeyCode.E))
        {
            playerAnimator.SetBool(IsEating, true);
            isEating = true;
        }
        else
        {
            playerAnimator.SetBool(IsEating, false);
            isEating = false;
        }

        // Apply gravity
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime); 
    }

    private void FixedUpdate()
    {
        if (isDashing)
        {
            return;
        }
    }

    private IEnumerator Dash()
    {
        movementAudio.volume = 0.3f;
        movementAudio.clip = dashSounds[Random.Range(0, dashSounds.Length)];
        movementAudio.Play();
        
        canDash = false;
        isDashing = true;
        float originalGravity = gravity; // change this
        gravity = 0f;
        Vector3 dashDirection = transform.forward * dashingPower;
        trailRenderer.emitting = true;
        float dashEndTime = Time.time + dashingTime;
        while (Time.time < dashEndTime)
        {
            controller.Move(dashDirection * Time.deltaTime);
            yield return null; 
        }
        
        trailRenderer.emitting = false;
        gravity = originalGravity;
        isDashing = false;
        yield return new WaitForSeconds(dashingCooldown);
        canDash = true;
    }

    private IEnumerator Die()
    {
        raccoonAudio.volume = 0.8f;
        raccoonAudio.clip = deathSound;
        raccoonAudio.Play();
        playerAnimator.SetBool(IsDead, true);
        yield return new WaitForSeconds(3f);
        isDeath = true;
        playerAnimator.SetBool(IsWalking, false);
        playerAnimator.SetBool(IsDead, true);
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("CottonCandy") && isEating)
        {
            raccoonAudio.volume = 0.8f;
            raccoonAudio.clip = eatingSound;
            raccoonAudio.Play();
            Destroy(other.gameObject);
            levelManager.cottonCandyCount++;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Puddle"))
        {
            if (levelManager.cottonCandyCount >= 1)
            {
                if (levelManager.cottonCandyCount >= 1 && !isDashing)
                {
                    levelManager.cottonCandyCount--;
                    currentSpeed = slowSpeed;
                }
            }
            else
            {
                StartCoroutine(Die());
            }
        }
        if (other.gameObject.CompareTag("Sprinkler"))
        {
            if (levelManager.cottonCandyCount >= 1 && !isDashing)
            {
                levelManager.cottonCandyCount--;
            }
            else if (levelManager.cottonCandyCount <= 0)
            {
                StartCoroutine(Die());
                //Destroy(gameObject);
            }
        }
        /*
        if (other.gameObject.CompareTag("CottonCandy"))
        {
            Destroy(other.gameObject);
            levelManager.cottonCandyCount++;
        } // not sure if also wanna add this
        */
        /*
        if (other.gameObject.CompareTag("PotHole"))
        {
            //enter the new level
            levelManager.enterNewLevel()
        }
        */
    }
    
    public void footstep()
    {
        movementAudio.volume = 0.8f;
        movementAudio.pitch = Random.Range(1f, 2f);
        movementAudio.clip = grassSounds[Random.Range(0, grassSounds.Length)];
        movementAudio.Play();
    }
    
    public void hitGrass()
    {
        movementAudio.volume = 0.8f;
        movementAudio.clip = dropSound;
        movementAudio.Play();
    }
    
}

