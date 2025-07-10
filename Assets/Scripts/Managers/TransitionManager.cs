using UnityEngine;

public class TransitionManager : MonoBehaviour
{
    [System.Serializable]
    public class NamedAnimator
    {
        public string name;
        public Animator animator;
    }

    public NamedAnimator[] animators;

    public void PlayTransition(string gameObject , string animation)
    {
        if (gameObject == "") return;
        
        foreach (var entry in animators)
        {
            if (entry.name == gameObject)
            {
                entry.animator.gameObject.SetActive(true);
                entry.animator.SetTrigger(animation); 
                return;
            }
        }

        Debug.LogWarning($"No se encontró la transición '{animation}'");
    }
}
