using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;


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
}
