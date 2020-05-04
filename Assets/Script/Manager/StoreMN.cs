using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoreMN : MonoBehaviour
{
    void Awake()
    {
        GameData.Instance._storeMN = this;
    }
}
