using UnityEngine;

[DisallowMultipleComponent]
public class PuzzleObject : MonoBehaviour
{
    [SerializeField] private string objectId;

    public string GetId() => objectId;

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (string.IsNullOrWhiteSpace(objectId))
            objectId = gameObject.name.Replace(" ", "_") + "_" + GetInstanceID();
    }
#endif
}