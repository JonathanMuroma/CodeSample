using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrintCombineFailMessage : MonoBehaviour
{
    private float printTextTimer = 0.0f; //Timer, which keeps count how long the message has been active.
    [SerializeField]
    private float printTextTimeOnScreen = 2.0f; //The time amount we want to print the message.
    private bool coroutineIsRunning = false; //Boolen which is used to check if the coroutine is running at the moment.
    private GameObject combineFailMessageHolder = null; //Holds the CombineFailMessagePanel, which is used to set it active etc.

    void Start()
    {
        combineFailMessageHolder = this.transform.Find("CombineFailMessagePanel").gameObject; //We find through code the CombineFailMessagePanel.
        if (combineFailMessageHolder == null) //We check the CombineFailMessagePanel was found correctly.
            Debug.Log("CombineFailMessageHolder was not found...");
    }

    private IEnumerator PrintCombiningMessageFailed() //Coroutine, which handles how long the message is on screen.
    {
        coroutineIsRunning = true; //We set our boolean to true, meaning coroutine is running now.
        while (printTextTimer < printTextTimeOnScreen) //We make a timer check.
        {
            printTextTimer++; //We add to our timer.
            Debug.Log("Coroutine running...");
            yield return new WaitForSeconds(1); //We tell our coroutine to wait for a second.
        }

        //Here we reset all the variables back to normal state and make the message disappear.
        coroutineIsRunning = false;
        combineFailMessageHolder.SetActive(false);
        printTextTimer = 0.0f;
    }

    public void StartPrintingMessage() //Method used to start printing the CombineFailMessage text.
    {
        if (!coroutineIsRunning) //We check if the coroutine is running at the moment. If not, we start it up, but if yes, we just reset the timer.
        {
            combineFailMessageHolder.SetActive(true); //We set the CombineFailMessagePanel as active.
            StartCoroutine(PrintCombiningMessageFailed()); //We start the coroutine, which handles the time how long the message is on screen.
        } else
            printTextTimer = 0.0f; //Here we simply restart the timer.
    }

    public void EndPrintingMessage()
    {
        StopCoroutine(PrintCombiningMessageFailed());
        coroutineIsRunning = false;
        combineFailMessageHolder.SetActive(false);
        printTextTimer = 0.0f;
    }

    public bool GetCoroutineIsRunning()
    {
        return coroutineIsRunning;
    }
}
