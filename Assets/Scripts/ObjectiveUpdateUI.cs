using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class ObjectiveUpdateUI : MonoBehaviour
{
    public static ObjectiveUpdateUI Instance;

    public TextMeshProUGUI updateText;
    public float messageDuration = 2.5f;

    private Queue<string> messageQueue = new Queue<string>();
    private bool isShowingMessage = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        if (updateText != null)
        {
            updateText.text = "";
        }
    }

    public void ShowMessage(string message)
    {
        if (updateText == null || string.IsNullOrEmpty(message))
        {
            return;
        }

        messageQueue.Enqueue(message);

        if (!isShowingMessage)
        {
            StartCoroutine(ProcessQueue());
        }
    }

    private IEnumerator ProcessQueue()
    {
        isShowingMessage = true;

        while (messageQueue.Count > 0)
        {
            string message = messageQueue.Dequeue();
            updateText.text = message;

            yield return new WaitForSeconds(messageDuration);
        }

        updateText.text = "";
        isShowingMessage = false;
    }
}
