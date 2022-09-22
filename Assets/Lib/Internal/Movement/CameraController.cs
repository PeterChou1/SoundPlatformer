using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float turnx, turny;
    public float sensitivity = 0.1f;
    public GameObject player;
    float xRot;
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }
    void Update()
    {
        turnx = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
        turny = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;
        xRot -= turny;
        xRot = Mathf.Clamp(xRot, -20f, 20f);
        transform.localRotation = Quaternion.Euler(xRot, 0f, 0f);
        player.transform.Rotate(Vector3.up * turnx);
    }
}
