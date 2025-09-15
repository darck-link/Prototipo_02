using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MemoryGameManagerUI : MinigamesBase
{
    public static MemoryGameManagerUI Instance { get; private set; }

    [SerializeField] private CardGroup cardGroup;
    [SerializeField] private List<CardSingleUI> cardSingleUIList = new List<CardSingleUI>();

    private DifficultyEnum currentDifficulty; // guardamos la dificultad actual

    [Header("Config Tiempo por Nivel (segundos)")]
    [SerializeField] private float easyTime = 60f;
    [SerializeField] private float normalTime = 45f;
    [SerializeField] private float hardTime = 30f;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        cardGroup.OnCardMatch += CardGroup_OnCardMatch;
    }

    private void OnEnable()
    {
        StartCoroutine(Initialize());
    }

    private IEnumerator Initialize()
    {
        yield return new WaitForSeconds(0.1f);

        DifficultyManager.Instance
            .ResetListeners()
            .OnEasyButtonClick(() =>
            {
                currentDifficulty = DifficultyEnum.Easy;
                DifficultyManager.Instance.Toggle(false);
                ToggleGameArea(true);

                GameHUDManager.Instance.StartGame(easyTime, currentDifficulty);
            })
            .OnNormalButtonClick(() =>
            {
                currentDifficulty = DifficultyEnum.Normal;
                DifficultyManager.Instance.Toggle(false);
                ToggleGameArea(true);

                GameHUDManager.Instance.StartGame(normalTime, currentDifficulty);
            })
            .OnHardButtonClick(() =>
            {
                currentDifficulty = DifficultyEnum.Hard;
                DifficultyManager.Instance.Toggle(false);
                ToggleGameArea(true);

                GameHUDManager.Instance.StartGame(hardTime, currentDifficulty);
            });
    }

    // Usa "new" para no chocar con el de MinigamesBase
    public new DifficultyEnum GetDifficulty() => currentDifficulty;

    public void Subscribe(CardSingleUI cardSingleUI)
    {
        if (cardSingleUIList == null)
            cardSingleUIList = new List<CardSingleUI>();

        if (!cardSingleUIList.Contains(cardSingleUI))
            cardSingleUIList.Add(cardSingleUI);
    }

    private void CardGroup_OnCardMatch(object sender, System.EventArgs e)
    {
        if (cardSingleUIList.All(x => x.GetObjectMatch()))
        {
            StartCoroutine(OnCompleteGame());
        }
    }

    private IEnumerator OnCompleteGame()
    {
        yield return new WaitForSeconds(0.75f);
        Debug.Log("Has ganado");
        GameHUDManager.Instance.WinGame();
    }

    public void Restart()
    {
        cardSingleUIList.Clear();

        float timeLimit = easyTime;

        switch (currentDifficulty)
        {
            case DifficultyEnum.Easy: timeLimit = easyTime; break;
            case DifficultyEnum.Normal: timeLimit = normalTime; break;
            case DifficultyEnum.Hard: timeLimit = hardTime; break;
        }

        GameHUDManager.Instance.StartGame(timeLimit, currentDifficulty);
    }
}
