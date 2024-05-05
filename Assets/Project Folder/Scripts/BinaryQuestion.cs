using Cysharp.Threading.Tasks;
using UnityEngine;

public class BinaryQuestion : MonoBehaviour, Question
{
    public string QuestionText;
    public Texture2D ImageAnswerA;
    public Texture2D ImageAnswerB;

    private BinaryAnswer _answerA;
    private BinaryAnswer _answerB;
    private TextPopUp question;

    public async UniTask WaitForAnswer()
    {


        throw new System.NotImplementedException();
    }

    void Start()
    {

        BinaryAnswer[] allAnswers = GetComponentsInChildren<BinaryAnswer>();
        _answerA = allAnswers[0];
        _answerB = allAnswers[1];

        _answerA.SetImage(ImageAnswerA);
        _answerB.SetImage(ImageAnswerB);


        question = GetComponentInChildren<TextPopUp>();
        question.SetTextAndAutoScale(QuestionText);
    }

}
