
using UnityEngine;
using UnityEngine.SceneManagement;

public class LearningController : MonoBehaviour
{
    public GameObject menuPanel;
    public GameObject diskonPanel;
    public GameObject untungRugiPanel;

    public void OpenDiskon()
    {
        menuPanel.SetActive(false);
        diskonPanel.SetActive(true);
    }

    public void OpenUntungRugi()
    {
        menuPanel.SetActive(false);
        untungRugiPanel.SetActive(true);
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void BackToLearningMenu()
    {
        diskonPanel.SetActive(false);
        untungRugiPanel.SetActive(false);
        menuPanel.SetActive(true);
    }
}
