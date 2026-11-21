using UnityEngine;
using UnityEngine.InputSystem;

public class CursorWorldInteraction : MonoBehaviour
{
    public static CursorWorldInteraction Instance = null;
    [SerializeField] private bool usePlane = false;
    [SerializeField] private Ray triedRay = default;
    [SerializeField] private Camera defaultChecker;

    [Header("StaticStuff")] [SerializeField]
    private InteractableBase currentDownInteractible = null;

    [SerializeField] private InteractableBase currentHoveredInteractible = null;
    [SerializeField] private LayerMask interactibleLayerMask;

    private Vector3? lastHitArea = null;

    private void Start()
    {
        if (!Instance)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Debug.Log("deleted self :3");
            Destroy(this);
        }
    }

    private void Update()
    {
        Vector3 newPos = default;
        if (defaultChecker)
        {
            if (usePlane)
            {
                triedRay = default;
                CursorToPlane(defaultChecker, defaultChecker.farClipPlane, out newPos);
            }
            else
            {
                CursorToWorld(defaultChecker, interactibleLayerMask, out newPos);
            }
        }

        if (newPos != default)
        {
            lastHitArea = newPos;
        }
        else
        {
            lastHitArea = null;
        }
    }

    public static bool CursorToPlane(Camera aCamera, float aDistance, out Vector3 aOutWorldPoint)
    {
        if (Mouse.current == null)
        {
            Debug.LogError("Unable to find mouse!", Instance.gameObject);
            aOutWorldPoint = default;
            return false;
        }

        Ray ray = aCamera.ScreenPointToRay(Mouse.current.position.ReadValue() * 0.25f);

        Vector3 forward = aCamera.transform.forward;
        Vector3 planePoint = aCamera.transform.position + (forward * aDistance);
        Plane plane = new Plane(-forward, planePoint);

        if (plane.Raycast(ray, out float enter) && enter >= 0f)
        {
            aOutWorldPoint = ray.GetPoint(enter);
            return true;
        }

        aOutWorldPoint = default;
        return false;
    }

    public static bool CursorToWorld(Camera aCamera, LayerMask aMask, out Vector3 aOutWorldPoint)
    {
        if (Mouse.current == null)
        {
            Debug.LogError("Unable to find mouse!", Instance.gameObject);
            aOutWorldPoint = default;
            return false;
        }

        Ray ray = aCamera.ScreenPointToRay(Mouse.current.position.ReadValue() * 0.25f);
        Instance.triedRay = ray;
        bool didHit = Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, aMask);

        InteractableBase hitInteractible = null;

        if (didHit)
        {
            aOutWorldPoint = hit.point;

            if ((Instance.interactibleLayerMask & (1 << hit.collider.gameObject.layer)) != 0)
                hit.collider.transform.TryGetComponentUpwards(out hitInteractible);
        }
        else
        {
            aOutWorldPoint = default;
        }

        HandleMouseButtons(hitInteractible);

        // only when not holding something down
        if (Instance.currentDownInteractible == null)
            SetHovered(hitInteractible);
        else
        {
            WorldToUI.InteractSymbol.gameObject.SetActive(false);
        }


        return didHit;
    }

    public static bool WorldToCamera(Camera aCamera, Vector3 aWorldPoint, out Vector2 aCameraPixelPos)
    {
        Vector3 viewport = aCamera.WorldToViewportPoint(aWorldPoint);

        if (viewport.z <= 0f)
        {
            aCameraPixelPos = default;
            return false;
        }

        aCameraPixelPos = new Vector2(
            viewport.x * aCamera.pixelWidth,
            viewport.y * aCamera.pixelHeight);

        return true;
    }

    private static void HandleMouseButtons(InteractableBase aHitInteractible)
    {
        if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            if (Instance.currentDownInteractible != null)
            {
                Instance.currentDownInteractible.OnPointerUp();
                Instance.currentDownInteractible = null;
            }

            return;
        }

        if (Mouse.current.leftButton.wasPressedThisFrame && aHitInteractible != null)
        {
            if (Instance.currentDownInteractible != null)
                Instance.currentDownInteractible.OnPointerUp();

            Instance.currentDownInteractible = aHitInteractible;
            Instance.currentDownInteractible.OnPointerDown();
        }
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private static void SetHovered(InteractableBase aNewHover)
    {
        if (Instance.currentHoveredInteractible == aNewHover)
        {
            return; 
        }

        if (Instance.currentHoveredInteractible != null)
        {
            Instance.currentHoveredInteractible.OnPointerExit();
        }

        Instance.currentHoveredInteractible = aNewHover;

        if (aNewHover != null)
        {
            WorldToUI.InteractSymbol.gameObject.SetActive(false);
            aNewHover.OnPointerEnter();
            WorldToUI.PlaceUIOverWorld(aNewHover.transform.position, WorldToUI.InteractSymbol.gameObject.GetComponent<RectTransform>());
            WorldToUI.InteractSymbol.gameObject.SetActive(true);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        if (lastHitArea != null)
        {
            Gizmos.DrawWireSphere((Vector3)lastHitArea, 0.2f);
            Gizmos.DrawLine(defaultChecker.transform.position, (Vector3)lastHitArea);
//            Debug.Log(lastHitArea);
        }

        if (!usePlane)
        {
            Gizmos.DrawRay(triedRay);
        }
    }
}