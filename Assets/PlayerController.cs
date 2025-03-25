using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
        public float speed = 5f;
        public bool grounded;
        bool canDoubleJump = true;
        private Vector2 moveInput;
        private bool jumpInput;
        public Rigidbody2D rigidbody;
        public PlayerInput playerInput;
        InputAction inputActions;
        InputAction jumpAction;
    InputAction attackAction;
    public float jumpForce;
    public float jumpForceOnBeatBonus;
    public float maxSpeedX;
    void Awake()
    {
        inputActions = playerInput.actions.FindAction("Move");
        jumpAction = playerInput.actions.FindAction("Jump");
        jumpAction.performed += DoJumpAction;
        attackAction = playerInput.actions.FindAction("Attack");
        attackAction.performed += DoAttackAction;
    }

    void OnEnable()
        {
            inputActions.Enable();
            jumpAction.Enable();
            attackAction.Enable();
    }

        void OnDisable()
        {
            inputActions.Disable();
            jumpAction.Disable();
            attackAction.Disable();
        }

        void Update()
        {
        GetComponent<Animator>().SetFloat("Speed", GameManager.instance.trackList[GameManager.instance.currentSong].bpm / 120);
        if (grounded)
        {
            canDoubleJump = true;
        }
        if (GameManager.instance.beat)
        {
            //transform.localScale = new Vector3(1f, 1);
        }
        else
        {
            //transform.localScale = new Vector3(0.5f, 1);

        }

            // Read movement input
            moveInput = inputActions.ReadValue<Vector2>();
        GetComponent<Animator>().SetFloat("Move", inputActions.ReadValue<Vector2>().x);
        // Move in world space relative to player rotation
        //Vector3 move = new Vector3(moveInput.x, 0, moveInput.y) * speed * Time.deltaTime;
        if (rigidbody.velocity.x >= maxSpeedX)
        {
            rigidbody.velocity = new Vector2(maxSpeedX, rigidbody.velocity.y);
        }
        else if (rigidbody.velocity.x <= -maxSpeedX)
        {
            rigidbody.velocity = new Vector2(-maxSpeedX, rigidbody.velocity.y);
        }
        rigidbody.velocity = new Vector2(rigidbody.velocity.x + (speed * moveInput.x), rigidbody.velocity.y);
    }
    void DoJumpAction(InputAction.CallbackContext context)
    {
        Jump();
    }
    void DoAttackAction(InputAction.CallbackContext context)
    {
        if (GameManager.instance.beat)
        {
            GetComponent<Animator>().Play("TEST ATTACK 2");
        }
        else
        {
            GetComponent<Animator>().Play("TEST ATTACK");
        }
    }
    void Jump()
    {
        if (grounded || canDoubleJump)
        {
            Debug.Log("JumpAction Success!!");
            Vector3 jump = new Vector3();
            if (GameManager.instance.beat)
            {
                jump = new Vector3(0, jumpForce + jumpForceOnBeatBonus, 0);
            }
            else
            {
                jump = new Vector3(0, jumpForce, 0);
            }
            
            rigidbody.AddForce(jump);
            if (!grounded)
            {
                canDoubleJump = false;
            }
        }
    }

}
