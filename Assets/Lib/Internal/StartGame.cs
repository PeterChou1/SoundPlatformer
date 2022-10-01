using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGame : MonoBehaviour
{
    public Animator animator;

    public Material material;

    
    void OnMouseOver()
    {
        //GetComponent<Animator>()["BackPackCloseStartTheGame"].wrapMode = WrapMode.Once;
        

        material.SetFloat("_Glossiness", 0.601f);

        if (Input.GetKey(KeyCode.Mouse0))
        {
            

            animator.Play("BackPackCloseStartTheGame");

            StartCoroutine(GameStartCoroutine());
            
            //animator.Play("FadeToBlackMainMenu"); 
            
        }
        
    }

    void OnMouseExit()
    {

        //animator.Play("BackPackOpenToIdle");

        material.SetFloat("_Glossiness", 0f);
    }

    IEnumerator GameStartCoroutine()
    {
        //Print the time of when the function is first called.
        Debug.Log("Started game");
        //animator.Play("FadeToBlackMainMenu");
        //yield on a new YieldInstruction that waits for 5 seconds.
        yield return new WaitForSeconds(5);

        //After we have waited 5 seconds print the time again.

        

        SceneManager.LoadScene("JakobDesign_Scene");
    }




}
