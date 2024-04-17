using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class FloatingBoard : MonoBehaviour
{
    [SerializeField]
    private TextPopUp board;
    [SerializeField]
    private TXRButtonTouch ContinueButton;

    public async UniTask ShowTextUntilContinue(string text)
    {
        SetText(text);
        await ShowUntilContinuePressed();
    }

    public void SetText(string text)
    {
        board.SetTextAndScale(text);
    }

    public async UniTask ShowUntilContinuePressed()
    {
        board.gameObject.SetActive(true);
        ContinueButton.gameObject.SetActive(true);
        await ContinueButton.WaitForButtonPress();
        
        //replace with a fade out animation TODO
        board.gameObject.SetActive(false);
        ContinueButton.gameObject.SetActive(false);
    }


    private void Start()
    {
        print("FloatingBoard.cs: in Start()");
        SetText("Hello World");
        ShowUntilContinuePressed().Forget();    
    }
}
