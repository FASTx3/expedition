using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TraningMN : MonoBehaviour
{
    void Awake()
    {
        GameData.Instance._trainingMN = this;        
    }

    public GameObject _obj_training;

    public void OpenTraining()
    {
        if(_obj_training == null) return;
        if(!_obj_training.activeSelf) _obj_training.SetActive(true);
    }

    public void CloseTraining()
    {
        if(_obj_training == null) return;
        if(_obj_training.activeSelf) _obj_training.SetActive(false);
    }
}
