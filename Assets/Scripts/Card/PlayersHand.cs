using System;
using System.Collections;
using UnityEngine;

public class PlayersHand : MonoBehaviour
{
    public GameManager gameManager;
    public float cardSpacing = 1.5f;
    
    private int _cardsCount;
    private Card[] _cards;
    private Card _selectedCard;
    private bool _isCrossing;

    private void Start()
    {
        _cards = new Card[gameManager.CantCardsInHand()];
    }

    private void Update()
    {
        CheckForSwap();
    }

    private void CheckForSwap()
    {
        if (!_selectedCard) return;
        if (_isCrossing) return;
        
        for (int i = 0; i < _cardsCount; i++)
        {
            if (_selectedCard.transform.position.x > _cards[i].transform.position.x)
            {
                if (_selectedCard.GetParentIndex() < _cards[i].GetParentIndex())
                {
                    Swap(i);
                    return;
                }
            }
            if (_selectedCard.transform.position.x < _cards[i].transform.position.x)
            {
                if (_selectedCard.GetParentIndex() > _cards[i].GetParentIndex())
                {
                    Swap(i);
                    return;
                }
            }
        }
    }

    private void Swap(int i)
    {
        _isCrossing = true;

        Transform focusedParent = _selectedCard.transform.parent;
        Transform crossedParent = _cards[i].transform.parent;

        _cards[i].transform.SetParent(focusedParent);
        _cards[i].transform.localPosition = Vector3.zero;
        int selectedCardLayer = focusedParent.GetComponent<SpriteRenderer>().sortingOrder;
        _cards[i].GetComponentInChildren<SpriteRenderer>().sortingOrder = selectedCardLayer;
        _selectedCard.transform.SetParent(crossedParent);

        _isCrossing = false;
    }

    //Se utiliza para recalcular las posiciones de las cartas en la mano.
    public IEnumerator RecalculateCoroutine()
    {
        yield return new WaitForEndOfFrame();
        _cardsCount = gameManager.CantCardsInHand();
        
        GetCardsInHand();
        SetCardsOrder();
        SetPositions();
    }

    private void GetCardsInHand()
    {
        int i = 0;
        foreach (Transform cardSlot in gameObject.transform)
        {
            _cards[i] = cardSlot.GetComponentInChildren<Card>();
            i++;
        }
    }

    private void SetCardsOrder()
    {
        int i = 0;
        foreach (Transform cardSlot in transform)
        {
            Card card = cardSlot.GetComponentInChildren<Card>();
            card.SetCardOrder(i * 2); //Colocamos las cartas solo en los índices pares pues en los impares van los números.
            i++;
        }
    }

    private void SetPositions()
    {
        float initialX = CalculateStartX(_cardsCount);
        int i = 0;
        foreach (Transform cardSlot in transform)
        {
            Card card = cardSlot.GetComponentInChildren<Card>();
            Vector3 currentPos = cardSlot.position;
            float newX = initialX + (i * cardSpacing);
            cardSlot.position = new Vector3(newX, currentPos.y, currentPos.z);

            card.transform.localPosition = Vector3.zero;
            i++;
        }
    }

    #region Utilities
    //Calcula la posición X inicial de la primera carta en la mano, para que todas queden centradas en pantalla.
    private float CalculateStartX(int count)
    {
        bool isEven = count % 2 == 0;
        float half = Mathf.Floor(count * 0.5f);

        if (isEven)
            return -1 * (cardSpacing * half - cardSpacing / 2f);
        else
            return -1 * (cardSpacing * half);
    }

    public void Recalculate()
    {
        StartCoroutine("RecalculateCoroutine");
    }
    
    public void SelectedCard(Card card)
    {
        _selectedCard = card;
    }
    #endregion
}
