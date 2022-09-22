using UnityEngine;

[CreateAssetMenu(fileName = "LevelObject")]
public class LevelObject : ScriptableObject
{
    public string levelName;
    public AudioClip levelAudio;
    public Sprite levelScreenshot;
}
