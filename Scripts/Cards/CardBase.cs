using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Cards")]
public class CardBase : ScriptableObject
{
    [System.Serializable]
    public class Info
    {
        public string myName;
        public int myId;
        public CardType type;
        public int attackDices;

        public bool critPlus;
        public int extraLife;
        public int extraArmor;

        public Sprite myImage;

        [TextArea(4, 8)]
        public string myDescription;
    }

    public Info cardInfo;

    public int adicionalFunction;

    public override bool Equals(object other)
    {
        if (other == null) return false;
        CardBase otherBase = other as CardBase;
        if (otherBase == null) return false;
        else return Equals(otherBase);
    }

    public override int GetHashCode()
    {
        return cardInfo.myId;
    }

    public bool Equals(CardBase other)
    {
        if (other == null) return false;
        return (this.cardInfo.myId.Equals(other.cardInfo.myId));
    }
}

public enum CardType
{
    Range,
    Close,
    Item,
    Ability
}
