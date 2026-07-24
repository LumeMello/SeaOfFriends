using UnityEngine;
using static CharacterBase;

public class TestScript : MonoBehaviour
{
    public DialogueBase dialogue;
    public CharacterBase attacker;
    public CharacterBase defender;
    public int id;
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
