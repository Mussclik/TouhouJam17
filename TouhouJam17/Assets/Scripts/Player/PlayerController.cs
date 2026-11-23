using System;
using System.Collections;
using System.Collections.Generic;
using Audio;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance = null;
    [SerializeField] private BaseStation[] stations = new BaseStation[(int)EStates.enumCount];

    [Header("Rotation")] [SerializeField] private Transform baseCameraPos;
    [SerializeField] private Transform playerRotation;
    [SerializeField] private AnimationCurve rotationCurve;
    [SerializeField] private Light flashlight = null;
    [SerializeField] private float rotationLength = 45f;
    [SerializeField] private float rotationTime = 0.3f;
    [SerializeField] public EStates currentState;
    public bool IsFlashlightEnabled => flashlight.gameObject.activeSelf;

    [Header("cameraMoving")] [SerializeField]
    private AnimationCurve cameraMoveCurve;

    [SerializeField] private float cameraMoveTime = 0.3f;

    private Coroutine myMovingCoroutine = null;
    public bool isFocused;
    private BaseStation CurrentStation => stations[(int)currentState];

    #region InputActions

    [Header("InputActions")] [SerializeField]
    private InputActionReference upRef;

    [SerializeField] private InputActionReference backRef;
    [SerializeField] private InputActionReference leftRef;
    [SerializeField] private InputActionReference rightRef;
    [SerializeField] private InputActionReference toggleFlashlightAction;
    [SerializeField] private InputActionReference pauseAction;
    [SerializeField] private GameObject pauseMenu;

    private void Awake()
    {
        if (!Instance) Instance = this;
        else Destroy(gameObject);
    }

    private void OnEnable()
    {
        toggleFlashlightAction.action.performed += ToggleFlashlight;
        pauseAction.action.performed += TogglePauseMenu;
    }

    private void OnDisable()
    {
        toggleFlashlightAction.action.performed -= ToggleFlashlight;
        pauseAction.action.performed -= TogglePauseMenu;
    }

    private void TogglePauseMenu(InputAction.CallbackContext cxt)
    {
        pauseMenu.SetActive(!pauseMenu.activeSelf);
    }

    private void Start()
    {
        AudioSource source = Sound.PlayFromResource("Atmosphere", transform.position, aIsThreeDimensional: false, aFollow: transform, aVolume: 0.8f, aMixerGroup: Resources.Load<AudioMixer>("Mixer/AudioMixer").FindMatchingGroups("Music")[0], aStoppable:true);
        source.loop = true;

    }

    private void Update()
    {
        UpdateKeys();
    }

    private void UpdateKeys()
    {
        if (upRef.action.IsPressed()) GoUp();
        if (backRef.action.IsPressed()) GoDown();
        if (leftRef.action.IsPressed()) GoLeft();
        if (rightRef.action.IsPressed()) GoRight();
    }

    #endregion

    #region Moving

    private IEnumerator MoveCamera(Transform aCamera, Transform aDestination, Transform aRotation)
    {
        Vector3 posStart = aCamera.position;
        Vector3 posEnd = aDestination.position;
        Quaternion rotStart = aCamera.rotation;
        Quaternion rotEnd = aRotation.rotation;
        float time = 0f;

        while (time < cameraMoveTime)
        {
            time += Time.deltaTime;
            if (aDestination != null)
            {
                aCamera.position = Vector3.Lerp(posStart, posEnd, cameraMoveCurve.Evaluate(time / cameraMoveTime));
            }

            if (aRotation != null)
            {
                aCamera.rotation = Quaternion.Lerp(rotStart, rotEnd, cameraMoveCurve.Evaluate(time / cameraMoveTime));
            }

            yield return null;
        }

        aCamera.position = posEnd;
        aCamera.rotation = rotEnd;
        myMovingCoroutine = null;
    }

    private IEnumerator RotateBody(bool aIsRotatingRight)
    {
        float time = 0f;
        float amountToRotate = rotationLength;
        if (!aIsRotatingRight) amountToRotate *= -1;

        Vector3 eulerNewRot = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y + amountToRotate, transform.eulerAngles.z);
        Quaternion endRot = Quaternion.Euler(eulerNewRot);
        Quaternion startRot = transform.rotation;
        while (time < rotationTime)
        {
            time += Time.deltaTime;
            transform.rotation = Quaternion.Lerp(startRot, endRot, rotationCurve.Evaluate(time / rotationTime));
            yield return null;
        }

        transform.rotation = endRot;

        myMovingCoroutine = null;
    }

    private void GoUp()
    {
        if (CurrentStation.canFocus && myMovingCoroutine == null && isFocused == false)
        {
            myMovingCoroutine = StartCoroutine(MoveCamera(Camera.main.transform, CurrentStation.IdealCameraPos, CurrentStation.IdealCameraPos));
            isFocused = true;
        }
        else if (currentState == EStates.NueSpot && myMovingCoroutine == null && isFocused == true)
        {
            GoDown();
        }
    }

    private void GoDown()
    {
        if (myMovingCoroutine == null && isFocused == true)
        {
            myMovingCoroutine = StartCoroutine(MoveCamera(Camera.main.transform, baseCameraPos, baseCameraPos));
            isFocused = false;
        }
    }

    private void GoLeft()
    {
        if (myMovingCoroutine == null && isFocused == false)
        {
            myMovingCoroutine = StartCoroutine(RotateBody(false));
            --currentState;
            currentState = (EStates)Mathf.Repeat((int)currentState, (int)EStates.enumCount);
        }
    }

    private void GoRight()
    {
        if (myMovingCoroutine == null && isFocused == false)
        {
            myMovingCoroutine = StartCoroutine(RotateBody(true));
            ++currentState;
            currentState = (EStates)Mathf.Repeat((int)currentState, (int)EStates.enumCount);
        }
    }

    #endregion

    private void ToggleFlashlight(InputAction.CallbackContext cxt)
    {
        flashlight.gameObject.SetActive(!flashlight.gameObject.activeSelf);
        Audio.Sound.PlayFromResource("Flashlight", Camera.main.transform.position, aFollow: Camera.main.transform, aVolume: 0.1f);
    }
    
    public void JumpScaare(EMonsterType aType)
    {
        SceneManager.LoadScene(2);
        //stop coroutines
        //block interactions with the world,
        //disable
        //teleport camera somewhere
        //play spoooky jumpscare infront of player

        //Lowkey just load a different scene
        //hardcut to menu
    }
}

public enum EStates
{
    Work1,
    Work2,
    RinWindow,
    Work3,
    Work4,
    MimiRadar,
    NueSpot,
    MimaSeal,
    enumCount,
}

public enum EMonsterType
{
    Mima,
    Mimi,
    Rin,
    Nue,
}