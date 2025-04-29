// EntryStrategyGeneratorEditor.cs
// Upgraded Scaffold: Supports Delay, SoundCue, and Batch Effects

using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class EntryStrategyGeneratorEditor : EditorWindow
{
    private string strategyName = "NewEntryStrategy";
    private EntryStrategyType selectedStrategyType = EntryStrategyType.AllowAll;

    private string requiredItemId;
    private string requiredPuzzleFlag;
    private EnergyType requiredEnergyType;
    private float cooldownDuration = 5f;
    private List<EntryStrategySO> compositeStrategies = new();
    private CompositeEntryStrategySO.LogicMode compositeLogicMode = CompositeEntryStrategySO.LogicMode.All;

    private string savePath = "Assets/_Project/ScriptableObjects/EntryStrategies";

    [MenuItem("Tools/Entry Strategy Generator")]
    public static void ShowWindow()
    {
        GetWindow<EntryStrategyGeneratorEditor>("Entry Strategy Generator");
    }

    private void OnGUI()
    {
        GUILayout.Label("Entry Strategy Generator", EditorStyles.boldLabel);

        strategyName = EditorGUILayout.TextField("Strategy Name", strategyName);

        selectedStrategyType = (EntryStrategyType)EditorGUILayout.EnumPopup("Strategy Type", selectedStrategyType);

        DrawStrategyParameters();

        GUILayout.Space(20);
        GUILayout.Label("Save Options", EditorStyles.boldLabel);
        savePath = EditorGUILayout.TextField("Save Path", savePath);

        GUILayout.Space(10);
        if (GUILayout.Button("Create Entry Strategy"))
        {
            CreateEntryStrategy();
        }
    }

    private void DrawStrategyParameters()
    {
        GUILayout.Space(10);
        GUILayout.Label("Strategy Specific Parameters", EditorStyles.boldLabel);

        switch (selectedStrategyType)
        {
            case EntryStrategyType.RequireItemHeld:
                requiredItemId = EditorGUILayout.TextField("Required Item ID", requiredItemId);
                break;

            case EntryStrategyType.RequirePuzzleSolved:
                requiredPuzzleFlag = EditorGUILayout.TextField("Required Puzzle Flag", requiredPuzzleFlag);
                break;

            case EntryStrategyType.RequireMatchingEnergy:
                requiredEnergyType = (EnergyType)EditorGUILayout.EnumPopup("Required Energy Type", requiredEnergyType);
                break;

            case EntryStrategyType.EntryCooldown:
                cooldownDuration = EditorGUILayout.FloatField("Cooldown Duration (seconds)", cooldownDuration);
                break;

            case EntryStrategyType.Composite:
                compositeLogicMode = (CompositeEntryStrategySO.LogicMode)EditorGUILayout.EnumPopup("Composite Mode", compositeLogicMode);
                if (compositeStrategies == null)
                    compositeStrategies = new List<EntryStrategySO>();

                for (int i = 0; i < compositeStrategies.Count; i++)
                {
                    compositeStrategies[i] = (EntryStrategySO)EditorGUILayout.ObjectField($"SubStrategy {i + 1}", compositeStrategies[i], typeof(EntryStrategySO), false);
                }
                if (GUILayout.Button("+ Add SubStrategy"))
                {
                    compositeStrategies.Add(null);
                }
                break;
        }
    }

    private void CreateEntryStrategy()
    {
        if (!Directory.Exists(savePath))
        {
            Directory.CreateDirectory(savePath);
        }

        string assetPath = Path.Combine(savePath, strategyName + ".asset");

        EntryStrategySO strategy = null;

        switch (selectedStrategyType)
        {
            case EntryStrategyType.AllowAll:
                strategy = ScriptableObject.CreateInstance<AllowAllEntryStrategySO>();
                break;
            case EntryStrategyType.DenyAll:
                strategy = ScriptableObject.CreateInstance<DenyAllEntryStrategySO>();
                break;
            case EntryStrategyType.RequireItemHeld:
                var itemStrategy = ScriptableObject.CreateInstance<RequireItemHeldEntrySO>();
                itemStrategy.SetRequiredItemId(requiredItemId);
                strategy = itemStrategy;
                break;
            case EntryStrategyType.RequirePuzzleSolved:
                var puzzleStrategy = ScriptableObject.CreateInstance<RequirePuzzleSolvedEntrySO>();
                puzzleStrategy.SetRequiredPuzzleFlag(requiredPuzzleFlag);
                strategy = puzzleStrategy;
                break;
            case EntryStrategyType.RequireMatchingEnergy:
                var energyStrategy = ScriptableObject.CreateInstance<RequireMatchingEnergyTypeSO>();
                energyStrategy.SetRequiredEnergyType(requiredEnergyType);
                strategy = energyStrategy;
                break;
            case EntryStrategyType.EntryCooldown:
                var cooldownStrategy = ScriptableObject.CreateInstance<EntryCooldownStrategySO>();
                cooldownStrategy.SetCooldown(cooldownDuration);
                strategy = cooldownStrategy;
                break;
            case EntryStrategyType.Composite:
                var compositeStrategy = ScriptableObject.CreateInstance<CompositeEntryStrategySO>();
                compositeStrategy.SetCompositeMode(compositeLogicMode);
                compositeStrategy.SetSubStrategies(compositeStrategies);
                strategy = compositeStrategy;
                break;
        }

        if (strategy != null)
        {
            AssetDatabase.CreateAsset(strategy, assetPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            EditorUtility.FocusProjectWindow();
            Selection.activeObject = strategy;

            Debug.Log("[Generator] Created Entry Strategy: " + strategyName);
        }
    }

    private enum EntryStrategyType
    {
        AllowAll,
        DenyAll,
        RequireItemHeld,
        RequirePuzzleSolved,
        RequireMatchingEnergy,
        EntryCooldown,
        Composite
    }
}
