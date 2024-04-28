using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;

public class SessionManager : TXRSingleton<SessionManager>
{
    //[SerializeField] private Round[] _rounds;
    //private int _currentRound;

    private FloatingBoard _floatingBoard;
    private List<Piece> _pieces;



    //If there is a higher level flow manager, remove this and use his start method
    private void Start()
    {
        RunSessionFlow().Forget();
    }

    public async UniTask RunSessionFlow()
    {
        StartSession();
        //this is where our flow goes

        await _floatingBoard.ShowTextUntilContinue("Go To Klimt");
        foreach (Piece p in _pieces)
        {
            await p.audioGuideButton.WaitForAudioGuideToFinish();
        }

        await _floatingBoard.ShowTextUntilContinue("Finished");



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
        _floatingBoard = SceneReferencer.Instance.floatingBoard;
        _pieces = SceneReferencer.Instance.pieces;

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
}