
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip menuMusic;
    public AudioClip gameplayMusic;

    [Header("Music Button Sprites")]
    public Sprite musicOnSprite;
    public Sprite musicOffSprite;

    private AudioClip currentMusic;
    private bool isMusicOn = true;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void Start()
    {
        PlayMusicForScene(SceneManager.GetActiveScene().name);
        SetupMusicButton();
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        PlayMusicForScene(scene.name);
        SetupMusicButton();
    }

    void PlayMusicForScene(string sceneName)
    {
        AudioClip selectedMusic = null;

        if (sceneName == "MainMenu" ||
            sceneName == "Learning" ||
            sceneName == "Exercise")
        {
            selectedMusic = menuMusic;
        }
        else if (sceneName == "Gameplay")
        {
            selectedMusic = gameplayMusic;
        }

        if (selectedMusic == null || currentMusic == selectedMusic)
            return;

        currentMusic = selectedMusic;
        audioSource.clip = currentMusic;
        audioSource.loop = true;
        audioSource.mute = !isMusicOn;
        audioSource.Play();
    }

    public void ToggleMusic()
    {
        isMusicOn = !isMusicOn;
        audioSource.mute = !isMusicOn;
        SetupMusicButton();
    }

    void SetupMusicButton()
    {
        GameObject buttonObject = GameObject.Find("MusicButton");

        if (buttonObject == null)
            return;

        Button button = buttonObject.GetComponent<Button>();
        Image image = buttonObject.GetComponent<Image>();

        if (button != null)
        {
            button.onClick.RemoveListener(ToggleMusic);
            button.onClick.AddListener(ToggleMusic);
        }

        if (image != null)
        {
            image.sprite = isMusicOn
                ? musicOnSprite
                : musicOffSprite;
        }
    }

    void OnDestroy()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }
}
