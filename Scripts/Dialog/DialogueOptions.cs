using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "New Dialogue Options", menuName = "DialoguesOptions")]
public class DialogueOptions : DialogueBase
{
    [System.Serializable]
    public class Options
    {
        public string buttonName;
        public EventsType myEvent;
        public DialogueBase dialogue;
    }
    public Options[] optionsInfo;

    [TextArea(2,10)]
    public string questionTextInfo;


}
