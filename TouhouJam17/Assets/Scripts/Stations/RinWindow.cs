using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = Unity.Mathematics.Random;

public class RinWindow : BaseStation
{
    [SerializeField] private List<Bush> myBushlist;

    [Header("transforms")] [SerializeField]
    private int currentBushIndex = 0;

    [SerializeField] private Transform rinSprite;

    [SerializeField] private Light playerSuperLight;
    [SerializeField] private float time = 0f;
    [SerializeField] private float timeToProceed = 40f;
    [SerializeField] private float timeToAttack = 10f;
    [SerializeField] private float proceedChance = 0.25f;
    [SerializeField] private float attackChacne = 0.85f;
    [SerializeField] private float timeFlashed = 0f;
    [SerializeField] private float timeToFlash = 1f;
    [SerializeField] private float flashlightTriggerDistance = 2f;

    [SerializeField] private AnimationCurve rinMoveCurve;
    [SerializeField] private float rinMoveTime = 0.5f;
    private Coroutine moveCoroutine = null;

    private bool isKill = false;

    private bool IsOnLastBush => currentBushIndex == (myBushlist.Count - 1);

    private void Start()
    {
        rinSprite.gameObject.SetActive(true);
    }

    private void Update()
    {
        if (isKill)
        {
            if (moveCoroutine == null)
            {
                PlayerController.Instance.JumpScaare(EMonsterType.Rin);
            }
        }
        else if (!IsOnLastBush)
        {
            if (!PlayerLookingNearDoor() && !PlayerShiningFlashlight())
            {
                WaitForProceed();
            }
            else
            {
                WaitForPlayerRepelling();
            }
        }
        else
        {
            if (!PlayerShiningFlashlight())
            {
                WaitForAttack();
            }
            else
            {
                WaitForPlayerRepelling();
            }
        }

        EnablePlayerSuperFlashlight();
    }

    private void WaitForPlayerRepelling()
    {
        if (currentBushIndex > 0)
        {
            timeFlashed += Time.deltaTime;
            if (timeFlashed >= timeToFlash)
            {
                moveCoroutine = StartCoroutine(MoveRin(myBushlist[currentBushIndex - 1].rinLocation.position));
                --currentBushIndex;
                PlayFoliage();
                timeFlashed = 0f;
            }
        }
    }

    private void WaitForProceed()
    {
        time += Time.deltaTime;
        if (time > timeToProceed)
        {
            float random = UnityEngine.Random.Range(0f, 1f);
            if (random <= proceedChance)
            {
                moveCoroutine = StartCoroutine(MoveRin(myBushlist[currentBushIndex + 1].rinLocation.position));
                ++currentBushIndex;
                PlayFoliage();
                timeFlashed = 0f;
            }

            time = 0f;
        }
    }

    private void WaitForAttack()
    {
        time += Time.deltaTime;
        if (time > timeToAttack)
        {
            float random = UnityEngine.Random.Range(0f, 1f);
            if (random <= attackChacne)
            {
                Vector3 halfToPlayer = Vector3.Lerp(rinSprite.position, PlayerController.Instance.transform.position, 0.5f);
                moveCoroutine = StartCoroutine(MoveRin(halfToPlayer));
                isKill = true;
            }

            time = 0f;
        }
    }

    private IEnumerator MoveRin(Vector3 aPosition)
    {
        float t = 0f;
        Vector3 start = myBushlist[currentBushIndex].rinLocation.position;
        start.y = rinSprite.transform.position.y;
        Vector3 end = aPosition;
        end.y =  rinSprite.transform.position.y;

        while (t < rinMoveTime)
        {
            t += Time.deltaTime;
            rinSprite.position = Vector3.Lerp(start, end, rinMoveCurve.Evaluate(t / rinMoveTime));
            yield return null;
        }

        rinSprite.position = end;
    }


    private bool PlayerShiningFlashlight()
    {
        if (PlayerController.Instance.IsFlashlightEnabled)
        {
            Vector3 normal = (Camera.main.transform.position - myBushlist[currentBushIndex].transform.position).normalized;
            Plane plane = new Plane(normal, myBushlist[currentBushIndex].transform.position);
            if (CursorWorldInteraction.CursorToPlane(Camera.main, plane, out Vector3 planePoint))
            {
                if (Vector3.Distance(myBushlist[currentBushIndex].transform.position, planePoint) < flashlightTriggerDistance)
                {
                    return true;
                }
            }
        }

        return false;
    }

    private bool PlayerIsHiding()
    {
        return PlayerController.Instance.currentState == EStates.NueSpot && PlayerController.Instance.isFocused == true;
    }

    private bool PlayerLookingNearDoor()
    {
        int rinSpot = (int)EStates.RinWindow;
        int currentPlayerSpot = (int)PlayerController.Instance.currentState;

        if (currentPlayerSpot == rinSpot) return true;
        return false;
    }

    private void EnablePlayerSuperFlashlight()
    {
        if (PlayerController.Instance.currentState == EStates.RinWindow && PlayerController.Instance.isFocused)
        {
            playerSuperLight.gameObject.SetActive(true);
        }
        else
        {
            playerSuperLight.gameObject.SetActive(false);
        }
    }

    private void PlayFoliage()
    {
        string[] foliage = new string[]
        {
            "Foliage1",
            "Foliage2",
            "Foliage3",
        };
        int i = UnityEngine.Random.Range(0, foliage.Length);
        string selected = foliage[i];
        Audio.Sound.PlayFromResource(selected, rinSprite.position, aFollow: rinSprite);
    }
}