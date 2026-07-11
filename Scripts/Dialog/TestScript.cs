using UnityEngine;
using static CharacterBase;

public class TestScript : MonoBehaviour
{
    public DialogueBase dialogue;
    public CharacterBase attacker;
    public CharacterBase defender;
    private CardBase Item;
    public int id;
    public void TriggerDialogue()
    {
        DialogueManager.instance.EnqueueDialogue(dialogue);
    }

    public void TriggerAttack()
    {
        CombatManager.instance.AttackAction(attacker,defender,Item);
    }

    public void TriggerRest()
    {
        CombatManager.instance.RestAction(attacker);
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
        if (Input.GetKeyDown(KeyCode.C))
        {
            foreach (Usages item in attacker.characterInfo.itensUsage)
            {
                if (item.cardId.cardInfo.myId == id)
                {
                    Item = item.cardId;
                    TriggerAttack();
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.V))
        {
            TriggerRest();
        }
    }
}
