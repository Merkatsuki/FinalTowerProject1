// ExitStrategyDebugger.cs - with OnEnter and OnExit logging support
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Text;

public class ExitStrategyDebugger : MonoBehaviour
{
    [Header("Target Exit Strategies")]
    [SerializeField] private List<ExitStrategySO> strategiesToCheck = new();
    [SerializeField] private MonoBehaviour interactorRef;
    [SerializeField] private MonoBehaviour targetRef;

    [Header("Debug UI")]
    [SerializeField] private Text outputText;
    [SerializeField] private float refreshRate = 0.5f;
    [SerializeField] private bool logLifecycleEvents = true;

    private IPuzzleInteractor interactor;
    private IWorldInteractable target;

    private void Start()
    {
        interactor = interactorRef as IPuzzleInteractor ?? GetComponent<IPuzzleInteractor>();
        target = targetRef as IWorldInteractable ?? GetComponent<IWorldInteractable>();

        foreach (var strategy in strategiesToCheck)
        {
            if (strategy != null && logLifecycleEvents)
            {
                strategy.OnEnter(interactor, target);
                Debug.Log($"[ExitStrategyDebugger] OnEnter: {strategy.name}");
            }
        }

        InvokeRepeating(nameof(UpdateDebugDisplay), 0f, refreshRate);
    }

    private void OnDestroy()
    {
        if (!logLifecycleEvents) return;

        foreach (var strategy in strategiesToCheck)
        {
            if (strategy != null)
            {
                strategy.OnExit(interactor, target);
                Debug.Log($"[ExitStrategyDebugger] OnExit: {strategy.name}");
            }
        }
    }

    private void UpdateDebugDisplay()
    {
        if (outputText == null) return;

        StringBuilder sb = new();
        sb.AppendLine("<b>Exit Strategy Debug</b>");

        foreach (var strategy in strategiesToCheck)
        {
            if (strategy == null) continue;

            bool result = strategy.ShouldExit(interactor, target);
            sb.AppendLine($"{strategy.name}: {(result ? "<color=green>✔</color>" : "<color=red>✘</color>")}");
        }

        outputText.text = sb.ToString();
    }
}
