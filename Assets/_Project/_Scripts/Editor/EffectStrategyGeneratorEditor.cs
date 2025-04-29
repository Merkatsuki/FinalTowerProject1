// EffectStrategyGeneratorEditor.cs
// Expanded Effect Generator (Now Supports PlaySound, Delay, SpawnPrefab)

using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class EffectStrategyGeneratorEditor : EditorWindow
{
    private string effectName = "NewEffect";
    private EffectType selectedEffectType = EffectType.GiveItem;

    [Header("Specific Parameters")]
    private ItemSO selectedItem;
    private FlagSO selectedFlag;
    private bool setValue = true;
    private List<EffectStrategySO> batchEffects = new();
    private AudioClip soundClip;
    private float soundVolume = 1.0f;
    private float delaySeconds = 1.0f;
    private GameObject prefabToSpawn;
    private Vector2 spawnOffset;

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

    private void DrawEffectParameters()
    {
        GUILayout.Space(10);
        GUILayout.Label("Effect Specific Parameters", EditorStyles.boldLabel);

        switch (selectedEffectType)
        {
            case EffectType.GiveItem:
                selectedItem = (ItemSO)EditorGUILayout.ObjectField("Item to Give", selectedItem, typeof(ItemSO), false);
                break;

            case EffectType.SetFlag:
                selectedFlag = (FlagSO)EditorGUILayout.ObjectField("Flag to Set", selectedFlag, typeof(FlagSO), false);
                setValue = EditorGUILayout.Toggle("Set Value (true/false)", setValue);
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

            case EffectType.PlaySound:
                soundClip = (AudioClip)EditorGUILayout.ObjectField("Sound Clip", soundClip, typeof(AudioClip), false);
                soundVolume = EditorGUILayout.Slider("Volume", soundVolume, 0f, 1f);
                break;

            case EffectType.Delay:
                delaySeconds = EditorGUILayout.FloatField("Delay (seconds)", delaySeconds);
                break;

            case EffectType.SpawnPrefab:
                prefabToSpawn = (GameObject)EditorGUILayout.ObjectField("Prefab to Spawn", prefabToSpawn, typeof(GameObject), false);
                spawnOffset = EditorGUILayout.Vector2Field("Spawn Offset", spawnOffset);
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
                var giveItem = ScriptableObject.CreateInstance<GiveItemEffect>();
                giveItem.SetItem(selectedItem);
                effect = giveItem;
                break;

            case EffectType.SetFlag:
                var setFlag = ScriptableObject.CreateInstance<SetFlagEffect>();
                setFlag.SetFlag(selectedFlag);
                setFlag.SetFlagValue(setValue);
                effect = setFlag;
                break;

            case EffectType.Batch:
                var batch = ScriptableObject.CreateInstance<BatchEffectStrategySO>();
                batch.SetEffects(batchEffects);
                effect = batch;
                break;

            case EffectType.PlaySound:
                var playSound = ScriptableObject.CreateInstance<PlaySoundEffect>();
                playSound.SetSoundClip(soundClip);
                playSound.SetVolume(soundVolume);
                effect = playSound;
                break;

            case EffectType.Delay:
                var delay = ScriptableObject.CreateInstance<DelayEffect>();
                delay.SetDelay(delaySeconds);
                effect = delay;
                break;

            case EffectType.SpawnPrefab:
                var spawn = ScriptableObject.CreateInstance<SpawnPrefabEffect>();
                spawn.SetPrefab(prefabToSpawn);
                spawn.SetOffset(spawnOffset);
                effect = spawn;
                break;
        }

        if (effect != null)
        {
            AssetDatabase.CreateAsset(effect, assetPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            EditorUtility.FocusProjectWindow();
            Selection.activeObject = effect;

            Debug.Log("[Generator] Created Effect Strategy: " + effectName);
        }
    }

    private enum EffectType
    {
        GiveItem,
        SetFlag,
        Batch,
        PlaySound,
        Delay,
        SpawnPrefab
    }
}
