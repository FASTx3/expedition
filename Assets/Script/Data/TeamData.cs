using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamData : MonoBehaviour
{
    public long _hp;//원정대 체력
    public long _hp_max;//원정대 최대 체력
    public float _hp_percent;//원정대 체력 퍼센트   
    public long _atk;//원정대 공격력
    public float _cri;//원정대 치명타율
    public long _def;//원정대 초당 회복량

    public int _lv;//원정대 레벨
    public int _map;//원정중인 맵
    public int _stage;//진행중인 스테이지
    public int _round;//진행중인 라운드
    public int _status;//원정대 상태
    public int _weopon;//원정대 무기
    public int _armor;//원정대 갑옷
    public int _acc;//원정대 악세서리
    public string _time; //원정 출발 시간
    public List<UnitData> _member = new List<UnitData>{null, null, null, null, null}; //원정대 멤버

    public int _data_code;
    public GameData.Team _team_data = new GameData.Team();

    public Battle _battle;

    public long _damage;//누적 데미지

    public void OnSet(int code)//기본 정보 세팅
    {
        _data_code = code;

        _team_data = GameData.Instance._playerData._team[_data_code];
        for(var i = 0; i < _team_data._member.Count; i++)
        {
            if(_team_data._member[i] < 0) continue;

            for(var j = 0; j < GameData.Instance._unitMN._unit_data.Count; j++)
            {
                if(GameData.Instance._unitMN._unit_data[j]._unit_index == _team_data._member[i])
                {
                    _member[i] = GameData.Instance._unitMN._unit_data[j];
                    _member[i].OnTeamIn(_data_code, i);
                }                
            }            
        }
        _lv = _team_data._lv;
        _map = _team_data._map;

        _stage = _team_data._stage;
        _round = _team_data._round;
        _status = _team_data._status;

        OnStatusSetting();
    }

    public void OnTeamIn(int slot, UnitData member) //팀 배치
    {
        _team_data = GameData.Instance._playerData._team[_data_code];
        _team_data._member[slot] = member._data_index;
        GameData.Instance._playerData._team[_data_code] = _team_data;

        _member[slot] = member;
        _member[slot].OnTeamIn(_data_code, slot);        
    }

    public void OnTeamOut(int slot, UnitData member) //팀 배치 해제
    {
        _team_data = GameData.Instance._playerData._team[_data_code];
        _team_data._member[slot] = -1;
        GameData.Instance._playerData._team[_data_code] = _team_data;

        _member[slot].OnTeamOut();
        _member[slot] = null;        
    }

    public void OnUpdateInfo()//최종적인 팀 정보 갱신 호출 함수
    {
        OnStatusSetting();
        _battle.OnTeamInfoUpdate();
        if(GameData.Instance._expeditionMN._team_now == _data_code) GameData.Instance._expeditionMN.OnShowInfo();//현재 화면에 표시중인 팀이면 UI 갱신  
    }

    public void OnStatusSetting()//팀 정보 갱신
    {        
        switch(_status)
        {
            case 0 :
                _hp = 0;
            break;

            case 1 :
                _battle.OnBattlePause();
                if(_member.Count > 0) _hp = -_damage;
            break;

            case 2 :
                _battle._rest = false;
            break;
        }

        _hp_max = 0;
        _hp_percent = 0;
        _atk = 0;
        _cri = 0;
        _def = 0;

        for(var i = 0; i < _member.Count; i++)
        {
            if(_member[i] == null) continue;
            if(_status < 2) _hp += _member[i]._hp;            
            _hp_max += _member[i]._hp;
            _atk += _member[i]._atk;
            _def += _member[i]._def;
        }

        if(_hp > _hp_max) _hp = _hp_max;
        _hp_percent = 1f/_hp_max;

        if(_status == 1) _battle.OnBattleRestart();
        else if(_status == 2) _battle._rest = true;
    }

    public void OnTeamStatus(int code)//원정대 상태(0 : 원정 대기 / 1 : 원정중 / 2 : 회복중)
    {
        if(code == 1) _damage = 0;

        _status = code;
        _team_data = GameData.Instance._playerData._team[_data_code];
        _team_data._status = code;
        GameData.Instance._playerData._team[_data_code] = _team_data;

        if(GameData.Instance._expeditionMN._team_now == _data_code) 
            GameData.Instance._expeditionMN.OnTeamStatusText(code);//팀 상태
    }
    
    public void OnDamage(long damage)
    {
        _damage += damage;
        _hp -= damage;
    }

    public void OnStage(bool next)
    {
        bool next_stage = false;

        _team_data = GameData.Instance._playerData._team[_data_code];
                
        if(next) 
        {
            _round += 1;

            if(_round > 5)
            {
                _round = 1;
                _stage += 1;
                _map = Random.Range(1, GameData.Instance._monsterDataIndex.Count+1);                                

                if(_stage % 10 <= 0) 
                {                    
                    _lv += 1;
                    _team_data._lv = _lv;                    
                }

                next_stage = true;
            }
        } 
        else _round = 1;

        _team_data._map = _map;
        _team_data._stage = _stage;
        _team_data._round = _round;

        GameData.Instance._playerData._team[_data_code] = _team_data;

        OnStageInfo();
        
        if(next_stage) _battle.OnWinNext();
        else _battle.OnWin();
    }

    public void OnStageInfo()
    {
        GameData.Instance._expeditionMN.OnStageInfo(this);
    }    
}
