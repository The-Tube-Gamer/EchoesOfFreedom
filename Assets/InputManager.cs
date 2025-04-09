using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static PlayerInput input;

    public static Vector2 movement;
    public static bool jumpWasPressed;
    public static bool jumpHeld;
    public static bool jumpReleased;
    public static bool runHeld;

    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction runAction;
    private void Awake()
    {
        input = GetComponent<PlayerInput>();

        moveAction = input.actions["Move"];
        jumpAction = input.actions["Jump"];
        runAction = input.actions["Run"];
    }

    // Update is called once per frame
    void Update()
    {
        movement = moveAction.ReadValue<Vector2>();

        jumpWasPressed = jumpAction.WasPressedThisFrame();
        jumpHeld = jumpAction.IsPressed();
        jumpReleased = jumpAction.WasReleasedThisFrame();

        runHeld = runAction.IsPressed();
        Debug.Log(jumpHeld);
    }
}
