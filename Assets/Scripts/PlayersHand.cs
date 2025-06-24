using System.Collections;
using UnityEngine;

public class PlayersHand : MonoBehaviour
{
    public GameManager gameManager;
    
    private int _cardsCount;
    private float _cardSpacing = 2.5f;
    private float _startPosition;
    private Card _grabbedCard;

    void Start()
    {
        Recalculate();
    }

    //Se utiliza para recalcular las posiciones de las cartas en la mano.
    public IEnumerator RecalculateCoroutine()
    {
        yield return new WaitForEndOfFrame();
        _cardsCount = gameManager.CantCardsInHand();

        SetCardsOrder();
        SetPositions();
    }
    
    private void SetCardsOrder()
    {
        int i = 0;
        foreach (Transform cardTransform in transform)
        {
            Card card = cardTransform.GetComponent<Card>();
            if (card != _grabbedCard)
            {
                card.SetCardOrder(i * 2); //Colocamos las cartas solo en los índices pares pues en los impares van los números.
                i++;
            }
        }
    }

    private void SetPositions()
    {
        float initialX = CalculateStartX(_cardsCount);
        int i = 0;
        foreach (Transform cardTransform in transform)
        {
            Card card = cardTransform.GetComponent<Card>();
            Vector3 currentPos = cardTransform.position;
            float newX = initialX + (i * _cardSpacing);
            cardTransform.position = new Vector3(newX, currentPos.y, currentPos.z);

            card.SetCardStartPosition();
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
            return -1 * (_cardSpacing * half - _cardSpacing / 2f);
        else
            return -1 * (_cardSpacing * half);
    }

    public int GetCardIndex(Transform cardTransform)
    {
        int index = 0;
        foreach (Transform card in gameObject.transform)
        {
            if (card.name == cardTransform.name)
                return index;
            index++;
        }
        return -1;
    }
    
    public void ToggleCardsEnable(bool enable)
    {
        foreach (Transform card in gameObject.transform)
        {
            card.GetComponent<Card>().SetCardActive(enable);
        }
    }

    public void Recalculate()
    {
        StartCoroutine("RecalculateCoroutine");
    }
    #endregion
}
