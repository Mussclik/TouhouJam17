using UnityEngine;

public class WorldToUI : MonoBehaviour
{
    public UIInteractSymbol interactSymbol;
    public static UIInteractSymbol InteractSymbol => Instance.interactSymbol;

    [SerializeField] private RectTransform mainCanvas;
    private static WorldToUI Instance = null;

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

    public static void SetNullsToDefault(ref Camera aCamera, ref RectTransform aCanvasRoot)
    {
        if (!aCamera)
        {
            aCamera = Camera.main;
        }

        if (!aCanvasRoot)
        {
            aCanvasRoot = Instance.mainCanvas;
        }
    }

    private static bool WorldToCanvas(Camera aCamera, Vector3 aWorldPos, RectTransform aCanvasRect, out Vector2 aCanvasLocalPos)
    {
        aCanvasLocalPos = default;
        if (!WorldToScreen(aCamera, aWorldPos, out Vector2 screenPos))
            return false;

        Canvas canvas = aCanvasRect.GetComponentInParent<Canvas>();
        Camera canvasCam = (canvas != null && canvas.renderMode == RenderMode.ScreenSpaceOverlay)
            ? null
            : (canvas != null ? canvas.worldCamera : aCamera);

        return RectTransformUtility.ScreenPointToLocalPointInRectangle(
            aCanvasRect, screenPos, canvasCam, out aCanvasLocalPos);
    }

    private static bool WorldToScreen(Camera aCamera, Vector3 aWorldPos, out Vector2 aScreenPos)
    {
        Vector3 sp = aCamera.WorldToScreenPoint(aWorldPos);
        if (sp.z <= 0f)
        {
            aScreenPos = default;
            return false;
        }

        aScreenPos = new Vector2(sp.x, sp.y);
        return true;
    }

    public static void PlaceUIOverWorld(
        Vector3 aWorldPos,
        RectTransform aUIElement,
        Camera aCamera = null,
        RectTransform aCanvasRect = null)
    {
        SetNullsToDefault(ref aCamera, ref aCanvasRect);
        if (aUIElement == null || aCamera == null || aCanvasRect == null)
            return;
        
        Vector3 vp = aCamera.WorldToViewportPoint(aWorldPos);
        if (vp.z <= 0f)
        {
            if (aUIElement.gameObject.activeSelf) aUIElement.gameObject.SetActive(false);
            return;
        }

        if (!aUIElement.gameObject.activeSelf) 
            aUIElement.gameObject.SetActive(true);

        Canvas canvas = aCanvasRect.GetComponentInParent<Canvas>();
        if (canvas == null) return;

        Rect cr = aCanvasRect.rect;
        Vector2 canvasLocal = new Vector2
        (
            (vp.x - aCanvasRect.pivot.x) * cr.width,
            (vp.y - aCanvasRect.pivot.y) * cr.height
        );

        RectTransform parentRect = aUIElement.parent as RectTransform;
        if (parentRect == null || parentRect == aCanvasRect)
        {
            aUIElement.localPosition = new Vector3(
                canvasLocal.x, canvasLocal.y, aUIElement.localPosition.z);
        }
        else
        {
            Vector3 worldPt = aCanvasRect.TransformPoint(
                new Vector3(canvasLocal.x, canvasLocal.y, 0f));
            Vector3 parentLocal = parentRect.InverseTransformPoint(worldPt);
            aUIElement.localPosition = new Vector3(
                parentLocal.x, parentLocal.y, aUIElement.localPosition.z);
        }
    }
}