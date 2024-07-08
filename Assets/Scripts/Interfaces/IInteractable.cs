using UnityEngine;

public interface IInteractable
{
    public void Interact(GameObject initiator);
    public void StopInteraction();
    public void HighlightInteractable();
    public void UnhighlightInteractable();
}
