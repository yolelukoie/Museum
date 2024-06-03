using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;

public class SessionManager : TXRSingleton<SessionManager>
{
    //[SerializeField] private Round[] _rounds;
    //private int _currentRound;

    private FloatingBoard _floatingBoard;
    private MultiChoiceQuestion _multiChoiceQuestion;
    private List<Piece> _pieces;
    private bool _activeTour = true;
    private ExperimentType _experimentType;

    List<List<String>> activeTourQuestions = new List<List<String>>();

    //If there is a higher level flow manager, remove this and use his start method
    private void Start()
    {
        RunSessionFlow().Forget();
    }


    private void InitActiveTourQuestionsList()
    {
        List<SerializedMultichoiceQuestion> MultichoiceQuestions = SceneReferencer.Instance.questions;
        foreach (SerializedMultichoiceQuestion q in MultichoiceQuestions)
        {
            activeTourQuestions.Add(new List<String> { q.Question, q.Answer1, q.Answer2, q.Answer3 });
        }

    }

    public async UniTask RunSessionFlow()
    {
        StartSession();
        await OperatorsInit();

        await ShowBeginningInstructions();

        foreach (Piece p in _pieces)
        {
            int pieceIndex = _pieces.IndexOf(p);
            switch (_experimentType)
            {
                case ExperimentType.Active:
                    await _multiChoiceQuestion.SetQuestionAndWaitForAnswer(activeTourQuestions[pieceIndex][0], activeTourQuestions[pieceIndex][1], activeTourQuestions[pieceIndex][2], activeTourQuestions[pieceIndex][3]);
                    break;

                case ExperimentType.Both:
                    if (pieceIndex < (SceneReferencer.Instance.NumberOfQuestionsInSemiActiveTour - 1))
                    {
                        await _multiChoiceQuestion.SetQuestionAndWaitForAnswer(activeTourQuestions[pieceIndex][0], activeTourQuestions[pieceIndex][1], activeTourQuestions[pieceIndex][2], activeTourQuestions[pieceIndex][3]);
                    }
                    else if (pieceIndex == SceneReferencer.Instance.NumberOfQuestionsInSemiActiveTour)
                    {
                        //await _floatingBoard.ShowTextUntilContinue("You won't be asked what do wou want to see next from now on.");
                    }
                    break;

                case ExperimentType.Passive:
                    break;
            }

            p.arrow.gameObject.SetActive(true);
            p.audioGuideButton.gameObject.SetActive(true);

            await _floatingBoard.ShowTextUntilContinue("Follow the arrow to the next piece");
            await p.audioGuideButton.WaitForAudioGuideToFinish();

            p.arrow.gameObject.SetActive(false);
            p.audioGuideButton.gameObject.SetActive(false);

        }

        await ShowEndInstructions();

        EndSession();
    }
    private void StartSession()
    {
        // setup session initial conditions.
        InitActiveTourQuestionsList();
        _floatingBoard = SceneReferencer.Instance.floatingBoard;
        _pieces = SceneReferencer.Instance.pieces;
        _multiChoiceQuestion = SceneReferencer.Instance.multiChoiceQuestion;
        foreach (Piece p in _pieces)
        {
            p.arrow.gameObject.SetActive(false);
            p.audioGuideButton.gameObject.SetActive(false);
        }
        _multiChoiceQuestion.gameObject.SetActive(false);

    }
    private void EndSession()
    {
        // setup end session conditions
    }
    private async UniTask BetweenRoundsFlow()
    {
        await UniTask.Yield();

        throw new NotImplementedException();
    }

    private async UniTask ShowBeginningInstructions()
    {
        foreach (string instruction in SceneReferencer.Instance.instructions)
        {
            await _floatingBoard.ShowTextUntilContinue(instruction);
            await UniTask.Delay(TimeSpan.FromSeconds(1));
        }
    }

    private async UniTask ShowEndInstructions()
    {
        foreach (string instruction in SceneReferencer.Instance.endInstructions)
        {
            await _floatingBoard.ShowTextUntilContinue(instruction);
            await UniTask.Delay(TimeSpan.FromSeconds(1));
        }
    }

    // experiment operator chooses type of experiment, etc
    private async UniTask OperatorsInit()
    {
        //wait for calibration to finish
        await _floatingBoard.ShowTextUntilContinue("Press continue when you finished calibrating");
        await UniTask.Delay(TimeSpan.FromSeconds(1));
        //set experiment type
        MultichoiceAnswer.OnAnswerSelected.AddListener(processExperimentType);
        await _multiChoiceQuestion.SetQuestionAndWaitForAnswer("Choose the type of experiment", "Active", "Passive", "Both");
        MultichoiceAnswer.OnAnswerSelected.RemoveListener(processExperimentType);
        await UniTask.Delay(TimeSpan.FromSeconds(1));
        //validate with operator
        await _floatingBoard.ShowTextUntilContinue("Starting " + _experimentType.ToString() + " tour");
        await UniTask.Delay(TimeSpan.FromSeconds(1));
    }

    private void processExperimentType(string selectedAnswer)
    {
        switch (selectedAnswer)
        {
            case "Active":
                _experimentType = ExperimentType.Active;
                break;
            case "Passive":
                _experimentType = ExperimentType.Passive;
                break;
            case "Both":
                _experimentType = ExperimentType.Both;
                break;
            default:
                break;
        }
    }

}

public enum ExperimentType
{
    Active,
    Passive,
    Both
}