using UnityEngine;

[CreateAssetMenu(menuName = "Strategies/Exit/Exit On Dialogue Complete")]
public class ExitOnDialogueCompleteSO : ExitStrategySO
{
    [SerializeField] private float timeout = 12f;
    private float timer = 0f;

    public override bool ShouldExit(IPuzzleInteractor actor, IWorldInteractable target)
    {
        timer += Time.deltaTime;

        if (DialogueManager.Instance == null || timer >= timeout)
            return true;

        return !DialogueManager.Instance.IsDialoguePlaying();
    }
}
