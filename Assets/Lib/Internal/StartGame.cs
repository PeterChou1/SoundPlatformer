using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGame : MonoBehaviour
{
    public Animator animator;

    


    void OnMouseOver()
    {
        //GetComponent<Animator>()["BackPackCloseStartTheGame"].wrapMode = WrapMode.Once;
        animator.Play("BackPackCloseStartTheGame");
        StartCoroutine(GameStartCoroutine());
    }

    /*void OnMouseExit()
    {
        
        animator.Play("BackPackOpenToIdle");
    }*/

    IEnumerator GameStartCoroutine()
    {
        //Print the time of when the function is first called.
        Debug.Log("Started game");

        //yield on a new YieldInstruction that waits for 5 seconds.
        yield return new WaitForSeconds(5);

        //After we have waited 5 seconds print the time again.

        

        SceneManager.LoadScene("JakobDesign_Scene");
    }
}
