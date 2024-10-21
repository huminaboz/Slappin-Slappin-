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
        if (!StateGame.debugModeOn) return;
        
        if (Input.GetKeyDown(KeyCode.N))
        {
            Time.timeScale -= 1f;
            Debug.Log($"N increased timescale to: {Time.timeScale}");
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            Time.timeScale += 1f;
            Debug.Log($"M decreased timescale to: {Time.timeScale}");
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            Debug.Log("B restores time to normal");
            Time.timeScale = 1f;
        }

        if (Input.GetKeyDown(KeyCode.Keypad0))
        {
            Debug.Break();
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