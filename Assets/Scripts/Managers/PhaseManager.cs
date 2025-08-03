using System;
using System.Collections;
using UnityEngine;

public class PhaseManager : MonoBehaviour
{
    public GameManager gameManager;
    public enum GamePhase {Draw, Discard, Colocation, Questions, Resolution}
    
    private GamePhase _currentPhase;

    private void Awake()
    {
        GameFlowManager.OnLevelStart += StartPhases;
    }

    private void OnDestroy()
    {
        GameFlowManager.OnLevelStart -= StartPhases;
    }

    private void StartPhases()
    {
        CurrentPhase = GamePhase.Draw;
        AdvancePhase();
    }
    
    private void AdvancePhase()
    {
        StartCoroutine(ExecutePhase());
    }
    
    //Se ejecuta la fase actual y luego de terminar se avanza a la siguiente.
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

    #region Phases

    private IEnumerator DrawPhase()
    {
        gameManager.GetPlayersHand().DrawCards();
        yield return new WaitForSeconds(0.5f);
    }
    
    private IEnumerator DiscardPhase()
    {
        gameManager.UpdateZones(_currentPhase.ToString());
        gameManager.GetDiscardZone().ToggleLightAnimation();
        yield return new WaitUntil(() => gameManager.GetPlayersHand().CantCardsInHand() <= 3);
    }
    
    private IEnumerator ColocationPhase()
    {
        gameManager.UpdateZones(_currentPhase.ToString());
        gameManager.GetDiscardZone().ToggleLightAnimation();
        yield return new WaitUntil(() => gameManager.GetPlayersHand().CantCardsInHand() <= 0);
    }

    private IEnumerator QuestionsPhase()
    {
        gameManager.GetPlayersHand().transform.position = new Vector3(0, -8, 0);
        yield return gameManager.GetUIManager().QuestionMode();
        yield return gameManager.GetQuestionManager().StartQuestions();
    }

    private IEnumerator ResolutionPhase()
    {
        yield return gameManager.GetUIManager().BattleMode();
        yield return gameManager.GetCombatManager().ResolveCombat();
        yield return gameManager.GetUIManager().NormalMode();
        gameManager.EndTurn();
    }

    #endregion
    
    public GamePhase CurrentPhase
    {
        get
        {
            return _currentPhase;
        }
        set
        {
            _currentPhase = value;
            gameManager.GetUIManager().UpdatePhaseText(_currentPhase);
        }
    }
}
