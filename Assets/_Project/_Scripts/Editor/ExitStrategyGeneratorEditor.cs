// ExitStrategyGeneratorEditor.cs
// Upgraded Scaffold: Supports Delay, SoundCue, and Batch Effects

using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class ExitStrategyGeneratorEditor : EditorWindow
{
    private string strategyName = "NewExitStrategy";
    private ExitType selectedExitType = ExitType.ExitAfterTimer;

    [Header("Specific Parameters")]
    private float timerSeconds = 1f;
    private float radius = 5f;
    private float minTimerSeconds = 1f;
    private FlagSO requiredFlag;
    private List<ExitStrategySO> subStrategies = new();
    private LogicMode compositeLogicMode = LogicMode.All;

    private string savePath = "Assets/_Project/ScriptableObjects/ExitStrategies";

    [MenuItem("Tools/Exit Strategy Generator")]
    public static void ShowWindow()
    {
        GetWindow<ExitStrategyGeneratorEditor>("Exit Strategy Generator");
    }

    private void OnGUI()
    {
        GUILayout.Label("Exit Strategy Generator", EditorStyles.boldLabel);

        strategyName = EditorGUILayout.TextField("Strategy Name", strategyName);
        selectedExitType = (ExitType)EditorGUILayout.EnumPopup("Exit Type", selectedExitType);

        DrawSpecificParameters();

        GUILayout.Space(20);
        GUILayout.Label("Save Options", EditorStyles.boldLabel);
        savePath = EditorGUILayout.TextField("Save Path", savePath);

        GUILayout.Space(10);
        if (GUILayout.Button("Create Exit Strategy"))
        {
            CreateExitStrategy();
        }
    }

    private void DrawSpecificParameters()
    {
        GUILayout.Space(10);
        GUILayout.Label("Specific Parameters", EditorStyles.boldLabel);

        switch (selectedExitType)
        {
            case ExitType.ExitAfterTimer:
                timerSeconds = EditorGUILayout.FloatField("Timer Seconds", timerSeconds);
                break;

            case ExitType.ExitAfterPlayerLeaves:
                radius = EditorGUILayout.FloatField("Exit Radius", radius);
                break;

            case ExitType.ExitAfterTimerThenConditional:
                minTimerSeconds = EditorGUILayout.FloatField("Minimum Timer Seconds", minTimerSeconds);
                break;

            case ExitType.ExitOnFlagSet:
                requiredFlag = (FlagSO)EditorGUILayout.ObjectField("Required Flag", requiredFlag, typeof(FlagSO), false);
                break;

            case ExitType.Composite:
                compositeLogicMode = (LogicMode)EditorGUILayout.EnumPopup("Composite Logic", compositeLogicMode);

                if (subStrategies == null)
                    subStrategies = new List<ExitStrategySO>();

                for (int i = 0; i < subStrategies.Count; i++)
                {
                    subStrategies[i] = (ExitStrategySO)EditorGUILayout.ObjectField($"SubStrategy {i + 1}", subStrategies[i], typeof(ExitStrategySO), false);
                }

                if (GUILayout.Button("+ Add SubStrategy"))
                {
                    subStrategies.Add(null);
                }
                break;

            case ExitType.Sequential:
                if (subStrategies == null)
                    subStrategies = new List<ExitStrategySO>();

                for (int i = 0; i < subStrategies.Count; i++)
                {
                    subStrategies[i] = (ExitStrategySO)EditorGUILayout.ObjectField($"Step {i + 1}", subStrategies[i], typeof(ExitStrategySO), false);
                }

                if (GUILayout.Button("+ Add Step"))
                {
                    subStrategies.Add(null);
                }
                break;
        }
    }

    private void CreateExitStrategy()
    {
        if (!Directory.Exists(savePath))
        {
            Directory.CreateDirectory(savePath);
        }

        string assetPath = Path.Combine(savePath, strategyName + ".asset");

        ExitStrategySO exitStrategy = null;

        switch (selectedExitType)
        {
            case ExitType.ExitAfterTimer:
                var timerExit = ScriptableObject.CreateInstance<ExitAfterTimerSO>();
                timerExit.SetWaitTime(timerSeconds);
                exitStrategy = timerExit;
                break;

            case ExitType.ExitAfterPlayerLeaves:
                var playerLeavesExit = ScriptableObject.CreateInstance<ExitAfterActorLeavesSO>();
                //playerLeavesExit.SetExitRadius(radius);
                exitStrategy = playerLeavesExit;
                break;

            case ExitType.ExitAfterTimerThenConditional:
                var timerCondExit = ScriptableObject.CreateInstance<ExitAfterTimerThenConditionalSO>();
                timerCondExit.SetMinTime(minTimerSeconds);
                exitStrategy = timerCondExit;
                break;

            case ExitType.ExitOnDialogueComplete:
                exitStrategy = ScriptableObject.CreateInstance<ExitOnDialogueCompleteSO>();
                break;

            case ExitType.ExitOnFlagSet:
                var flagExit = ScriptableObject.CreateInstance<ExitOnFlagSetSO>();
                //flagExit.SetRequiredFlag(requiredFlag);
                exitStrategy = flagExit;
                break;

            case ExitType.Composite:
                var compositeExit = ScriptableObject.CreateInstance<CompositeExitStrategySO>();
                compositeExit.SetLogicMode(compositeLogicMode);
                compositeExit.SetSubStrategies(subStrategies);
                exitStrategy = compositeExit;
                break;

            case ExitType.Sequential:
                var sequentialExit = ScriptableObject.CreateInstance<SequentialExitStrategySO>();
                sequentialExit.SetSubStrategies(subStrategies);
                exitStrategy = sequentialExit;
                break;
        }

        if (exitStrategy != null)
        {
            AssetDatabase.CreateAsset(exitStrategy, assetPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            EditorUtility.FocusProjectWindow();
            Selection.activeObject = exitStrategy;

            Debug.Log("[Generator] Created Exit Strategy: " + strategyName);
        }
    }

    private enum ExitType
    {
        ExitAfterTimer,
        ExitAfterPlayerLeaves,
        ExitAfterCompanionInteracts,
        ExitAfterPlayerCommandedMove,
        ExitAfterTimerThenConditional,
        ExitOnDialogueComplete,
        ExitOnFlagSet,
        Composite,
        Sequential
    }
}
