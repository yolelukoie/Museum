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

        //REMOVE AFTER COPYING QUESTIONS:
        //activeTourQuestions.Add(new List<String> { "What would you like to see next?", "Art piece from beggining of 20th century", "phobistic art piece", "dutch artist" });
        //activeTourQuestions.Add(new List<String> { "What would you like to see next?", "landscape painting", "an artist from paris", "Cubist art" });
        //activeTourQuestions.Add(new List<String> { "What would you like to see next?", "Expressionist painting", "Abstract painting", "Art created in 2nd world war" });
        //activeTourQuestions.Add(new List<String> { "What would you like to see next?", "a swiss artist", "Multidisciplinary artist", "an artist from paris" });
        //activeTourQuestions.Add(new List<String> { "What would you like to see next?", "A painting that refers to the art of sculpture", "Surreal art", "Of a greek artist" });
        //activeTourQuestions.Add(new List<String> { "What would you like to see next?", "Dada art", "an Israeli artist", "Defined art" });
        //activeTourQuestions.Add(new List<String> { "What would you like to see next?", "Intimate portrait", "a spanish artist", "colorful & strong art piece" });
        //activeTourQuestions.Add(new List<String> { "What would you like to see next?", "a swiss artist", "Multidisciplinary artist", "an artist from paris" });
    }

    public async UniTask RunSessionFlow()
    {
        StartSession();

        await ShowBeginningInstructions();

        foreach (Piece p in _pieces)
        {
            if (_activeTour)
            {
                await _multiChoiceQuestion.SetQuestionAndWaitForAnswer(activeTourQuestions[_pieces.IndexOf(p)][0], activeTourQuestions[_pieces.IndexOf(p)][1], activeTourQuestions[_pieces.IndexOf(p)][2], activeTourQuestions[_pieces.IndexOf(p)][3]);
            }
            p.arrow.gameObject.SetActive(true);
            await _floatingBoard.ShowTextUntilContinue("Follow the arrow to the next piece");
            await p.audioGuideButton.WaitForAudioGuideToFinish();
            p.arrow.gameObject.SetActive(false);
        }
        await _floatingBoard.ShowTextUntilContinue("Thank you for participating!");



        //while (_currentRound < _rounds.Length)
        //{
        //    await RoundManager.Instance.RunRoundFlow(_rounds[_currentRound]);
        //    await BetweenRoundsFlow();
        //    _currentRound++;
        //}


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
        }
    }
}