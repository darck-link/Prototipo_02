using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardGroup : MonoBehaviour
{
    [SerializeField] private List<CardSingleUI> cardSingleUIList = new List<CardSingleUI>();
    [SerializeField] private List<CardSingleUI> selectedCardList = new List<CardSingleUI>();

    [SerializeField] private Sprite cardIdle;
    [SerializeField] private Sprite cardActive;

    public event EventHandler OnCardMatch;

    public void Subscribe(CardSingleUI cardSingleUI)
    {
        if (cardSingleUIList == null)
            cardSingleUIList = new List<CardSingleUI>();

        if (!cardSingleUIList.Contains(cardSingleUI))
            cardSingleUIList.Add(cardSingleUI);
    }

    public void OnCardSelected(CardSingleUI cardSingleUI)
    {
        selectedCardList.Add(cardSingleUI);

        cardSingleUI.Select();
        cardSingleUI.GetCardFrontBackground().sprite = cardActive;

        if (selectedCardList.Count == 2)
        {
            if (CheckIfMatch())
            {
                foreach (CardSingleUI cardSingle in selectedCardList)
                {
                    cardSingle.DisableCardBackButton();
                    cardSingle.SetObjectMatch();
                }

                selectedCardList.Clear();
                OnCardMatch?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                StartCoroutine(DontMatch());
            }
        }

        ResetTabs();
    }

    public void ResetTabs()
    {
        if (selectedCardList != null && selectedCardList.Count < 3) return;

        foreach (CardSingleUI cardSingleUI in selectedCardList)
        {
            cardSingleUI.GetCardBackBackground().sprite = cardIdle;
        }
    }

    private IEnumerator DontMatch()
    {
        yield return new WaitForSeconds(0.5f);

        foreach (CardSingleUI cardSingleUI in selectedCardList)
        {
            cardSingleUI.Deselect();
        }

        selectedCardList.Clear();
    }

    // ahora compara por matchID en vez de name
    private bool CheckIfMatch()
    {
        if (selectedCardList.Count != 2) return false;

        return selectedCardList[0].GetMatchID() == selectedCardList[1].GetMatchID();
    }
}
