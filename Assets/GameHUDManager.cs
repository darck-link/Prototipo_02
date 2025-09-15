using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameHUDManager : MonoBehaviour
{
    public static GameHUDManager Instance;

    [Header("UI Texts")]
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text errorsText;
    [SerializeField] private TMP_Text chancesText;
    [SerializeField] private TMP_Text timerText;

    [Header("Panels")]
    [SerializeField] private GameObject winnerPanel;
    [SerializeField] private GameObject gameOverPanel;

    [Header("Configurable Chances por Nivel")]
    [SerializeField] private int easyChances = 10;
    [SerializeField] private int normalChances = 8;
    [SerializeField] private int hardChances = 7;

    private int score = 0;
    private int errors = 0;
    private int chances; // ahora configurable
    private float timer;
    private bool isPlaying;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (isPlaying && timer > 0)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                GameOver();
            }
            UpdateHUD();
        }
    }

    public void StartGame(float timeLimit, DifficultyEnum difficulty)
    {
        score = 0;
        errors = 0;

        // asignar oportunidades según dificultad
        switch (difficulty)
        {
            case DifficultyEnum.Easy:
                chances = easyChances;
                break;
            case DifficultyEnum.Normal:
                chances = normalChances;
                break;
            case DifficultyEnum.Hard:
                chances = hardChances;
                break;
            default:
                chances = 3; // fallback
                break;
        }

        timer = timeLimit;
        isPlaying = true;

        winnerPanel.SetActive(false);
        gameOverPanel.SetActive(false);

        UpdateHUD();
    }

    public void AddScore(int amount)
    {
        score += amount;
        UpdateHUD();
    }

    public void AddError()
    {
        errors++;
        chances--;

        if (chances <= 0)
        {
            GameOver();
        }

        UpdateHUD();
    }

    public void GameOver()
    {
        isPlaying = false;
        gameOverPanel.SetActive(true);
    }

    public void WinGame()
    {
        isPlaying = false;
        winnerPanel.SetActive(true);
    }

    private void UpdateHUD()
    {
        scoreText.text = $"Score: {score}";
        errorsText.text = $"Errors: {errors}";
        chancesText.text = $"Chances: {chances}";
        timerText.text = $"Time: {Mathf.CeilToInt(timer)}s";
    }

    public void BackToMenu()
    {
        winnerPanel.SetActive(false);
        gameOverPanel.SetActive(false);
        DifficultyManager.Instance.Toggle(true);
    }
}
