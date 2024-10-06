using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    [SerializeField] private SlapAttack slapAttack;
    
    private void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            Debug.Log("Firing Slap");
            slapAttack.DropSlap();
        }
    }
}
