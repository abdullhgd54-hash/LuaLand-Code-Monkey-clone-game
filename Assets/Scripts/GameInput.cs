using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameInput : MonoBehaviour
{
    public static GameInput Instance { get; private set; }
    private InputActions inputActions;
    public Vector2 MovementInput => inputActions.Player.Movement.ReadValue<Vector2>();
    public event EventHandler OnMenuButtonPressed;
    private void OnDestroy()
    {
        inputActions.Disable();
    }
    private void Awake()
    {
        Instance = this;
        inputActions = new();
        inputActions.Enable();

        inputActions.Player.Menu.performed += Menu_OnPerformed;
    }

    private void Menu_OnPerformed(InputAction.CallbackContext context)
    {
        OnMenuButtonPressed(this, EventArgs.Empty);
    }

    public bool IsUpActionPressed() => inputActions.Player.LanderUp.IsPressed();
    public bool IsRightActionPressed() => inputActions.Player.LanderRight.IsPressed();
    public bool IsLeftActionPressed() => inputActions.Player.LanderLeft.IsPressed();
}
