using Cysharp.Threading.Tasks;
using UnityEngine;

public class FloatingBoard : MonoBehaviour
{
    [SerializeField]
    private TextPopUp board;
    [SerializeField]
    private TXRButtonTouch ContinueButton;

    private void Start()
    {

    }


    public async UniTask ShowTextUntilContinue(string text)
    {
        Debug.Log("before SetText, text: " + text);
        SetText(text);
        Debug.Log("after setText");
        await ShowUntilContinuePressed();
        Debug.Log("after showUntilContinuePressed");
    }

    private void SetText(string text)
    {
        board.SetTextAndAutoScale(text);
    }


    //TODO update to TextPopUp's Show() and Hide() methods
    private async UniTask ShowUntilContinuePressed()
    {

        Debug.Log("before show");
        board.Show();
        Debug.Log("after show");
        //board.gameObject.SetActive(true);

        Debug.Log("beforeset button active");
        ContinueButton.gameObject.SetActive(true);
        Debug.Log("after set button active");
        await ContinueButton.WaitForButtonPress();
        Debug.Log("after wait for button press");
        board.Hide();
        Debug.Log("after hide");
        //board.gameObject.SetActive(false);  
        ContinueButton.gameObject.SetActive(false);
    }



}
