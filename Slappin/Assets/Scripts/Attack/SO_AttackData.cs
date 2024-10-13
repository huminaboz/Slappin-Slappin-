using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "AttackData_", menuName = "Slappin/AttackData")]
public class SO_AttackData : ScriptableObject
{
    [SerializeField] [TextArea(3,100)] private string Notes;

    [SerializeField] public int baseDamage;
    [SerializeField] public float attackSpeed;
    [SerializeField] public float slapGoUpSpeed = 1f;
    [SerializeField] public float bonus_goodHit = 1.1f;
    [SerializeField] public float bonus_greatHit = 1.2f;
    [SerializeField] public float bonus_criticalHit = 1.5f;
    [SerializeField] public float bonus_legendaryHit = 3f;
    

    [Header("Sound")]
    [SerializeField] public SFXScrob playSFXOnHit;
}
