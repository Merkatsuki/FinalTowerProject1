using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using System.IO;
using System.Collections.Generic;

public class InteractableGeneratorEditor : EditorWindow
{
    private string interactableName = "NewInteractable";

    private bool addSpriteRenderer = true;
    private bool addHighlightLight = true;

    private bool addDialogueFeature = false;
    private bool addLightToggleFeature = false;
    private bool addEnergyFeature = false;
    private bool addDoorFeature = false;
    private bool addPortalFeature = false;
    private bool addCollectibleFeature = false;
    private bool addPuzzleUnlockFeature = false;
    private bool addLockedDoorFeature = false;

    private bool saveAsPrefab = false;
    private string prefabSavePath = "Assets/_Project/Prefabs/Interactables";

    private ScriptableObject linkedScriptableObject;
    private GameObject linkedPrefab;
    private MonoScript linkedScript;

    private int selectedPresetIndex = 0;
    private readonly string[] presetOptions = new string[] { "None", "Memory Fragment", "Locked Puzzle Door", "Energy Gate", "Lore Terminal" };

    [MenuItem("Tools/Interactable Generator")]
    public static void ShowWindow()
    {
        GetWindow<InteractableGeneratorEditor>("Interactable Generator");
    }

    private void OnGUI()
    {
        GUILayout.Label("Interactable Generator", EditorStyles.boldLabel);

        interactableName = EditorGUILayout.TextField("Interactable Name", interactableName);

        selectedPresetIndex = EditorGUILayout.Popup("Preset", selectedPresetIndex, presetOptions);
        if (selectedPresetIndex != 0)
        {
            ApplyPreset(selectedPresetIndex);
        }

        GUILayout.Space(10);
        GUILayout.Label("Basic Components", EditorStyles.boldLabel);
        addSpriteRenderer = EditorGUILayout.Toggle("Add Sprite Renderer", addSpriteRenderer);
        addHighlightLight = EditorGUILayout.Toggle("Add Highlight Light", addHighlightLight);

        GUILayout.Space(10);
        GUILayout.Label("Features", EditorStyles.boldLabel);
        addDialogueFeature = EditorGUILayout.Toggle("Add Dialogue Feature", addDialogueFeature);
        addLightToggleFeature = EditorGUILayout.Toggle("Add Light Toggle Feature", addLightToggleFeature);
        addEnergyFeature = EditorGUILayout.Toggle("Add Energy Feature", addEnergyFeature);
        addDoorFeature = EditorGUILayout.Toggle("Add Door Feature", addDoorFeature);
        addPortalFeature = EditorGUILayout.Toggle("Add Portal Feature", addPortalFeature);
        addCollectibleFeature = EditorGUILayout.Toggle("Add Collectible Feature", addCollectibleFeature);
        addPuzzleUnlockFeature = EditorGUILayout.Toggle("Add Puzzle Unlock Feature", addPuzzleUnlockFeature);
        addLockedDoorFeature = EditorGUILayout.Toggle("Add Locked Door Feature", addLockedDoorFeature);

        GUILayout.Space(10);
        GUILayout.Label("Linked Assets (Optional)", EditorStyles.boldLabel);
        linkedScriptableObject = (ScriptableObject)EditorGUILayout.ObjectField("Link ScriptableObject", linkedScriptableObject, typeof(ScriptableObject), false);
        linkedPrefab = (GameObject)EditorGUILayout.ObjectField("Link Prefab", linkedPrefab, typeof(GameObject), false);
        linkedScript = (MonoScript)EditorGUILayout.ObjectField("Link Script", linkedScript, typeof(MonoScript), false);

        GUILayout.Space(10);
        GUILayout.Label("Prefab Options", EditorStyles.boldLabel);
        saveAsPrefab = EditorGUILayout.Toggle("Save as Prefab", saveAsPrefab);
        prefabSavePath = EditorGUILayout.TextField("Prefab Save Path", prefabSavePath);

        GUILayout.Space(20);
        if (GUILayout.Button("Create Interactable"))
        {
            CreateInteractable();
        }
    }

    private void ApplyPreset(int presetIndex)
    {
        addDialogueFeature = false;
        addLightToggleFeature = false;
        addEnergyFeature = false;
        addDoorFeature = false;
        addPortalFeature = false;
        addCollectibleFeature = false;
        addPuzzleUnlockFeature = false;
        addLockedDoorFeature = false;

        switch (presetIndex)
        {
            case 1:
                addDialogueFeature = true;
                addHighlightLight = true;
                addSpriteRenderer = true;
                break;
            case 2:
                addDoorFeature = true;
                addLockedDoorFeature = true;
                addHighlightLight = true;
                break;
            case 3:
                addPuzzleUnlockFeature = true;
                addEnergyFeature = true;
                addHighlightLight = true;
                break;
            case 4:
                addDialogueFeature = true;
                addHighlightLight = true;
                addSpriteRenderer = true;
                break;
        }
    }

    private void CreateInteractable()
    {
        GameObject go = new GameObject(interactableName);
        var baseInteractable = go.AddComponent<InteractableBase>();

        FeatureMetadata metadata = go.AddComponent<FeatureMetadata>();

        if (addSpriteRenderer)
        {
            var spriteRenderer = go.AddComponent<SpriteRenderer>();
            spriteRenderer.sortingOrder = 1;
        }

        // Set to Notables Layer if available
        int notablesLayer = LayerMask.NameToLayer("Notables");
        if (notablesLayer != -1)
            go.layer = notablesLayer;

        Light2D highlightLight = null;
        if (addHighlightLight)
        {
            var lightObj = new GameObject("HighlightLight");
            lightObj.transform.SetParent(go.transform);
            lightObj.transform.localPosition = Vector3.zero;
            highlightLight = lightObj.AddComponent<Light2D>();
            highlightLight.intensity = 0f;
            highlightLight.lightType = Light2D.LightType.Point;
            highlightLight.pointLightOuterRadius = 2.5f;
            baseInteractable.SetHighlightLight(highlightLight);
        }

        if (addDialogueFeature) { go.AddComponent<DialogueFeature>(); metadata.AddFeatureTag("Dialogue"); }
        if (addLightToggleFeature)
        {
            var feature = go.AddComponent<LightToggleFeature>();
            metadata.AddFeatureTag("LightToggle");
            // Create Toggleable Light2D
            var toggleObj = new GameObject("ToggleableLight");
            toggleObj.transform.SetParent(go.transform);
            toggleObj.transform.localPosition = Vector3.zero;
            var toggleLight = toggleObj.AddComponent<Light2D>();
            toggleLight.intensity = 0f;
            toggleLight.lightType = Light2D.LightType.Point;
            toggleLight.pointLightOuterRadius = 2.5f;
            feature.SetToggleLight(toggleLight);
        }
        if (addEnergyFeature)
        {
            var feature = go.AddComponent<EnergyFeature>();
            metadata.AddFeatureTag("Energy");
            // Create Energy Light2D
            var energyObj = new GameObject("EnergyLight");
            energyObj.transform.SetParent(go.transform);
            energyObj.transform.localPosition = Vector3.zero;
            var energyLight = energyObj.AddComponent<Light2D>();
            energyLight.intensity = 0f;
            energyLight.lightType = Light2D.LightType.Point;
            energyLight.pointLightOuterRadius = 2.5f;
            feature.SetEnergyLight(energyLight);
        }
        if (addDoorFeature) { go.AddComponent<DoorFeature>(); metadata.AddFeatureTag("Door"); }
        if (addPortalFeature) { go.AddComponent<PortalFeature>(); metadata.AddFeatureTag("Portal"); }
        if (addCollectibleFeature) { go.AddComponent<CollectibleFeature>(); metadata.AddFeatureTag("Collectible"); }
        if (addPuzzleUnlockFeature) { go.AddComponent<PuzzleUnlockFeature>(); metadata.AddFeatureTag("PuzzleUnlock"); }
        if (addLockedDoorFeature) { go.AddComponent<LockedDoorFeature>(); metadata.AddFeatureTag("LockedDoor"); }

        if (addPortalFeature && IsTagDefined("Portal"))
        {
            go.tag = "Portal";
        }
        else if (addCollectibleFeature && IsTagDefined("Collectible"))
        {
            go.tag = "Collectible";
        }
        else if ((addPuzzleUnlockFeature || addLockedDoorFeature) && IsTagDefined("PuzzleObject"))
        {
            go.tag = "PuzzleObject";
        }

        if (go.GetComponent<Collider2D>() == null)
        {
            var collider = go.AddComponent<CircleCollider2D>();
            collider.isTrigger = true;
            collider.radius = 0.5f;
        }

        if (saveAsPrefab)
        {
            if (!Directory.Exists(prefabSavePath))
            {
                Directory.CreateDirectory(prefabSavePath);
            }

            string prefabPath = Path.Combine(prefabSavePath, interactableName + ".prefab");
            PrefabUtility.SaveAsPrefabAsset(go, prefabPath);
            Debug.Log($"[Generator] Saved Prefab at: {prefabPath}");
            DestroyImmediate(go);
        }
        else
        {
            Selection.activeGameObject = go;
            Debug.Log($"[Generator] Created Interactable: {interactableName}");
        }
    }

    private bool IsTagDefined(string tag)
    {
        foreach (var definedTag in UnityEditorInternal.InternalEditorUtility.tags)
        {
            if (definedTag == tag)
                return true;
        }
        return false;
    }
}