using System;
using System.Collections;
using System.Collections.Generic;
using Audio;
using UnityEngine;

public class BallInteractible : InteractableBase
{
    public event Action onExitBounds = () => { };
    public Vector3 startPosition;
    [SerializeField] private bool isActive = true;
    [SerializeField] private List<Collider> triggers;

    private Coroutine failCoroutine = null;

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

            if (triggers.Count == 0 && failCoroutine == null)
            {
                Debug.Log("exitedBounds");
                onExitBounds.Invoke();
                failCoroutine = StartCoroutine(Fail());
            }
        }
    }

    IEnumerator Fail()
    {
        CursorWorldInteraction.ForceHeldInteractableRelease();
        AlarmLight.PlayAlarm();
        isEnabled = false;
        
        yield return new WaitForSeconds(0.5f);
        transform.position = startPosition;
        yield return new WaitForSeconds(0.5f);

        isEnabled = true;
        failCoroutine = null;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (!triggers.Contains(other))
            triggers.Add(other);
    }

    private void OnTriggerExit(Collider other)
    {
        if (triggers.Contains(other))
            triggers.Remove(other);
    }
}