using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "AttackData_Flick", menuName = "Slappin/AttackData_Flick")]
public class SO_AttackData_Flick : SO_AttackData
{
    [FormerlySerializedAs("chargeMaxDamage")]
    [Header("Flick Specific Stuff")]

    [SerializeField] public float chargeMaxDamageMultiplier = 2f;
    [SerializeField] public AnimationCurve chargeCurve;
    [SerializeField] public float distanceBase = 1f;
    [FormerlySerializedAs("distanceMax")] [SerializeField] public float distanceMaxMultiplier = 2f;
    [SerializeField] public float attackWidthMultiplier = 1f;
    [SerializeField] public float maxChargeTime = 2f;
}
