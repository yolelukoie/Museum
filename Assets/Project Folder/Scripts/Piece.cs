using UnityEngine;
using UnityEngine.UI;

public class Piece : MonoBehaviour
{
    public RawImage imageComponent;
    public Arrow arrow;
    public AudioGuideButton audioGuideButton;
    public AudioClip audioGuideClip;
    //public Texture2D origImage;
    public Texture2D resizedImage;

    private void Awake()
    {
        imageComponent.texture = resizedImage;
        //ImageResizeHelper.AdjustImageWidth(origImage, imageComponent);
    }

}
