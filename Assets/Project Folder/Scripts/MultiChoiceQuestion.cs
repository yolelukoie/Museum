using Cysharp.Threading.Tasks;
using UnityEngine;

public class MultiChoiceQuestion : MonoBehaviour
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
        board.SetTextAndAutoScale(text);
    }



    private async UniTask ShowUntilContinuePressed()
    {
        board.Show();
        ContinueButton.gameObject.SetActive(true);

        await ContinueButton.WaitForButtonPress();

        board.Hide();
        ContinueButton.gameObject.SetActive(false);
    }



}
