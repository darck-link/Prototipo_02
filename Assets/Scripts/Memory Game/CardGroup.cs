using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CardGroup : MonoBehaviour
{
    [SerializeField] private List<CardSingleUI> cardSingleUIList = new List<CardSingleUI>();
    [SerializeField] private List<CardSingleUI> selectedCardList = new List<CardSingleUI>();

    [SerializeField] private Sprite cardIdle;
    [SerializeField] private Sprite cardActive;

    public event EventHandler OnCardMatch;

    // Se suscribe cada carta a este grupo
    public void Subscribe(CardSingleUI cardSingleUI)
    {
        if (cardSingleUIList == null)
            cardSingleUIList = new List<CardSingleUI>();

        if (!cardSingleUIList.Contains(cardSingleUI))
            cardSingleUIList.Add(cardSingleUI);
    }

    // Cuando seleccionas una carta
    public void OnCardSelected(CardSingleUI cardSingleUI)
    {
        selectedCardList.Add(cardSingleUI);

        cardSingleUI.Select();
        cardSingleUI.GetCardFrontBackground().sprite = cardActive;

        // Solo procesamos cuando hay 2 cartas seleccionadas
        if (selectedCardList.Count == 2)
        {
            if (CheckIfMatch())
            {
                // Coinciden
                foreach (CardSingleUI cardSingle in selectedCardList)
                {
                    cardSingle.DisableCardBackButton();
                    cardSingle.SetObjectMatch();
                }

                // Sumar puntaje
                GameHUDManager.Instance.AddScore(10);

                selectedCardList.Clear();
                OnCardMatch?.Invoke(this, EventArgs.Empty);

                // Si todas están emparejadas-  GANASTE
                if (cardSingleUIList.All(c => c.GetObjectMatch()))
                {
                    GameHUDManager.Instance.WinGame();
                }
            }
            else
            {
                // No coinciden- Error y restar oportunidades
                GameHUDManager.Instance.AddError();
                StartCoroutine(DontMatch());
            }
        }
        Debug.Log($"Cartas suscritas: {cardSingleUIList.Count} - Cartas completadas: {cardSingleUIList.Count(c => c.GetObjectMatch())}");


        ResetTabs();
    }

    // Resetea visual si hay más de 2 seleccionadas
    public void ResetTabs()
    {
        if (selectedCardList != null && selectedCardList.Count < 3) return;

        foreach (CardSingleUI cardSingleUI in selectedCardList)
        {
            cardSingleUI.GetCardBackBackground().sprite = cardIdle;
        }
    }

    public void ResetGroup()
    {
        cardSingleUIList.Clear();
        selectedCardList.Clear();
    }


    // Corrutina cuando NO coinciden
    private IEnumerator DontMatch()
    {
        yield return new WaitForSeconds(0.5f);

        foreach (CardSingleUI cardSingleUI in selectedCardList)
        {
            cardSingleUI.Deselect();
        }

        selectedCardList.Clear();
    }

    // Ahora compara por matchID (ya no por name)
    private bool CheckIfMatch()
    {
        if (selectedCardList.Count != 2) return false;

        return selectedCardList[0].GetMatchID() == selectedCardList[1].GetMatchID();
    }


}
