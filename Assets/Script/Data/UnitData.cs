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

    public long _hp;
    public long _atk;
    public float _cri;
    public long _rcv;

    public void OnSet(int _index_data)
    {
        _data_index = _index_data;//저장된 캐릭터 인덱스
        _unit_index = GameData.Instance._playerData._unit[_index_data]._index;//캐릭터 인덱스
        _lev = GameData.Instance._playerData._unit[_index_data]._lev;
        _buy_count = GameData.Instance._playerData._unit[_index_data]._buy_count;
        
        OnHp();
        OnAtk();
        OnCri();
        OnRcv();

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
    }

    public void OnCri()
    {
        _cri = GameData.Instance._unitDataIndex[_unit_index]._cri;
    }
    public void OnRcv()
    {
        _rcv = GameData.Instance._unitDataIndex[_unit_index]._rcv;
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

    public void OnFire()//직원 해고
    {
        if(_team > -1) 
        {
            TeamData _team_data = GameData.Instance._expeditionMN._team_data[_team];
            _team_data.OnTeamOut(_team_slot, this);
            _team_data.OnUpdateInfo();
        }

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
}
