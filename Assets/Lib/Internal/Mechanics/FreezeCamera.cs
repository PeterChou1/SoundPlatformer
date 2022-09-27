using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreezeCamera : MonoBehaviour
{
    private Animator _animator;
    private bool equippedPhone;
    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            equippedPhone = !equippedPhone;
            if (equippedPhone)
            {
                _animator.Play("FreezeCamera");
            }
            else
            {
                _animator.Play("UnFreezeCamera");
            }
        }
        
    }
}
