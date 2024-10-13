using System;
using UnityEngine;
using QFSW.QC;

public class DebugHelper : MonoBehaviour
{
    [SerializeField] QuantumConsole _qc;


    private void Awake()
    {
        _qc = _qc
              ?? GetComponent<QuantumConsole>()
              ?? QuantumConsole.Instance;
    }

    private void OnEnable()
    {
        _qc.OnActivate += OnQuantumActivate;
        _qc.OnDeactivate += OnQuantumDeactivate;
    }

    private void OnDisable()
    {
        _qc.OnActivate -= OnQuantumActivate;
        _qc.OnDeactivate -= OnQuantumDeactivate;
    }

    private void Update()
    {
        if (Input.GetButtonDown("Pause"))
        {
            Debug.LogWarning("PAUSING OR UNPAUSING GAME");
            Time.timeScale = Time.timeScale == 1f ? 0f : 1f;
        }
            
    }

    private void OnQuantumActivate()
    {
        StateGame.PlayerInGameControlsEnabled = false;
    }

    private void OnQuantumDeactivate()
    {
        StateGame.PlayerInGameControlsEnabled = true;
    }
}