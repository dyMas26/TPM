using UnityEngine;
using UnityEngine.SceneManagement;

public class LearningController : MonoBehaviour
{
    [Header("Menu Panel")]
    [SerializeField] private GameObject learningMenuPanel;

    [Header("Discount Panels")]
    [SerializeField] private GameObject[] discountPanels;

    [Header("Profit Loss Panels")]
    [SerializeField] private GameObject[] profitLossPanels;

    private GameObject[] currentPanels;
    private int currentIndex = 0;

    private void Start()
    {
        ShowMenu();
    }

    public void ShowMenu()
    {
        learningMenuPanel.SetActive(true);

        SetPanelsActive(discountPanels, false);
        SetPanelsActive(profitLossPanels, false);

        currentPanels = null;
        currentIndex = 0;
    }

    public void ShowDiscount()
    {
        learningMenuPanel.SetActive(false);

        currentPanels = discountPanels;
        currentIndex = 0;

        SetPanelsActive(profitLossPanels, false);
        UpdatePanels();
    }

    public void ShowProfitLoss()
    {
        learningMenuPanel.SetActive(false);

        currentPanels = profitLossPanels;
        currentIndex = 0;

        SetPanelsActive(discountPanels, false);
        UpdatePanels();
    }

    public void NextPanel()
    {
        if (currentPanels == null)
            return;

        if (currentIndex < currentPanels.Length - 1)
        {
            currentIndex++;
            UpdatePanels();
        }
    }

    public void PreviousPanel()
    {
        if (currentPanels == null)
            return;

        if (currentIndex > 0)
        {
            currentIndex--;
            UpdatePanels();
        }
    }

    public void BackToMenu()
    {
        ShowMenu();
    }

    public void OpenMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    private void UpdatePanels()
    {
        SetPanelsActive(discountPanels, false);
        SetPanelsActive(profitLossPanels, false);

        if (currentPanels != null &&
            currentIndex >= 0 &&
            currentIndex < currentPanels.Length)
        {
            currentPanels[currentIndex].SetActive(true);
        }
    }

    private void SetPanelsActive(GameObject[] panels, bool active)
    {
        foreach (GameObject panel in panels)
        {
            if (panel != null)
            {
                panel.SetActive(active);
            }
        }
    }
}