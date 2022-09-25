using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WavingTrigger : MonoBehaviour
{
    public Animator animator;
    public Animator animator2;

    public bool Once = true;
    private void OnTriggerEnter(Collider other)
    {
        
        if (other.CompareTag("Player") && Once)
        {
            animator.Play("BusDriver_Waving");
            Once = false;
        }

        if (other.CompareTag("Player") && Once)
        {
            animator2.Play("Mother_Hugging");
            Once = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            animator.Play("BusDriver_IdleDriving");
        }

        if (other.CompareTag("Player"))
        {
            animator2.Play("Mother_Idle");
        }
    }
}
