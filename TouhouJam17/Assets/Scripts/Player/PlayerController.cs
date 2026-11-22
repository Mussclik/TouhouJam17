using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private BaseStation[] stations = new BaseStation[(int)EStates.enumCount];

    [Header("Rotation")] [SerializeField] private Transform baseCameraPos;
    [SerializeField] private Transform playerRotation;
    [SerializeField] private float rotationLength = 45f;
    [SerializeField] private AnimationCurve rotationCurve;
    [SerializeField] private float rotationTime = 0.3f;
    [SerializeField] private EStates currentState;


    [Header("cameraMoving")] [SerializeField]
    private AnimationCurve cameraMoveCurve;

    [SerializeField] private float cameraMoveTime = 0.3f;

    private Coroutine myMovingCoroutine = null;
    private bool isFocused;
    private BaseStation CurrentStation => stations[(int)currentState];

    #region InputActions
    [Header("InputActions")]
    [SerializeField] private InputActionReference upRef;
    [SerializeField] private InputActionReference backRef;
    [SerializeField] private InputActionReference leftRef;
    [SerializeField] private InputActionReference rightRef;

    private void OnEnable()
    {
        upRef.action.performed += GoUp;
        backRef.action.performed += GoDown;
        leftRef.action.performed += GoLeft;
        rightRef.action.performed += GoRight;
    }

    private void OnDisable()
    {
        upRef.action.performed -= GoUp;
        backRef.action.performed -= GoDown;
        leftRef.action.performed -= GoLeft;
        rightRef.action.performed -= GoRight;
    }

    #endregion

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

    private void GoUp(InputAction.CallbackContext cxt)
    {
        if (CurrentStation.canFocus && myMovingCoroutine == null && isFocused == false)
        {
            StartCoroutine(MoveCamera(Camera.main.transform, CurrentStation.IdealCameraPos, CurrentStation.IdealCameraPos));
            isFocused = true;
        }
    }

    private void GoDown(InputAction.CallbackContext cxt)
    {
        if (myMovingCoroutine == null && isFocused == true)
        {
            StartCoroutine(MoveCamera(Camera.main.transform, baseCameraPos, baseCameraPos));
            isFocused = false;
        }
    }

    private void GoLeft(InputAction.CallbackContext cxt)
    {
        if (myMovingCoroutine == null && isFocused == false)
        {
            myMovingCoroutine = StartCoroutine(RotateBody(false));
            --currentState;
            currentState = (EStates)Mathf.Repeat((int)currentState, (int)EStates.enumCount);
        }

    }

    private void GoRight(InputAction.CallbackContext cxt)
    {
        if (myMovingCoroutine == null && isFocused == false)
        {
            myMovingCoroutine = StartCoroutine(RotateBody(true));
            ++currentState;
            currentState = (EStates)Mathf.Repeat((int)currentState, (int)EStates.enumCount);
        }
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