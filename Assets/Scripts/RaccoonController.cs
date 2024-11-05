using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaccoonController : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Insert Character Controller")]
    private CharacterController controller;
    
    [SerializeField]
    [Tooltip("Insert Animator Controller")]
    private Animator playerAnimator;
    
    // Start is called before the first frame update
    private static readonly int IsThrowing = Animator.StringToHash("IsSwimming");
    private static readonly int IsRunning = Animator.StringToHash("IsEating");
    private static readonly int IsWalking = Animator.StringToHash("IsWalking");
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
