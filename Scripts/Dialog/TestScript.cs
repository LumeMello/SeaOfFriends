using UnityEngine;
using static CharacterBase;

public class TestScript : MonoBehaviour
{
    public DialogueBase dialogue;
    private bool haveAlready = false;
    
    public int id;
    public void TriggerDialogue()
    {
        DialogueManager.instance.EnqueueDialogue(dialogue);
    }

    

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if(!haveAlready)
            {
                TriggerDialogue();
                haveAlready = true;
            }
            
        }
        
    }
}
