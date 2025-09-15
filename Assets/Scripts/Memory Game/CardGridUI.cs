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

    // Referencia al CardGroup
    [SerializeField] private CardGroup cardGroup;

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
        // Limpiar hijos anteriores en el contenedor (excepto el prefab base oculto)
        foreach (Transform child in cardContainer)
        {
            if (child != cardPrefab)
                Destroy(child.gameObject);
        }

        // Limpiar listas
        cardListToSort.Clear();

        // Resetear el grupo de cartas
        if (cardGroup != null)
        {
            cardGroup.ResetGroup();
        }

        int cardsToShow = 0;

        switch (MemoryGameManagerUI.Instance.GetDifficulty())
        {
            case DifficultyEnum.Easy:
                cardsToShow = 12; // 6 pares
                break;
            case DifficultyEnum.Normal:
                cardsToShow = 16; // 8 pares
                break;
            case DifficultyEnum.Hard:
                cardsToShow = 20; // 10 pares
                break;
        }

        // Agrupamos las cartas por matchID
        var groupedByMatch = cardList.GroupBy(c => c.matchID).ToList();

        // Tomamos tantos pares como necesitemos
        int pairsToTake = cardsToShow / 2;
        var selectedGroups = groupedByMatch.Take(pairsToTake);

        foreach (var group in selectedGroups)
        {
            foreach (var card in group) // cada grupo debe tener 2 cartas: ES + EN
            {
                cardListToSort.Add(card);
            }
        }

        // Mezclamos
        System.Random rnd = new System.Random();
        IOrderedEnumerable<Card> randomized = cardListToSort.OrderBy(i => rnd.Next());

        // Instanciamos
        foreach (Card card in randomized)
        {
            Transform cardTransform = Instantiate(cardPrefab, cardContainer);
            cardTransform.gameObject.SetActive(true);
            cardTransform.name = card.cardName;

            var cardUI = cardTransform.GetComponent<CardSingleUI>();
            cardUI.SetCard(card);

            // Ahora sí, el CardGroup conoce TODAS las cartas
            if (cardGroup != null)
                cardGroup.Subscribe(cardUI);
            else
                Debug.LogWarning("CardGroup no está asignado en el inspector");
        }
    }
}
