using UnityEngine;
using UnityEngine.UI;

public class PlayerResourceUI : MonoBehaviour
{
    [Header("References")]
    public PlayerHealth playerHealth;
    public PlayerMana playerMana;

    [Header("HP UI")]
    public RectTransform hpFillMask;
    public RectTransform hpFillFullWidthReference;
    public float hpSmoothSpeed = 250f;

    [Header("Mana UI")]
    public Image manaFillImage;
    public float manaSmoothSpeed = 2.5f;

    private float targetHPPercent = 1f;
    private float displayedHPPercent = 1f;

    private float targetManaPercent = 1f;
    private float displayedManaPercent = 1f;

    private float fullHPWidth;

    private void Start()
    {
        if (hpFillFullWidthReference != null)
        {
            fullHPWidth = hpFillFullWidthReference.rect.width;
        }
        else if (hpFillMask != null)
        {
            fullHPWidth = hpFillMask.rect.width;
        }

        if (playerHealth != null)
        {
            targetHPPercent = (float)playerHealth.CurrentHP / playerHealth.MaxHP;
            displayedHPPercent = targetHPPercent;
        }

        if (playerMana != null)
        {
            targetManaPercent = (float)playerMana.CurrentMana / playerMana.MaxMana;
            displayedManaPercent = targetManaPercent;
        }

        UpdateHPBarImmediate();
        UpdateManaImmediate();
    }

    private void Update()
    {
        if (playerHealth != null)
        {
            targetHPPercent = (float)playerHealth.CurrentHP / playerHealth.MaxHP;
        }

        if (playerMana != null)
        {
            targetManaPercent = (float)playerMana.CurrentMana / playerMana.MaxMana;
        }

        displayedHPPercent = Mathf.MoveTowards(
            displayedHPPercent,
            targetHPPercent,
            hpSmoothSpeed * Time.deltaTime / Mathf.Max(fullHPWidth, 1f)
        );

        displayedManaPercent = Mathf.MoveTowards(
            displayedManaPercent,
            targetManaPercent,
            manaSmoothSpeed * Time.deltaTime
        );

        UpdateHPBarVisual();
        UpdateManaVisual();
    }

    private void UpdateHPBarImmediate()
    {
        if (hpFillMask == null)
        {
            return;
        }

        float width = fullHPWidth * displayedHPPercent;
        hpFillMask.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
    }

    private void UpdateManaImmediate()
    {
        if (manaFillImage == null)
        {
            return;
        }

        manaFillImage.fillAmount = displayedManaPercent;
    }

    private void UpdateHPBarVisual()
    {
        if (hpFillMask == null)
        {
            return;
        }

        float width = fullHPWidth * displayedHPPercent;
        hpFillMask.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
    }

    private void UpdateManaVisual()
    {
        if (manaFillImage == null)
        {
            return;
        }

        manaFillImage.fillAmount = displayedManaPercent;
    }
}
