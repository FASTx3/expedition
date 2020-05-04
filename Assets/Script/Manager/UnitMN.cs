using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LitJson;
using System.Linq;
using Assets.HeroEditor.Common.CharacterScripts;

public class UnitMN : MonoBehaviour
{
    //데이터 불러오는 영역
    private JsonData _jsonList;
    public List<int> _unit = new List<int>();

	public IEnumerator SetUnitMNData()
    {        
        yield return StartCoroutine(LoadUnitIndexData());
    }

    public IEnumerator LoadUnitIndexData()
	{
		TextAsset t = (TextAsset)Resources.Load("agent", typeof(TextAsset));
		yield return t;
		yield return StartCoroutine(SetDatUnitIndex(t.text));
	}

    public IEnumerator SetDatUnitIndex(string jsonString)
	{
        GameData.Instance._monsterDataIndex.Clear();
		_jsonList = JsonMapper.ToObject(jsonString);
		for(var i = 0; i< _jsonList.Count;i++)
        {            
            GameData.Instance._setUnit._index = System.Convert.ToInt32(_jsonList[i]["index"].ToString());
            GameData.Instance._setUnit._total = System.Convert.ToInt64(_jsonList[i]["total"].ToString());
            GameData.Instance._setUnit._hp = System.Convert.ToInt64(_jsonList[i]["hp"].ToString());
            GameData.Instance._setUnit._atk = System.Convert.ToInt64(_jsonList[i]["atk"].ToString());
            GameData.Instance._setUnit._cri = (System.Convert.ToInt32(_jsonList[i]["cri"].ToString())*0.01f);
            GameData.Instance._setUnit._rcv = System.Convert.ToInt64(_jsonList[i]["rcv"].ToString());
            GameData.Instance._setUnit._grade = _jsonList[i]["grade"].ToString();
            GameData.Instance._setUnit._name = _jsonList[i]["name"].ToString();

			GameData.Instance._unitDataIndex.Add(GameData.Instance._setUnit._index, GameData.Instance._setUnit); 

            _unit.Add(GameData.Instance._setUnit._index);
        }
        yield return null;  
    }
    void Awake()
    {
        GameData.Instance._unitMN = this;
    }

    public void OpenUnitMN()
    {
        GameData.Instance._popMN.OnEquipPop(0);
        GameData.Instance._popMN.OpenEquipPop();

        OnLoadUnitUI();
        if(_unit_data.Count > 0 && _sellect_unit != null) 
            OnSellectUnit(_sellect_unit); 
    }

    public void CloseUnitMN()
    {
        GameData.Instance._popMN.CloseEquipPop();

        ClearUnitObjectAll();
    }

    GameData.MyUnit _base_unit;

    public void BuyUnit()//대원 모집
    {
        int _unit_index = Random.Range(1, _unit.Count+1);
        /*
        int _unit_multiple_check = -1;

        for(var i = 0; i < GameData.Instance._playerData._unit.Count; i++)//빈 데이터 여부 확인
        {
            if(GameData.Instance._playerData._unit[i]._lev <= 0) continue;
            if(GameData.Instance._playerData._unit[i]._index == _unit_index) _unit_multiple_check = i;
        }
        if(_unit_multiple_check < 0)//중복된 대원인 아닐 경우
        {
        */
            //뽑은 직원 능력치 초기화
            _base_unit._index = _unit_index;
            _base_unit._lev = 1;

            //빈 데이터 여부 확인
            int _unit_data_check = -1;
            for(var i = 0; i < GameData.Instance._playerData._unit.Count; i++)
            {
                if(GameData.Instance._playerData._unit[i]._lev <= 0)
                {
                    _unit_data_check = i;
                    break;
                }
            }
            if(_unit_data_check < 0) //빈 데이터가 없을 경우
            {
                _unit_data_check = GameData.Instance._playerData._unit.Count;
                GameData.Instance._playerData._unit.Add(GameData.Instance._playerData._unit.Count, _base_unit);
            }                
            else //빈 데이터가 있을 경우
                GameData.Instance._playerData._unit[_unit_data_check] = _base_unit;

            CreateUnitData(_unit_data_check);//대원 데이터 프리팹 생성

            ClearUnitObjectAll();
            OnLoadUnitUI();
            OnSellectUnit(_sellect_unit);   
        
        /*                       
        }       
        else//중복된 대원인 경우
        {
            for(var i = 0; i < _unit_data.Count; i++)
            {
                if(_unit_data[i]._data_index == _unit_multiple_check) 
                    _unit_data[i].OnLevelUp();
            }
        }
        */
    }

    public void GetUnit(int _unit_index)//대원 모집
    {
        //뽑은 직원 능력치 초기화
        _base_unit._index = _unit_index;
        _base_unit._lev = 1;

        //빈 데이터 여부 확인
        int _unit_data_check = -1;
        for(var i = 0; i < GameData.Instance._playerData._unit.Count; i++)
        {
            if(GameData.Instance._playerData._unit[i]._lev <= 0)
            {
                _unit_data_check = i;
                break;
            }
        }
        if(_unit_data_check < 0) //빈 데이터가 없을 경우
        {
            _unit_data_check = GameData.Instance._playerData._unit.Count;
            GameData.Instance._playerData._unit.Add(GameData.Instance._playerData._unit.Count, _base_unit);
        }                
        else //빈 데이터가 있을 경우
            GameData.Instance._playerData._unit[_unit_data_check] = _base_unit;

        CreateUnitData(_unit_data_check);//대원 데이터 프리팹 생성

        ClearUnitObjectAll();
        OnLoadUnitUI();
        OnSellectUnit(_sellect_unit);        
    }

    public void DelUnit()//대원 해고
    {
        if(_sellect_unit == null) return;

        if(_sellect_unit._unit_obj != null)//UI 삭제
        {
            _obj_unit.Remove(_sellect_unit._unit_obj);
            Destroy(_sellect_unit._unit_obj.gameObject);
        }

        _sellect_unit.OnFire();

        _unit_data.Remove(_sellect_unit);
        Destroy(_sellect_unit.gameObject);

        for(var i = 0; i < _unit_data.Count; i++)
        {
            if(_unit_data[i]._lev <= 0) continue;
            _sellect_unit = _unit_data[i];
            break;
        }
        if( _obj_unit.Count > 0) OnSellectUnit(_sellect_unit);
        else
        {
            _camp_text[0].text = "";//이름
            _camp_text[1].text = "";//레벨
            _camp_text[2].text = "";//레벨
            _camp_text[3].text = "";//체력
            _camp_text[4].text = "";//공격력
            _camp_text[5].text = "";//치명타율
            _camp_text[6].text = "";//상태
            _camp_text[7].text = "";//스킬 이름
            _camp_text[8].text = "";//스킬 내용

            _obj_btn[0].SetActive(true);
            _obj_btn[1].SetActive(false);
        }            
    }

    
    public UnitData _unit_data_ori;//대원 데이터 원본
    public List<UnitData> _unit_data = new List<UnitData>(); //대원 데이터 목록
    public void OnLoadUnitData()//대원 데이터 불러오기
    {
        for(var i = 0; i < GameData.Instance._playerData._unit.Count; i++)
        {
            if(GameData.Instance._playerData._unit[i]._lev < 1) continue;
            CreateUnitData(i);
        }
    }
    public void CreateUnitData(int index)//대원 데이터 생성 
    {
        UnitData obj = null;
        obj = Instantiate<UnitData>(_unit_data_ori, transform);
        obj.transform.localScale = Vector3.one;
        obj.transform.localPosition = Vector3.zero;
        obj.OnSet(index);

        _unit_data.Add(obj);
        _sellect_unit = obj;
    }
    
    public UnitObject _obj_unit_ori;//대원 목록 UI 원본
    public List<UnitObject> _obj_unit = new List<UnitObject>();//대원 목록
    public Transform _unit_parent;//대원 목록 UI의 속한 부모 오브젝트
    public void OnLoadUnitUI()//대원 목록 UI 불러오기
    {
        if(_sellect_unit == null)
        {
            _camp_text[0].text = "";//이름
            _camp_text[1].text = "";//레벨
            _camp_text[2].text = "";//레벨
            _camp_text[3].text = "";//체력
            _camp_text[4].text = "";//공격력
            _camp_text[5].text = "";//치명타율
            _camp_text[6].text = "";//상태
            _camp_text[7].text = "";//스킬 이름
            _camp_text[8].text = "";//스킬 내용

            _obj_btn[0].SetActive(true);
            _obj_btn[1].SetActive(false);
        }

        for(var i = 0; i < _unit_data.Count; i++)
        {
            CreateUnitObject(_unit_data[i]);
        }
    }
    public void CreateUnitObject(UnitData data)//대원 목록 UI 생성
    {
        UnitObject obj = null;
        obj = Instantiate<UnitObject>(_obj_unit_ori, _unit_parent);
        obj.transform.localScale = Vector3.one;
        obj.transform.localPosition = Vector3.zero;
        obj.OnSet(data);

        _obj_unit.Add(obj);
    }

    public void ClearUnitObjectAll()//대원 UI 모두 삭제
    {
        for(var i = 0; i < _obj_unit.Count; i++) Destroy(_obj_unit[i].gameObject);
        _obj_unit.Clear();
    }

    public List<GameObject> _obj_btn = new List<GameObject>();
    public UnitData _sellect_unit;//현재 선택중인 직원
    public List<Text> _camp_text = new List<Text>();
    public void OnSellectUnit(UnitData sellect)//직원 선택
    {
        _sellect_unit = sellect;

        for(var i = 0; i < _obj_unit.Count; i++)
        {
            if(_obj_unit[i]._unit_info != _sellect_unit) 
                _obj_unit[i]._sellect.SetActive(false);
            else
            {
                _obj_unit[i]._sellect.SetActive(true);

                _camp_text[0].text = GameData.Instance._unitDataIndex[sellect._unit_index]._name.ToString();//이름
                _camp_text[1].text = "Lv."+sellect._lev;//레벨
                _camp_text[2].text = GameData.Instance._unitDataIndex[sellect._unit_index]._grade.ToString();;//레벨
                _camp_text[3].text = sellect._hp.ToString();//체력
                _camp_text[4].text = sellect._atk.ToString();//공격력
                _camp_text[5].text = sellect._cri.ToString();//치명타율
                if(sellect._team > -1) 
                {
                    _camp_text[6].text = string.Format("{0}번 원정대 배치", (sellect._team + 1));
                    _obj_btn[0].SetActive(false);
                    _obj_btn[1].SetActive(true);
                }
                else 
                {
                    _camp_text[6].text = "배치 안됨";//상태
                    _obj_btn[0].SetActive(true);
                    _obj_btn[1].SetActive(false);
                }
                _camp_text[7].text = "";//스킬 이름
                _camp_text[8].text = "";//스킬 내용
            }
        }
    }

    public void UnEquip()
    {
        if(_sellect_unit == null) return;
        if(_sellect_unit._team > -1)
        {          
            TeamData _team_data = GameData.Instance._expeditionMN._team_data[_sellect_unit._team];
            _team_data.OnTeamOut(_sellect_unit._team_slot, _sellect_unit);
            _team_data.OnUpdateInfo();

            OnSellectUnit(_sellect_unit);
        }
    }

    public List<Character> _unit_prafab = new List<Character>();
}
