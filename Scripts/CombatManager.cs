using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using static CharacterBase;
using static UnityEditor.Progress;

public class CombatManager : MonoBehaviour
{
    public static CombatManager instance;
    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("fix this" + gameObject.name);
        }
        else
        {
            instance = this;
        }
    }

    public void AttackAction(CharacterBase attacker, CharacterBase defender, CardBase Item, int maxExaust = 1)
    {


        foreach (Usages item in attacker.characterInfo.itensUsage.ToList())
        {
            if (item.cardId == Item && item.uses < maxExaust)
            {
                item.uses += 1;
                break;
            }
            else
            {
                Debug.Log("item já exaustado");
            }
        }
    }

    public void RestAction(CharacterBase character)
    {
        foreach (Usages item in character.characterInfo.itensUsage.ToList())
        {
            item.uses = 0;
        }
    }


}
