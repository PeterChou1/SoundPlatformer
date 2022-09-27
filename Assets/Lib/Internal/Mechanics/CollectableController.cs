using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableController : MonoBehaviour
{
    public Collectables _collectables;
    private Animator animator;
    private AudioSource source;


    private void Awake()
    {
        source = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
        source.clip = _collectables.clip;
        GetComponentInChildren<SpriteRenderer>().sprite = _collectables.image;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            source.Play();
            animator.Play("Collected");
        }
    }

    public void DestroyCollectable()
    {
        Destroy(gameObject);
    }
}
