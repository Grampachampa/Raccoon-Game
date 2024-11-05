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
        movement.Normalize();
        controller.Move(movement * (speed * Time.deltaTime));
        
    }
}
