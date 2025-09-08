using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CardGridUI : MonoBehaviour
{
    [System.Serializable]
    public class Card
    {
        public string cardName;
        public Sprite cardImage;
        public string textLabel;
        public bool useTextOnly;

        [Header("Match ID (igual para cartas que son pareja)")]
        public int matchID;
    }

    [SerializeField] private List<Card> cardList = new List<Card>();
    [SerializeField] private List<Card> cardListToSort = new List<Card>();
    [SerializeField] private Transform cardContainer;
    [SerializeField] private Transform cardPrefab;

    private void Start() 
    {
        cardPrefab.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        FillGrid();
    }

    private void FillGrid()
    {
        cardListToSort.Clear();

        int cardsToShow = 0;

        switch (MemoryGameManagerUI.Instance.GetDifficulty())
        {
            case DifficultyEnum.Easy:
                cardsToShow = 12; // significa 6 pares
                break;
            case DifficultyEnum.Normal:
                cardsToShow = 16; // significa 8 pares 
                break;
            case DifficultyEnum.Hard:
                cardsToShow = 20; // significa 10pares
                break;
        }

        // 1) Agrupamos las cartas por matchID
        var groupedByMatch = cardList.GroupBy(c => c.matchID).ToList();

        // 2) Tomamos tantos pares como necesitemos
        int pairsToTake = cardsToShow / 2;
        var selectedGroups = groupedByMatch.Take(pairsToTake);

        foreach (var group in selectedGroups)
        {
            foreach (var card in group) // aquí entran las dos (ES y EN)
            {
                cardListToSort.Add(card);
            }
        }

        // 3) Mezclamos
        System.Random rnd = new System.Random();
        IOrderedEnumerable<Card> randomized = cardListToSort.OrderBy(i => rnd.Next());

        // 4) Instanciamos
        foreach (Card card in randomized)
        {
            Transform cardTransform = Instantiate(cardPrefab, cardContainer);
            cardTransform.gameObject.SetActive(true);
            cardTransform.name = card.cardName;
            cardTransform.GetComponent<CardSingleUI>().SetCard(card);
        }
    }


}