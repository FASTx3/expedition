using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectMN : MonoBehaviour
{
    void Awake()
    {
        GameData.Instance._collectMN = this;
    }

    public GameObject _obj_collect;

    public void OpenCollect()
    {
        if(_obj_collect == null) return;
        if(!_obj_collect.activeSelf) _obj_collect.SetActive(true);
    }

    public void CloseCollect()
    {
        if(_obj_collect == null) return;
        if(_obj_collect.activeSelf) _obj_collect.SetActive(false);
    }
}
