using System.Collections;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    [ReadOnly] public int hp;
    [SerializeField] private int maxHp = 5;
    [SerializeField] private UnityEvent OnDeath;
    private bool immuneToDamage = false;
    private bool isAlive = true;

    private IHpAdjustmentListener[] hpAdjustmentListeners;

    private void Awake()
    {
        //This is so anything we need to inform of this object's untimely demise can know
        hpAdjustmentListeners = GetComponents<IHpAdjustmentListener>();
    }

    private void OnEnable()
    {
        //This might be something to do in the pooling engine
        hp = maxHp;
    }

    public void AdjustHp(int amount, GameObject attacker)
    {
        int oldHealth = hp;
        hp += amount;
        Debug.Log($"{gameObject.name} damaged for {amount}. "
                  + $"\nHp is now {hp}");

        if (oldHealth < hp)
        {
            foreach (IHpAdjustmentListener damageListeners in hpAdjustmentListeners)
            {
                damageListeners.Healed(amount, attacker);
            }

            return;
        }

        if (oldHealth > hp)
        {
            foreach (IHpAdjustmentListener damageListeners in hpAdjustmentListeners)
            {
                damageListeners.TookDamage(amount, attacker);
            }
        }

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
            StartCoroutine(Cleanup(maxWaitTime + 0.1f));
        }
    }

    IEnumerator Cleanup(float waitTime)
    {
        yield return waitTime;

        Destroy(gameObject);
    }
}