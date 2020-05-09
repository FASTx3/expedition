using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemData : MonoBehaviour
{
    public GameData.MyItem _item_data;
    public Item _item_obj;

    public int _data_index;//데이터 내 인덱스
    public int _item_index;//아이템 인덱스
    public int _class;//아이템 종류
    public int _type;//아이템 속성 
    public int _lev;//아이템 레벨  
    public long _power;
    public UnitData _equip_unit;

    public void OnSet(int _index_data)
    {
        _data_index = _index_data;
        _item_index = GameData.Instance._playerData._item[_index_data]._index;
        _lev = GameData.Instance._playerData._item[_index_data]._lev;
        _type = GameData.Instance._playerData._item[_index_data]._type;
        _class = GameData.Instance._itemDataIndex[_item_index]._class;

        OnPower();

        _equip_unit = null;
    }

    public void OnPower()
    {
        _power = GameData.Instance._itemDataIndex[_item_index]._power;
    }

    public void OnDel()//아이템 삭제
    {

        if(_equip_unit != null) _equip_unit.UnEquipItem(this);
            
        _item_data = GameData.Instance._playerData._item[_data_index];
        _item_data._lev = 0;
        GameData.Instance._playerData._item[_data_index] = _item_data;
    }

    public void OnLevelUp()
    {
        OnPower();

        if(_equip_unit != null)
        {
            if(_class == 0) _equip_unit.OnAtk();
            else if(_class == 1) _equip_unit.OnDef();

            if(_equip_unit._team > -1) GameData.Instance._expeditionMN._team_data[_equip_unit._team].OnUpdateInfo();
        }

        if(GameData.Instance._itemMN._sellect_item == this)
            GameData.Instance._itemMN.OnSellectItem(this);
    }
}
