using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "AttackData_Slap", menuName = "Slappin/AttackData_Slap")]
public class SO_AttackData_Slap : SO_AttackData
{
    [FormerlySerializedAs("attackRadius")]
    [Header("Slap Specific Stuff")]
    [SerializeField] public float attackRadiusMultiplier;
}
