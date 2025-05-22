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

    public event UnityAction<bool> Jump = delegate { };

    public event UnityAction<bool> Dash = delegate { };


    private InputSystem_Actions inputActions;

    public Vector3 Direction => (Vector3)inputActions.Player.Move.ReadValue<Vector2>();

    
    //ScriptableObject가 활성화될 때 Input System 초기화 및 활성화
    void OnEnable()
    {
        if (inputActions == null)
        {
            inputActions = new InputSystem_Actions();
            inputActions.Player.SetCallbacks(this);
        }
        
    }

    public void EnablePlayerActions()
    {
        inputActions.Enable();
    }
    
    public void DisableInputActions()
    {
        inputActions.UI.Disable();
        inputActions.Disable();
        inputActions.Dispose();
        if (Application.isPlaying && inputActions != null)
        {
            

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
        
        switch (context.phase)
        {
            case InputActionPhase.Started:
                Jump.Invoke(true);
                break;
            case InputActionPhase.Canceled:
                Jump.Invoke(false);
                break;
        }
    }

    public void OnPrevious(InputAction.CallbackContext context)
    {
        // noop
    }

    public void OnNext(InputAction.CallbackContext context)
    {
        // noop
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            //우클릭 눌렀을때
            case InputActionPhase.Started:
                Dash.Invoke(true);
                break;
            //우클릭 떌떄
            case InputActionPhase.Canceled: 
                Dash.Invoke(false) ;
                break;
            
        }
    }

    public void OnMouseControlCamera(InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            //우클릭 눌렀을때
            case InputActionPhase.Started:
                EnableMouseControlCamera.Invoke();
                break;
            //우클릭 떌떄
            case InputActionPhase.Canceled: 
                DisableMouseControlCamera.Invoke() ;
                break;
            
        }
    }
    
    
    
}
