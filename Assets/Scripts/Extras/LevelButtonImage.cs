using System;
using UnityEngine;
using UnityEngine.UI;

public class LevelButtonImage : MonoBehaviour
{
    [SerializeField] private Sprite blocked;
    [SerializeField] private Sprite approved;
    [SerializeField] private Sprite failed;
    [SerializeField] private Image image;

    public enum State
    {
        Blocked,
        Approved,
        Failed,
        Able
    }

    private Button _button;
    
    private void Awake()
    {
        _button = GetComponentInChildren<Button>();
    }

    public void UpdateSprite(State state)
    {
        image.enabled = true;
        switch (state)
        {
            case State.Blocked: 
                image.sprite = blocked;
                break;
            case State.Approved:
                image.sprite = approved;
                break;
            case State.Failed:
                image.sprite = failed;
                break;
            case State.Able:
                image.enabled = false;
                break;
        }
    }
}
