using UnityEngine;
using UnityEngine.Serialization;

public class SO_AttackData : ScriptableObject
{
    [SerializeField] [TextArea(3,100)] private string Notes;

    [SerializeField] public int baseDamage = 1;
    [FormerlySerializedAs("goDownSpeed")] [FormerlySerializedAs("attackSpeed")] [SerializeField] public float baseGoDownSpeed = 200f;
    [FormerlySerializedAs("slapGoUpSpeed")] [SerializeField] public float goBackUpSpeed = 200f;
    [SerializeField] public float bonus_goodHit = 1.1f;
    [SerializeField] public float bonus_greatHit = 1.2f;
    [SerializeField] public float bonus_criticalHit = 1.5f;
    [SerializeField] public float bonus_legendaryHit = 3f;
    

    [Header("Sound")]
    [SerializeField] public SFXScrob playSFXOnHit;
}
