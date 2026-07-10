using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "New Profile", menuName = "Character Profile")]
public class CharacterProfile : ScriptableObject
{
    public string myname;
    private Sprite myPortrait;
    public AudioClip myVoice;
    public Font myFont;
    public Color myFontColor;


    public bool haveOutline;
    public Outline outline;

    public Sprite MyPortrait
    {
        get
        {
            SetEmotionType(Emotion);

            return myPortrait;
        }
    }

    [System.Serializable]
    public class EmotionPortrait
    {
        public Sprite standard;
        public Sprite happy;
        public Sprite sad;
        public Sprite angry;
    }

    public EmotionPortrait emotionPortrait;

    public EmotionType Emotion { get; set; }

    public void SetEmotionType(EmotionType newEmotion)
    {
        Emotion = newEmotion;
        switch (Emotion)
        {
            case EmotionType.Standard:
                myPortrait = emotionPortrait.standard;
                break;
            case EmotionType.Sad:
                myPortrait = emotionPortrait.sad;
                break;
            case EmotionType.Happy:
                myPortrait = emotionPortrait.happy;
                break;
            case EmotionType.Angry:
                myPortrait = emotionPortrait.angry;
                break;
        }
    }
}

public enum EmotionType
{
    Standard,
    Happy,
    Sad,
    Angry
}