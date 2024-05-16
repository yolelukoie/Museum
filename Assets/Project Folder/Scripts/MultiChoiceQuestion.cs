using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

public class MultiChoiceQuestion : MonoBehaviour, Question
{
    [SerializeField]
    private TextPopUp board;
    [SerializeField]
    private List<MultichoiceAnswer> _answers;
    private UniTaskCompletionSource<string> answerCompletionSource;

    private void InitAnswers()
    {
        _answers = new List<MultichoiceAnswer>();
        _answers.AddRange(GetComponentsInChildren<MultichoiceAnswer>());
        print("Debug MultiChiceQuestion, _answers.count: " + _answers.Count);
    }

    private void Start()
    {
        InitAnswers();
        MultichoiceAnswer.OnAnswerSelected.AddListener(ProcessAnswer);

    }

    void ProcessAnswer(string selectedAnswer)
    {

        // Complete the UniTask when an answer is selected
        answerCompletionSource?.TrySetResult(selectedAnswer);

        // TODO report selected answer
        Debug.Log("Selected answer: " + selectedAnswer);
    }



    public void setQuestion(string question, string answer1, string answer2, string answer3)
    {
        board.SetTextAndAutoScale(question);
        _answers[0].SetText(answer1);
        _answers[1].SetText(answer2);
        _answers[2].SetText(answer3);

    }

    public async UniTask WaitForAnswer()
    {
        // Create a new UniTaskCompletionSource
        answerCompletionSource = new UniTaskCompletionSource<string>();

        // Wait until an answer is selected
        string selectedAnswer = await answerCompletionSource.Task;

        // Clean up
        answerCompletionSource = null;

        Hide();

    }

    public async UniTask SetQuestionAndWaitForAnswer(string question, string answer1, string answer2, string answer3)
    {
        Show();
        InitAnswers();
        setQuestion(question, answer1, answer2, answer3);
        await WaitForAnswer();

        Hide();
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
    public void Show()
    {
        gameObject.SetActive(true);
    }
}
