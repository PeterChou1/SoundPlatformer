using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraAnimator : MonoBehaviour
{
    private Animator _animator;
    private bool _equippedPhone;
    private AudioSource _source;
    public AudioClip equipPhone;

    public AudioClip unequipPhone;
    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
        _source = GetComponent<AudioSource>();
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            _equippedPhone = !_equippedPhone;
            if (_equippedPhone)
            {
                _animator.Play("EquipPhone");
                _source.PlayOneShot(equipPhone);
            }
            else
            {
                _animator.Play("UnequipPhone");
                _source.PlayOneShot(unequipPhone);
            }
        }
    }
}
