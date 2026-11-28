
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class GameplayController : MonoBehaviour
{
    [Serializable]
    public class StoreItem
    {
        public string itemName;
        public int costPrice;
        public int sellingPrice;

        public StoreItem(string name, int cost, int selling)
        {
            itemName = name;
            costPrice = cost;
            sellingPrice = selling;
        }
    }

    private class SaleRecord
    {
        public string itemName;
        public int costPrice;
        public int sellingPrice;
        public int discount;
        public int customerNumber;
    }

    [Header("PANELS")]
    public GameObject gameplayPanel;
    public GameObject cashierScreenPanel;
    public GameObject reportPanel;
    public GameObject evaluationPanel;

    [Header("GAMEPLAY TEXT")]
    public TMP_Text dayText;
    public TMP_Text customerCountText;
    public TMP_Text customerDialogueText;
    public TMP_Text itemListText;
    public TMP_Text attemptText;

    [Header("CUSTOMER CHARACTER")]
    public Image customerImage;
    public Sprite[] customerSprites;

    [Header("CASHIER INPUT")]
    public TMP_InputField totalInputField;
    public Button submitButton;
    public Button mainMenuButton;

    [Header("REPORT")]
    public TMP_Text salesListText;
    public TMP_Text totalModalText;
    public TMP_Text totalRevenueText;
    public Button reportContinueButton;

    [Header("EVALUATION")]
    public TMP_Text evaluationFeedbackText;
    public TMP_InputField amountInputField;
    public Button profitButton;
    public Button lossButton;
    public Button breakEvenButton;
    public Button submitEvaluationButton;
    public Button continueGameButton;
    public Button evaluationMainMenuButton;
    public Button backToReportButton;

    private readonly CultureInfo idCulture =
        new CultureInfo("id-ID");

    private StoreItem[] itemCatalog;

    private readonly List<SaleRecord> allSales =
        new List<SaleRecord>();

    private readonly List<SaleRecord> currentPurchase =
        new List<SaleRecord>();

    private int currentDay = 1;
    private int currentCustomer = 1;
    private int attemptsUsed = 0;
    private int currentCustomerTotal = 0;
    private int selectedResult = 99;

    private int totalModal = 0;
    private int totalRevenue = 0;

    private bool evaluationPreparedForDay = false;

    private void Start()
    {
        CreateItemCatalog();
        ConnectButtons();
        StartNewDay();
    }

    private void CreateItemCatalog()
    {
        itemCatalog = new StoreItem[]
        {
            new StoreItem("Kaos", 40000, 55000),
            new StoreItem("Celana jeans", 80000, 105000),
            new StoreItem("Kemeja", 70000, 95000),
            new StoreItem("Jaket", 120000, 155000),
            new StoreItem("Topi", 30000, 42000),
            new StoreItem("Sepatu", 150000, 190000),
            new StoreItem("Sandal", 35000, 48000),
            new StoreItem("Tas", 90000, 120000),
            new StoreItem("Jam tangan", 100000, 135000),
            new StoreItem("Dompet", 45000, 60000),
            new StoreItem("Ikat pinggang", 35000, 48000),
            new StoreItem("Kaos kaki", 15000, 22000)
        };
    }

    private void ConnectButtons()
    {
        submitButton.onClick.AddListener(CheckPayment);
        mainMenuButton.onClick.AddListener(OpenMainMenu);

        reportContinueButton.onClick.AddListener(OpenEvaluation);

        profitButton.onClick.AddListener(() => SelectResult(1));
        lossButton.onClick.AddListener(() => SelectResult(-1));
        breakEvenButton.onClick.AddListener(() => SelectResult(0));

        submitEvaluationButton.onClick.AddListener(CheckEvaluation);
        continueGameButton.onClick.AddListener(ContinueGame);
        evaluationMainMenuButton.onClick.AddListener(OpenMainMenu);

        backToReportButton.onClick.AddListener(BackToReport);
    }

    private void StartNewDay()
    {
        allSales.Clear();

        currentCustomer = 1;
        totalModal = 0;
        totalRevenue = 0;

        evaluationPreparedForDay = false;

        gameplayPanel.SetActive(true);
        cashierScreenPanel.SetActive(false);
        reportPanel.SetActive(false);
        evaluationPanel.SetActive(false);

        mainMenuButton.gameObject.SetActive(false);

        continueGameButton.gameObject.SetActive(false);
        evaluationMainMenuButton.gameObject.SetActive(false);

        dayText.text = "HARI KE-" + currentDay;

        StartCustomer();
    }

    private void StartCustomer()
    {
        currentPurchase.Clear();

        attemptsUsed = 0;
        currentCustomerTotal = 0;

        customerCountText.text =
            "PELANGGAN " + currentCustomer + " / 5";

        totalInputField.text = "";
        totalInputField.interactable = true;

        submitButton.interactable = true;

        mainMenuButton.gameObject.SetActive(false);

        attemptText.text = "Kesempatan tersisa: 3";

        if (customerSprites != null &&
            customerSprites.Length > 0)
        {
            int randomCharacter =
                UnityEngine.Random.Range(
                    0, customerSprites.Length
                );

            customerImage.sprite =
                customerSprites[randomCharacter];
        }

        GenerateCustomerPurchase();

        cashierScreenPanel.SetActive(false);

        StartCoroutine(ShowCustomerDialogue());
    }

    private void GenerateCustomerPurchase()
    {
        currentPurchase.Clear();

        List<StoreItem> availableItems =
            new List<StoreItem>(itemCatalog);

        int itemCount = UnityEngine.Random.Range(1, 4);

        for (int i = 0; i < itemCount; i++)
        {
            int randomIndex =
                UnityEngine.Random.Range(
                    0, availableItems.Count
                );

            StoreItem chosenItem =
                availableItems[randomIndex];

            availableItems.RemoveAt(randomIndex);

            int discount =
                UnityEngine.Random.Range(0, 11) * 5;

            int finalPrice =
                chosenItem.sellingPrice *
                (100 - discount) / 100;

            SaleRecord record = new SaleRecord();

            record.itemName = chosenItem.itemName;
            record.costPrice = chosenItem.costPrice;
            record.sellingPrice = finalPrice;
            record.discount = discount;
            record.customerNumber = currentCustomer;

            currentPurchase.Add(record);

            currentCustomerTotal += finalPrice;
        }
    }

    private IEnumerator ShowCustomerDialogue()
    {
        customerDialogueText.text =
            "Selamat pagi! Saya ingin membeli beberapa barang.";

        yield return new WaitForSeconds(2f);

        string dialogue = "Saya ingin membeli:\n";

        foreach (SaleRecord item in currentPurchase)
        {
            dialogue += "- " + item.itemName + "\n";
        }

        customerDialogueText.text = dialogue;

        yield return new WaitForSeconds(2f);

        string itemText = "";

        for (int i = 0; i < currentPurchase.Count; i++)
        {
            SaleRecord item = currentPurchase[i];

            int itemIndex = Array.FindIndex(
                itemCatalog,
                x => x.itemName == item.itemName
            );

            int originalPrice =
                itemCatalog[itemIndex].sellingPrice;

            itemText +=
                (i + 1) + ". " + item.itemName + "\n" +
                "   Harga normal: " +
                FormatRupiah(originalPrice) + "\n" +
                "   Diskon: " + item.discount + "%\n\n";
        }

        itemListText.text = itemText;

        cashierScreenPanel.SetActive(true);
    }

    // MEMERIKSA JAWABAN PEMBAYARAN PELANGGAN
    private void CheckPayment()
    {
        if (!int.TryParse(
            totalInputField.text,
            NumberStyles.Integer,
            CultureInfo.InvariantCulture,
            out int playerAnswer))
        {
            customerDialogueText.text =
                "Masukkan nominal pembayaran berupa angka saja.";
            return;
        }

        if (playerAnswer == currentCustomerTotal)
        {
            // SUARA JAWABAN BENAR
            if (SFXManager.Instance != null)
                SFXManager.Instance.PlayCorrectAnswer();

            customerDialogueText.text =
                "Terima kasih! Pembayaran sudah benar.";

            allSales.AddRange(currentPurchase);

            submitButton.interactable = false;
            totalInputField.interactable = false;

            StartCoroutine(NextCustomer());
        }
        else
        {
            // SUARA JAWABAN SALAH
            if (SFXManager.Instance != null)
                SFXManager.Instance.PlayWrongAnswer();

            attemptsUsed++;

            int remaining = 3 - attemptsUsed;

            attemptText.text =
                "Kesempatan tersisa: " + remaining;

            if (remaining > 0)
            {
                customerDialogueText.text =
                    "Jumlah pembayaran belum tepat. Silakan coba lagi.";
            }
            else
            {
                customerDialogueText.text =
                    "Sepertinya kamu belum siap untuk bermain. " +
                    "Silakan belajar terlebih dahulu.";

                submitButton.interactable = false;
                totalInputField.interactable = false;

                mainMenuButton.gameObject.SetActive(true);
            }
        }
    }

    private IEnumerator NextCustomer()
    {
        yield return new WaitForSeconds(1.5f);

        if (currentCustomer < 5)
        {
            currentCustomer++;
            StartCustomer();
        }
        else
        {
            ShowReport();
        }
    }

    private void ShowReport()
    {
        gameplayPanel.SetActive(false);
        evaluationPanel.SetActive(false);
        reportPanel.SetActive(true);

        totalModal = 0;
        totalRevenue = 0;

        string report = "";

        for (int customer = 1; customer <= 5; customer++)
        {
            report += "\nPELANGGAN " + customer + "\n";

            foreach (SaleRecord item in allSales)
            {
                if (item.customerNumber == customer)
                {
                    report +=
                        item.itemName +
                        " | Modal: " +
                        FormatRupiah(item.costPrice) +
                        " | Terjual: " +
                        FormatRupiah(item.sellingPrice) +
                        " | Diskon: " +
                        item.discount + "%\n";

                    totalModal += item.costPrice;
                    totalRevenue += item.sellingPrice;
                }
            }
        }

        salesListText.text = report;

        totalModalText.text =
            "TOTAL MODAL: " + FormatRupiah(totalModal);

        totalRevenueText.text =
            "TOTAL PENJUALAN: " + FormatRupiah(totalRevenue);

        reportContinueButton.interactable = true;
    }

    private void OpenEvaluation()
    {
        reportPanel.SetActive(false);
        evaluationPanel.SetActive(true);

        // Persiapkan evaluasi hanya saat pertama kali dibuka.
        // Jika pemain kembali ke laporan, jawaban tidak dihapus.
        if (!evaluationPreparedForDay)
        {
            selectedResult = 99;

            amountInputField.text = "";
            amountInputField.interactable = true;

            evaluationFeedbackText.text = "";

            continueGameButton.gameObject.SetActive(false);
            evaluationMainMenuButton.gameObject.SetActive(false);

            profitButton.interactable = true;
            lossButton.interactable = true;
            breakEvenButton.interactable = true;

            submitEvaluationButton.interactable = true;

            evaluationPreparedForDay = true;
        }
    }

    private void BackToReport()
    {
        evaluationPanel.SetActive(false);
        reportPanel.SetActive(true);
    }

    private void SelectResult(int result)
    {
        selectedResult = result;

        profitButton.interactable = result != 1;
        lossButton.interactable = result != -1;
        breakEvenButton.interactable = result != 0;
    }

    // MEMERIKSA JAWABAN EVALUASI UNTUNG, RUGI, ATAU IMPAS
    private void CheckEvaluation()
    {
        if (selectedResult == 99)
        {
            evaluationFeedbackText.text =
                "Pilih terlebih dahulu: Untung, Rugi, atau Impas.";
            return;
        }

        if (!int.TryParse(
            amountInputField.text,
            NumberStyles.Integer,
            CultureInfo.InvariantCulture,
            out int playerAmount))
        {
            evaluationFeedbackText.text =
                "Masukkan nominal berupa angka saja.";
            return;
        }

        int difference = totalRevenue - totalModal;

        int correctResult =
            difference > 0 ? 1 :
            difference < 0 ? -1 : 0;

        int correctAmount = Math.Abs(difference);

        string resultName =
            correctResult == 1 ? "UNTUNG" :
            correctResult == -1 ? "RUGI" : "IMPAS";

        string playerResultName =
            selectedResult == 1 ? "UNTUNG" :
            selectedResult == -1 ? "RUGI" : "IMPAS";

        bool isCorrect =
            selectedResult == correctResult &&
            playerAmount == correctAmount;

        // SUARA JAWABAN BENAR ATAU SALAH
        if (SFXManager.Instance != null)
        {
            if (isCorrect)
                SFXManager.Instance.PlayCorrectAnswer();
            else
                SFXManager.Instance.PlayWrongAnswer();
        }

        string explanation =
            "Total penjualan: " + FormatRupiah(totalRevenue) +
            "\nTotal modal: " + FormatRupiah(totalModal) +
            "\nSelisih: " + FormatRupiah(correctAmount);

        evaluationFeedbackText.text =
            (isCorrect ? "JAWABAN BENAR!" : "JAWABAN BELUM TEPAT.") +
            "\nJawabanmu: " + playerResultName +
            ", " + FormatRupiah(playerAmount) +
            "\nJawaban benar: " + resultName +
            ", " + FormatRupiah(correctAmount) +
            "\n\nPEMBAHASAN\n" + explanation;

        profitButton.interactable = false;
        lossButton.interactable = false;
        breakEvenButton.interactable = false;

        submitEvaluationButton.interactable = false;
        amountInputField.interactable = false;

        continueGameButton.gameObject.SetActive(true);
        evaluationMainMenuButton.gameObject.SetActive(true);
    }

    private void ContinueGame()
    {
        currentDay++;
        StartNewDay();
    }

    private void OpenMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    private string FormatRupiah(int amount)
    {
        return "Rp" + amount.ToString("N0", idCulture);
    }
}
