using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

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

    private bool isDialogueOptions;
    [SerializeField] private GameObject dialogueOptionUI;
    [SerializeField] private Text questionText;
    [SerializeField] private GameObject[] OptionsButtons;

    private bool inDialogue;
    private int optionsAmount;

    private bool isCurrentlyTyping;
    private string completeText;

    private int selectedOptionIndex = 0;
    public bool optionsActive = false;

    public void EnqueueDialogue(DialogueBase db)
    {
        if (inDialogue || optionsActive)
        {
            return;
        }
        inDialogue = true;

        dialogueBox.SetActive(true);
        dialogueInfo.Clear();

        if (db is DialogueOptions)
        {
            isDialogueOptions = true;
            DialogueOptions dialogueOptions = db as DialogueOptions;
            optionsAmount = dialogueOptions.optionsInfo.Length;
            questionText.text = dialogueOptions.questionTextInfo;
            for (int i = 0; i < optionsAmount; i++)
            {
                OptionsButtons[i].SetActive(true);
                OptionsButtons[i].transform.GetChild(0).gameObject.GetComponent<Text>().text = dialogueOptions.optionsInfo[i].buttonName;
                EventBehaviour myEventHandler = OptionsButtons[i].GetComponent<EventBehaviour>();

                myEventHandler.eventType = dialogueOptions.optionsInfo[i].myEvent;

                if (myEventHandler.eventType == EventsType.CallADialog)
                {
                    myEventHandler.dialogue = dialogueOptions.optionsInfo[i].dialogue;
                }
            }
        }
        else
        {
            isDialogueOptions = false;
        }

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
        OptionsLogic();
        inDialogue = false;
    }

    private void OptionsLogic()
    {
        if (isDialogueOptions)
        {
            dialogueOptionUI.SetActive(true);

            optionsActive = true;
            selectedOptionIndex = 0;

            UpdateOptionsVisuals();
        }
    }

    public void ChangeSelectedOption(int diretion)
    {
        selectedOptionIndex += diretion;
        if (selectedOptionIndex < 0)
        {
            selectedOptionIndex = optionsAmount - 1;
        }
        if (selectedOptionIndex >= optionsAmount)
        {
            selectedOptionIndex = 0;
        }

        UpdateOptionsVisuals();
    }

    private void UpdateOptionsVisuals()
    {
        for (int i = 0; i < optionsAmount; i++)
        {
            if (OptionsButtons[i] == null)
            {
                continue;
            }
            Image buttonImage = OptionsButtons[i].GetComponent<Image>();
            if (buttonImage!= null)
            {
                buttonImage.color = (i == selectedOptionIndex) ? Color.gray : Color.black;
            }
        }
    }

    public void ConfirmSelectedOption()
    {
        EventBehaviour myEventHandler = OptionsButtons[selectedOptionIndex].GetComponent<EventBehaviour>();
        dialogueOptionUI.SetActive(false);
        optionsActive = false;
        
        if (myEventHandler != null)
        {
            myEventHandler.StartEvent();
        }
        
    }

}
