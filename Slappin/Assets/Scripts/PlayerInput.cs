using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    [SerializeField] private SlapAttack slapAttack;
    
    private void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            slapAttack.DropSlap();
        }
    }
}
