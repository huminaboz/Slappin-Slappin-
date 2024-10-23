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
    public SFXScrob squishHitGround;
    public SFXScrob farted;
    public SFXScrob dotHit;
    
    [Header("Enemy")] 
    public SFXScrob enemyDied;
    public SFXScrob enemyAttacked;
    public SFXScrob bouncerExploded;
    public SFXScrob bouncerBlocked;
    public SFXScrob projectileShot;
    public SFXScrob snuffedProjectile;

    [Header("Pickups")] 
    public SFXScrob pickedUpCurrency1;
    public SFXScrob healthPickup;

    [Header("Effects")] public SFXScrob gameStarted;
    
    [Header("UI")]
    public SFXScrob WaveEnded;
    public SFXScrob WaveStart;
    public SFXScrob BoughtUpgrade;
    public SFXScrob HoverUpgrade;
    public SFXScrob CantBuy;
    public SFXScrob changedSFXVolume;
    
    
    [Header("Player")] 
    public SFXScrob playerDied;
    public SFXScrob playerTookDamage;



    [Header("Music")]
    public MusicScrob playing;
    public MusicScrob store;
    
}
