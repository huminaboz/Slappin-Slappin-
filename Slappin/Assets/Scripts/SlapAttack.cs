using DG.Tweening;
using UnityEngine;

public class SlapAttack : MonoBehaviour
{
    [SerializeField] private Transform shadow;
    [SerializeField] private GameObject slap;

    [SerializeField] private Health playerHealth;

    [SerializeField] private SO_AttackData attackData;
    

    private const float OffScreenSlapYPosition = .88f;
    private float groundYPosition;
    
    private void Start()
    {
        groundYPosition = slap.transform.localScale.y;
    }

    private DOTween downTween;
    
    public void DropSlap()
    {
        //Warp to the off screen Y position and the x and z position of the shadow
        slap.transform.position = new Vector3(shadow.position.x, OffScreenSlapYPosition, shadow.position.z);
        
        //Tween down to the ground
        slap.transform.DOMoveY(groundYPosition, .2f)
            .SetEase(Ease.InOutQuint)
            .OnComplete(()=>
            {
                GoBackUp();
            });
    }

    private void OnTriggerEnter(Collider other)
    {
        //I think it makes more since to just let whatever the hand hits to handle what it does when it gets slapped
        
        //If hitting a spike, take damage and go back up
        //playerHealth.AdjustHp(-1);
        //GoBackUp();
        
        //If hitting an enemy, do damage to it
        if(other.GetComponent(typeof(IDamageable)) != null)
        {
            IDamageable damageable = other.GetComponent<IDamageable>();
            damageable.AdjustHealth(-attackData.baseDamage);
        }
    }

    private void GoBackUp()
    {
        //tween back up from current position
        slap.transform.DOMoveY(OffScreenSlapYPosition, .2f)
            .SetDelay(.2f)
            .SetEase(Ease.InOutQuint);
    }
}
