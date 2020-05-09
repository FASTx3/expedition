using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour
{
    public ItemData _item_info;
    public GameObject _sellect;
    public Transform _parent;

    public void OnSet(ItemData _data)
    {        
        _item_info = _data;       
        _data._item_obj = this; 
        _sellect.SetActive(false);

        //Transform _obj = Instantiate<Transform>(GameData.Instance._unitMN._unit_prafab[_data._unit_index-1].transform, _parent);
        //_obj.localPosition = Vector3.zero;
        //_obj.localScale = new Vector3(30, 30, 1);
    }
    public void OnSellect()
    {
        GameData.Instance._itemMN.OnSellectItem(_item_info);
    }

    public void OnLevelUp()
    {
        
    }
}
