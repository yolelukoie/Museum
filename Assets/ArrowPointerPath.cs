using System.Collections.Generic;
using UnityEngine;

public class ArrowPointerPath : TXRSingleton<ArrowPointerPath>
{

    public List<Transform> pathPointes = new List<Transform>();

    // Start is called before the first frame update
    void Awake()
    {
        foreach (Transform child in transform)
        {
            pathPointes.Add(child);
        }
    }

}
