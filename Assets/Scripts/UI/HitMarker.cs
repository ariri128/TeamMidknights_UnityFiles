using UnityEngine;
using UnityEngine.InputSystem;

public class HitMarker : MonoBehaviour
{
    public RectTransform hitMarkerRect;

    public Vector2 screenOffset = new Vector2(120f, 20f);

    private void Start()
    {
        ApplyOffset();
    }

    private void OnEnable()
    {
        ApplyOffset();
    }

    private void OnValidate()
    {
        ApplyOffset();
    }

    public void ApplyOffset()
    {
        if (hitMarkerRect == null)
        {
            return;
        }

        hitMarkerRect.anchorMin = new Vector2(0.5f, 0.5f);
        hitMarkerRect.anchorMax = new Vector2(0.5f, 0.5f);
        hitMarkerRect.pivot = new Vector2(0.5f, 0.5f);
        hitMarkerRect.anchoredPosition = screenOffset;
    }

    public Vector2 GetHitMarkerScreenPosition()
    {
        return new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
    }
}
