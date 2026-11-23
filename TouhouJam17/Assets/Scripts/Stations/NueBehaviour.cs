using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class NueBehaviour : BaseStation
{
    [Header("transforms")] [SerializeField]
    private Transform door;

    [SerializeField] private Transform doorOpen;
    [SerializeField] private Transform doorClosed;
    [SerializeField] private Transform nueSprite;
    [SerializeField] private Transform mimaSprite;

    [Header("attacking Vars")] EMonsterType whoAttacking;
    [SerializeField] private float time = 0f;
    [SerializeField] private float timeToEnter = 40f;
    [SerializeField] private float timeToAttack = 10f;
    [SerializeField] private float enterChacne = 0.25f;
    [SerializeField] private float attackChacne = 0.85f;
    [SerializeField] private bool isInsideOffice = false;
    [SerializeField] private float timeHide = 0f;
    [SerializeField] private float timeToHide = 1f;
    [SerializeField] private float flashlightTriggerDistance = 10f;

    [Header("door Vars")]
    [SerializeField] private AnimationCurve doorMoveCurve;
    [SerializeField] private float doorMoveTime = 0.5f;

    private void Start()
    {
        nueSprite.gameObject.SetActive(false);
        mimaSprite.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (!isInsideOffice)
        {
            if (!PlayerLookingNearDoor())
            {
                WaitForEnter();
            }
        }
        else
        {
            if (whoAttacking == EMonsterType.Nue)
            {
                if (!PlayerIsHiding())
                {
                    bool willAttackFaster = false;
                    if (PlayerShiningFlashlight()) willAttackFaster = true;

                    WaitForAttack(willAttackFaster);
                }
                else
                {
                    WaitForPlayerRepelling();
                }
            }
            else //mima
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
        }
    }

    private void WaitForPlayerRepelling()
    {
        timeHide += Time.deltaTime;
        if (timeHide >= timeToHide)
        {
            isInsideOffice = false;
            StartCoroutine(MoveDoor(true));
            SetSpriteState(whoAttacking, false);
            Audio.Sound.PlayFromResource("Door", door.position, aPitchShift: false);
            timeHide = 0f;
        }
    }

    private void WaitForEnter()
    {
        time += Time.deltaTime;
        if (time > timeToEnter)
        {
            float random = Random.Range(0f, 1f);
            if (random <= enterChacne)
            {
                int type = Random.Range(0, 1 + 1);
                if (type == 0)
                {
                    whoAttacking = EMonsterType.Nue;
                }
                else
                {
                    whoAttacking = EMonsterType.Mima;
                }

                isInsideOffice = true;
                StartCoroutine(MoveDoor(false));
                SetSpriteState(whoAttacking, true);
                Audio.Sound.PlayFromResource("Door", door.position, aPitchShift: false);
                timeHide = 0f;
            }

            time = 0f;
        }
    }

    private void WaitForAttack(bool aIsFaster = false)
    {
        time += Time.deltaTime;
        if (time > timeToAttack)
        {
            float random = Random.Range(0f, 1f);
            if (random <= attackChacne)
            {
                timeHide = 0f;
                PlayerController.Instance.JumpScaare(whoAttacking);
            }

            time = 0f;
        }
    }

    private IEnumerator MoveDoor(bool aWillClose)
    {
        float t = 0f;
        Vector3 start = door.position;
        Vector3 end;

        if (aWillClose) end = doorClosed.position;
        else end = doorOpen.position;

        
        
        while (t < doorMoveTime)
        {
            t += Time.deltaTime;
            door.position = Vector3.Lerp(start, end, doorMoveCurve.Evaluate(t / doorMoveTime));
            yield return null;
        }

        door.position = end;
    }

    private bool PlayerShiningFlashlight()
    {
        if (PlayerController.Instance.IsFlashlightEnabled)
        {
            Vector3 normal = (Camera.main.transform.position - nueSprite.position).normalized;
            Plane plane = new Plane(normal, nueSprite.position);
            if (CursorWorldInteraction.CursorToPlane(Camera.main, plane, out Vector3 planePoint))
            {
                if (Vector3.Distance(nueSprite.position, planePoint) < flashlightTriggerDistance)
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
        int nueSpot = (int)EStates.NueSpot;
        int currentPlayerSpot = (int)PlayerController.Instance.currentState;

        if (currentPlayerSpot >= nueSpot - 1 && currentPlayerSpot <= nueSpot + 1) return true;
        return false;
    }

    private void SetSpriteState(EMonsterType aType, bool aSpriteState)
    {
        if (aType == EMonsterType.Nue)
        {
            nueSprite.gameObject.SetActive(aSpriteState);
        }
        else
        {
            mimaSprite.gameObject.SetActive(aSpriteState);
        }
    }
    
}