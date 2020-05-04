using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestData : MonoBehaviour
{
    public Quest _quest_ui;

    public int _index;
    public int _lev;
    public long _gold;
    public float _time;
    public float _time_max;
    public float _time_percent;

    public bool _play_quest;//퀘스트 진행
    public bool _auto;//퀘스트 자동 진행

    public long _upgrade_gold;
    public long _reward_gold;

    public void OnSet(int index)
    {
        _index = index;

        _time_max = GameData.Instance._questDataIndex[index]._time;
        _time_percent = 1/_time_max;
        _time = 0;

        OnLevelSet();
    }

    public void OnLevelSet()
    {
        if(GameData.Instance._playerData._quest.ContainsKey(_index))
        {
            _lev = GameData.Instance._playerData._quest[_index];//퀘스트 레벨
            if(GameData.Instance._playerData._quest[_index] >= 30)
            {
                if(!_auto) 
                {
                    _auto = true;
                    if(!_play_quest) _play_quest = true;
                }                
            }
            else
            {
                if(_auto) 
                {
                    _auto = false;
                    if(_play_quest) _play_quest = false;
                }                
            }
        }            
        else             
        {
            _lev =  0;//퀘스트 레벨
        }

        Reward_Gold();
        Upgrade_Gold();

        if(_quest_ui != null) _quest_ui.OnInfoUpdate();
    }

    public void Reward_Gold()//보상 골드 계산
    {
        if(GameData.Instance._playerData._quest.ContainsKey(_index) && GameData.Instance._playerData._quest[_index] > 0)
            _reward_gold = GameData.Instance._questDataIndex[_index]._reward * GameData.Instance._playerData._quest[_index];   
        else
            _reward_gold = 0;        
    }
    
    public void Upgrade_Gold()//퀘스트 레벨업 비용 계산
    {
        if(GameData.Instance._playerData._quest.ContainsKey(_index) && GameData.Instance._playerData._quest[_index] > 0)
        {
            _upgrade_gold = GameData.Instance._questDataIndex[_index]._upgrade;
            for(var i = 0; i < GameData.Instance._playerData._quest[_index]; i++)            
                _upgrade_gold += GameData.Instance.StringToLong((_upgrade_gold*0.11f).ToString("F0"));            
        }
        else
            _upgrade_gold = GameData.Instance._questDataIndex[_index]._upgrade;    
    }

    public void OnLevelUp()
    {      
        if(_index > 0)
        {
            if(!GameData.Instance._playerData._quest.ContainsKey(_index-1)) return;
            else
            {
                if(GameData.Instance._playerData._quest[_index-1] < 1) return;  
            }    
        }
        
        //if(GameData.Instance._playerData._gold < _upgrade_gold) return;

        if(GameData.Instance._playerData._quest.ContainsKey(_index)) GameData.Instance._playerData._quest[_index]++;
        else GameData.Instance._playerData._quest.Add(_index, 1);

        OnLevelSet();
    }

    public void OnQuest()
    {        
        if(!GameData.Instance._playerData._quest.ContainsKey(_index)) return;
        if(GameData.Instance._playerData._quest[_index] <= 0) return;

        _time = _time_max;
        _play_quest = true;
    }

    void Update()
    {
        if(_play_quest)
        {
            _time -= GameData.Instance._gm._timeRate;
            
            if(_quest_ui != null) 
                _quest_ui._time_guage.value = _time*_time_percent;

            if(_time <= 0)
            {
                if(!_auto) _play_quest = false;
                _time = _time_max;
            }
        }
    }
}
