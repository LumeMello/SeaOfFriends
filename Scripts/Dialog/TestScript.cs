using UnityEngine;

public class TestScript : MonoBehaviour
{
    public DialogueBase dialogue;

    public void TriggerDialogue()
    {
        DialogueManager.instance.EnqueueDialogue(dialogue);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (DialogueManager.instance.dialogueBox.activeInHierarchy == true)
            {
                DialogueManager.instance.DequeueDialogue();
            }
            else
            {
                TriggerDialogue();
            }
            
        }
    }
}
