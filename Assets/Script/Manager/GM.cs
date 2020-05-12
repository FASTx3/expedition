using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GM : MonoBehaviour
{
    void Awake()
    {
        GameData.Instance._gm = this;
    }

    public float _timeRate;
    void Update()
    {
        _timeRate = Time.deltaTime;
    }

    public bool _save_reset;
    void Start()
    {
        GameData.Instance._popMN.OnPopReset();
        StartCoroutine(StartLoadData());//데이터 로드
    }

    IEnumerator StartLoadData()
    {
        yield return StartCoroutine(GameData.Instance._unitMN.SetUnitMNData());//유닛 데이터 불러오기
        yield return StartCoroutine(GameData.Instance._monsterMN.SetMonsterMNData());//몬스터 데이터 불러오기
        yield return StartCoroutine(GameData.Instance._itemMN.SetItemMNData());//몬스터 데이터 불러오기
        yield return StartCoroutine(GameData.Instance._questMN.SetQuestMNData());//퀘스트 데이터 불러오기

        if(_save_reset) PlayerPrefs.DeleteAll();

        yield return StartCoroutine(GameData.Instance.LoadData());

        if(GameData.Instance._playerData._save_data)
        {
            GameData.Instance._itemMN.OnLoadItemData();//보유한 아이템 데이터 불러오기
            GameData.Instance._unitMN.OnLoadUnitData();//보유한 유닛 데이터 불러오기
            GameData.Instance._expeditionMN.OnLoadTeam();//파티 슬롯 데이터 불러오기
            GameData.Instance._battleMN.OnLoadBattle();//원정 데이터 불러오기            
        }
        else FirstData();//게임 최초 시작시 생성되는 데이터
        
        GameData.Instance._battleMN.OnBattlePosition();
        GameData.Instance._questMN.OnLoadQuest();//퀘스트 데이터 불러오기
        GameData.Instance._questMN.OpenQuest();
    }

    public void FirstData()//처음 시작시 데이터
    {
        GameData.Instance._playerData._save_data = true;

        GameData.Instance._playerData._test_gold.Add(0);
        
        GameData.Instance._expeditionMN.OnAddTeam();//팀 세팅
        
        GameData.Instance._unitMN.GetUnit(1);//최초 캐릭터 획득
        GameData.Instance._expeditionMN.OnTeamIn(0);//원정대 배치
    }

    public void OnPopup()
    {

    }

    public void ClosePopup()
    {

    }

    public void OnToast()
    {

    }
}
