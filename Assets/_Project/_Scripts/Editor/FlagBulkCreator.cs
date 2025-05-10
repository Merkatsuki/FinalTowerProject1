using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class FlagBulkCreator : EditorWindow
{
    private string folderPath = "Assets/_Project/_Scripts/GameState/Flags";
    private List<string> flagNames = new() { "FlagName1", "FlagName2" };
    private FlagSO.FlagType selectedFlagType = FlagSO.FlagType.Bool;

    [MenuItem("Tools/Flags/Bulk Create Flags")]
    public static void ShowWindow()
    {
        GetWindow<FlagBulkCreator>("Bulk Flag Creator");
    }

    private void OnGUI()
    {
        GUILayout.Label("Bulk Flag Creator", EditorStyles.boldLabel);

        EditorGUILayout.LabelField("Save Folder:");
        folderPath = EditorGUILayout.TextField(folderPath);

        selectedFlagType = (FlagSO.FlagType)EditorGUILayout.EnumPopup("Flag Type:", selectedFlagType);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Flag Names (one per line):");

        string namesRaw = string.Join("\n", flagNames);
        namesRaw = EditorGUILayout.TextArea(namesRaw, GUILayout.Height(100));
        flagNames = new List<string>(namesRaw.Split('\n'));

        if (GUILayout.Button("Create Flags"))
        {
            CreateFlags();
        }
    }

    private void CreateFlags()
    {
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        foreach (string rawName in flagNames)
        {
            string trimmedName = rawName.Trim();
            if (string.IsNullOrEmpty(trimmedName)) continue;

            FlagSO newFlag = ScriptableObject.CreateInstance<FlagSO>();
            newFlag.name = trimmedName;
            newFlag.displayName = trimmedName;
            newFlag.flagType = selectedFlagType;

            string path = Path.Combine(folderPath, trimmedName + ".asset");
            AssetDatabase.CreateAsset(newFlag, path);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log($"Created {flagNames.Count} flags in {folderPath}.");
    }
}