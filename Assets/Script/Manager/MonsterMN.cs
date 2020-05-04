using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using Assets.HeroEditor.Common.CharacterScripts;

public class MonsterMN : MonoBehaviour
{
    private JsonData _jsonList;

	public IEnumerator SetMonsterMNData()
    {        
        yield return StartCoroutine(LoadMonsterIndexData());
    }

    public IEnumerator LoadMonsterIndexData()
	{
		TextAsset t = (TextAsset)Resources.Load("monster", typeof(TextAsset));
		yield return t;
		yield return StartCoroutine(SetDatMonsterIndex(t.text));
	}

    public IEnumerator SetDatMonsterIndex(string jsonString)
	{
        GameData.Instance._monsterDataIndex.Clear();
		_jsonList = JsonMapper.ToObject(jsonString);
		for(var i = 0; i< _jsonList.Count;i++)
        {            
            GameData.Instance._setMonster._index = System.Convert.ToInt32(_jsonList[i]["index"].ToString());
            GameData.Instance._setMonster._hp = System.Convert.ToInt32(_jsonList[i]["hp"].ToString());
            GameData.Instance._setMonster._atk = System.Convert.ToInt32(_jsonList[i]["atk"].ToString());
            GameData.Instance._setMonster._cri = (System.Convert.ToInt32(_jsonList[i]["cri"].ToString())*0.01f);
            GameData.Instance._setMonster._name = _jsonList[i]["name"].ToString();

			GameData.Instance._monsterDataIndex.Add(GameData.Instance._setMonster._index, GameData.Instance._setMonster); 
        }
        yield return null;  

        SetMonsterCall();// 불러올 몬스터 확률 정리
    }


    int _monster_total = 0;
    List<int> _monster_rate = new List<int>();
    public void SetMonsterCall()//불러올 몬스터 확률 정리
    {
        _monster_total = 0;
        _monster_rate.Clear();

        for(var i = 1; i < GameData.Instance._monsterDataIndex.Count+1; i++)
        {            
            _monster_total += GameData.Instance._monsterDataIndex[i]._rate;
            _monster_rate.Add(_monster_total);
        }
    }

    void Awake()
    {
        GameData.Instance._monsterMN = this;
    }

    public List<Character> _monster_prafab = new List<Character>();
}
