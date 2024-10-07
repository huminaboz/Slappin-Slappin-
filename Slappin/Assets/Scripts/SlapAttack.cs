using DG.Tweening;
using UnityEngine;

public class SlapAttack : MonoBehaviour
{
    [SerializeField] private Transform shadow;
    [SerializeField] private GameObject slap;

    [SerializeField] private Health playerHealth;

    [SerializeField] private SO_AttackData attackData;

    [SerializeField] private Player player;

    private const float OffScreenSlapYPosition = .88f;
    private float groundYPosition;

    [SerializeField] private AnimationCurve startSlapCurve;

    private void Start()
    {
        groundYPosition = slap.transform.localScale.y;
    }

    private Tween downTween;

    public void DropSlap()
    {
        downTween.Kill();
        //Warp to the off screen Y position and the x and z position of the shadow
        slap.transform.position = new Vector3(shadow.position.x, OffScreenSlapYPosition, shadow.position.z);

        //Tween down to the ground
        downTween = slap.transform.DOMoveY(groundYPosition, attackData.attackSpeed)
            .SetEase(startSlapCurve)
            .OnComplete(() => { GoBackUp(); });
        player.DisableInputs();
    }

    private void OnTriggerEnter(Collider other)
    {
        //I think it makes more since to just let whatever the hand hits to handle what it does when it gets slapped

        //If hitting a spike, take damage and go back up
        if (other.GetComponent(typeof(Enemy_Spike)) != null)
        {
            downTween.Kill();
            Enemy_Spike enemySpike = other.GetComponent<Enemy_Spike>();
            playerHealth.AdjustHp(-enemySpike.handStabDamage, gameObject);
            GoBackUp();
            return;
        }

        //If hitting an enemy, do damage to it
        if (other.GetComponent(typeof(Health)) != null)
        {
            Health health = other.GetComponent<Health>();
            health.AdjustHp(-attackData.baseDamage, gameObject);
        }
    }

    private void GoBackUp()
    {
        //tween back up from current position
        slap.transform.DOMoveY(OffScreenSlapYPosition, .1f * attackData.slapRecoveryMultiplier)
            .SetDelay(.1f * attackData.slapRecoveryMultiplier)
            .SetEase(Ease.InQuint)
            .OnComplete(() =>
            {
                player.EnableInputs();
            });
    }
}