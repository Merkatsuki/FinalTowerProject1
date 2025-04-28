// InteractableGeneratorEditor.cs
// Updated with customizable strategy editing + optional sprite assignment

using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using System.IO;
using System.Collections.Generic;

public class InteractableGeneratorEditor : EditorWindow
{
    // =========================
    // Basic Options
    // =========================

    private string interactableName = "NewInteractable";

    // =========================
    // Feature Toggles
    // =========================

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

    // =========================
    // Strategy Bundles and Custom Lists
    // =========================

    private StrategyBundleSO selectedStrategyBundle;
    private List<EntryStrategySO> customEntryStrategies = new List<EntryStrategySO>();
    private List<ExitStrategySO> customExitStrategies = new List<ExitStrategySO>();

    // =========================
    // Linked Assets
    // =========================

    private ScriptableObject linkedScriptableObject;
    private GameObject linkedPrefab;
    private MonoScript linkedScript;
    private Sprite assignedSprite;

    // =========================
    // Prefab Saving Options
    // =========================

    private bool saveAsPrefab = false;
    private string prefabSavePath = "Assets/_Project/Prefabs/Interactables";

    [MenuItem("Tools/Interactable Generator")]
    public static void ShowWindow()
    {
        GetWindow<InteractableGeneratorEditor>("Interactable Generator");
    }

    private void OnGUI()
    {
        DrawBasicOptions();
        DrawFeatureOptions();
        DrawStrategyOptions();
        DrawLinkedAssets();
        DrawPrefabOptions();

        GUILayout.Space(20);
        if (GUILayout.Button("Create Interactable"))
        {
            CreateInteractable();
        }
    }

    #region GUI Sections

    private void DrawBasicOptions()
    {
        GUILayout.Label("Interactable Generator", EditorStyles.boldLabel);
        interactableName = EditorGUILayout.TextField("Interactable Name", interactableName);
    }

    private void DrawFeatureOptions()
    {
        GUILayout.Space(10);
        GUILayout.Label("Feature Toggles", EditorStyles.boldLabel);
        addSpriteRenderer = EditorGUILayout.Toggle("Add Sprite Renderer", addSpriteRenderer);
        if (addSpriteRenderer)
        {
            assignedSprite = (Sprite)EditorGUILayout.ObjectField("Assigned Sprite", assignedSprite, typeof(Sprite), false);
        }

        addHighlightLight = EditorGUILayout.Toggle("Add Highlight Light", addHighlightLight);

        addDialogueFeature = EditorGUILayout.Toggle("Add Dialogue Feature", addDialogueFeature);
        addLightToggleFeature = EditorGUILayout.Toggle("Add Light Toggle Feature", addLightToggleFeature);
        addEnergyFeature = EditorGUILayout.Toggle("Add Energy Feature", addEnergyFeature);
        addDoorFeature = EditorGUILayout.Toggle("Add Door Feature", addDoorFeature);
        addPortalFeature = EditorGUILayout.Toggle("Add Portal Feature", addPortalFeature);
        addCollectibleFeature = EditorGUILayout.Toggle("Add Collectible Feature", addCollectibleFeature);
        addPuzzleUnlockFeature = EditorGUILayout.Toggle("Add Puzzle Unlock Feature", addPuzzleUnlockFeature);
        addLockedDoorFeature = EditorGUILayout.Toggle("Add Locked Door Feature", addLockedDoorFeature);
    }

    private void DrawStrategyOptions()
    {
        GUILayout.Space(10);
        GUILayout.Label("Strategy Bundles (Optional)", EditorStyles.boldLabel);
        var previousBundle = selectedStrategyBundle;
        selectedStrategyBundle = (StrategyBundleSO)EditorGUILayout.ObjectField("Strategy Bundle", selectedStrategyBundle, typeof(StrategyBundleSO), false);

        if (selectedStrategyBundle != null && selectedStrategyBundle != previousBundle)
        {
            customEntryStrategies.Clear();
            customExitStrategies.Clear();

            customEntryStrategies.AddRange(selectedStrategyBundle.entryStrategies);
            customExitStrategies.AddRange(selectedStrategyBundle.exitStrategies);
        }

        GUILayout.Space(10);
        GUILayout.Label("Entry Strategies (Editable)", EditorStyles.boldLabel);
        DrawStrategyList(customEntryStrategies);

        GUILayout.Space(5);
        GUILayout.Label("Exit Strategies (Editable)", EditorStyles.boldLabel);
        DrawStrategyList(customExitStrategies);
    }

    private void DrawLinkedAssets()
    {
        GUILayout.Space(10);
        GUILayout.Label("Linked Assets (Optional)", EditorStyles.boldLabel);
        linkedScriptableObject = (ScriptableObject)EditorGUILayout.ObjectField("Link ScriptableObject", linkedScriptableObject, typeof(ScriptableObject), false);
        linkedPrefab = (GameObject)EditorGUILayout.ObjectField("Link Prefab", linkedPrefab, typeof(GameObject), false);
        linkedScript = (MonoScript)EditorGUILayout.ObjectField("Link Script", linkedScript, typeof(MonoScript), false);
    }

    private void DrawPrefabOptions()
    {
        GUILayout.Space(10);
        GUILayout.Label("Prefab Save Options", EditorStyles.boldLabel);
        saveAsPrefab = EditorGUILayout.Toggle("Save as Prefab", saveAsPrefab);
        prefabSavePath = EditorGUILayout.TextField("Prefab Save Path", prefabSavePath);
    }

    private void DrawStrategyList<T>(List<T> strategyList) where T : ScriptableObject
    {
        if (strategyList == null) return;

        for (int i = 0; i < strategyList.Count; i++)
        {
            strategyList[i] = (T)EditorGUILayout.ObjectField($"Strategy {i + 1}", strategyList[i], typeof(T), false);
        }

        if (GUILayout.Button("+ Add Strategy"))
        {
            strategyList.Add(null);
        }
    }

    #endregion

    #region Interactable Creation

    private void CreateInteractable()
    {
        GameObject go = new GameObject(interactableName);
        var baseInteractable = go.AddComponent<InteractableBase>();
        var metadata = go.AddComponent<FeatureMetadata>();

        ApplyVisualComponents(go, baseInteractable);
        ApplyFeatures(go, metadata);
        ApplyStrategies(baseInteractable);
        ApplyTags(go);

        SaveOrSpawn(go);
    }

    private void ApplyVisualComponents(GameObject go, InteractableBase baseInteractable)
    {
        if (addSpriteRenderer)
        {
            var spriteRenderer = go.AddComponent<SpriteRenderer>();
            spriteRenderer.sortingOrder = 1;
            if (assignedSprite != null)
            {
                spriteRenderer.sprite = assignedSprite;
            }
        }

        if (addHighlightLight)
        {
            var lightObj = new GameObject("HighlightLight");
            lightObj.transform.SetParent(go.transform);
            lightObj.transform.localPosition = Vector3.zero;
            var highlightLight = lightObj.AddComponent<Light2D>();
            highlightLight.intensity = 0f;
            highlightLight.lightType = Light2D.LightType.Point;
            highlightLight.pointLightOuterRadius = 2.5f;
            baseInteractable.SetHighlightLight(highlightLight);
        }

        if (go.GetComponent<Collider2D>() == null)
        {
            var collider = go.AddComponent<CircleCollider2D>();
            collider.isTrigger = true;
            collider.radius = 0.5f;
        }
    }

    private void ApplyFeatures(GameObject go, FeatureMetadata metadata)
    {
        if (addDialogueFeature) { go.AddComponent<DialogueFeature>(); metadata.AddFeatureTag("Dialogue"); }
        if (addLightToggleFeature)
        {
            var feature = go.AddComponent<LightToggleFeature>();
            metadata.AddFeatureTag("LightToggle");
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
    }

    private void ApplyStrategies(InteractableBase baseInteractable)
    {
        if (baseInteractable.entryStrategies == null)
            baseInteractable.entryStrategies = new List<EntryStrategySO>();
        if (baseInteractable.exitStrategies == null)
            baseInteractable.exitStrategies = new List<ExitStrategySO>();

        baseInteractable.entryStrategies.AddRange(customEntryStrategies);
        baseInteractable.exitStrategies.AddRange(customExitStrategies);
    }

    private void ApplyTags(GameObject go)
    {
        if (addPortalFeature && IsTagDefined("Portal"))
            go.tag = "Portal";
        else if (addCollectibleFeature && IsTagDefined("Collectible"))
            go.tag = "Collectible";
        else if ((addPuzzleUnlockFeature || addLockedDoorFeature) && IsTagDefined("PuzzleObject"))
            go.tag = "PuzzleObject";

        int notablesLayer = LayerMask.NameToLayer("Notables");
        if (notablesLayer != -1)
            go.layer = notablesLayer;
    }

    private void SaveOrSpawn(GameObject go)
    {
        if (saveAsPrefab)
        {
            if (!Directory.Exists(prefabSavePath))
                Directory.CreateDirectory(prefabSavePath);

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

    #endregion
}