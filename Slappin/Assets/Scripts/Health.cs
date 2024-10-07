using Unity.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    [ReadOnly] public int hp;
    [SerializeField] private int maxHp = 5;
    [SerializeField] private UnityEvent OnDeath;
    private bool immuneToDamage = false;
    public bool isAlive = true;

    private IHpAdjustmentListener[] hpAdjustmentListeners;

    private void Awake()
    {
        //This is so anything we need to inform of this object's untimely demise can know
        hpAdjustmentListeners = GetComponents<IHpAdjustmentListener>();
    }

    public void Initialize()
    {
        hp = maxHp;
        isAlive = true;
    }

    public void AdjustHp(int amount, GameObject attacker)
    {
        if (!isAlive) return;
        
        int oldHealth = hp;
        hp += amount;
        Debug.Log($"{gameObject.name} damaged for {amount}. "
                  + $"\nHp is now {hp}");

        //HEALING
        if (oldHealth < hp)
        {
            foreach (IHpAdjustmentListener damageListeners in hpAdjustmentListeners)
            {
                damageListeners.Healed(amount, attacker);
            }
            return;
        }

        //HURTING
        if (oldHealth > hp)
        {
            foreach (IHpAdjustmentListener damageListeners in hpAdjustmentListeners)
            {
                damageListeners.TookDamage(amount, attacker);
            }
        }

        //DEADING
        if (hp <= 0)
        {
            isAlive = false;

            float maxWaitTime = 0;
            foreach (IHpAdjustmentListener damageListeners in hpAdjustmentListeners)
            {
                Debug.Log($"{damageListeners} is handling death.");
                maxWaitTime = Mathf.Max(damageListeners.HandleDeath(amount, attacker), maxWaitTime);
            }

            OnDeath?.Invoke();
            // StartCoroutine(QueueCleanup(maxWaitTime + 0.1f));
        }
    }

    // IEnumerator QueueCleanup(float waitTime)
    // {
    //     yield return waitTime;
    //     Cleanup();
    // }
    //
    // private void Cleanup()
    // {
    //     gameObject.SetActive(false);
    // }
}