// EffectStrategyGeneratorEditor.cs
// Upgraded Scaffold: Supports Delay, SoundCue, and Batch Effects

using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class EffectStrategyGeneratorEditor : EditorWindow
{
    private string effectName = "NewEffect";
    private EffectType selectedEffectType = EffectType.GiveItem;

    // Shared parameters
    private float delay = 0f;
    private AudioClip soundCue;

    // GiveItem parameters
    private ItemSO selectedItem;
    private bool onlyOnSuccess = true;

    // SetFlag parameters
    private FlagSO selectedFlag;
    private bool flagValue = true;

    // Batch parameters
    private List<EffectStrategySO> batchEffects = new();

    private string savePath = "Assets/_Project/ScriptableObjects/Effects";

    [MenuItem("Tools/Effect Strategy Generator")]
    public static void ShowWindow()
    {
        GetWindow<EffectStrategyGeneratorEditor>("Effect Strategy Generator");
    }

    private void OnGUI()
    {
        GUILayout.Label("Effect Strategy Generator", EditorStyles.boldLabel);

        effectName = EditorGUILayout.TextField("Effect Name", effectName);

        selectedEffectType = (EffectType)EditorGUILayout.EnumPopup("Effect Type", selectedEffectType);

        DrawSharedParameters();
        DrawEffectParameters();

        GUILayout.Space(20);
        GUILayout.Label("Save Options", EditorStyles.boldLabel);
        savePath = EditorGUILayout.TextField("Save Path", savePath);

        GUILayout.Space(10);
        if (GUILayout.Button("Create Effect Strategy"))
        {
            CreateEffectStrategy();
        }
    }

    private void DrawSharedParameters()
    {
        GUILayout.Space(10);
        GUILayout.Label("Shared Parameters", EditorStyles.boldLabel);
        delay = EditorGUILayout.FloatField("Delay (Seconds)", delay);
        soundCue = (AudioClip)EditorGUILayout.ObjectField("Sound Cue", soundCue, typeof(AudioClip), false);
    }

    private void DrawEffectParameters()
    {
        GUILayout.Space(10);
        GUILayout.Label("Effect Specific Parameters", EditorStyles.boldLabel);

        switch (selectedEffectType)
        {
            case EffectType.GiveItem:
                selectedItem = (ItemSO)EditorGUILayout.ObjectField("Item to Give", selectedItem, typeof(ItemSO), false);
                onlyOnSuccess = EditorGUILayout.Toggle("Only On Success", onlyOnSuccess);
                break;

            case EffectType.SetFlag:
                selectedFlag = (FlagSO)EditorGUILayout.ObjectField("Flag", selectedFlag, typeof(FlagSO), false);
                flagValue = EditorGUILayout.Toggle("Flag Value", flagValue);
                onlyOnSuccess = EditorGUILayout.Toggle("Only On Success", onlyOnSuccess);
                break;

            case EffectType.Batch:
                if (batchEffects == null)
                    batchEffects = new List<EffectStrategySO>();

                for (int i = 0; i < batchEffects.Count; i++)
                {
                    batchEffects[i] = (EffectStrategySO)EditorGUILayout.ObjectField($"Effect {i + 1}", batchEffects[i], typeof(EffectStrategySO), false);
                }
                if (GUILayout.Button("+ Add Effect"))
                {
                    batchEffects.Add(null);
                }
                break;

            default:
                GUILayout.Label("No specific parameters needed.");
                break;
        }
    }

    private void CreateEffectStrategy()
    {
        if (!Directory.Exists(savePath))
        {
            Directory.CreateDirectory(savePath);
        }

        string assetPath = Path.Combine(savePath, effectName + ".asset");

        EffectStrategySO effect = null;

        switch (selectedEffectType)
        {
            case EffectType.GiveItem:
                var giveItemEffect = ScriptableObject.CreateInstance<GiveItemEffect>();
                giveItemEffect.SetItem(selectedItem);
                giveItemEffect.SetOnlyOnSuccess(onlyOnSuccess);
                effect = giveItemEffect;
                break;

            case EffectType.SetFlag:
                var setFlagEffect = ScriptableObject.CreateInstance<SetFlagEffect>();
                setFlagEffect.SetFlag(selectedFlag);
                setFlagEffect.SetFlagValue(flagValue);
                setFlagEffect.SetOnlyOnSuccess(onlyOnSuccess);
                effect = setFlagEffect;
                break;

            case EffectType.Batch:
                var batchEffect = ScriptableObject.CreateInstance<BatchEffectStrategySO>();
                batchEffect.SetEffects(batchEffects);
                effect = batchEffect;
                break;
        }

        if (effect != null)
        {
            // Set shared parameters if available
            if (effect is ISharedEffectSettings sharedEffect)
            {
                sharedEffect.SetDelay(delay);
                sharedEffect.SetSoundCue(soundCue);
            }

            AssetDatabase.CreateAsset(effect, assetPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            EditorUtility.FocusProjectWindow();
            Selection.activeObject = effect;

            Debug.Log("[Generator] Created Effect Strategy: " + effectName);
        }
    }
}





