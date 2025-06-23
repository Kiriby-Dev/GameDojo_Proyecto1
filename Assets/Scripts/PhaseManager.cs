using System;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class PhaseManager : MonoBehaviour
{
    public enum GamePhase {Draw, Discard, Colocation, Questions, Resolution}
    public GameManager gameManager;
    
    private GamePhase _currentPhase;

    private void Start()
    {
        CurrentPhase = GamePhase.Draw;
        AdvancePhase();
    }
    
    private void AdvancePhase()
    {
        if (gameManager.IsGameOver()) return;
        StartCoroutine(ExecutePhase());
    }
    
    private IEnumerator ExecutePhase()
    {
        switch (_currentPhase)
        {
            case GamePhase.Draw:
                yield return DrawPhase();
                CurrentPhase = GamePhase.Discard;
                break;
            
            case GamePhase.Discard:
                yield return DiscardPhase();
                CurrentPhase = GamePhase.Colocation;
                break;
            
            case GamePhase.Colocation:
                yield return ColocationPhase();
                CurrentPhase = GamePhase.Questions;
                break;

            case GamePhase.Questions:
                yield return QuestionsPhase();
                CurrentPhase = GamePhase.Resolution;
                break;

            case GamePhase.Resolution:
                yield return ResolutionPhase();
                CurrentPhase = GamePhase.Draw;
                break;
        }
        AdvancePhase();
    }
    
    private IEnumerator DrawPhase()
    {
        gameManager.ResetVariables();
        gameManager.DrawCards();
        gameManager.GetPlayersHand().Recalculate();
        yield return new WaitForSeconds(0.5f);
    }

    private IEnumerator QuestionsPhase()
    {
        yield return gameManager.GetQuestionManager().StartQuestions();
    }

    private IEnumerator ResolutionPhase()
    {
        gameManager.ResolveCombat();
        yield return new WaitForSeconds(2f);
    }

    private IEnumerator DiscardPhase()
    {
        gameManager.UpdateZones(_currentPhase.ToString());
        yield return new WaitUntil(() => gameManager.CardsCountInHand() <= 3);
    }

    private IEnumerator ColocationPhase()
    {
        gameManager.UpdateZones(_currentPhase.ToString());
        yield return new WaitUntil(() => gameManager.CardsCountInHand() <= 0);
    }
    
    public GamePhase CurrentPhase
    {
        get
        {
            return _currentPhase;
        }
        set
        {
            _currentPhase = value;
            gameManager.GetUIManager().UpdatePhaseText(_currentPhase.ToString());
        }
    }
}
