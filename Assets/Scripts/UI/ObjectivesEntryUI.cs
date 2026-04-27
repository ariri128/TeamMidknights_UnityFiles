using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ObjectivesEntryUI : MonoBehaviour
{
    public Toggle toggle;
    public TextMeshProUGUI objectiveText;

    public string objectiveName;
    public bool usesCount;
    public int requiredAmount = 1;

    private int currentAmount;
    private bool isComplete;

    private void Start()
    {
        if (toggle != null)
        {
            toggle.isOn = false;
            toggle.interactable = false;
        }

        RefreshUI();
    }

    public void AddProgress(int amount)
    {
        if (isComplete)
        {
            return;
        }

        currentAmount += amount;
        currentAmount = Mathf.Clamp(currentAmount, 0, requiredAmount);

        if (currentAmount >= requiredAmount)
        {
            isComplete = true;
        }

        RefreshUI();
    }

    public void CompleteObjective()
    {
        isComplete = true;

        if (usesCount)
        {
            currentAmount = requiredAmount;
        }

        RefreshUI();
    }

    private void RefreshUI()
    {
        if (toggle != null)
        {
            toggle.isOn = isComplete;
        }

        if (objectiveText != null)
        {
            if (usesCount)
            {
                objectiveText.text = objectiveName + ": " + currentAmount + "/" + requiredAmount;
            }
            else
            {
                objectiveText.text = objectiveName;
            }

            objectiveText.color = isComplete ? Color.gray : Color.black;
        }
    }
}
