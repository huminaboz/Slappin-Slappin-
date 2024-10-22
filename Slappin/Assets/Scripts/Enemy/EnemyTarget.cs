using UnityEngine;

public class EnemyTarget : Singleton<EnemyTarget>
{
    [SerializeField] public Transform targetTransform;
    [SerializeField] public Transform hurtLine;
    [SerializeField] public Transform fartLine;
    [SerializeField] public Transform mageLine;

    [Header("Debug Stuff")] 
    [SerializeField] private Transform spawnLeft;
    [SerializeField] private Transform spawnRight;
    [SerializeField] private Transform hurtLeft;
    [SerializeField] private Transform hurtRight;

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(spawnLeft.position, hurtLeft.position);
        Gizmos.DrawLine(spawnRight.position, hurtRight.position);
    }
}