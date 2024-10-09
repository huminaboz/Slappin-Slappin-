using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    [SerializeField] private SlapAttack slapAttack;
    
    private void Update()
    {
        if (!StateGame.PlayerInGameControlsEnabled) return;
        
        if (Input.GetButtonDown("Fire1"))
        {
            slapAttack.DropSlap();
        }
    }
}
