using System.Collections.Generic;
using UnityEngine;


// Stores references for everything needer to refer to in the scene.
public class SceneReferencer : TXRSingleton<SceneReferencer>
{
    //[Header("Configurations")]


    //[Header("Objects")]
    [Header("Art Pieces")]
    public List<Piece> pieces;

    [Space(10)]
    public FloatingBoard floatingBoard;
    public MultiChoiceQuestion multiChoiceQuestion;

}
