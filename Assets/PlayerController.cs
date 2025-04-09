using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    public MovementStats stats;
    [SerializeField] private Collider2D feetCollider;
    [SerializeField] private Collider2D bodyCollider;
    public Rigidbody2D rigidbody;

    private Vector2 moveVel;
    private bool facingRight;

    private RaycastHit2D groundHit;
    private RaycastHit2D headHit;
    public bool grounded;
    public bool hitHead;
    public float health = 5;
    public float jumpForceOnBeatBonus;

    public float VerticalVelocity { get; private set; }
    private bool jumping;
    private bool fastFalling;
    private bool falling;
    private float fastFallTime;
    private float fastFallReleaseSpeed;
    private int numJumpsUsed;

    //apex variables
    private float apexPoint;
    private float timePastApexThreshold;
    private bool pastApexThreshold;

    //jump buffer variables
    private float jumpBufferTimer;
    private bool jumpReleasedDuringBuffer;


    //coyote time variables
    private float coyoteTimer;
    void Awake()
    {
        //Initialisiation
        rigidbody = GetComponent<Rigidbody2D>();

        facingRight = true;
    }
    private void FixedUpdate()
    {
        Jump();
        CollisionCheck();
        if (grounded)
        {
            Move(stats.GroundAcc, stats.GroundDecc, InputManager.movement);
        }
        else
        {
            Move(stats.AirAcc, stats.AirDecc, InputManager.movement);
        }
    }
    void OnEnable()
    {

    }

    void OnDisable()
    {

    }

    void Update()
    {
        Debug.Log("World Gravity = " + Physics2D.gravity.y);
        Debug.Log("Player Gravity = " + stats.Gravity);
        Debug.Log("Player Init Jump Velocity = " + stats.InitJumpVel);
        Debug.Log("Player Falling = " + falling);
        Debug.Log("Player Jumping = " + jumping);
        Debug.Log("Player Hit Head = " + hitHead);
        Debug.Log("Player Fast Falling = " + fastFalling);
        Debug.Log("Player Vertical Velocity = " + VerticalVelocity);
        Debug.Log("Player Jumps Used = " + numJumpsUsed);
        JumpCheck();
        CountTimers();
        //GetComponent<Animator>().SetFloat("Speed", GameManager.instance.GetSongBPM() / 120);

        // Read movement input
        GetComponent<Animator>().SetFloat("Move", InputManager.movement.x);
        // Move in world space relative to player rotation
        //Vector3 move = new Vector3(moveInput.x, 0, moveInput.y) * speed * Time.deltaTime;
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
    private void Move(float acc, float decc, Vector2 moveInput)
    {
        Debug.Log(moveInput);
        if (moveInput != Vector2.zero)
        {
            TurnCheck(moveInput);
            Vector2 targetVel = Vector2.zero;
            /*if (running)
            {
                targetVel = new Vector2(moveInput.x, 0f) * stats.MaxSpeedRun;
            }
            else {targetVel = new Vector2(moveInput.x, 0f) * stats.MaxSpeedWalk;}
            */
            targetVel = new Vector2(moveInput.x, 0f) * stats.MaxSpeedWalk;

            moveVel = Vector2.Lerp(moveVel, targetVel, acc * Time.fixedDeltaTime);
            rigidbody.velocity = new Vector2(moveVel.x, rigidbody.velocity.y);
        }
        else if (moveInput == Vector2.zero)
        {
            moveVel = Vector2.Lerp(moveVel, Vector2.zero, decc * Time.fixedDeltaTime);
            rigidbody.velocity = new Vector2(moveVel.x, rigidbody.velocity.y);
        }
    }
    private void TurnCheck(Vector2 moveInput)
    {
        if (!facingRight && moveInput.x < 0)
        {
            Turn(false);
        }
    }
    private void Turn(bool turnRight)
    {
        if (turnRight)
        {
            facingRight = true;
            transform.Rotate(0f, 100f, 0f);
        }
        else
        {
            facingRight = false;
            transform.Rotate(0f, -100f, 0f);
        }
    }
    private void IsGrounded()
    {
        Vector2 boxCastOrigin = new Vector2(feetCollider.bounds.center.x, feetCollider.bounds.min.y);
        Vector2 boxCastSize = new Vector2(feetCollider.bounds.size.x, stats.groundDectectRayLength);

        groundHit = Physics2D.BoxCast(boxCastOrigin, boxCastSize, 0f, Vector2.down, stats.groundDectectRayLength, stats.groundLayer);
        if (groundHit.collider != null)
        {
            grounded = true;
        }
        else
        {
            grounded = false;
        }
        Color rayColour;
        if (grounded)
        {
            rayColour = Color.green;
        }
        else
        {
            rayColour = Color.red;
        }
        Debug.DrawRay(new Vector3(boxCastOrigin.x - boxCastSize.x / 2, boxCastOrigin.y), Vector2.down * stats.groundDectectRayLength, rayColour);
    }

    private void CollisionCheck()
    {
        IsGrounded();
        HitHead();
    }

    private void JumpCheck()
    {
        if (InputManager.jumpWasPressed)
        {
            Debug.Log("Jump Pressed");
            jumpBufferTimer = stats.jumpBufferTime; 
            jumpReleasedDuringBuffer = false;
        }
        if (InputManager.jumpReleased)
        {
            if (jumpBufferTimer > 0f)
            {
                Debug.Log("Jump Released during buffer");
                jumpReleasedDuringBuffer = true;
            }
            Debug.Log("Jump Released not during buffer");
            if (jumping && VerticalVelocity > 0f)
            {
                if(pastApexThreshold)
                {
                    pastApexThreshold = false;
                    fastFalling = true;
                    fastFallTime = stats.TimeForUpCancel;
                    VerticalVelocity = 0f;
                }
                else
                {
                    fastFalling = true;
                    fastFallReleaseSpeed = VerticalVelocity;
                }
            }    
        }
        if (jumpBufferTimer > 0f && !jumping && (!grounded || coyoteTimer > 0f))
        {
            Debug.Log("Jump Intialised");
            InitJump(1);
            if (jumpReleasedDuringBuffer)
            {
                Debug.Log("Fast falling due to jump buffer");
                fastFalling = true;
                fastFallReleaseSpeed = VerticalVelocity;
            }
        }
        else if (jumpBufferTimer > 0f && jumping && numJumpsUsed < stats.NumJumpsAllowed)
        {
            fastFalling = false;
            InitJump(1);
        }
        else if (jumpBufferTimer > 0f && falling && numJumpsUsed < stats.NumJumpsAllowed - 1)
        {
            InitJump(2);
            fastFalling = false;
        }

        if((jumping || falling) && grounded && VerticalVelocity <= 0f)
        {
            Debug.Log("Grounded. Reset variables");
            jumping = false;
            falling = false;
            fastFalling = false;
            fastFallTime = 0f;
            pastApexThreshold = false;
            numJumpsUsed = 0;

            VerticalVelocity = Physics2D.gravity.y;
        }
    }

    private void InitJump(int numberJumpsUsed)
    {
        if (!jumping)
        {
            jumping = true;
        }

        jumpBufferTimer = 0f;
        numJumpsUsed += numberJumpsUsed;
        VerticalVelocity = stats.InitJumpVel;
    }
    void Jump()
    {
        /*if (GameManager.instance.beat)
        {
            jump = new Vector3(0, jumpForce + jumpForceOnBeatBonus, 0);
        }
        else
        {
            jump = new Vector3(0, jumpForce, 0);
        }
        */
        if (jumping)
        {
            if (hitHead)
            {
                fastFalling = true;
            }
            if (VerticalVelocity >= 0f)
            {
                apexPoint = Mathf.InverseLerp(stats.InitJumpVel, 0f, VerticalVelocity);

                //Checks if past apex threshold
                if (apexPoint > stats.apexThreshold)
                {
                    if (!pastApexThreshold)
                    {
                        pastApexThreshold = true;
                        timePastApexThreshold = 0f;
                    }
                    if (pastApexThreshold)
                    {
                        timePastApexThreshold += Time.fixedDeltaTime;
                        if (timePastApexThreshold < stats.apexHangTime)
                        {
                            VerticalVelocity = 0f;
                        }
                        else
                        {
                            VerticalVelocity = -0.01f;
                        }
                    }
                }
                else
                {
                    VerticalVelocity += stats.Gravity;
                    if (pastApexThreshold)
                    {
                        pastApexThreshold = false;
                    }
                }
            }

            //If not fast falling
            else if (!fastFalling)
            {
                VerticalVelocity += stats.Gravity * stats.gravityOnReleaseMultiplier * Time.fixedDeltaTime;
            }
            else if (VerticalVelocity < 0f)
            {
                if (!falling)
                {
                    falling = true;
                }
            }
        }

        //If fast falling
        if (fastFalling)
        {
            if (fastFallTime >= stats.TimeForUpCancel)
            {
                VerticalVelocity += stats.Gravity * stats.gravityOnReleaseMultiplier * Time.fixedDeltaTime;
            }
            else if (fastFallTime < stats.TimeForUpCancel)
            {
                VerticalVelocity = Mathf.Lerp(fastFallReleaseSpeed, 0f, (fastFallTime / stats.TimeForUpCancel));
            }

            //Updates fast fall time
            fastFallTime += Time.fixedDeltaTime;
        }

        //Checks if not grounded and not jumping. If true, sets falling to true
        if (!grounded && !jumping)
        {
            if (!falling)
            {
                falling = true;
            }
            VerticalVelocity += stats.Gravity * Time.fixedDeltaTime;
        }
        Debug.Log("Vertical Velocity Pre Clamp  = " + VerticalVelocity);
        VerticalVelocity = Mathf.Clamp(VerticalVelocity, -stats.maxFallSpeed, 50f);

        rigidbody.velocity = new Vector2(rigidbody.velocity.x, VerticalVelocity);
    }
    private void HitHead()
    {
        Vector2 boxCastOrigin = new Vector2(feetCollider.bounds.center.x, bodyCollider.bounds.max.y);
        Vector2 boxCastSize = new Vector2(feetCollider.bounds.size.x * stats.HeadWidth, stats.HeadDetectionRayLength);

        headHit = Physics2D.BoxCast(boxCastOrigin, boxCastSize, 0f, Vector2.up, stats.HeadDetectionRayLength, stats.groundLayer);

        if (stats.DebugShowGroundedBox)
        {
            float headWidth = stats.HeadWidth;

            Color rayColour;
            if(!hitHead)
            {
                rayColour = Color.green;
            }
            else { rayColour = Color.red; }

            Debug.DrawRay(new Vector2(boxCastOrigin.x - boxCastSize.x / 2 * headWidth, boxCastOrigin.y), Vector2.up * stats.HeadDetectionRayLength, rayColour);
            Debug.DrawRay(new Vector2(boxCastOrigin.x + (boxCastSize.x / 2) * headWidth, boxCastOrigin.y), Vector2.up * stats.HeadDetectionRayLength, rayColour);
            Debug.DrawRay(new Vector2(boxCastOrigin.x - boxCastSize.x / 2 * headWidth, boxCastOrigin.y + stats.HeadDetectionRayLength), Vector2.right * boxCastSize.x * headWidth, rayColour);
        }
    }
    private void CountTimers()
    {
        jumpBufferTimer -= Time.deltaTime;

        if (!grounded)
        {
            coyoteTimer -= Time.deltaTime;
        }
        else
        {
            coyoteTimer = stats.jumpCoyoteTime;
        }
    }
    public void Damage(int amount)
    {
        health -= amount;
    }
    private void DrawJumpArc(float moveSpeed, Color gizmoColour)
    {
        Vector2 startPosition = new Vector2(feetCollider.bounds.center.x, feetCollider.bounds.min.y);
        Vector2 previousPosition = startPosition;
        float speed = 0f;
        if(stats.DrawRight)
        {
            speed = moveSpeed;
        }
        else { speed = -moveSpeed; }
        Vector2 velocity = new Vector2(speed, stats.InitJumpVel);

        Gizmos.color = gizmoColour;

        float timeStep = 2 * stats.timeTillJumpApex / stats.ArcResolution;

        for (int i = 9; i < stats.VisualisationSteps; i ++)
        {
            float simulationTime = i * timeStep;
            Vector2 displacement;
            Vector2 drawPoint;

            if (simulationTime < stats.timeTillJumpApex / stats.ArcResolution)
            {
                displacement = velocity * simulationTime * 0.5f * new Vector2(0, stats.Gravity) * simulationTime * simulationTime;
            }
            else if ( simulationTime < stats.timeTillJumpApex + stats.apexHangTime)
            {
                float apexTime = simulationTime - stats.timeTillJumpApex;
                displacement = velocity * stats.timeTillJumpApex;
                displacement += new Vector2(speed, 0) * apexTime;
            }
            else
            {
                float descendTime = simulationTime - (stats.timeTillJumpApex + stats.apexHangTime);
                displacement = velocity * stats.timeTillJumpApex + 0.5f * new Vector2(0, stats.Gravity) * stats.timeTillJumpApex * stats.timeTillJumpApex;
                displacement += new Vector2(speed, 0) * stats.apexHangTime;
                displacement += new Vector2(speed, 0) * new Vector2(0, stats.Gravity) * descendTime * descendTime;
            }
            drawPoint = startPosition + displacement;
            if (stats.StopOnCollision)
            {
                RaycastHit2D hit = Physics2D.Raycast(previousPosition, drawPoint - previousPosition, Vector2.Distance(previousPosition, drawPoint), stats.groundLayer);
                if (hit.collider != null)
                {
                    Gizmos.DrawLine(previousPosition, hit.point);
                    break;
                }
            }
            Gizmos.DrawLine(previousPosition, drawPoint);
            previousPosition = drawPoint;
        }
    }
    private void OnDrawGizmos()
    {
        if (stats.ShowWalkJumpArc)
        {
            DrawJumpArc(stats.MaxSpeedWalk, Color.white);
        }
        if (stats.ShowRunJumpArc)
        {
            DrawJumpArc(stats.MaxSpeedRun, Color.red);
        }
    }
}
