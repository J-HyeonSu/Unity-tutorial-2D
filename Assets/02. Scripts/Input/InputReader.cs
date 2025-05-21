using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "InputReader", menuName = "Input/InputReader")]
public class InputReader : ScriptableObject, InputSystem_Actions.IPlayerActions
{
    public event UnityAction<Vector2> Move = delegate { };
    public event UnityAction<Vector2, bool> Look = delegate { };
    public event UnityAction EnableMouseControlCamera = delegate { };
    public event UnityAction DisableMouseControlCamera = delegate { };

    private InputSystem_Actions inputActions;

    public Vector3 Direction => (Vector3)inputActions.Player.Move.ReadValue<Vector2>();

    void OnEnable()
    {
        if (inputActions == null)
        {
            inputActions = new InputSystem_Actions();
            inputActions.Player.SetCallbacks(this);
        }
        inputActions.Enable();
    }
    
    public void DisableInputActions()
    {
        if (Application.isPlaying && inputActions != null)
        {
            inputActions.Dispose();    
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        Move.Invoke(context.ReadValue<Vector2>());
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        Look.Invoke(context.ReadValue<Vector2>(), IsDeviceMouse(context));
    }

    private bool IsDeviceMouse(InputAction.CallbackContext context) => context.control.device.name == "Mouse";


    public void OnAim(InputAction.CallbackContext context)
    {
        // noop
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        // noop
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        // noop
    }

    public void OnCrouch(InputAction.CallbackContext context)
    {
        // noop
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        // noop
    }

    public void OnPrevious(InputAction.CallbackContext context)
    {
        // noop
    }

    public void OnNext(InputAction.CallbackContext context)
    {
        // noop
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        // noop
    }

    public void OnMouseControlCamera(InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            case InputActionPhase.Started:
                EnableMouseControlCamera.Invoke();
                break;
            case InputActionPhase.Canceled: 
                DisableMouseControlCamera.Invoke() ;
                break;
            
        }
    }
    
    
    
}
