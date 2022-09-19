using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public float speed = 2f;
    public Camera Cam2D,Cam3D;
    private void Start()
    {
        Cam3D.enabled = true;
        Cam2D.enabled = false;
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
            }
        }
        if (other.gameObject.tag == "driver")
        {
            if (Input.GetMouseButtonDown(0))
            {
                // trigger wave animation
            }
        }
        if (other.gameObject.tag == "door")
        {
            if (Input.GetMouseButtonDown(0))
            {
                // trigger leave animation
            }
        }
        if (other.gameObject.tag == "phone")
        {
          
        }

    }


}
