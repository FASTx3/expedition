using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Weapon : MonoBehaviour
{
    public WeaponData _data;
    public Image _icon; 
    public List<Text> _text = new List<Text>();

    public GameObject _sellect;
    public GameObject _lock;
    public GameObject _btn_lev;

    public int _weapon_code;

    public void OnSet(WeaponData data)
    {
        _data = data;
        _index = data._data_index;

        _text[0].text = GameData.Instance._itemDataIndex[_index]._name;
        OnValue();

        _lock.SetActive(false);

        if(GameData.Instance._playerData._weapon.Count == _index) _sellect.SetActive(true);
        else _sellect.SetActive(false);        
    }

    public void OnValue()
    {
        _text[1].text = string.Format("Lv. {0} / 10", GameData.Instance._playerData._weapon[_index]);
        _text[2].text = _data._atk.ToString();
        _text[3].text = _data._price.ToString();

        if(GameData.Instance._playerData._weapon[_index] >= 10) 
        {
            _btn_lev.SetActive(false);
            if(GameData.Instance._itemMN._weapon_list.ContainsKey(_index+1)) GameData.Instance._itemMN._weapon_list[_index+1].OpenLock();
        }
        else
        {
            if(!_btn_lev.activeSelf) _btn_lev.SetActive(true);
        }
    }

    public long _null_price;
    public int _index;
    public void OnNullSet(int code)
    {
        _index = code;

        _null_price = GameData.Instance._itemDataIndex[code]._price;

        _text[0].text = GameData.Instance._itemDataIndex[code]._name;
        _text[1].text = "Lv. 0 / 10";
        _text[2].text = GameData.Instance._itemDataIndex[code]._power.ToString();
        _text[3].text = GameData.Instance._itemDataIndex[code]._price.ToString();

        if(GameData.Instance._playerData._weapon.ContainsKey(_index-1) && GameData.Instance._playerData._weapon[_index-1] >= 10) _lock.SetActive(false);
        else _lock.SetActive(true);

        _sellect.SetActive(false);
    }

    public void OnLevelUp()
    {
        if(_data ==  null)
        {
            //if(_null_price > GameData.Instance._playerData._gold) return;
            GameData.Instance._itemMN.OnAddWeapon(_index);
            GameData.Instance._expeditionMN.OnShowInfo();

            if(GameData.Instance._itemMN._weapon_list.ContainsKey(_index-1)) GameData.Instance._itemMN._weapon_list[_index-1]._sellect.SetActive(false);
        }
        else _data.OnLevelUp();
    }

    public void OpenLock()
    {
        if(_lock.activeSelf) _lock.SetActive(false);
    }
}
