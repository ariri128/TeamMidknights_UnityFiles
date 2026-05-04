using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class TimeSlowVignette : MonoBehaviour
{
    [Tooltip("Assign the Global Volume in your scene.")]
    public Volume postProcessVolume;

    [Tooltip("How intense the vignette gets at full strength. 0.35 is subtle.")]
    public float maxIntensity = 0.35f;

    [Tooltip("How fast the vignette fades in and out.")]
    public float fadeSpeed = 3f;

    private Vignette vignette;
    private float targetIntensity = 0f;

    private void Start()
    {
        if (postProcessVolume != null)
            postProcessVolume.profile.TryGet(out vignette);
    }

    private void Update()
    {
        if (vignette == null) return;

        // Smoothly animate toward target
        float current = vignette.intensity.value;
        vignette.intensity.Override(
            Mathf.Lerp(current, targetIntensity, fadeSpeed * Time.deltaTime)
        );
    }

    public void SetActive(bool active)
    {
        targetIntensity = active ? maxIntensity : 0f;
    }
}
