using UnityEngine;

public class EndingTracker : MonoBehaviour
{
    public static EndingTracker Instance { get; private set; }

    // Each choice: true = killed, false = spared
    private bool? kingChoice = null; // Level 1
    private bool? generalChoice = null; // Level 2
    private bool? princeChoice = null; // Level 3

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // ──────────────────────────────────────────────
    // Called from each level's decision panel
    // ──────────────────────────────────────────────

    public void SetKingChoice(bool killed) { kingChoice = killed; }
    public void SetGeneralChoice(bool killed) { generalChoice = killed; }
    public void SetPrinceChoice(bool killed) { princeChoice = killed; }

    // ──────────────────────────────────────────────
    // Status checks
    // ──────────────────────────────────────────────

    public bool AllChoicesMade()
    {
        return kingChoice.HasValue && generalChoice.HasValue && princeChoice.HasValue;
    }

    public bool KingKilled => kingChoice.HasValue && kingChoice.Value;
    public bool GeneralKilled => generalChoice.HasValue && generalChoice.Value;
    public bool PrinceKilled => princeChoice.HasValue && princeChoice.Value;

    /// <summary>
    /// Returns ending index 0-7 based on kill/spare combinations.
    /// King = bit 2, General = bit 1, Prince = bit 0
    /// 0 = all spared, 7 = all killed
    /// </summary>
    public int GetEndingIndex()
    {
        int index = 0;
        if (KingKilled) index |= 4;
        if (GeneralKilled) index |= 2;
        if (PrinceKilled) index |= 1;
        return index;
    }

    // ──────────────────────────────────────────────
    // Reset (called when returning to Main Menu)
    // ──────────────────────────────────────────────

    public void ResetAllChoices()
    {
        kingChoice = null;
        generalChoice = null;
        princeChoice = null;
    }
}
