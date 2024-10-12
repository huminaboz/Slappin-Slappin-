using UnityEngine;

[CreateAssetMenu(fileName = "New MusicScrob", 
    menuName = "Slappin/MusicScrob")]
public class MusicScrob : AudioScrob
{
    [Range(0,1)] public float volume = 1f;
    public float fadeInTime = 0.5f;
    public float fadeOutTime = 0.5f;
    public bool looping = true;
}
