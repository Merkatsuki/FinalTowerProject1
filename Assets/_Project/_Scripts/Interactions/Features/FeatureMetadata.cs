using System.Collections.Generic;
using UnityEngine;

public class FeatureMetadata : MonoBehaviour
{
    [Tooltip("List of tags representing features this interactable has.")]
    public List<string> featureTags = new List<string>();

    public bool HasFeatureTag(string tag)
    {
        return featureTags.Contains(tag);
    }

    public void AddFeatureTag(string tag)
    {
        if (!featureTags.Contains(tag))
        {
            featureTags.Add(tag);
        }
    }
}
