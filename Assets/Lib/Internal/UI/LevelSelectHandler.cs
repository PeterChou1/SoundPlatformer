using System;
using System.Security;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectHandler : MonoBehaviour
{
    public Image levelImage;
    public TMP_Text levelName;
    public LevelObject levelObject;
    // components needed to setup level
    public TMP_Text levelTitle;
    public AudioSource audioSource;
    public LevelCreator creator;
    
    private void Awake()
    {
        levelName.text = levelObject.levelName;
        levelImage.sprite = levelObject.levelScreenshot;
    }

    public void SetUpLevel()
    {
        levelTitle.text = levelObject.levelName;
        audioSource.clip = levelObject.levelAudio;
        creator.CreateLevel();
        audioSource.Play();
    }
}
