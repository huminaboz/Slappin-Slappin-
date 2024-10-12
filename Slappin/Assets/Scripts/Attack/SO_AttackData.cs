using UnityEngine;

[CreateAssetMenu(fileName = "AttackData_", menuName = "Slappin/AttackData")]
public class SO_AttackData : ScriptableObject
{
    [SerializeField] [TextArea(3,100)] private string Notes;

    [SerializeField] public int baseDamage;
    [SerializeField] public float attackSpeed;
    [SerializeField] public float slapGoUpSpeed = 1f;
    [SerializeField] public float slapRecoverFromSpikeTimer = 1f;

    [Header("Sound")]
    [SerializeField] public SFXScrob playSFXOnHit;
}
