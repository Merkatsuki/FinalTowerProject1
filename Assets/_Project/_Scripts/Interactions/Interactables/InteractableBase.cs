using UnityEngine;

public abstract class InteractableBase : MonoBehaviour
{
    [Tooltip("Text shown in the interaction prompt UI.")]
    public string promptMessage = "Press [E] to interact";

    public virtual bool CanInteract => true;

    // Called when the player triggers an interaction
    public virtual void OnInteract() { }

    // Called when the player is now targeting this object
    public virtual void OnFocusEnter() { }

    // Called when the player stops targeting this object
    public virtual void OnFocusExit() { }

    public virtual void SetHighlighted(bool isHighlighted) { }
}
