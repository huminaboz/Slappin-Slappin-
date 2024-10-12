using UnityEngine;
using UnityEngine.Audio;

public class AudioEventsStorage : Singleton<AudioEventsStorage>
{
    [TextArea(3,100)]public string notes;
    
    [Header("Mixer Groups")] 
    public AudioMixerGroup master;
    public AudioMixerGroup music;
    public AudioMixerGroup soundEffects;

    [Header("Attacks")] public SFXScrob slapHitGround;

    //[Header("Enemy")] public SFXScrob hitAnEnemy;

    [Header("Pickups")] public SFXScrob pickedUpCurrency1;
    
    [Header("Effects")] 
    
    
    [Header("UI")]
    public SFXScrob UI_Menu_Back;
    public SFXScrob UI_Menu_Scroll;
    public SFXScrob UI_New_Game_Select;
    public SFXScrob UI_Toggle;
    public SFXScrob UI_Confirmation;
    
    
    [Header("Misc")] 



    [Header("Music")]
    public MusicScrob asdf;
    
}
