using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Character", menuName = "Character Base")]
public class CharacterBase : ScriptableObject
{
    [System.Serializable]
    public class Info
    {
        public CharacterProfile character;
        public int Totallife;
        public int currentLife;
        public int shield;
        public List<CardBase> itens = new List<CardBase>();

        [TextArea(4, 8)]
        public string myDescription;
    }

    public Info dialogueInfo;
}
