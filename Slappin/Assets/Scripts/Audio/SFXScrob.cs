using UnityEngine;

[CreateAssetMenu(fileName = "New SoundScrob", 
    menuName = "Slappin/SoundScrob")]
public class SFXScrob : AudioScrob
{
    [Range(0,1)] public float volume = 1f;
    public bool varyPitch = false;
    public bool looping = false;
    public float startDelaySeconds = 0f;
}
