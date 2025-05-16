// InteractableGeneratorEditor.cs
// Cleaned, Refactored, Regioned, and Fully Documented
// Author: You! (with a little help)

using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class InteractableGeneratorEditor : EditorWindow
{
    #region Fields

    // Interactable Basic Settings
    private string interactableName = "NewInteractable";
    private Sprite sprite;
    private bool addCollider = true;
    private bool addHighlightLight = true;

    // Save Settings
    private string savePath = "Assets/_Project/Prefabs/Interactables";
    private bool saveAsPrefab = false;

    // Foldout States
    private bool showFeaturesFoldout = true;
    private bool showEntryStrategiesFoldout = false;
    private bool showExitStrategiesFoldout = false;
    private bool showSaveFoldout = false;

    // Feature Toggles
    private bool addCollectibleFeature;
    private bool addDialogueFeature;
    private bool addPortalFeature;
    private bool addLightToggleFeature;
    private bool addPuzzleUnlockFeature;
    private bool addEmotionNodeFeature;
    private bool addLockedDoorFeature;
    private bool addDoorFeature;
    private bool addSwitchFeature;
    private bool addMovingPlatformFeature;

    // Feature Effects Lists
    private List<EffectStrategySO> collectibleFeatureEffects = new();
    private List<EffectStrategySO> dialogueFeatureEffects = new();
    private List<EffectStrategySO> portalFeatureEffects = new();
    private List<EffectStrategySO> lightToggleFeatureEffects = new();
    private List<EffectStrategySO> puzzleUnlockFeatureEffects = new();
    private List<EffectStrategySO> energyFeatureEffects = new();
    private List<EffectStrategySO> lockedDoorFeatureEffects = new();
    private List<EffectStrategySO> doorFeatureEffects = new();
    private List<EffectStrategySO> switchFeatureEffects = new();

    // Entry / Exit Strategies
    private List<EntryStrategySO> entryStrategies = new();
    private List<ExitStrategySO> exitStrategies = new();

    #endregion

    #region Editor Window Setup

    [MenuItem("Tools/Interactable Generator")]
    public static void ShowWindow()
    {
        GetWindow<InteractableGeneratorEditor>("Interactable Generator");
    }

    #endregion

    #region OnGUI

    private void OnGUI()
    {
        GUILayout.Label("Interactable Generator", EditorStyles.boldLabel);

        // Basic Info
        interactableName = EditorGUILayout.TextField("Name", interactableName);
        sprite = (Sprite)EditorGUILayout.ObjectField("Sprite", sprite, typeof(Sprite), false);
        addCollider = EditorGUILayout.Toggle("Add Collider2D", addCollider);
        addHighlightLight = EditorGUILayout.Toggle("Add Highlight Light", addHighlightLight);

        EditorGUILayout.Space(10);

        // Feature Toggles
        showFeaturesFoldout = EditorGUILayout.Foldout(showFeaturesFoldout, "Features");
        if (showFeaturesFoldout)
        {
            DrawFeatureToggles();
        }

        EditorGUILayout.Space(10);

        // Entry Strategies
        showEntryStrategiesFoldout = EditorGUILayout.Foldout(showEntryStrategiesFoldout, "Entry Strategies");
        if (showEntryStrategiesFoldout)
        {
            DrawEntryStrategies();
        }

        EditorGUILayout.Space(10);

        // Exit Strategies
        showExitStrategiesFoldout = EditorGUILayout.Foldout(showExitStrategiesFoldout, "Exit Strategies");
        if (showExitStrategiesFoldout)
        {
            DrawExitStrategies();
        }

        EditorGUILayout.Space(10);

        // Save Options
        showSaveFoldout = EditorGUILayout.Foldout(showSaveFoldout, "Save Settings");
        if (showSaveFoldout)
        {
            saveAsPrefab = EditorGUILayout.Toggle("Save As Prefab", saveAsPrefab);
            savePath = EditorGUILayout.TextField("Save Path", savePath);
        }

        EditorGUILayout.Space(20);

        if (GUILayout.Button("Generate Interactable"))
        {
            GenerateInteractable();
        }
    }

    #endregion

    #region Feature Toggles and Effects

    private void DrawFeatureToggles()
    {
        addCollectibleFeature = EditorGUILayout.ToggleLeft("Add Collectible Feature", addCollectibleFeature);
        if (addCollectibleFeature) DrawEffectList(collectibleFeatureEffects, "Collectible Feature Effects");

        addDialogueFeature = EditorGUILayout.ToggleLeft("Add Dialogue Feature", addDialogueFeature);
        if (addDialogueFeature) DrawEffectList(dialogueFeatureEffects, "Dialogue Feature Effects");

        addPortalFeature = EditorGUILayout.ToggleLeft("Add Portal Feature", addPortalFeature);
        if (addPortalFeature) DrawEffectList(portalFeatureEffects, "Portal Feature Effects");

        addLightToggleFeature = EditorGUILayout.ToggleLeft("Add Light Toggle Feature", addLightToggleFeature);
        if (addLightToggleFeature) DrawEffectList(lightToggleFeatureEffects, "Light Toggle Feature Effects");

        addPuzzleUnlockFeature = EditorGUILayout.ToggleLeft("Add Puzzle Unlock Feature", addPuzzleUnlockFeature);
        if (addPuzzleUnlockFeature) DrawEffectList(puzzleUnlockFeatureEffects, "Puzzle Unlock Feature Effects");

        addEmotionNodeFeature = EditorGUILayout.ToggleLeft("Add Energy Feature", addEmotionNodeFeature);
        if (addEmotionNodeFeature) DrawEffectList(energyFeatureEffects, "Energy Feature Effects");

        addLockedDoorFeature = EditorGUILayout.ToggleLeft("Add Locked Door Feature", addLockedDoorFeature);
        if (addLockedDoorFeature) DrawEffectList(lockedDoorFeatureEffects, "Locked Door Feature Effects");

        addDoorFeature = EditorGUILayout.ToggleLeft("Add Door Feature", addDoorFeature);
        if (addDoorFeature) DrawEffectList(doorFeatureEffects, "Door Feature Effects");

        addSwitchFeature = EditorGUILayout.ToggleLeft("Add Switch Feature", addSwitchFeature);
        if (addSwitchFeature) DrawEffectList(switchFeatureEffects, "Switch Feature Effects");

        addMovingPlatformFeature = EditorGUILayout.ToggleLeft("Add Moving Platform Feature", addMovingPlatformFeature);
        if (addMovingPlatformFeature)
        {
            EditorGUILayout.HelpBox("Default waypoints will be created above and below object. Customize later in inspector.", MessageType.Info);
        }

    }

    private void DrawEffectList(List<EffectStrategySO> effectsList, string label)
    {
        GUILayout.Label(label, EditorStyles.boldLabel);

        for (int i = 0; i < effectsList.Count; i++)
        {
            effectsList[i] = (EffectStrategySO)EditorGUILayout.ObjectField($"Effect {i + 1}", effectsList[i], typeof(EffectStrategySO), false);
        }

        if (GUILayout.Button("+ Add Effect", GUILayout.MaxWidth(150)))
        {
            effectsList.Add(null);
        }
    }

    #endregion

    #region Strategy Sections

    private void DrawEntryStrategies()
    {
        for (int i = 0; i < entryStrategies.Count; i++)
        {
            entryStrategies[i] = (EntryStrategySO)EditorGUILayout.ObjectField($"Entry {i + 1}", entryStrategies[i], typeof(EntryStrategySO), false);
        }

        if (GUILayout.Button("+ Add Entry Strategy", GUILayout.MaxWidth(200)))
        {
            entryStrategies.Add(null);
        }
    }

    private void DrawExitStrategies()
    {
        for (int i = 0; i < exitStrategies.Count; i++)
        {
            exitStrategies[i] = (ExitStrategySO)EditorGUILayout.ObjectField($"Exit {i + 1}", exitStrategies[i], typeof(ExitStrategySO), false);
        }

        if (GUILayout.Button("+ Add Exit Strategy", GUILayout.MaxWidth(200)))
        {
            exitStrategies.Add(null);
        }
    }

    #endregion

    #region Generate Interactable

    private void GenerateInteractable()
    {
        GameObject go = new GameObject(interactableName);

        var spriteRenderer = go.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = sprite;

        if (addCollider)
        {
            var collider = go.AddComponent<CircleCollider2D>();
            collider.isTrigger = true;
        }

        var interactable = go.AddComponent<InteractableBase>();
        //interactable.entryStrategies = new List<EntryStrategySO>(entryStrategies);
        //interactable.exitStrategies = new List<ExitStrategySO>(exitStrategies);

        var metadata = go.AddComponent<FeatureMetadata>();

        ApplyFeatures(go, metadata);

        if (addHighlightLight)
        {
            SetupHighlight(go);
        }

        if (saveAsPrefab)
        {
            SaveAsPrefab(go);
        }
    }

    #endregion

    #region Apply Features

    private void ApplyFeatures(GameObject go, FeatureMetadata metadata)
    {
        if (addCollectibleFeature)
        {
            var feature = go.AddComponent<CollectibleFeature>();
            metadata.AddFeatureTag("Collectible");
            feature.SetFeatureEffects(collectibleFeatureEffects);
        }

        if (addDialogueFeature)
        {
            var feature = go.AddComponent<DialogueFeature>();
            metadata.AddFeatureTag("Dialogue");
            feature.SetFeatureEffects(dialogueFeatureEffects);
        }

        if (addPortalFeature)
        {
            var feature = go.AddComponent<PortalFeature>();
            metadata.AddFeatureTag("Portal");
            feature.SetFeatureEffects(portalFeatureEffects);
        }

        if (addLightToggleFeature)
        {
            var feature = go.AddComponent<LightToggleFeature>();
            metadata.AddFeatureTag("LightToggle");
            feature.SetFeatureEffects(lightToggleFeatureEffects);

            // Setup a dedicated Toggleable Light for toggling
            GameObject toggleLightObj = new GameObject("ToggleableLight");
            toggleLightObj.transform.SetParent(go.transform);
            toggleLightObj.transform.localPosition = Vector3.zero;

            var toggleLight2D = toggleLightObj.AddComponent<UnityEngine.Rendering.Universal.Light2D>();
            toggleLight2D.intensity = 0f;
            toggleLight2D.pointLightOuterRadius = 2.5f;

            feature.SetToggleLight(toggleLight2D);
        }

        if (addSwitchFeature)
        {
            var feature = go.AddComponent<SwitchFeature>();
            metadata.AddFeatureTag("Switch");
            feature.SetFeatureEffects(switchFeatureEffects);
        }

        if (addMovingPlatformFeature)
        {
            var feature = go.AddComponent<MovingPlatformFeature>();
            metadata.AddFeatureTag("Platform");

            // Create the visual object that actually moves
            GameObject platformVisual = new GameObject("PlatformVisual");
            platformVisual.transform.SetParent(go.transform);
            platformVisual.transform.localPosition = Vector3.zero;

            // Add sprite renderer
            var sr = platformVisual.AddComponent<SpriteRenderer>();
            sr.sprite = sprite;

            // Add trigger collider for interaction
            var triggerCol = platformVisual.AddComponent<CircleCollider2D>();
            triggerCol.isTrigger = true;

            // Add physics box collider for riding/collision
            var boxCol = platformVisual.AddComponent<BoxCollider2D>();
            boxCol.isTrigger = false;

            // Add highlight light
            GameObject lightObj = new GameObject("HighlightLight");
            lightObj.transform.SetParent(platformVisual.transform);
            lightObj.transform.localPosition = Vector3.zero;

            var light2D = lightObj.AddComponent<UnityEngine.Rendering.Universal.Light2D>();
            light2D.intensity = 0f;
            light2D.pointLightOuterRadius = 1.5f;

            // Register light and trigger with InteractableBase
            var interactable = go.GetComponent<InteractableBase>();
            interactable.SetHighlightLight(light2D);
            //interactable.SetTriggerColliderOverride(triggerCol);

            // Create two default waypoints
            GameObject wp0 = new GameObject("Waypoint_0");
            wp0.transform.SetParent(go.transform);
            wp0.transform.localPosition = Vector3.zero;

            GameObject wp1 = new GameObject("Waypoint_1");
            wp1.transform.SetParent(go.transform);
            wp1.transform.localPosition = Vector3.up * 2f;

            feature.SetWaypoints(new Transform[] { wp0.transform, wp1.transform });
            feature.SetPlatformTransform(platformVisual.transform);
        }



        if (addPuzzleUnlockFeature)
        {
            var feature = go.AddComponent<PuzzleUnlockFeature>();
            metadata.AddFeatureTag("PuzzleUnlock");
            feature.SetFeatureEffects(puzzleUnlockFeatureEffects);
        }

        if (addEmotionNodeFeature)
        {
            var feature = go.AddComponent<EmotionNodeFeature>();
            metadata.AddFeatureTag("EmotionNode");
            feature.SetFeatureEffects(energyFeatureEffects);

            GameObject glowObj = new GameObject("EmotionGlowLight");
            glowObj.transform.SetParent(go.transform);
            glowObj.transform.localPosition = Vector3.zero;

            var glowLight = glowObj.AddComponent<UnityEngine.Rendering.Universal.Light2D>();
            glowLight.intensity = 0f;
            glowLight.pointLightOuterRadius = 2.5f;

            feature.SetEmotionLight(glowLight);
        }

        if (addLockedDoorFeature)
        {
            var feature = go.AddComponent<LockedDoorFeature>();
            metadata.AddFeatureTag("LockedDoor");
            feature.SetFeatureEffects(lockedDoorFeatureEffects);

            // Setup Animator
            Animator animator = go.GetComponent<Animator>();
            if (animator == null)
            {
                animator = go.AddComponent<Animator>();
            }
            feature.SetDoorAnimator(animator);
        }

        if (addDoorFeature)
        {
            var feature = go.AddComponent<DoorFeature>();
            metadata.AddFeatureTag("Door");
            feature.SetFeatureEffects(doorFeatureEffects);

            // Setup Animator
            Animator animator = go.GetComponent<Animator>();
            if (animator == null)
            {
                animator = go.AddComponent<Animator>();
            }
            feature.SetDoorAnimator(animator);
        }
    }

    #endregion

    #region Highlight Light Setup

    private void SetupHighlight(GameObject go)
    {
        GameObject lightObj = new GameObject("HighlightLight");
        lightObj.transform.SetParent(go.transform);
        lightObj.transform.localPosition = Vector3.zero;

        var light2D = lightObj.AddComponent<UnityEngine.Rendering.Universal.Light2D>();
        light2D.intensity = 0f;
        light2D.pointLightOuterRadius = 1.5f;

        go.GetComponent<InteractableBase>().SetHighlightLight(light2D);
    }

    #endregion

    #region Save Prefab

    private void SaveAsPrefab(GameObject go)
    {
        if (!Directory.Exists(savePath))
        {
            Directory.CreateDirectory(savePath);
        }

        string prefabPath = Path.Combine(savePath, interactableName + ".prefab");
        PrefabUtility.SaveAsPrefabAsset(go, prefabPath);
        DestroyImmediate(go);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"[Interactable Generator] Saved prefab at {prefabPath}");
    }

    #endregion
}
