using System;
using QFSW.QC;
using UnityEngine;
using UnityEngine.Serialization;

public class EnemyAnimations : MonoBehaviour, IHpAdjustmentListener
{
    /*States:
     idle - defaults to this if reached the line and doesn't do anything else
     walkingTowardsPlayer - can be interrupted by damaging or switching what the animation is
     running (based on speed)
     attacking - plays on a timer - can be interrupted
     getting damaged - plays on a timer then returns to walking towardsPlayer - can't be interrupted except by damaging
     deathing - cannot be interrupted, allows cleanup upon animation completion

     dizzy - could implement this later after getting knocked back
     */

    public enum AnimationFrames
    {
        WalkFWD,
        RunFWD,
        GetHit, 
        Attack01,
        Dizzy,
        Victory,
        IdleNormal,
        Taunt
    }

    [SerializeField] private Animator animator;
    private AnimatorStateInfo _stateInfo;

    public string currentClip;
    public string previousClip;
    private Action _callback;

    private void OnEnable()
    {
        _callback = null;
    }

    private void Update()
    {
        if (!animator) return;
        _stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        if (_stateInfo.IsName(currentClip) == false 
            // || _stateInfo.loop
            ) return;
        if (IsAnimationFinished(_stateInfo))
        {
            _callback?.Invoke();
            _callback = null;
        }
    }

    private bool IsAnimationFinished(AnimatorStateInfo stateInfo)
    {
        return stateInfo.normalizedTime >= 1f;
    }

    public void PlayPreviousClip()
    {
        Play(previousClip);
    }

    private void Play(string clip, Action callback = null)
    {
        previousClip = currentClip;
        currentClip = clip;
        Debug.LogWarning("current clip set to: " + currentClip);
        animator.Play(clip);
        _callback = callback;
    }

    public void Play(AnimationFrames animationFrames, Action callback = null)
    {
        Play(animationFrames.ToString(), callback);
    }

    public void TookDamage(int damageAmount, GameObject attacker)
    {
    }

    public void Healed(int healAmount, GameObject healer)
    {
    }

    public float HandleDeath(int lastAttack, GameObject killer)
    {
        _callback = null;
        return 0;
    }
}