  using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Player Movement")]
public class MovementStats : ScriptableObject
{
    [Header("Walk")]
    [Range(1, 100f)] public float MaxSpeedWalk = 12.5f;
    [Range(0.25f, 50f)] public float GroundAcc = 5f;
    [Range(0.25f, 50f)] public float GroundDecc = 20f;
    [Range(0.25f, 50f)] public float AirAcc = 5f;
    [Range(0.25f, 50f)] public float AirDecc = 5f;

    [Header("Walk")]
    public float MaxSpeedRun = 12.5f;

    [Header("Ground Checks")]
    public LayerMask groundLayer;
    public float groundDectectRayLength = 0.02f;
    public float HeadDetectionRayLength = 0.02f;
    [Range(0f, 1f)] public float HeadWidth = 0.75f;

    [Header("Jump Cut")]
    public float jumpHeight = 6.5f;
    [Range(1f, 1.1f)] public float jumpHeightCompensationFactor = 1.054f;
    public float timeTillJumpApex = 0.35f;
    [Range(0.01f, 5f)] public float gravityOnReleaseMultiplier = 2f;
    public float maxFallSpeed = 26f;
    [Range(1, 5)] public int NumJumpsAllowed = 2;

    [Header("Jump Cut")]
    [Range(0.02f, 0.3f)] public float TimeForUpCancel = 0.027f;

    [Header("Jump Apex")]
    [Range(0.5f, 1f)] public float apexThreshold = 0.97f;
    [Range(0.5f, 1f)] public float apexHangTime = 0.075f;

    [Header("Jump Buffer")]
    [Range(0f, 1f)] public float jumpBufferTime = 0.125f;

    [Header("Jump Coyote Time")]
    [Range(0f, 1f)] public float jumpCoyoteTime = 0.1f;

    [Header("Debug")]
    public bool DebugShowGroundedBox;
    public bool DebugShowHeadHitBox;

    [Header("Jump Visualisation Tool")]
    public bool ShowWalkJumpArc = false;
    public bool ShowRunJumpArc = false;
    public bool StopOnCollision = true;
    public bool DrawRight = true;

    [Range(5, 100)] public int ArcResolution = 20;
    [Range(0, 500)] public int VisualisationSteps = 90;

    public float Gravity { get; private set; }
    public float InitJumpVel {get; private set; }
    public float adjustedJumpHeight { get; private set; }

    private void OnValidate()
    {
        CalculateValues();
    }
    private void OnEnable()
    {
        
    }
    private void CalculateValues()
    {
        adjustedJumpHeight = jumpHeight * jumpHeightCompensationFactor;
        Gravity = -(2f * adjustedJumpHeight) / Mathf.Pow(timeTillJumpApex, 2f);
        InitJumpVel = Mathf.Abs(Gravity) * timeTillJumpApex;
        //Gravity = -0.2f;
    }
}
