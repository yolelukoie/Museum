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
#if UNITY_EDITOR
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


    void SetBinaryQuestion(string questionText, Texture2D imageAnswerA, Texture2D imageAnswerB)
    {
        QuestionText = questionText;
        ImageAnswerA = imageAnswerA;
        ImageAnswerB = imageAnswerB;

        _answerA.SetImage(imageAnswerA);
        _answerB.SetImage(imageAnswerB);
    }

    void Hide()
    {
        question.Hide();
        _answerA.gameObject.SetActive(false);
        _answerB.gameObject.SetActive(false);
    }
    public void Show()
    {
        question.Show();
        _answerA.gameObject.SetActive(true);
        _answerB.gameObject.SetActive(true);
    }
#endif // UNITY_EDITOR
}
