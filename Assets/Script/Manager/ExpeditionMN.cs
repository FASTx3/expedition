using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExpeditionMN : MonoBehaviour
{
    void Awake()
    {
        GameData.Instance._expeditionMN = this;
    }
    
    public int _team_now;
    public List<Text> _text = new List<Text>();
    public Dictionary<int, Member> _member = new Dictionary<int, Member>();

    public TeamData _team_data_ori;
    public List<TeamData> _team_data = new List<TeamData>();

    public void OnLoadTeam()//팀 데이터 불러오기
    {
        for(var i = 0; i < GameData.Instance._playerData._team.Count; i++) CreateTeamData(i);

        if(_team_data.Count > 0) OnShowInfo();
    }
    
    public GameData.Team _set_team = new GameData.Team();
    public void AddTeam()// 팀 추가
    {
        _set_team._map = 0;
        _set_team._stage = 1;
        _set_team._round = 1;
        _set_team._status = 0;

        _set_team._member = new List<int>{-1, -1, -1, -1, -1};
        _set_team._weopon = 0;
        _set_team._armor = 0;
        _set_team._acc = 0;

        _set_team._time = null;

        GameData.Instance._playerData._team.Add(_set_team);
    } 

    public void CreateTeamData(int index)//팀 데이터 생성
    {
        TeamData obj = null;
        obj = Instantiate<TeamData>(_team_data_ori, transform);
        obj.transform.localScale = Vector3.one;
        obj.transform.localPosition = Vector3.zero;
        obj.OnSet(index);

        _team_data.Add(obj);
    }

    public GameObject _equip_obj;

    public void OpenTeamSetting()//팀 배치하기 세팅
    {   
        if(GameData.Instance._unitMN._sellect_unit == null) return;

        _equip_obj.SetActive(true);
        GameData.Instance._popMN.CloseEquipPop();
    }

    public void CloseTeamSetting()//팀 배치하기 세팅 닫기
    {
        _equip_obj.SetActive(false);        
        GameData.Instance._popMN.OpenEquipPop();
    }

    public void OnTeamIn(int slot)//팀 배치하기
    {
        /*
        if(_team_data[_team_now]._status > 0)
        {
            _team_data[_team_now].OnTeamStatus(0);
            GameData.Instance._battleMN._battle[_team_now].OnMaintenance();
            //GameData.Instance._battleMN._battle[_team_now].OnBattlePause();
        }
        */        

        if(_team_data[_team_now]._member[slot] != null)//배치하는 자리에 배치된 멤버가 있을 경우
        {
            if(_team_data[_team_now]._member[slot] == GameData.Instance._unitMN._sellect_unit) return;
            _team_data[_team_now].OnTeamOut(slot, _team_data[_team_now]._member[slot]);
        }

        if(GameData.Instance._unitMN._sellect_unit._team > -1)//배치할 멤버가 이미 배치되어 있는 경우
        {
            if(GameData.Instance._unitMN._sellect_unit._team == _team_now && GameData.Instance._unitMN._sellect_unit._team_slot == slot) return;
            _team_data[GameData.Instance._unitMN._sellect_unit._team].OnTeamOut(GameData.Instance._unitMN._sellect_unit._team_slot, GameData.Instance._unitMN._sellect_unit);
        }

        _equip_obj.SetActive(false);
        GameData.Instance._unitMN.CloseUnitMN();
        _team_data[_team_now].OnTeamIn(slot, GameData.Instance._unitMN._sellect_unit);
        _team_data[_team_now].OnUpdateInfo();
    }

    public List<GameObject> _btn_obj = new List<GameObject>();

    public void OnBtnSetting()
    {
        _text[4].text = string.Format("제 {0} 원정대", (_team_now+1));

        //왼쪽 버튼 세팅
        if(_team_now > 0) _btn_obj[0].SetActive(true);
        else _btn_obj[0].SetActive(false);

        //오른쪽 버튼 세팅
        if(_team_now < _team_data.Count-1) 
        {
            _btn_obj[1].SetActive(true);
            _btn_obj[2].SetActive(false);
        }
        else
        {
            _btn_obj[1].SetActive(false);
            _btn_obj[2].SetActive(true);
        }
    }

    public void OnLeft()
    {
        if(_team_now <= 0) return;

        if(GameData.Instance._battleMN._page) return;
        else GameData.Instance._battleMN._page = true;

        _team_now--;

        OnShowInfo();
        GameData.Instance._battleMN.OnLeft();

        OnStageInfo(_team_data[_team_now]);
    }

    public void OnRight()
    {
        if(_team_now == _team_data.Count-1) return;

        if(GameData.Instance._battleMN._page) return;
        else GameData.Instance._battleMN._page = true;
        
        _team_now++;

        OnShowInfo();
        GameData.Instance._battleMN.OnRight();

        OnStageInfo(_team_data[_team_now]);
    }

    public void OnShowInfo()
    {
        if(_team_data.Count <= 0) return;

        _text[0].text = _team_data[_team_now]._atk.ToString();//팀 공격력
        _text[1].text = _team_data[_team_now]._cri.ToString();//팀 치명타율
        _text[2].text = _team_data[_team_now]._rcv.ToString();//팀 초당 회복량

        _text[3].text = "";
        OnTeamStatusText(_team_data[_team_now]._status);//팀 상태

        for(var i = 0; i < _team_data[_team_now]._member.Count; i++)
        {
            if(_team_data[_team_now]._member[i] == null) 
                _member[i].OnNull();
            else
                _member[i].OnUnitData(_team_data[_team_now]._member[i]);
        }

        OnBtnSetting();
    }

    public void OnTeamStatusText(int code)
    {
        switch(code)
        {
            case 0 :
                _text[3].text = "대기중";
            break;

            case 1 :
                _text[3].text = "원정중";
            break;

            case 2 :
                _text[3].text = "회복중";
            break;
        }
    }

    public void OnStageInfo(TeamData team)
    {
        if(_team_data[_team_now] != team) return;

        _text[7].text = string.Format("{0} 지역", team._stage);
        _text[8].text = string.Format("{0} / 5", team._round);
    }

    public void OnAddTeam()
    {
        //신규 원정 팀 추가
        AddTeam();
        CreateTeamData(GameData.Instance._playerData._team.Count-1);
        //신규 원정 지 추가
        GameData.Instance._battleMN.CreateBattle(_team_data[_team_data.Count-1]);

        OnRight();
    }

    public void OpenExpedition()
    {
        if(!GameData.Instance._popMN.OnMainPop(1)) return;
    }
    
    public void OnGold(long gold)
    {
        //_text[7].text = GameData.Instance._playerData._gold.ToString();
    }

    public void OnGold(List<int> gold)
    {
        List<int> aaa = new List<int>{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0};
        //_text[7].text = GameData.Instance._playerData._gold.ToString();
    }
}
