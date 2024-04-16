using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using TMPro;


public class TextPopUp : MonoBehaviour
{

    private const float DefaultTextWidth = 0.48f;
    private const float DefaultTextHeight = 0.08f;
    private const float DefaultTextFontSize = 0.3368976f;
    private const int NumberOfLettersUntilLineWrap = 32;
    private const int NumberOfLettersWhenScaleIsOne = 55;

    [SerializeField] private bool _useAnimation = true;

    [SerializeField] private Transform _background;
    [SerializeField] private TextMeshPro _textUI;

    [SerializeField] private float _fontSizeMultiplier = 1;




    //old variables
    private const float ScaleMultiplierScalar = 0.5f;
    [SerializeField] private GameObject _backFace;
    [SerializeField] private GameObject _pointer;


    [TextArea(1, 10)] [SerializeField] private string _text;

    private void OnEnable()
    {
        if (!_useAnimation)
        {
            GetComponent<Animator>().Play("Birth", -1, 1);
        }
    }


    public void SetText(string newText)
    {
        _textUI.text = newText;
    }

    public void SetTextAndScale(string newText)
    {
        _textUI.text = newText;
        _textUI.fontSize = DefaultTextFontSize * _fontSizeMultiplier;
        float newScale = (float)newText.Length / NumberOfLettersWhenScaleIsOne;
        newScale = Mathf.Sqrt(newScale) * _fontSizeMultiplier;
        UpdateTextRect(newScale);
        _textUI.rectTransform.sizeDelta = new Vector2(DefaultTextWidth * newScale,
            DefaultTextHeight * newScale);
        _background.localScale = new Vector3(1, newScale, newScale);
    }

    private void UpdateTextRect(float scaleMultiplier)
    {
        _textUI.rectTransform.sizeDelta = new Vector2(DefaultTextWidth * scaleMultiplier,
            DefaultTextHeight * scaleMultiplier);
    }

    //public void SetTextAndScaleFromSerializedField()
    //{
    //    _textUI.text = _text;
    //    SetNewScale();
    //}

    //--------------------------------------------

    //public void SetNewScale()
    //{
    //    float scaleMultiplier = CalculateScaleMultiplier();
    //    UpdateBackFace(scaleMultiplier);
    //    UpdateTextUI(scaleMultiplier);
    //    UpdatePointer(scaleMultiplier);
    //}


    //private float CalculateScaleMultiplier()
    //{
    //    float scaleMultiplier = (float)_textUI.text.Length / NumberOfLettersWhenScaleIsOne;
    //    scaleMultiplier = Mathf.Lerp(1, scaleMultiplier, ScaleMultiplierScalar);
    //    if (_textUI.text.Length > 300)
    //    {
    //        scaleMultiplier /= 1.5f;
    //    }

    //    return scaleMultiplier;
    //}

    //private void UpdateBackFace(float scaleMultiplier)
    //{
    //    //TODO: instead of changing scale, change rectangle size
    //    _backFace.transform.localScale = new Vector3(scaleMultiplier, scaleMultiplier, 1);
    //}

    //private void UpdateTextUI(float scaleMultiplier)
    //{
    //    _textUI.rectTransform.sizeDelta = new Vector2(DefaultTextWidth * scaleMultiplier,
    //        DefaultTextHeight * scaleMultiplier);
    //}

    private void UpdatePointer(float scaleMultiplier)
    {
        //TODO: instead of changing scale, change rectangle size
        _pointer.transform.localPosition = new Vector3(-0.0034f, 0,
            _textUI.rectTransform.offsetMax.x + 0.02f * scaleMultiplier);
        _pointer.transform.localScale = new Vector3(scaleMultiplier, scaleMultiplier, 1);
    }

    //-----------------

#if UNITY_EDITOR
    [Button]
    public void GetTextFromComponent()
    {
        _text = _textUI.text;
    }

    [Button]
    public void SetTextAndScale()
    {
        SetTextAndScale(_text);
    }

    [Button]
    public void DebugNumberOfLetters()
    {
        Debug.Log(_textUI.text.Length);
    }

    [Button]
    public void ResetScale()
    {
        _textUI.rectTransform.sizeDelta = new Vector2(0.48f, 0.08f);
        _background.transform.localScale = Vector3.one;
    }

#endif
}