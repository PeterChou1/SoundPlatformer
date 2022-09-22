using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderController : MonoBehaviour
{
    public AudioSource source;
    public Slider slider;
    private float targetProgress;
    // Start is called before the first frame update
    // Update is called once per frame
    void Update()
    {
        if (source != null)
        {
            slider.value = source.time / source.clip.length;
        }
    }
}
