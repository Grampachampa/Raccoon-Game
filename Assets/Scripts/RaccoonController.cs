using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaccoonController : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Insert Main Camera")]
    private Transform mainCamera;
    
    [SerializeField]
    [Tooltip("Insert Character Controller")]
    private CharacterController controller;
    
    [SerializeField]
    [Tooltip("Insert Animator Controller")]
    private Animator playerAnimator;
    
    private Vector3 velocity;
    private float gravity = -9.81f;
    
    public float speed = 2f;
    
    // Start is called before the first frame update
    private static readonly int IsSwimming = Animator.StringToHash("IsSwimming");
    private static readonly int IsEating = Animator.StringToHash("IsEating");
    private static readonly int IsWalking = Animator.StringToHash("IsWalking");
    void Start()
    {

        
    }

    // Update is called once per frame
    void Update()
    {
        Transform playerTransform = transform;
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        Vector3 camForward = mainCamera.forward;
        camForward.y = 0;
        Vector3 camRight = mainCamera.right;
        camRight.y = 0;
        
        Vector3 movement = (camForward * x) + (camRight * z);
        if (movement.magnitude > 0)
        {
            playerAnimator.SetBool("IsWalking", true);
        }
        else
        {
            playerAnimator.SetBool("IsWalking", false);
        }
        
        if (Input.GetKeyDown(KeyCode.Space))
        {
            playerAnimator.SetBool("IsEating", true);
        }
        else
        {
            playerAnimator.SetBool("IsEating", false);
        }
        movement.Normalize();
        controller.Move(movement * (speed * Time.deltaTime));
        
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime); 
        
        
    }
}
