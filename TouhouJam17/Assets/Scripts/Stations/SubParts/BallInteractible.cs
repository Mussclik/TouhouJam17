using System;
using System.Collections.Generic;
using UnityEngine;

public class BallInteractible : InteractableBase
{
    private bool isActive = true;
    private bool blockInput = false;
    private float startLocalZ = 0;
    [SerializeField] private List<Collider> triggers;

    void Start()
    {
        startLocalZ = transform.localPosition.z;
    }

    void Update()
    {
        if (isActive)
        {
            if (isHeld)
            {
                Vector3 newPos;
                Plane ballPlane = new Plane(transform.forward, transform.position);
                CursorWorldInteraction.CursorToPlane(Camera.main, ballPlane, out newPos);

                transform.position = newPos;
                //clamp position just in case :/
            }

            if (triggers.Count < 0)
            {
                Debug.Log("Do stuff");
            }
        }
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

    public override void OnPointerDown()
    {
        base.OnPointerDown();
    }

    public override void OnPointerUp()
    {
        base.OnPointerUp();
    }

    public override void OnPointerEnter()
    {
        base.OnPointerEnter();
    }

    public override void OnPointerExit()
    {
        base.OnPointerExit();
    }
}