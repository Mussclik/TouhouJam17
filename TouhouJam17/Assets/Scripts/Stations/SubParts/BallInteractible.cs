using System;
using System.Collections;
using System.Collections.Generic;
using Audio;
using UnityEngine;

public class BallInteractible : InteractableBase
{
    public event Action onExitBounds = () => { };
    public event Action onWinEnter = () => { };
    public Vector3 startPosition;
    [SerializeField] private bool isActive = true;
    [SerializeField] private List<Collider> triggers;

    private Coroutine myActiveCoroutine = null;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        if (isActive == true && isEnabled)
        {
            if (isHeld)
            {
                Vector3 newPos;
                Plane ballPlane = new Plane(transform.forward, transform.position);
                CursorWorldInteraction.CursorToPlane(Camera.main, ballPlane, out newPos);

                transform.position = newPos;
                //clamp position just in case :/ // Nuh uh :3
            }

            if (triggers.Count == 0 && myActiveCoroutine == null)
            {
                Debug.Log("exitedBounds");
                onExitBounds.Invoke();
                myActiveCoroutine = StartCoroutine(Fail());
            }
        }
    }

    private IEnumerator Win()
    {
        CursorWorldInteraction.ForceHeldInteractableRelease();
        isEnabled = false;
        
        yield return new WaitForSeconds(0.5f);
        transform.position = startPosition;
        yield return new WaitForSeconds(0.5f);

        onWinEnter.Invoke();
        isEnabled = true;
        myActiveCoroutine = null;
    }
    
    private IEnumerator Fail()
    {
        CursorWorldInteraction.ForceHeldInteractableRelease();
        AlarmLight.PlayAlarm(AlarmType.MazeFail);
        isEnabled = false;
        
        yield return new WaitForSeconds(0.5f);
        transform.position = startPosition;
        yield return new WaitForSeconds(0.5f);

        isEnabled = true;
        myActiveCoroutine = null;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (!triggers.Contains(other))
            triggers.Add(other);

        if (other.CompareTag("MazeWin"))
        {
            onWinEnter.Invoke();
            myActiveCoroutine = StartCoroutine(Win());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (triggers.Contains(other))
            triggers.Remove(other);
    }
}