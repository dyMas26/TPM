
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public void OpenLearning()
    {
        SceneManager.LoadScene("Learning");
    }

    public void OpenExercise()
    {
        SceneManager.LoadScene("Exercise");
    }

    public void OpenGameplay()
    {
        SceneManager.LoadScene("Gameplay");
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
