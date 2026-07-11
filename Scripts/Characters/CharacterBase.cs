using System.Collections.Generic;
using UnityEngine;
public class CharacterBase : MonoBehaviour
{
    [System.Serializable]
    public class Usages
    {
        public CardBase cardId;
        public int uses;

        public override bool Equals(object other)
        {
            if (other == null) return false;
            Usages otherUsage = other as Usages;
            if (otherUsage == null || otherUsage.cardId == null) return false;
            else return Equals(otherUsage);
        } 

        public override int GetHashCode()
        {
            return cardId.cardInfo.myId;
        }

        public bool Equals(Usages other)
        {
            if (other == null) return false;
            return (this.cardId.cardInfo.myId.Equals(other.cardId.cardInfo.myId));
        }
    }

    [System.Serializable]
    public class Info
    {
        public CharacterProfile character;
        public int Totallife;
        public int currentLife;
        public int shield;
        public List<Usages> itensUsage = new List<Usages>();

        [TextArea(4, 8)]
        public string myDescription;

        public void AddItem(CardBase cardBase, int usages)
        {
            Usages use = new Usages();
            use.cardId = cardBase;
            use.uses = usages;
            itensUsage.Add(use);
        }

        public void RemoveItem(CardBase cardBase)
        {
            Usages use = new Usages();
            use.cardId = cardBase;
            use.uses = 0;
            itensUsage.Remove(use);
        }
    }



    public Info characterInfo;

    
}
