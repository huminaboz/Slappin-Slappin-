using System.Collections;
using UnityEngine;

public class PoolableParticleEffect : MonoBehaviour, IObjectPool<PoolableParticleEffect>
{
    private ParticleSystem particleSystem;
    
    
    
    public void SetupObjectFirstTime()
    {
        gameObject.SetActive(false);
        particleSystem = GetComponent<ParticleSystem>();
    }

    public void InitializeObjectFromPool()
    {
        gameObject.SetActive(true);
        particleSystem.Play();
        StartCoroutine(WaitForParticleToFinish());
    }
    
    
    
    private IEnumerator WaitForParticleToFinish()
    {
        // Wait until the particle system has finished playing
        while (particleSystem.IsAlive(true)) // Pass true to check for children systems as well
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
