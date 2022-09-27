using System;
using TMPro;
using UnityEngine;
using System.Collections;
using Cinemachine;
using UnityEngine.UIElements;
using UnityEngine.Audio;
using Cursor = UnityEngine.Cursor;

[RequireComponent(typeof(Transform))]
public class PlayerController : MonoBehaviour
{
    public float mouseSensitivity = 3.5f;
    public float walkSpeed = 6f;
    public TMP_Text popUpText;
    public Animator cameraAnimator;
    public Animator BusAnimator;
    public Animator worldAnimator;
    public AudioSource source;
    public AudioClip seatClip;
    public AudioClip exteriorFootStep;
    public AudioClip interiorFootStep;
    [SerializeField] private bool lockCursor = true;
    [SerializeField] [Range(0.0f, 0.5f)] private float moveSmoothTime = 0.3f;
    [SerializeField] [Range(0.0f, 0.5f)] private float mouseSmoothTime = 0.03f;
    [SerializeField] private float gravity = -13f;
    private Vector3 velocity;
    private float velocityY;
    private CharacterController controller;
    private Vector2 currentDir = Vector2.zero;
    private Vector2 currentDirVelocity = Vector2.zero;
    private Vector2 currentMouseDelta = Vector2.zero;
    private Vector2 currentMouseVelocity = Vector2.zero;
    private Transform cameraTransform;
    private float walkThreshold;
    private bool inSeatArea;
    private bool equippedPhone;
    private bool interior;
    private bool seated;



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
        UpdateSound();
    }

    void UpdateMouseLook()
    {
        if (!equippedPhone)
        {
            Vector2 targetMouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
            currentMouseDelta =
                Vector2.SmoothDamp(currentMouseDelta, targetMouseDelta, ref currentMouseVelocity, mouseSmoothTime);
            transform.Rotate(Vector3.up * currentMouseDelta.x * mouseSensitivity);
        }
    }

    void UpdateMovement()
    {
        if (!seated && !equippedPhone)
        {
            Vector2 targetDir = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            targetDir.Normalize();
            currentDir = Vector2.SmoothDamp(currentDir, targetDir, ref currentDirVelocity, moveSmoothTime);
            if (controller.isGrounded)
                velocityY = 0.0f;
            velocityY += gravity * Time.deltaTime;
            velocity = (cameraTransform.forward * currentDir.y + cameraTransform.right * currentDir.x) * walkSpeed + Vector3.up * velocityY;
            controller.Move(velocity * Time.deltaTime);
        }
    }

    void UpdateSound()
    {
        if (velocity.magnitude >= 1f && !source.isPlaying)
        {
            source.clip = interior ?  interiorFootStep : exteriorFootStep;
            source.pitch = 0.5f; // 1 is too fast
            source.loop = true;
            source.Play();
        } else if (velocity.magnitude < 1f && source.isPlaying)
        {
            source.pitch = 1f;
            source.loop = false;
            source.Stop();
        }
        if (source.clip == exteriorFootStep && interior)
        {
            source.clip = interiorFootStep;
            source.pitch = 0.5f;
            source.loop = true;
            source.Play();
        } else if (source.clip == interiorFootStep && !interior)
        {
            source.clip = exteriorFootStep;
            source.pitch = 0.5f; 
            source.loop = true;
            source.Play();
        }
    }
    
    void UpdateInteraction()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            equippedPhone = !equippedPhone;
            if (equippedPhone)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
        if(Input.GetKey(KeyCode.E) && inSeatArea && !seated)
        {
            source.Stop();
            source.PlayOneShot(seatClip);
            cameraAnimator.Play("Seat Camera");
            BusAnimator.Play("Bus_DoorClose");
            popUpText.enabled = false;
            popUpText.text = "";
            controller.enabled = false;
            seated = true;
            StartCoroutine(WaitForAnimation(3, () => {
                worldAnimator.Play("BusLoopStart_Animation");
                popUpText.enabled = true;
                popUpText.text = "Press Q to equip and unequip your phone";
                StartCoroutine(WaitForInput(KeyCode.Q, () =>
                {
                    popUpText.enabled = false;
                    popUpText.text = "";
                }));
            }));
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
        } else if (other.CompareTag("Interior"))
        {
            interior = true;
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
        } else if (other.CompareTag("Interior"))
        {
            interior = false;
        }
    }
    
    private IEnumerator WaitForAnimation ( float time , Action lambda)
    {
        yield return new WaitForSeconds(time);
        lambda();
    }
    
    private IEnumerator WaitForInput ( KeyCode code , Action lambda)
    {
        while (!Input.GetKeyDown(code))
        {
            yield return null;
        }
        lambda();
    }
    
}
