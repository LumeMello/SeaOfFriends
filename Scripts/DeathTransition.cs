using UnityEngine;

public class DeathTransition : MonoBehaviour
{
    public static DeathTransition instance;
    public Animator an {  get; private set; }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(instance);
        }

        an = GetComponent<Animator>();
    }

    public void SetTransition(bool active)
    {
        an.SetBool("active", active);
    }
}
