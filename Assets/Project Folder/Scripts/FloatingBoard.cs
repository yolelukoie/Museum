using Cysharp.Threading.Tasks;
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

    private void SetText(string text)
    {
        board.SetTextAndScale(text);
    }

    private async UniTask ShowUntilContinuePressed()
    {
        board.gameObject.SetActive(true);
        ContinueButton.gameObject.SetActive(true);
        await ContinueButton.WaitForButtonPress();

        //replace with a fade out animation TODO
        board.gameObject.SetActive(false);
        ContinueButton.gameObject.SetActive(false);
    }



}
