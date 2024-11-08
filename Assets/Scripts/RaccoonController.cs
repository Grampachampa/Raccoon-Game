using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

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
    
    public float speed = 2f;
    public float slowSpeed = 1f;
    public float currentSpeed;
    public LevelManager levelManager;
    
    
    
    private static readonly int IsSwimming = Animator.StringToHash("IsSwimming");
    private static readonly int IsEating = Animator.StringToHash("IsEating");
    private static readonly int IsWalking = Animator.StringToHash("IsWalking");

    void Update()
    {
        if (isDashing)
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

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("CottonCandy") && isEating)
        {
            Destroy(other.gameObject);
            levelManager.cottonCandyCount++;
        }
        /*
        if (other.gameObject.CompareTag("Puddle"))
        {
            if (levelManager.cottonCandyCount >= 1)
            {
                levelManager.cottonCandyCount--;
                currentSpeed = slowSpeed;
            }
            else
            {
                Destroy(gameObject);
            }
        }
        if (other.gameObject.CompareTag("Rain"))
        {
            if (levelManager.cottonCandyCount >= 1)
            {
                levelManager.cottonCandyCount--;
            }
            else
            {
                Destroy(gameObject);
            }
        }
        */
    }
    
    /*

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("PotHole"))
        {
            //enter the new level
        }
    }
    */
}

