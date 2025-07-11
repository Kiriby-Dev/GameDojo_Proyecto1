using System;
using System.Collections;
using UnityEngine;

public class PlayersHand : MonoBehaviour
{
    public GameManager gameManager;
    public float cardSpacing;
    
    private int _cardsCount;
    private Card[] _cards;
    private Card _selectedCard;
    private bool _isCrossing;

    private void Start()
    {
        _cards = new Card[gameManager.CantCardsInHand()];
        EnableAllSlots();
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
            if (transform.GetChild(i).gameObject.activeInHierarchy)
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
    }

    private void Swap(int i)
    {
        _isCrossing = true;

        Transform selectedParent = _selectedCard.transform.parent;
        Transform crossedParent = _cards[i].transform.parent;

        // Obtener posicioó objetivo
        Vector3 crossedTarget = selectedParent.position;

        // Cambiar los padres (no cambia posición aún)
        _selectedCard.transform.SetParent(crossedParent);
        _cards[i].transform.SetParent(selectedParent);

        // Mover la carta suavemente a su nueva posición
        StartCoroutine(MoveCardToPosition(_cards[i].transform, crossedTarget, 0.15f));

        // Sorting order correcto
        int selectedCardLayer = selectedParent.GetComponent<SpriteRenderer>().sortingOrder;
        _cards[i].SetCardOrder(selectedCardLayer);

        _isCrossing = false;
    }
    
    private IEnumerator MoveCardToPosition(Transform cardTransform, Vector3 targetPosition, float duration)
    {
        float timeElapsed = 0f;
        Vector3 startPos = cardTransform.position;

        while (timeElapsed < duration)
        {
            cardTransform.position = Vector3.Lerp(startPos, targetPosition, timeElapsed / duration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        cardTransform.position = targetPosition;
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
            _cards[i] = null;
            if (cardSlot.gameObject.activeInHierarchy)
            {
                _cards[i] = cardSlot.GetComponentInChildren<Card>();
            }
            i++;
        }
    }

    private void SetCardsOrder()
    {
        int i = 0;
        foreach (Transform cardSlot in transform)
        {
            if (cardSlot.gameObject.activeInHierarchy)
            {
                Card card = cardSlot.GetComponentInChildren<Card>();
                card.SetCardOrder(i * 3); //Colocamos las cartas solo en los índices pares pues en los impares van los números.
                i++;
            }
        }
    }

    private void SetPositions()
    {
        float initialX = CalculateStartX(_cardsCount);
        int i = 0;
        foreach (Transform cardSlot in transform)
        {
            if (cardSlot.gameObject.activeInHierarchy)
            {
                Card card = cardSlot.GetComponentInChildren<Card>();
                Vector3 currentPos = cardSlot.position;
                float newX = initialX + (i * cardSpacing);
                cardSlot.position = new Vector3(newX, currentPos.y, currentPos.z);

                card.transform.localPosition = Vector3.zero;
                i++;
            }
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

    public void EnableAllSlots()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(true);
        }
    }

    public void DisableSlot(int index)
    {
        transform.GetChild(index).gameObject.SetActive(false);
    }
    #endregion
}
