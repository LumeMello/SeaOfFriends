using System.Collections.Generic;
using UnityEngine;
public class CharacterBase : MonoBehaviour
{    

    [System.Serializable]
    public class Info
    {
        public CharacterProfile character;
        public int Totallife;
        public int currentLife;
        public int shield;

        [TextArea(4, 8)]
        public string myDescription;

        
    }



    public Info characterInfo;

    
}
