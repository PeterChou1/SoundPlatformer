using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Movement : MonoBehaviour
{
    public float speed = 2f;
    public Camera Cam2D,Cam3D;
    public Animator animator;
    
    private bool sit = true;
    private bool play = true;
    public TMP_Text popUpText;
    public Image popUpBg;
    private void Start()
    {
        Cam3D.enabled = true;
        Cam2D.enabled = false;
        popUpText.enabled = false;
        popUpBg.enabled = false;
    }

    void Update()
    {
       float x = Input.GetAxis("Horizontal");
       float z = Input.GetAxis("Vertical");
       // Vector3 move = transform.right * x + transform.forward * z;
        
        if (Input.GetKey(KeyCode.LeftShift)){
            transform.Translate(x* speed * Time.deltaTime, 0f,z * speed* Time.deltaTime);
            //controller.Move(move * speed * 2 * Time.deltaTime);
        }
        else
        {
            transform.Translate(Input.GetAxis("Horizontal") * speed * Time.deltaTime, 0f, Input.GetAxis("Vertical") * speed * Time.deltaTime);
            //controller.Move(move * speed *  Time.deltaTime);
        }
        if (Input.GetMouseButtonDown(1))
        {
            // trigger click animation
            animator.SetBool("isPlaying", play);
            if (play == false)
            {
                popUpText.text = "You left the game";
            }
        


            StartCoroutine(waiter());
            
            play = !play;
            // switch to 2D
            Cam3D.enabled = !Cam3D.enabled;
            Cam2D.enabled = !Cam2D.enabled;

            // start game
        }
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.tag == "seat")
        {
           
            if (Input.GetMouseButtonDown(0))
            {
                
                // trigger sit animation
                animator.SetBool("isSitting", sit);
                popUpText.text = "You are sitting";
                StartCoroutine(waiter());

                //message pop up
                popUpText.text = "You are sitting";
                StartCoroutine(waiter());
                //Debug.Log("howdy");
                sit = !sit;
            }
        }
        if (other.gameObject.tag == "driver")
        {
            if (Input.GetMouseButton(0))
            {
                // trigger wave animation
                animator.SetBool("isWaving", true);
                popUpText.text = "You are waving";
                StartCoroutine(waiter());
            }
            /*if (Input.GetMouseButtonUp(0))
            {
                animator.SetBool("isWaving", false);
            }*/
        }
        if (other.gameObject.tag == "door")
        {
            if (Input.GetMouseButtonDown(0))
            {
                // trigger leave animation
                animator.SetBool("isLeaving", true);
                popUpText.text = "You left the bus";
                StartCoroutine(waiter());
            }
        }
        

    }
    IEnumerator waiter()
    {
        popUpText.enabled = !popUpText.enabled;
        popUpBg.enabled = !popUpBg.enabled;

        yield return new WaitForSecondsRealtime(2);

        popUpText.enabled = !popUpText.enabled;
        popUpBg.enabled = !popUpBg.enabled;
    }


}
