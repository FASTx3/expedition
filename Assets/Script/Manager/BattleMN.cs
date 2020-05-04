using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BattleMN : MonoBehaviour
{
    void Awake()
    {
        GameData.Instance._battleMN = this;
    }

    public Transform _battle_parent;
    public Battle _battle_ori;
    public List<Battle> _battle = new List<Battle>(); 
    
    public void OnLoadBattle()
    {
        for(var i = 0; i < GameData.Instance._expeditionMN._team_data.Count; i++)
        {
            CreateBattle(GameData.Instance._expeditionMN._team_data[i]);
        }
    }
    public void CreateBattle(TeamData team)
    {
        Battle obj = null;
        obj = Instantiate<Battle>(_battle_ori, _battle_parent);
        obj.transform.localScale = Vector3.one;
        obj.transform.localPosition = new Vector3(10000, 0, 0);
        obj.OnTeamSetting(team);

        _battle.Add(obj);
    }

    public void OnBattlePosition()
    {
        _battle[GameData.Instance._expeditionMN._team_now].transform.localPosition = Vector3.zero;
        _battle[GameData.Instance._expeditionMN._team_now]._team_data.OnStageInfo();
    }

    public bool _page = false;

    public void OnLeft()
    {
        _battle[GameData.Instance._expeditionMN._team_now+1].transform.localPosition = new Vector3(10000, 0, 0);
        _battle[GameData.Instance._expeditionMN._team_now].transform.localPosition = Vector3.zero; 
        
        _page = false; 
    }

    public void OnRight()
    {
        _battle[GameData.Instance._expeditionMN._team_now-1].transform.localPosition = new Vector3(10000, 0, 0);
        _battle[GameData.Instance._expeditionMN._team_now].transform.localPosition = Vector3.zero; 

        _page = false;
    }

    public Material _map_ori;
    public List<Text> _text = new List<Text>();
}
