using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponData : MonoBehaviour
{
    public int _data_index;

    public long _base_atk;
    public long _base_price;

    public long _atk;
    public long _price;
    

    public void OnSet(int index)
    {
        _data_index = index;

        _base_atk = GameData.Instance._itemDataIndex[_data_index]._power;
        _base_price = GameData.Instance._itemDataIndex[_data_index]._price;

        OnAtk();
        OnPrice();
    }

    public void OnAtk()
    {        
        _atk = _base_atk + (GameData.Instance.StringToLong((_base_atk * 0.2f).ToString("F0")) * (GameData.Instance._playerData._weapon[_data_index]-1));
    }

    public void OnPrice()
    {
        _price = _base_price + (GameData.Instance.StringToLong((_base_price * 0.5f).ToString("F0")) * GameData.Instance._playerData._weapon[_data_index]);
    }

    public void OnLevelUp()
    {
        //if(_price > GameData.Instance._playerData._gold) return;

        GameData.Instance._playerData._weapon[_data_index] += 1;

        OnAtk();
        OnPrice();

        GameData.Instance._expeditionMN.OnShowInfo();

        if(GameData.Instance._itemMN._weapon_list.ContainsKey(_data_index)) GameData.Instance._itemMN._weapon_list[_data_index].OnValue();      
    }
}
