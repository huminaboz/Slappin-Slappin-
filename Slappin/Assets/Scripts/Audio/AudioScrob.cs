using UnityEngine;
using UnityEngine.Audio;

public class AudioScrob : ScriptableObject
{
    [TextArea(3,10)] public string notes;
    public AudioClip clip;
    public AudioMixerGroup mixerGroup;
}
