using System.Collections;
using UnityEngine;

public class PoolableParticleEffect : MonoBehaviour, IObjectPool<PoolableParticleEffect>
{
    private ParticleSystem thisParticleSystem;
    
    public void SetupObjectFirstTime()
    {
        gameObject.SetActive(false);
        thisParticleSystem = GetComponent<ParticleSystem>();
    }

    public void InitializeObjectFromPool()
    {
        gameObject.SetActive(true);
        thisParticleSystem.Play();
        StartCoroutine(WaitForParticleToFinish());
    }
    
    private IEnumerator WaitForParticleToFinish()
    {
        // Wait until the particle system has finished playing
        while (thisParticleSystem.IsAlive(true)) // Pass true to check for children systems as well
        {
            yield return null; // Wait for the next frame
        }

        // Callback logic for when the particle system finishes
        OnParticleSystemFinished();
    }

    // Callback function when the particle system is finished
    private void OnParticleSystemFinished()
    {
        Debug.Log("Particle system finished playing!");
        ReturnObjectToPool();
    }
    
    
    

    public void ReturnObjectToPool()
    {
        ObjectPoolManager<PoolableParticleEffect>.ReturnObject(this);
    }
}
