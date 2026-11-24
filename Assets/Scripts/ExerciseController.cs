
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections.Generic;

[System.Serializable]
public class ExerciseQuestion
{
    [Header("Pertanyaan")]
    [TextArea(2, 5)]
    public string questionText;

    [Header("Pilihan Jawaban")]
    public string answerA;
    public string answerB;
    public string answerC;
    public string answerD;

    [Header("Kunci Jawaban")]
    [Range(0, 3)]
    public int correctAnswerIndex;

    [Header("Pembahasan")]
    [TextArea(3, 8)]
    public string explanationText;

    public Sprite explanationImage;
}

public class ExerciseController : MonoBehaviour
{
    [Header("BANK SOAL DISKON")]
    [SerializeField]
    private ExerciseQuestion[] discountQuestions =
        new ExerciseQuestion[10];

    [Header("BANK SOAL UNTUNG RUGI")]
    [SerializeField]
    private ExerciseQuestion[] profitLossQuestions =
        new ExerciseQuestion[10];


    [Header("PANELS")]
    [SerializeField] private GameObject exerciseMenuPanel;
    [SerializeField] private GameObject exercisePanel;
    [SerializeField] private GameObject resultPanel;
    [SerializeField] private GameObject feedbackPanel;


    [Header("MENU BUTTONS")]
    [SerializeField] private Button discountExerciseButton;
    [SerializeField] private Button profitLossExerciseButton;
    [SerializeField] private Button mainMenuButton;


    [Header("EXERCISE UI")]
    [SerializeField] private TMP_Text exerciseHeader;
    [SerializeField] private TMP_Text questionNumberText;
    [SerializeField] private TMP_Text progressText;
    [SerializeField] private TMP_Text questionText;

    [SerializeField] private Button[] answerButtons =
        new Button[4];

    [SerializeField] private Button nextButton;
    [SerializeField] private Button exerciseBackButton;


    [Header("RESULT UI")]
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text correctAnswerText;
    [SerializeField] private TMP_Text wrongAnswerText;

    [SerializeField] private Button feedbackButton;
    [SerializeField] private Button exerciseMenuButton;


    [Header("FEEDBACK UI")]
    [SerializeField] private TMP_Text[] feedbackContentTexts =
        new TMP_Text[5];

    [SerializeField] private Image[] explanationImages =
        new Image[5];

    [SerializeField] private Button backToResultButton;


    [Header("NEXT BUTTON COLORS")]
    [SerializeField] private Color normalYellow;
    [SerializeField] private Color highlightedYellow;
    [SerializeField] private Color pressedYellow;
    [SerializeField] private Color selectedYellow;

    [SerializeField] private Color normalGreen;
    [SerializeField] private Color highlightedGreen;
    [SerializeField] private Color pressedGreen;
    [SerializeField] private Color selectedGreen;


    private List<ExerciseQuestion> selectedQuestions =
        new List<ExerciseQuestion>();

    private int[] playerAnswers = new int[5];

    private int currentQuestionIndex;
    private int selectedAnswerIndex = -1;
    private int correctCount;
    private int wrongCount;
    private int finalScore;

    private bool isDiscountExercise;


    private void Awake()
    {
        discountExerciseButton.onClick.AddListener(StartDiscountExercise);
        profitLossExerciseButton.onClick.AddListener(StartProfitLossExercise);
        mainMenuButton.onClick.AddListener(OpenMainMenu);

        nextButton.onClick.AddListener(NextQuestion);
        exerciseBackButton.onClick.AddListener(BackToExerciseMenu);

        feedbackButton.onClick.AddListener(ShowFeedback);
        exerciseMenuButton.onClick.AddListener(BackToExerciseMenu);
        backToResultButton.onClick.AddListener(BackToResult);

        for (int i = 0; i < answerButtons.Length; i++)
        {
            int buttonIndex = i;

            answerButtons[i].onClick.AddListener(
                () => SelectAnswer(buttonIndex)
            );
        }

        SetNextButtonColors();
    }


    private void Start()
    {
        ShowExerciseMenu();
    }


    // =====================================
    // MENU LATIHAN
    // =====================================

    public void ShowExerciseMenu()
    {
        exerciseMenuPanel.SetActive(true);
        exercisePanel.SetActive(false);
        resultPanel.SetActive(false);
        feedbackPanel.SetActive(false);
    }


    public void StartDiscountExercise()
    {
        isDiscountExercise = true;
        StartExercise(discountQuestions);
    }


    public void StartProfitLossExercise()
    {
        isDiscountExercise = false;
        StartExercise(profitLossQuestions);
    }


    public void OpenMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }


    // =====================================
    // MEMULAI LATIHAN
    // =====================================

    private void StartExercise(ExerciseQuestion[] questionBank)
    {
        List<ExerciseQuestion> availableQuestions =
            new List<ExerciseQuestion>();

        foreach (ExerciseQuestion question in questionBank)
        {
            if (question != null)
            {
                availableQuestions.Add(question);
            }
        }

        if (availableQuestions.Count < 5)
        {
            Debug.LogError(
                "Bank soal harus memiliki minimal 5 soal yang sudah diisi."
            );

            return;
        }

        ShuffleQuestions(availableQuestions);

        selectedQuestions =
            availableQuestions.GetRange(0, 5);

        currentQuestionIndex = 0;
        selectedAnswerIndex = -1;

        correctCount = 0;
        wrongCount = 0;
        finalScore = 0;

        for (int i = 0; i < playerAnswers.Length; i++)
        {
            playerAnswers[i] = -1;
        }

        exerciseMenuPanel.SetActive(false);
        exercisePanel.SetActive(true);
        resultPanel.SetActive(false);
        feedbackPanel.SetActive(false);

        exerciseHeader.text = isDiscountExercise
            ? "LATIHAN - DISKON"
            : "LATIHAN - UNTUNG RUGI";

        DisplayQuestion();
    }


    private void ShuffleQuestions(List<ExerciseQuestion> questions)
    {
        for (int i = questions.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);

            ExerciseQuestion temporary = questions[i];

            questions[i] = questions[randomIndex];
            questions[randomIndex] = temporary;
        }
    }


    // =====================================
    // MENAMPILKAN SOAL
    // =====================================

    private void DisplayQuestion()
    {
        ExerciseQuestion question =
            selectedQuestions[currentQuestionIndex];

        questionNumberText.text =
            "SOAL - " + (currentQuestionIndex + 1);

        progressText.text =
            (currentQuestionIndex + 1) + "/5";

        questionText.text = question.questionText;

        answerButtons[0].GetComponentInChildren<TMP_Text>().text =
            "A. " + question.answerA;

        answerButtons[1].GetComponentInChildren<TMP_Text>().text =
            "B. " + question.answerB;

        answerButtons[2].GetComponentInChildren<TMP_Text>().text =
            "C. " + question.answerC;

        answerButtons[3].GetComponentInChildren<TMP_Text>().text =
            "D. " + question.answerD;

        selectedAnswerIndex = -1;

        nextButton.interactable = false;

        TMP_Text nextText =
            nextButton.GetComponentInChildren<TMP_Text>();

        if (currentQuestionIndex == 4)
        {
            nextText.text = "KIRIM";
            SetNextButtonColors(true);
        }
        else
        {
            nextText.text = "SELANJUTNYA";
            SetNextButtonColors(false);
        }
    }


    // =====================================
    // MEMILIH JAWABAN
    // =====================================

    private void SelectAnswer(int answerIndex)
    {
        selectedAnswerIndex = answerIndex;

        nextButton.interactable = true;
    }


    // =====================================
    // TOMBOL SELANJUTNYA DAN KIRIM
    // =====================================

    private void NextQuestion()
    {
        if (selectedAnswerIndex == -1)
        {
            return;
        }

        playerAnswers[currentQuestionIndex] =
            selectedAnswerIndex;

        if (currentQuestionIndex < 4)
        {
            currentQuestionIndex++;

            DisplayQuestion();
        }
        else
        {
            CalculateResult();
            ShowResult();
        }
    }


    // =====================================
    // MENGHITUNG NILAI
    // =====================================

    private void CalculateResult()
    {
        correctCount = 0;
        wrongCount = 0;

        for (int i = 0; i < selectedQuestions.Count; i++)
        {
            if (playerAnswers[i] ==
                selectedQuestions[i].correctAnswerIndex)
            {
                correctCount++;
            }
            else
            {
                wrongCount++;
            }
        }

        finalScore = correctCount * 20;
    }


    // =====================================
    // MENAMPILKAN HASIL
    // =====================================

    private void ShowResult()
    {
        exercisePanel.SetActive(false);
        resultPanel.SetActive(true);
        feedbackPanel.SetActive(false);

        scoreText.text = "NILAI - " + finalScore;

        correctAnswerText.text =
            "Benar : " + correctCount;

        wrongAnswerText.text =
            "Salah : " + wrongCount;
    }


    // =====================================
    // MENAMPILKAN PEMBAHASAN
    // =====================================

    private void ShowFeedback()
    {
        resultPanel.SetActive(false);
        feedbackPanel.SetActive(true);

        for (int i = 0; i < 5; i++)
        {
            ExerciseQuestion question =
                selectedQuestions[i];

            string selectedAnswer =
                GetAnswerText(question, playerAnswers[i]);

            string correctAnswer =
                GetAnswerText(
                    question,
                    question.correctAnswerIndex
                );

            feedbackContentTexts[i].text =
                "SOAL " + (i + 1) + "\n\n" +
                question.questionText + "\n\n" +
                "Jawaban Kamu: " + selectedAnswer + "\n\n" +
                "Jawaban Benar: " + correctAnswer + "\n\n" +
                "Pembahasan:\n" + question.explanationText;

            explanationImages[i].sprite =
                question.explanationImage;

            explanationImages[i].gameObject.SetActive(
                question.explanationImage != null
            );
        }
    }


    private string GetAnswerText(
        ExerciseQuestion question,
        int answerIndex)
    {
        switch (answerIndex)
        {
            case 0:
                return "A. " + question.answerA;

            case 1:
                return "B. " + question.answerB;

            case 2:
                return "C. " + question.answerC;

            case 3:
                return "D. " + question.answerD;

            default:
                return "Belum menjawab";
        }
    }


    // =====================================
    // KEMBALI ANTARPANEL
    // =====================================

    private void BackToExerciseMenu()
    {
        ShowExerciseMenu();
    }


    private void BackToResult()
    {
        feedbackPanel.SetActive(false);
        resultPanel.SetActive(true);
    }


    // =====================================
    // WARNA NEXT BUTTON
    // =====================================

    private void SetNextButtonColors(bool green = false)
    {
        ColorBlock colors = nextButton.colors;

        if (green)
        {
            colors.normalColor = normalGreen;
            colors.highlightedColor = highlightedGreen;
            colors.pressedColor = pressedGreen;
            colors.selectedColor = selectedGreen;
        }
        else
        {
            colors.normalColor = normalYellow;
            colors.highlightedColor = highlightedYellow;
            colors.pressedColor = pressedYellow;
            colors.selectedColor = selectedYellow;
        }

        colors.colorMultiplier = 1f;
        colors.fadeDuration = 0.1f;

        nextButton.colors = colors;
        nextButton.transition = Selectable.Transition.ColorTint;
    }
}
