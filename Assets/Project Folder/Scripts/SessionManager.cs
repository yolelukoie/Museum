using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;

public class SessionManager : TXRSingleton<SessionManager>
{
    const String ACTIVE_TYPE = "Type 1";
    const String BOTH_TYPE = "Type 2";
    const String PASSIVE_TYPE = "Type 3";

    const int FIRST_PIECE_INDEX = 0;

    private FloatingBoard _floatingBoard;
    private MultiChoiceQuestion _multiChoiceQuestion;
    private List<Piece> _pieces;
    private String _experimentType;
    private int _maxQuestionsInSemiActiveTour;
    private List<SerializedMultichoiceQuestion> _questions;
    private ArrowPointer _directionArrow;

    List<List<String>> activeTourQuestions = new List<List<String>>();
    private int question_index = 0;


    //If there is a higher level flow manager, remove this and use his start method
    private void Start()
    {
        RunSessionFlow().Forget();
    }


    public async UniTask RunSessionFlow()
    {
        StartSession();

        //wait for calibration to finish
        await OperatorsInit();

        //run the session
        await ShowInstructions(SceneReferencer.Instance.ScaledInstructions);

        //main session loop
        foreach (Piece p in _pieces)
        {
            int pieceIndex = _pieces.IndexOf(p);

            p.arrow.gameObject.SetActive(true);
            p.audioGuideButton.gameObject.SetActive(true);

            _directionArrow.ShowAndSetTarget(p.audioGuideButton.transform);
            await ShowInstructions(SceneReferencer.Instance.BetweenPiecesMsg);

            await p.audioGuideButton.WaitForAudioGuideToFinish();

            p.audioGuideButton.gameObject.SetActive(false);

            //should we show the question?
            switch (_experimentType)
            {
                case PASSIVE_TYPE:
                    break;

                // If the experiment type is active or both (but than the piece index is within the active range), show the question
                case ACTIVE_TYPE:
                case BOTH_TYPE:
                    if ((_experimentType == ACTIVE_TYPE) || (pieceIndex <= _maxQuestionsInSemiActiveTour))
                    {
                        SetQuestionPosition(p.questionBoardPositioner);
                        await ShowNextQuestion();
                    }
                    break;
            }

        }

        await ShowInstructions(SceneReferencer.Instance.endInstructionsAndScales);
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
        _questions = SceneReferencer.Instance.questions;
        _directionArrow = SceneReferencer.Instance.arrowPointer;

        //make sure everything is hidden
        foreach (Piece p in _pieces)
        {
            p.arrow.gameObject.SetActive(false);
            p.audioGuideButton.gameObject.SetActive(false);
        }
        _multiChoiceQuestion.gameObject.SetActive(false);
        _directionArrow.Hide();

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


    // experiment operator chooses type of experiment, etc
    private async UniTask OperatorsInit()
    {
        //wait for calibration to finish
        await _floatingBoard.ShowTextUntilContinue("Press continue when you've finished calibrating");
        await UniTask.Delay(TimeSpan.FromSeconds(1));

        //set experiment type
        MultichoiceAnswer.OnAnswerSelected.AddListener(processExperimentType);
        await _multiChoiceQuestion.SetQuestionAndWaitForAnswer("Choose the type of experiment", ACTIVE_TYPE, BOTH_TYPE, PASSIVE_TYPE);
        MultichoiceAnswer.OnAnswerSelected.RemoveListener(processExperimentType);
        await UniTask.Delay(TimeSpan.FromSeconds(1));

        //validate with operator
        await _floatingBoard.ShowTextUntilContinue("Starting " + _experimentType + " tour");
        await UniTask.Delay(TimeSpan.FromSeconds(1));
    }

    private void processExperimentType(string selectedAnswer)
    {
        _experimentType = selectedAnswer;

        string message = "Selected experiment type: " + _experimentType + ", ";
        switch (_experimentType)
        {
            case ACTIVE_TYPE:
                message += "Active";
                break;
            case PASSIVE_TYPE:
                message += "Passive";
                break;
            case BOTH_TYPE:
                message += "Semi-active/both";
                break;
            default:
                message = "something went wrong with tour type selection";
                break;
        }


        print(message);
        TXRDataManager.Instance.LogLineToFile(message);
    }

    private void SetQuestionPosition(Transform Positioner)
    {
        _multiChoiceQuestion.transform.position = Positioner.position;
        _multiChoiceQuestion.transform.rotation = Positioner.rotation;
    }

    private async UniTask ShowNextQuestion()
    {
        await _multiChoiceQuestion.SetQuestionAndScaleAndWaitForAnswer(_questions[question_index].Question, _questions[question_index].Answer1, _questions[question_index].Answer2, _questions[question_index].Answer3, _questions[question_index].size_x, _questions[question_index].size_y);
        question_index++;
    }

    //general method to display a list of instructions on the floating board one after the other
    private async UniTask ShowInstructions(List<TextAndScaleTuple> instructions)
    {
        foreach (TextAndScaleTuple instruction in instructions)
        {
            ////TODO change to if hebrew text
            //if (true)
            //{
            //    print("reversing instruction: " + instruction.text);
            //    instruction.text = HebrewText.ReverseString(instruction.text);
            //    print("reversed instruction: " + instruction.text);
            //}


            //check if the instruction should be shown
            bool shouldShowMsg;


            //if none is checked, show the message
            if (!(instruction.active || instruction.passive || instruction.semi))
            {
                shouldShowMsg = true;
            }
            else
            {
                switch (_experimentType)
                {
                    case ACTIVE_TYPE:
                        shouldShowMsg = instruction.active;
                        break;
                    case PASSIVE_TYPE:
                        shouldShowMsg = instruction.passive;
                        break;
                    case BOTH_TYPE:
                        shouldShowMsg = instruction.semi;
                        break;
                    default:
                        shouldShowMsg = false;
                        break;
                }
            }

            //print("should show msg (mumber " + instructions.IndexOf(instruction) + "): " + shouldShowMsg);

            //show the instruction
            if (shouldShowMsg)
            {
                await _floatingBoard.ShowTextAndScaleUntilContinue(instruction);
                await UniTask.Delay(TimeSpan.FromSeconds(1));
            }
        }
    }
}
