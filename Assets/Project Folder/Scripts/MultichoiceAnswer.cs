using UnityEngine;
using UnityEngine.Events;

public class MultichoiceAnswer : MonoBehaviour
{

    public string answerText;
    public TXRButtonTouch button;

    public static UnityEvent<string> OnAnswerSelected = new UnityEvent<string>();


    void Start()
    {
        button = GetComponentInChildren<TXRButtonTouch>();
        button.Released.AddListener(() => SelectAnswer(answerText));

    }


    void SelectAnswer(string selectedAnswer)
    {
        OnAnswerSelected.Invoke(selectedAnswer);
    }
}
