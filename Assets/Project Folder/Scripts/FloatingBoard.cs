using Cysharp.Threading.Tasks;
using UnityEngine;

public class FloatingBoard : MonoBehaviour
{
    [SerializeField]
    private TextPopUp board;
    [SerializeField]
    private TXRButton ContinueButton;


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
        //ContinueButton.SetState(ButtonState.Interactable);

        print("FloatingBoard: ShowUntilContinuePressed() before WaitForButtonPress");
        await ContinueButton.WaitForButtonPress();
        print("FloatingBoard: ShowUntilContinuePressed() after WaitForButtonPress");

        board.Hide();
        ContinueButton.gameObject.SetActive(false);
        //ContinueButton.SetState(ButtonState.Hidden);
    }



}
