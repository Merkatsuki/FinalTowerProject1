// Refactored PuzzlePanelFeature.cs to inherit from FeatureBase
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PuzzlePanelFeature : FeatureBase
{
    [Header("Panel Settings")]
    [SerializeField] private GameObject panelUIPrefab;
    [SerializeField] private Transform panelUIParent;
    [SerializeField] private bool deactivateAfterSolve = true;

    [Header("Unlock Targets")]
    [SerializeField] private List<GameObject> unlockTargets = new();

    [Header("Optional Events")]
    [SerializeField] private UnityEvent onPuzzleSolved;

    private GameObject spawnedUIPanel;

    public override void OnInteract(IPuzzleInteractor actor)
    {
        if (isSolved) return;
        OpenPuzzlePanel();
    }

    private void OpenPuzzlePanel()
    {
        // UI logic placeholder
        PuzzleSolved(); // simulate solve for now
    }

    public void PuzzleSolved()
    {
        if (isSolved) return;
        isSolved = true;

        Debug.Log("[PuzzlePanelFeature] Puzzle solved!");

        foreach (var target in unlockTargets)
        {
            if (target != null)
                target.SetActive(true);
        }

        RunFeatureEffects();
        onPuzzleSolved?.Invoke();
        NotifyPuzzleInteractionSuccess();

        if (deactivateAfterSolve && spawnedUIPanel != null)
            Destroy(spawnedUIPanel);
    }

    public override void ResetPuzzleComponent()
    {
        base.ResetPuzzleComponent();
        if (deactivateAfterSolve && spawnedUIPanel != null)
            Destroy(spawnedUIPanel);
    }
}
