using UnityEngine;

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

    public void AttackAction(CharacterBase attacker, CharacterBase defender, CardBase Item)
    {
        ///colocar lógica de ataque
    }


}
