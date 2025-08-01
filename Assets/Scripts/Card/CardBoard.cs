using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardBoard : MonoBehaviour
{
    public enum CardColor { Red, Yellow, Green, White }
    
    [SerializeField] private Image cardImage;
    [SerializeField] private TextMeshProUGUI cardText;

    public void ChangeImage(Sprite sprite)
    {
        cardImage.sprite = sprite;
    }

    public void ChangeText(int cardValue)
    {
        cardText.text = "+" + cardValue;
    }

    public int GetCardValue()
    {
        return int.Parse(cardText.text);
    }

    public void ChangeColor(CardColor color = CardColor.White)
    {
        switch (color)
        {
            case CardColor.Red: 
                cardImage.color = Color.red; 
                break;
            case CardColor.Yellow: 
                cardImage.color = Color.yellow; 
                break;
            case CardColor.Green: 
                cardImage.color = Color.green; 
                break;
            case CardColor.White: 
                cardImage.color = Color.white; 
                break;
        }
    }
}
