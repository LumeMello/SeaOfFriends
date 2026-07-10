using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerScript : MonoBehaviour
{
    public void Load_New_Scene(string name)
    {
        SceneManager.LoadScene(name, LoadSceneMode.Single);
    }
}
