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
    //public SpriteRenderer spriteComponent;
    //public Vector2 sizeInCm;

    static private int pixelsPerUnit = 15;

    private void Awake()
    {
        //Vector2 size = sizeInCm * 10f;
        imageComponent.texture = resizedImage;
        //spriteComponent.sprite = Sprite.Create(resizedImage, new Rect(0, 0, size.x, size.y), new Vector2(0.5f, 0.5f), pixelsPerUnit);
    }


}

