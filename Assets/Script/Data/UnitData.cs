using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitData : MonoBehaviour
{
    public GameData.MyUnit _unit_data;
    public UnitObject _unit_obj;
    public Member _unit_party;

    public int _data_index;//데이터 내 인덱스
    public int _unit_index;//대원 인덱스
    public int _lev;   
    public int _team = -1;
    public int _team_slot = -1;
    public int _buy_count;

    public ItemData _weapon;
    public ItemData _armor;
    public ItemData _acc;

    public long _hp;
    public long _atk;
    public float _cri;
    public long _def;

    public void OnSet(int _index_data)
    {
        _data_index = _index_data;//저장된 캐릭터 인덱스
        _unit_index = GameData.Instance._playerData._unit[_index_data]._index;//캐릭터 인덱스
        _lev = GameData.Instance._playerData._unit[_index_data]._lev;
        _buy_count = GameData.Instance._playerData._unit[_index_data]._buy_count;
        
        OnWeapon();
        OnArmor();

        OnHp();
        OnAtk();
        OnCri();
        OnDef();

        _team = -1;
        _team_slot = -1;
    }

    public void OnHp()
    {
        Debug.Log("_unit_index : " + _unit_index);
        _hp = GameData.Instance._unitDataIndex[_unit_index]._hp;
    }

    public void OnAtk()
    {
        _atk = GameData.Instance._unitDataIndex[_unit_index]._atk;
        if(_weapon != null) _atk += _weapon._power;
    }

    public void OnCri()
    {
        _cri = GameData.Instance._unitDataIndex[_unit_index]._cri;
    }
    public void OnDef()
    {
        _def = GameData.Instance._unitDataIndex[_unit_index]._def;
        if(_armor != null) _def += _armor._power;
    }

    public void OnWeapon()
    {
        int _weapon_code = GameData.Instance._playerData._unit[_data_index]._weapon;

        if(_weapon_code > -1)
        {
            for(var i = 0 ; i < GameData.Instance._itemMN._item_data.Count; i++)
            {
                if(GameData.Instance._itemMN._item_data[i]._data_index == _weapon_code)
                {
                    _weapon = GameData.Instance._itemMN._item_data[i];
                    _weapon._equip_unit = this; 
                }
                     
            }
        }
    }

    public void OnArmor()
    {
        int _armor_code = GameData.Instance._playerData._unit[_data_index]._armor;

        if(_armor_code > -1)
        {
            for(var i = 0 ; i < GameData.Instance._itemMN._item_data.Count; i++)
            {
                if(GameData.Instance._itemMN._item_data[i]._data_index == _armor_code)
                    _armor = GameData.Instance._itemMN._item_data[i];
                    _armor._equip_unit = this;
            }
        }
    }

    public void OnLevelUp()
    {
        _unit_data = GameData.Instance._playerData._unit[_data_index];
        _unit_data._lev += 1;
        GameData.Instance._playerData._unit[_data_index] = _unit_data;

        _lev = GameData.Instance._playerData._unit[_data_index]._lev;
        OnHp();
        OnAtk();
        OnCri();
        
        if(_unit_obj != null) _unit_obj.OnLevelUp();
        if(_team > -1) GameData.Instance._expeditionMN._team_data[_team].OnUpdateInfo();
    }

    public void OnDel()//직원 해고
    {
        if(_team > -1) 
        {
            TeamData _team_data = GameData.Instance._expeditionMN._team_data[_team];
            _team_data.OnTeamOut(_team_slot, this);
            _team_data.OnUpdateInfo();
        }

        if(_weapon != null) _weapon._equip_unit = null;//무기를 착용하고 있으면 자동 해제
        if(_armor != null) _armor._equip_unit = null;//방어구를 착용하고 있으면 자동 해제

        _unit_data = GameData.Instance._playerData._unit[_data_index];
        _unit_data._lev = 0;
        GameData.Instance._playerData._unit[_data_index] = _unit_data;
    }

    public void OnTeamIn(int team, int slot)//팀
    {
        _team = team;
        _team_slot = slot;
    }

    public void OnTeamOut()
    {
        _team = -1;
        _team_slot = -1;
    }

    public void OnEquipItem(ItemData item)
    {
        item._equip_unit = this;
        _unit_data = GameData.Instance._playerData._unit[_data_index];

        if(item._class == 0)//무기
        {
            if(_weapon != null) _weapon._equip_unit = null;
            _unit_data._weapon = item._data_index;
            _weapon = item;
            OnAtk();
        }
        else if(item._class == 1)//방어구
        {
            if(_armor != null) _armor._equip_unit = null;
            _unit_data._armor = item._data_index;            
            _armor = item;
            OnDef();
        }
        GameData.Instance._playerData._unit[_data_index] = _unit_data;

        if(_team > -1) GameData.Instance._expeditionMN._team_data[_team].OnUpdateInfo();
    }

    public void UnEquipItem(ItemData item)
    {
        item._equip_unit = null;
        _unit_data = GameData.Instance._playerData._unit[_data_index];

        if(item._class == 0)//무기
        {
            _unit_data._weapon = -1;
            GameData.Instance._playerData._unit[_data_index] = _unit_data;
            _weapon = null;
            OnAtk();
        }
        else if(item._class == 1)//방어구
        {
            _unit_data._armor = -1;
            GameData.Instance._playerData._unit[_data_index] = _unit_data;
            _armor = null;
            OnDef();
        }

        if(_team > -1) GameData.Instance._expeditionMN._team_data[_team].OnUpdateInfo();
    }
}
