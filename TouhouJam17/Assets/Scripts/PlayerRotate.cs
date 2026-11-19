using UnityEngine;
using UnityEngine.InputSystem;
using PS = EPlayerStations;

public class PlayerRotate : MonoBehaviour
{
    [SerializeField] private InputActionReference turnLeftAction;
    [SerializeField] private InputActionReference turnRightAction;
    [SerializeField] private InputActionReference focusAction;
    [SerializeField] private InputActionReference unfocusAction;

    [field: SerializeField] public PS CurrentStation { private set; get; } = PS.RinWindow;
    

    private void Start()
    {
        
    }

    private void FocusConsole(InputAction.CallbackContext cxt)
    {

    }

    private void UnfocusConsole(InputAction.CallbackContext cxt)
    {

    }


    private void TurnRight(InputAction.CallbackContext cxt)
    {
        if (CurrentStation == PS.MimaSeal)
        {
            CurrentStation = PS.Work1;
        }
        else
        {
            CurrentStation = (PS)((int)CurrentStation + 1);
        }
        
    }

    private void TurnLeft(InputAction.CallbackContext cxt)
    {
        if (CurrentStation == PS.Work1)
        {
            CurrentStation = PS.MimaSeal;
        }
        else
        {
            CurrentStation = (PS)((int)CurrentStation - 1);
        }
    }

    private void OnEnable()
    {
        turnRightAction.action.performed += TurnRight;
        turnLeftAction.action.performed += TurnLeft;
    }

    private void OnDisable()
    {
        turnRightAction.action.performed -= TurnRight;
        turnLeftAction.action.performed -= TurnLeft;
    }
}

public enum EPlayerStations
{
    Work1,
    Work2,
    RinWindow,
    Work3,
    Work4,
    MimiRadar,
    NueSpot,
    MimaSeal,
}
