using System;
using System.Collections;
using UnityEngine;

public class PlayersHand : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private float cardSpacing;
    
    private int _cardsCount;
    private Card[] _cards;
    private Card _selectedCard;
    private bool _isCrossing;
    private int _cantCardsInHand;
    private bool _isDisabled;

    private void Awake()
    {
        GameFlowManager.OnGamePaused += DisableCardsInteraction;
    }

    private void OnDestroy()
    {
        GameFlowManager.OnGamePaused -= DisableCardsInteraction;
    }

    private void Start()
    {
        _cards = new Card[transform.childCount];
        EnableAllSlots();
    }

    private void Update()
    {
        CheckForSwap();
    }

    public void DrawCards()
    {
        EnableAllSlots();
        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject card = Instantiate(cardPrefab, transform.GetChild(i));
            Card actualCard = card.GetComponent<Card>();
            int difficulty = actualCard.GenerateCardValue();
            //actualCard.ToggleAnimator(true);
            ChangeCardSprite(actualCard, difficulty);
            card.name = "Card" + i;
            AddCardToHand();
        }

        StartCoroutine(MovePlayersHand(new Vector3(0, -3.3f, 0)));
        Recalculate();
    }
    
    private void ChangeCardSprite(Card actualCard, int difficulty)
    {
        QuestionData.Subject subject = gameManager.GetLevelsManager().GetActualSubject();
        switch (subject)
        {
            case QuestionData.Subject.History:
                switch (difficulty)
                {
                    case 1:
                        actualCard.ChangeSprite(Card.CardSprites.HistoryEasy);
                        break;
                    case 2:
                        actualCard.ChangeSprite(Card.CardSprites.HistoryMedium);
                        break;
                    case 3:
                        actualCard.ChangeSprite(Card.CardSprites.HistoryHard);
                        break;
                }
                break;
            case QuestionData.Subject.Science:
                switch (difficulty)
                {
                    case 1:
                        actualCard.ChangeSprite(Card.CardSprites.ScienceEasy);
                        break;
                    case 2:
                        actualCard.ChangeSprite(Card.CardSprites.ScienceMedium);
                        break;
                    case 3:
                        actualCard.ChangeSprite(Card.CardSprites.ScienceHard);
                        break;
                }
                break;
            case QuestionData.Subject.Entertainment:
                switch (difficulty)
                {
                    case 1:
                        actualCard.ChangeSprite(Card.CardSprites.EntertainmentEasy);
                        break;
                    case 2:
                        actualCard.ChangeSprite(Card.CardSprites.EntertainmentMedium);
                        break;
                    case 3:
                        actualCard.ChangeSprite(Card.CardSprites.EntertainmentHard);
                        break;
                }
                break;
            case QuestionData.Subject.Geography:
                switch (difficulty)
                {
                    case 1:
                        actualCard.ChangeSprite(Card.CardSprites.GeographyEasy);
                        break;
                    case 2:
                        actualCard.ChangeSprite(Card.CardSprites.GeographyMedium);
                        break;
                    case 3:
                        actualCard.ChangeSprite(Card.CardSprites.GeographyHard);
                        break;
                }
                break;
        }
    }

    private IEnumerator MovePlayersHand(Vector3 targetPosition, float duration = 0.5f)
    {
        float elapsedTime = 0f;
        Vector3 startingPos = transform.position;

        while (elapsedTime < duration)
        {
            transform.position = Vector3.Lerp(startingPos, targetPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPosition; // Asegura que termina exactamente en destino
    }

    public int CantCardsInHand() => _cantCardsInHand;

    public void RemoveCardFromHand()
    {
        _cantCardsInHand--;
        Recalculate();
    }
    private void AddCardToHand() => _cantCardsInHand++;
    
    #region SwapCards

    private void CheckForSwap()
    {
        if (!_selectedCard) return;
        if (_isCrossing) return;
        
        for (int i = 0; i < 5; i++)
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

        float direction =  _selectedCard.transform.position.x - _cards[i].transform.position.x;
        _cards[i].PlayMoveAnimation(direction);
        
        // Mover la carta suavemente a su nueva posición
        MoveCardToPosition(_cards[i], crossedTarget, 0.15f);

        // Sorting order correcto
        int selectedCardLayer = selectedParent.GetComponent<SpriteRenderer>().sortingOrder;
        _cards[i].SetCardOrder(selectedCardLayer);

        _isCrossing = false;
    }
    
    private IEnumerator MoveCardToPositionCoroutine(Card card, Vector3 targetPosition, float duration)
    {
        float timeElapsed = 0f;
        Vector3 startPos = card.transform.position;
        
        while (timeElapsed < duration)
        {
            card.transform.position = Vector3.Lerp(startPos, targetPosition, timeElapsed / duration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        
        card.transform.position = targetPosition;
    }

    #endregion

    #region CardsPositions

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

    //Se utiliza para recalcular las posiciones de las cartas en la mano.
    public IEnumerator RecalculateCoroutine()
    {
        yield return new WaitForEndOfFrame();
        _cardsCount = CantCardsInHand();
        
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
    #endregion
    
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

    private void Recalculate()
    {
        StartCoroutine("RecalculateCoroutine");
    }
    
    public void SelectedCard(Card card)
    {
        _selectedCard = card;
    }

    private void EnableAllSlots()
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

    public void MoveCardToPosition(Card card, Vector3 targetPosition, float duration)
    {
        StartCoroutine(MoveCardToPositionCoroutine(card, targetPosition, duration));
    }

    private void DisableCardsInteraction(bool isDisabled)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform cardSlot = transform.GetChild(i);
            Transform card = cardSlot.GetChild(0);
            card.GetComponent<Card>().DisableInteraction(isDisabled);
        }
    }

    #endregion
}
