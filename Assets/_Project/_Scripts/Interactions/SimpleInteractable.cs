using UnityEngine;

public class SimpleInteractable : InteractableBase
{
    public override void OnInteract()
    {
        Debug.Log("Interacted with: " + gameObject.name);
    }

    public override void OnFocusEnter()
    {
        Debug.Log("Focused: " + gameObject.name);
    }

    public override void OnFocusExit()
    {
        Debug.Log("Focus left: " + gameObject.name);
    }
}
