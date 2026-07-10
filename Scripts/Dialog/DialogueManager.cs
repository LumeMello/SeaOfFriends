using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    private readonly List<char> punctuationCharacters = new List<char>
    {
        '.',
        ',',
        '!',
        '?'
    };


    public static DialogueManager instance;
    private void Awake()
    {
        if(instance != null)
        {
            Debug.LogWarning("fix this" + gameObject.name);
        }
        else
        {
            instance = this;
        }
    }

    public GameObject dialogueBox;

    public Text dialogueName;
    public Text dialogueText;
    public Image dialoguePortrait;
    [SerializeField]private float delay;
    [SerializeField] private float punctuationPauseTime;
    [SerializeField] private Outline dialogueOutline;
    [SerializeField]private Color transparent;

    public Queue<DialogueBase.Info> dialogueInfo = new Queue<DialogueBase.Info>();

    private bool isCurrentlyTyping;
    private string completeText;

    public void EnqueueDialogue(DialogueBase db)
    {
        dialogueBox.SetActive(true);
        dialogueInfo.Clear();

        foreach (DialogueBase.Info info in db.dialogueInfo)
        {
            dialogueInfo.Enqueue(info);
        }

        DequeueDialogue();
    }

    public void DequeueDialogue()
    {
        if (isCurrentlyTyping == true)
        {
            CompleteText();
            StopAllCoroutines();
            isCurrentlyTyping = false;
            return;
        }

        if (dialogueInfo.Count == 0)
        {
            EndofDialogue();
            return;
        }

        DialogueBase.Info info = dialogueInfo.Dequeue();

        completeText = info.mytext;

        dialogueName.text = info.character.myname;
        dialogueText.text = "";
        dialogueText.font = info.character.myFont;
        dialogueText.color = info.character.myFontColor;

        info.ChangeEmotion();
        
        if (info.character.haveOutline)
        {
            dialogueOutline.effectColor = info.character.outline.effectColor;
            dialogueOutline.effectDistance = info.character.outline.effectDistance;
        }
        else
        {
            dialogueOutline.effectColor = transparent;
        }
        dialoguePortrait.sprite = info.character.MyPortrait;

        StartCoroutine(TypeText(info));
    }

    private bool CheckPunctuation(char c)
    {
        if (punctuationCharacters.Contains(c))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    IEnumerator TypeText(DialogueBase.Info info)
    {
        isCurrentlyTyping = true;
        
        foreach(char c in info.mytext.ToCharArray())
        {
            yield return new WaitForSeconds(delay);
            dialogueText.text += c;
            yield return null;
            

            if (CheckPunctuation(c)){
                yield return new WaitForSeconds(punctuationPauseTime);
            }
            else
            {
                AudioManager.instance.PlayClip(info.character.myVoice);
            }

        }
        isCurrentlyTyping = false;
    }

    private void CompleteText()
    {
        dialogueText.text = completeText;
    }

    public void EndofDialogue()
    {
        dialogueBox.SetActive(false);
    }

}
