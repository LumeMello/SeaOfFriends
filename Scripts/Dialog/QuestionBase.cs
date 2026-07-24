using UnityEngine;


[CreateAssetMenu(fileName = "New Question", menuName = "Questions")]

public class QuestionBase : ScriptableObject
{
    [System.Serializable]
    public class Info
    {
        public CharacterProfile questioner;

        [TextArea(4, 8)]
        public string question;
    }

    [System.Serializable]
    public class Question
    {
        public string anser;
        public DialogueBase continuation;
    }

    public Info questionInfo;
    public Question[] questionAnswers;
}
