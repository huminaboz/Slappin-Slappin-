using UnityEngine;

public class VFXSpawner : Singleton<VFXSpawner>
{
    [SerializeField] private GameObject damageNumberPrefab;
    [SerializeField] private GameObject hitEffectPrefab;

    [SerializeField] private Transform cameraTransform;
    
    public void SpawnDamageNumber(int damage, Vector3 startPosition)
    {
        DamageNumber damageNumber = ObjectPoolManager<DamageNumber>.GetObject(damageNumberPrefab);
        damageNumber.Spawn(damage, startPosition);
    }
    
    public void SpawnHitFX(Transform hitTransform)
    {
        if(hitTransform is null) return;
        PoolableParticleEffect spawnedHit = ObjectPoolManager<PoolableParticleEffect>
            .GetObject(hitEffectPrefab);
        //TODO:: I think maybe there's no cleanup for the spawn hit fx so it just maxes out
        spawnedHit.transform.position = hitTransform.position;
        spawnedHit.transform.LookAt(cameraTransform);
    }
}
