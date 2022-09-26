using System;
using TMPro;
using UnityEngine;
using System.Collections;
using UnityEngine.UIElements;
using UnityEngine.Audio;
using Cursor = UnityEngine.Cursor;

[RequireComponent(typeof(Transform))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] public float mouseSensitivity = 3.5f;
    [SerializeField] public float walkSpeed = 6f;
    public TMP_Text popUpText;
    public Animator cameraAnimator;
    [SerializeField] private bool lockCursor = false;
    [SerializeField] [Range(0.0f, 0.5f)] private float moveSmoothTime = 0.3f;
    [SerializeField] [Range(0.0f, 0.5f)] private float mouseSmoothTime = 0.03f;
    [SerializeField] private float gravity = -13f;
    private float velocityY;
    private CharacterController controller;
    private Vector2 currentDir = Vector2.zero;
    private Vector2 currentDirVelocity = Vector2.zero;
    private Vector2 currentMouseDelta = Vector2.zero;
    private Vector2 currentMouseVelocity = Vector2.zero;
    private Transform cameraTransform;
    private bool inSeatArea;

    public Animator animator;
    public AudioSource source;
    public AudioClip clip;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        if (lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        popUpText.enabled = false;
        cameraTransform = Camera.main.transform;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateMouseLook();
        UpdateMovement();
        UpdateInteraction();
    }

    void UpdateMouseLook()
    {
        Vector2 targetMouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        currentMouseDelta =
            Vector2.SmoothDamp(currentMouseDelta, targetMouseDelta, ref currentMouseVelocity, mouseSmoothTime);
        transform.Rotate(Vector3.up * currentMouseDelta.x * mouseSensitivity);
    }

    void UpdateMovement()
    {
        Vector2 targetDir = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        targetDir.Normalize();
        currentDir = Vector2.SmoothDamp(currentDir, targetDir, ref currentDirVelocity, moveSmoothTime);
        if (controller.isGrounded)
            velocityY = 0.0f;
        velocityY += gravity * Time.deltaTime;
        Vector3 velocity = (cameraTransform.forward * currentDir.y + cameraTransform.right * currentDir.x) * walkSpeed + Vector3.up * velocityY;
        controller.Move(velocity * Time.deltaTime);
    }


    void UpdateInteraction()
    {
        if(Input.GetKey(KeyCode.E) && inSeatArea)
        {
            source.PlayOneShot(clip);
            cameraAnimator.Play("Seat Camera");
            popUpText.enabled = false;
            popUpText.text = "";
            animator.Play("BusLoopStart_Animation");
            GameObject.Find("Player").GetComponent<CharacterController>().enabled = false;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerSeat"))
        {
            // transition to sitting
            popUpText.enabled = true;
            popUpText.text = "Press E to Sit";
            inSeatArea = true;
        }
    }
    
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("PlayerSeat"))
        {
            // transition to sitting
            popUpText.enabled = false;
            popUpText.text = "";
            inSeatArea = false;
        }
    }
    
}
