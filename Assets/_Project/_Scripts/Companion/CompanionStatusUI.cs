using UnityEngine;
using System.Collections.Generic;

public class CompanionStatusUI : MonoBehaviour
{
    [System.Serializable]
    public struct StateIcon
    {
        public CompanionStateType state;
        public GameObject iconObject;  // Already present in the scene under iconAnchor
    }

    [Header("Setup")]
    [SerializeField] private List<StateIcon> stateIcons;  // Manual setup in Inspector

    private Dictionary<CompanionStateType, GameObject> _iconLookup = new();
    private GameObject _currentIcon;

    private void Awake()
    {
        foreach (var entry in stateIcons)
        {
            if (entry.iconObject == null) continue;

            entry.iconObject.SetActive(false);  // Hide on start
            _iconLookup[entry.state] = entry.iconObject;
        }
    }

    public void UpdateIcon(CompanionStateType newState)
    {
        if (_currentIcon != null)
        {
            _currentIcon.SetActive(false);
            _currentIcon = null;
        }

        if (_iconLookup.TryGetValue(newState, out GameObject newIcon))
        {
            newIcon.SetActive(true);
            _currentIcon = newIcon;
        }
        else
        {
            Debug.LogWarning($"No icon object assigned for state: {newState}");
        }
    }
}
