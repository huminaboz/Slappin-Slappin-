using UnityEngine;
using UnityEngine.Audio;

public class AudioEventsStorage : Singleton<AudioEventsStorage>
{
    [TextArea(3,100)]public string notes;
    
    //Example use:
    //SFXPlayer.I.Play(AudioEventsStorage.I.slapHitGround);
    
    [Header("Mixer Groups")] 
    public AudioMixerGroup master;
    public AudioMixerGroup music;
    public AudioMixerGroup soundEffects;

    [Header("Attacks")] public SFXScrob slapHitGround;
    public SFXScrob releasedFlick;
    public SFXScrob farted;
    
    [Header("Enemy")] 
    public SFXScrob enemyDied;
    public SFXScrob enemyAttacked;

    [Header("Pickups")] 
    public SFXScrob pickedUpCurrency1;
    public SFXScrob pickedUpCurrency2;
    
    [Header("Effects")] 
    
    
    [Header("UI")]
    public SFXScrob UI_Menu_Back;
    public SFXScrob UI_Menu_Scroll;
    public SFXScrob UI_New_Game_Select;
    public SFXScrob UI_Toggle;
    public SFXScrob UI_Confirmation;
    
    
    [Header("Player")] 
    public SFXScrob playerDied;
    public SFXScrob playerTookDamage;



    [Header("Music")]
    public MusicScrob asdf;
    
}
