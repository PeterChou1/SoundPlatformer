using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WavingTrigger : MonoBehaviour
{
    public Animator animator;
    

    public bool Once = true;
    private void OnTriggerEnter(Collider other)
    {
        
        if (other.CompareTag("Player") && Once)
        {
            animator.Play("BusDriver_Waving");
            Once = false;
        }

       
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            animator.Play("BusDriver_IdleDriving");
        }

        
    }
}
