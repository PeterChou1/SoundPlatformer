using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WavingTrigger2 : MonoBehaviour
{
    public Animator animator;

    public TMP_Text popUpText;

    public Animator cameraAnimator;

    
    private void OnTriggerEnter(Collider other)
    {

        

        if (other.CompareTag("Player"))
        {
            animator.Play("Mother_Hugging");

            popUpText.enabled = true;
            popUpText.text = "Darling, I missed you so much.";
        }

        if (other.CompareTag("Player"))                   //Input.GetKeyDown(KeyCode.E)
        {
            cameraAnimator.Play("Hug Camera");

            StartCoroutine(GameQuitCoroutine());
            /*popUpText.enabled = false;
            popUpText.text = "";*/



        }
    }

    IEnumerator GameQuitCoroutine()
    {
        //Print the time of when the function is first called.
        Debug.Log("Started game quit");

        //yield on a new YieldInstruction that waits for 5 seconds.
        yield return new WaitForSeconds(10);

        //After we have waited 5 seconds print the time again.
        Application.Quit();
    }


}
