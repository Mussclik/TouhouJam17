using UnityEngine;
using UnityEngine.Assertions;

public static class Extenstions
{
    // public static Vector3 ClosestPointOnLine(this Vector3 aPoint, Vector3 aStart, Vector3 aEnd)
    // {
    //     return ClosestPointOnLine(aPoint, aStart, aEnd, out float t);
    // }

    public static Vector3 ClosestPointOnLine(this Vector3 aPoint, Vector3 aStart, Vector3 aEnd, out float aT)
    {
        Vector3 difference = aEnd - aStart;
        aT = Vector3.Dot(aPoint - aStart, difference) / Vector3.Dot(difference, difference);
        aT = Mathf.Clamp01(aT);
        return aStart + aT * difference;
    }

    public static bool TryGetComponentUpwards<T>(this Transform aSelf, out T aOutComponent) where T : Component
    {
        Transform current = aSelf;
        while (current != null)
        {
            if (current.TryGetComponent(out aOutComponent))
                return true;
            current = current.parent;
        }

        aOutComponent = null;
        return false;
    }

    public static int RepeatInt(this Mathf math, int val, int top)
    {
        if (top <= 0)
        {
            Debug.LogError("top was not greater than 0!");
        }
        
        while (val > top)
        {
            val -= top;
        }

        while (val < 0)
        {
            val += top;
        }

        return val;
    }
}