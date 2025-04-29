using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

public class PuzzlePanelFeature : MonoBehaviour, IInteractableFeature
{
    [Header("Panel Settings")]
    [SerializeField] private GameObject panelUIPrefab;
    [SerializeField] private Transform panelUIParent;
    [SerializeField] private bool deactivateAfterSolve = true;


    [Header("Unlock Targets")]
    [SerializeField] private List<GameObject> unlockTargets = new();

    [Header("Feature Effects")]
    [SerializeField] private List<EffectStrategySO> featureEffects = new();

    [Header("Optional Events")]
    [SerializeField] private UnityEvent onPuzzleSolved;

    private GameObject spawnedUIPanel;
    private bool solved = false;

    public void OnInteract(IPuzzleInteractor actor)
    {
        if (solved) return;

        OpenPuzzlePanel();
    }

    private void OpenPuzzlePanel()
    {
        // Either insert UI logic or effect game world logic here.
        // Gotta decide how we want this to interact with the "puzzles" here, maybe instead of UI it changes camera to a static one to "see" the whole puzzle.
    }

    public void PuzzleSolved()
    {
        if (solved) return;

        solved = true;

        Debug.Log("[PuzzlePanelFeature] Puzzle solved!");

        foreach (var target in unlockTargets)
        {
            if (target != null)
            {
                target.SetActive(true);
            }
        }

        RunFeatureEffects();

        onPuzzleSolved?.Invoke();

        if (deactivateAfterSolve)
        {
            Destroy(spawnedUIPanel);
        }
    }

    private void RunFeatureEffects()
    {
        foreach (var effect in featureEffects)
        {
            if (effect != null && TryGetComponent(out IWorldInteractable interactable))
            {
                effect.ApplyEffect(null, interactable, InteractionResult.Success);
            }
        }
    }

    public void SetFeatureEffects(List<EffectStrategySO> effects)
    {
        featureEffects = effects ?? new List<EffectStrategySO>();
    }
}


