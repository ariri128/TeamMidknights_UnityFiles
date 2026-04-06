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

    /*
    public static ObjectiveUpdateUI Instance;

    public TextMeshProUGUI updateText;
    public float messageDuration = 2.5f;
    public float minimumMessageTime = 0.6f;

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

            float timer = 0f;

            while (timer < messageDuration)
            {
                timer += Time.deltaTime;

                if (timer >= minimumMessageTime && messageQueue.Count > 0)
                {
                    break;
                }

                yield return null;
            }
        }

        updateText.text = "";
        isShowingMessage = false;
    }
    */

    /*
    public static ObjectiveUpdateUI Instance;

    public TextMeshProUGUI updateText;
    public float messageDuration = 2.5f;

    private Coroutine currentRoutine;

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
        if (updateText == null)
        {
            return;
        }

        if (currentRoutine != null)
        {
            StopCoroutine(currentRoutine);
        }

        currentRoutine = StartCoroutine(ShowMessageRoutine(message));
    }

    private IEnumerator ShowMessageRoutine(string message)
    {
        updateText.text = message;
        yield return new WaitForSeconds(messageDuration);
        updateText.text = "";
        currentRoutine = null;
    }
    */
}
