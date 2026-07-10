using UnityEngine;

[CreateAssetMenu(fileName ="New Dialogue", menuName = "Dialogues")]
public class DialogueBase : ScriptableObject
{
    [System.Serializable]
    public class Info {
        public CharacterProfile character;
        
        [TextArea(4, 8)]
        public string mytext;

        public EmotionType characterEmotion;

        public void ChangeEmotion()
        {
            character.Emotion = characterEmotion;

        }
    }

    public Info[] dialogueInfo;
    
}
