using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CampMN : MonoBehaviour
{
    void Awake()
    {
        GameData.Instance._campMN = this;
    }

    public void OpenCamp()
    {
        if(!GameData.Instance._popMN.OnMainPop(3)) return;
    }
}
