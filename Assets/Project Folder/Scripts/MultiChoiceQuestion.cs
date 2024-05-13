using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

public class MultiChoiceQuestion : MonoBehaviour
{
    [SerializeField]
    private TextPopUp board;
    [SerializeField]
    private List<MultichoiceAnswer> _answers;
    private void Start()
    {
        MultichoiceAnswer.OnAnswerSelected.AddListener(ProcessAnswer);
    }

    void ProcessAnswer(string selectedAnswer)
    {
        // TODO report selected answer
        Debug.Log("Selected answer: " + selectedAnswer);
    }



    public async UniTask ShowTextUntilContinue(string text)
    {
        SetText(text);
        await ShowUntilContinuePressed();
    }

    private void SetText(string text)
    {
        board.SetTextAndAutoScale(text);
    }



    private async UniTask ShowUntilContinuePressed()
    {
        board.Show();
        ContinueButton.gameObject.SetActive(true);

        await ContinueButton.WaitForButtonPress();

        board.Hide();
        ContinueButton.gameObject.SetActive(false);
    }



}
