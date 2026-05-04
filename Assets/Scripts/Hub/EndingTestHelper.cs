using UnityEngine;
using UnityEngine.InputSystem;

public class EndingTestHelper : MonoBehaviour
{
    [Header("Test Choices")]
    public bool kingKilled = true;
    public bool generalKilled = true;
    public bool princeKilled = true;

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.digit1Key.wasPressedThisFrame)
            ApplyTestChoices();
    }

    [ContextMenu("Apply Test Choices Now")]
    public void ApplyTestChoices()
    {
        if (EndingTracker.Instance == null)
        {
            Debug.LogError("EndingTestHelper: EndingTracker not found!");
            return;
        }

        EndingTracker.Instance.SetKingChoice(kingKilled);
        EndingTracker.Instance.SetGeneralChoice(generalKilled);
        EndingTracker.Instance.SetPrinceChoice(princeKilled);

        int index = EndingTracker.Instance.GetEndingIndex();
        Debug.Log($"EndingTestHelper: Choices applied. Ending index = {index} " +
                  $"(King {(kingKilled ? "Killed" : "Spared")}, " +
                  $"General {(generalKilled ? "Killed" : "Spared")}, " +
                  $"Prince {(princeKilled ? "Killed" : "Spared")})");
    }
}
