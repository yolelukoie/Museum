using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using UnityEngine;


[Serializable]
[ShowOdinSerializedPropertiesInInspector]
public class SerializedMultichoiceQuestion
{
    [OdinSerialize]
    public string Question;
    [OdinSerialize]
    public string Answer1;
    [OdinSerialize]
    public string Answer2;
    [OdinSerialize]
    public string Answer3;

    public float size_x;
    public float size_y;
}


[Serializable]
public class TextAndScaleTuple
{
    [TextArea(3, 10)]
    public string text;
    public float scale_x;
    public float scale_y;
}

