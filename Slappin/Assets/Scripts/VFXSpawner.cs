using UnityEngine;

public class VFXSpawner : Singleton<VFXSpawner>
{
    [SerializeField] private GameObject damageNumberPrefab;

    public void SpawnDamageNumber(int damage, Vector3 startPosition)
    {
        DamageNumber damageNumber = ObjectPoolManager<DamageNumber>.GetObject(damageNumberPrefab);
        damageNumber.Spawn(damage, startPosition);
    }
}
