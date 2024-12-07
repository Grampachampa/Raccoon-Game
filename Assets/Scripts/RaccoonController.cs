using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
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

    private bool dead = false;
    private bool canDash = true;
    private bool isDashing = false;
    private float dashingPower = 14f;
    private float dashingTime = 0.2f;
    private float dashingCooldown = 1f;
    private bool isEating = false;
    private bool isDeath = false;
    private bool inPuddle = false;

    public int ateCandy = 0; // to keep track of amount of cotton candy eaten
    
    private float speed = 3.5f;
    private float slowSpeed = 2f;
    public float currentSpeed;
    private float rotationSpeed = 800f;
    
    public LevelManager levelManager;
    
    [SerializeField] private AudioClip[] dashSounds;
    [SerializeField] private AudioClip[] grassSounds;
    [SerializeField] private AudioClip[] waterSounds;
    [SerializeField] private AudioClip[] deathSounds;
    [SerializeField] private AudioClip dropSound;
    [SerializeField] private AudioClip[] eatingSounds;
    
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
        currentSpeed = speed;
    }
    
    void Update()
    {
        if (SceneManager.GetActiveScene().name == "Start")
        {
            canDash = false;
        }
        
        if (isDashing)
        {
            return;
        }

        if (isDeath)
        {
            return;
           
        }
        
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

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

        // Move the character
        controller.Move(movement * (currentSpeed * Time.deltaTime));
        
        Vector3 movementDirection = new Vector3(movement.x, 0, movement.z);
        movementDirection.Normalize();
        
        transform.Translate(movementDirection * currentSpeed * Time.deltaTime, Space.World);

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
        
        
        // Check for game end
        if (levelManager.cottonCandyCount <= 0)
        {
            StartCoroutine(Die());
            //Destroy(gameObject);
        }
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
        raccoonAudio.volume = 0.3f;
        raccoonAudio.pitch = Random.Range(1f, 1.5f);
        raccoonAudio.clip = dashSounds[Random.Range(0, dashSounds.Length)];
        raccoonAudio.Play();
        
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
        raccoonAudio.pitch = Random.Range(2f, 2.5f);
        raccoonAudio.clip = deathSounds[Random.Range(0, deathSounds.Length)];;
        raccoonAudio.Play();
        playerAnimator.SetBool(IsDead, true);
        yield return new WaitForSeconds(0.5f);
        isDeath = true;
        playerAnimator.SetBool(IsWalking, false);
        playerAnimator.SetBool(IsDead, true);
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
        SceneManager.LoadScene("End");
    }

    private bool hasEatenCandy = false;

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("CottonCandy") && isEating && !hasEatenCandy)
        {
            raccoonAudio.volume = 0.8f;
            raccoonAudio.pitch = Random.Range(2f, 3f);
            raccoonAudio.clip = eatingSounds[Random.Range(0, eatingSounds.Length)];
            raccoonAudio.Play();
            
            Destroy(other.gameObject);
            levelManager.hp += 60;
            ateCandy++;
            
            hasEatenCandy = true;
            
            StartCoroutine(ResetEatingFlag());
        }
        
        if (other.gameObject.CompareTag("Puddle"))
        {
            inPuddle = true;
            if (levelManager.hp >= 0.01 && !isDashing)
            {
                currentSpeed = slowSpeed;
                levelManager.hp--;
                //levelManager.hp -= (int)(Time.deltaTime * 10);
            }
            else if (levelManager.hp <= 0 && !dead)
            {
                dead = true;
                StartCoroutine(Die());
                //Destroy(gameObject);
            }
        }
        if (other.gameObject.CompareTag("Sprinkler"))
        {
            if (levelManager.hp >= 0.01 && !isDashing)
            {
                levelManager.hp--;
                //levelManager.hp -= (int)(Time.deltaTime * 5);
            }
            else if (levelManager.hp <= 0 && !dead)
            {
                dead = true;
                StartCoroutine(Die());
                //Destroy(gameObject);
            }
        }
        if (other.gameObject.CompareTag("PotHole") && Input.GetKey(KeyCode.E))
        {
            //enter the new level
            levelManager.enterNewLevel();
        }

    }
    
    private void OnTriggerEnter(Collider other)
    {
       
        
       
       
        /*
        if (other.gameObject.CompareTag("CottonCandy"))
        {
            Destroy(other.gameObject);
            levelManager.cottonCandyCount++;
        } // not sure if also wanna add this
        */
      
    }

    private IEnumerator ResetEatingFlag()
    {
        yield return new WaitForSeconds(0.2f);
        hasEatenCandy = false;
    }
  
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Puddle"))
        {
            inPuddle = false;
            currentSpeed = speed;
        }
    }
    
    public void footstep()
    {
        if (!inPuddle)
        {
            movementAudio.volume = 0.9f;
            movementAudio.clip = grassSounds[Random.Range(0, grassSounds.Length)];
        }
        else
        {
            movementAudio.volume = 0.8f;
            movementAudio.clip = waterSounds[Random.Range(0, waterSounds.Length)];
        }
        movementAudio.pitch = Random.Range(1f, 2f);
        movementAudio.Play();
    }
    
    public void hitGrass()
    {
        movementAudio.volume = 0.8f;
        movementAudio.clip = dropSound;
        movementAudio.Play();
    }
    
}

