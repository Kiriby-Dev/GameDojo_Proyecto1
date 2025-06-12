using System;
using UnityEngine;

public class Card : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        //detectar zona de accion para jugar carta
    }

    private void OnMouseDown()
    {
        
    }

    private void OnMouseUp()
    {
        //resetear la posicion de la carta
    }

    private void OnMouseEnter()
    {
        //ejecutar animacion
    }

    private void OnMouseExit()
    {
        //detener animacion
    }

    public void ChangeColor(bool correct)
    {
        if (correct)
            _spriteRenderer.color = Color.green;
        else
            _spriteRenderer.color = Color.red;
    }

    public void GenerateNewCard()
    {
        
    }
}
