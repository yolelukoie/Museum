using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;

public class SessionManager : TXRSingleton<SessionManager>
{
    const String ACTIVE_TYPE = "Type 1";
    const String PASSIVE_TYPE = "Type 2";
    const String BOTH_TYPE = "Type 3";

    const int FIRST_PIECE_INDEX = 0;

    private FloatingBoard _floatingBoard;
    private MultiChoiceQuestion _multiChoiceQuestion;
    private List<Piece> _pieces;
    private String _experimentType;
    private int _maxQuestionsInSemiActiveTour;

    List<List<String>> activeTourQuestions = new List<List<String>>();


    //If there is a higher level flow manager, remove this and use his start method
    private void Start()
    {
        RunSessionFlow().Forget();
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
                case PASSIVE_TYPE:
                    break;

                // If the piece is not the first piece and the experiment type is active or both (but than the piece index is within the active range), show the question
                case ACTIVE_TYPE:
                case BOTH_TYPE:
                    if ((pieceIndex > FIRST_PIECE_INDEX) & ((_experimentType == ACTIVE_TYPE) || (pieceIndex <= _maxQuestionsInSemiActiveTour)))
                    {
                        await _multiChoiceQuestion.SetQuestionAndWaitForAnswer(activeTourQuestions[pieceIndex][0], activeTourQuestions[pieceIndex][1], activeTourQuestions[pieceIndex][2], activeTourQuestions[pieceIndex][3]);
                    }
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

        //get some references:
        _floatingBoard = SceneReferencer.Instance.floatingBoard;
        _pieces = SceneReferencer.Instance.pieces;
        _multiChoiceQuestion = SceneReferencer.Instance.multiChoiceQuestion;
        _maxQuestionsInSemiActiveTour = SceneReferencer.Instance.NumberOfQuestionsInSemiActiveTour;

        //make sure everything is hidden
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

    private void InitActiveTourQuestionsList()
    {
        List<SerializedMultichoiceQuestion> MultichoiceQuestions = SceneReferencer.Instance.questions;
        foreach (SerializedMultichoiceQuestion q in MultichoiceQuestions)
        {
            activeTourQuestions.Add(new List<String> { q.Question, q.Answer1, q.Answer2, q.Answer3 });
        }

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
        await _multiChoiceQuestion.SetQuestionAndWaitForAnswer("Choose the type of experiment", ACTIVE_TYPE, PASSIVE_TYPE, BOTH_TYPE);
        MultichoiceAnswer.OnAnswerSelected.RemoveListener(processExperimentType);
        await UniTask.Delay(TimeSpan.FromSeconds(1));

        //validate with operator
        await _floatingBoard.ShowTextUntilContinue("Starting " + _experimentType + " tour");
        await UniTask.Delay(TimeSpan.FromSeconds(1));
    }

    private void processExperimentType(string selectedAnswer)
    {
        switch (selectedAnswer)
        {
            case "Active":
                _experimentType = ACTIVE_TYPE;
                break;
            case "Passive":
                _experimentType = PASSIVE_TYPE;
                break;
            case "Both":
                _experimentType = BOTH_TYPE;
                break;
            default:
                break;
        }
    }

}
