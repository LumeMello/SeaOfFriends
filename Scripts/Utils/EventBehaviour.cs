using UnityEngine;

public class EventBehaviour : MonoBehaviour
{
    

    public EventsType eventType;
    public DialogueBase dialogue;

    public void StartEvent()
    {
        if (eventType == EventsType.Test)
        {
            TestEvent();
        }
        else if (eventType == EventsType.CallADialog)
        {
            TriggerDialogue();
        }
    }

    private void TestEvent()
    {
        Debug.Log("Sucess");
    }

    private void TriggerDialogue()
    {
        DialogueManager.instance.EnqueueDialogue(dialogue);
    }
}
public enum EventsType
{
    Test,
    CallADialog
}