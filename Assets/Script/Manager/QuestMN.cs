using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;

public class QuestMN : MonoBehaviour
{
    private JsonData _jsonList;

    public IEnumerator SetQuestMNData()
    {        
        yield return StartCoroutine(LoadQuestData());
    }

    public IEnumerator LoadQuestData()
	{
		TextAsset t = (TextAsset)Resources.Load("quest", typeof(TextAsset));
		yield return t;
		yield return StartCoroutine(SetDataQuestIndex(t.text));
	}

    public IEnumerator SetDataQuestIndex(string jsonString)
	{
        GameData.Instance._questDataIndex.Clear();
		_jsonList = JsonMapper.ToObject(jsonString);
		for(var i = 0; i< _jsonList.Count;i++)
        {            
            GameData.Instance._setQuest._index = System.Convert.ToInt32(_jsonList[i]["index"].ToString());
            GameData.Instance._setQuest._name = _jsonList[i]["name"].ToString();
            GameData.Instance._setQuest._time = System.Convert.ToInt32(_jsonList[i]["time"].ToString());
            GameData.Instance._setQuest._reward = System.Convert.ToInt64(_jsonList[i]["reward"].ToString());
            GameData.Instance._setQuest._upgrade = System.Convert.ToInt64(_jsonList[i]["upgrade"].ToString());
            GameData.Instance._setQuest._plus = System.Convert.ToInt64(_jsonList[i]["plus"].ToString());

			GameData.Instance._questDataIndex.Add(GameData.Instance._setQuest._index, GameData.Instance._setQuest); 
        }
        yield return null;  
    }

    void Awake()
    {
        GameData.Instance._questMN = this;
    }

    public QuestData _quest_data_ori;
    public List<QuestData> _quest_data = new List<QuestData>();
    public void OnLoadQuest()
    {
        for(var i = 0; i < GameData.Instance._questDataIndex.Count; i++) CreateQuestData(i);
    }

    public void CreateQuestData(int index)
    {
        QuestData obj = null;
        obj = Instantiate<QuestData>(_quest_data_ori, transform);
        obj.transform.localScale = Vector3.one;
        obj.OnSet(index);

        _quest_data.Add(obj);
    }

    public Transform _quest_parent;
    public Quest _quest_ori;
    public List<Quest> _quest = new List<Quest>();
    public void OpenQuest()
    {
        if(!GameData.Instance._popMN.OnMainPop(0)) return;

        OnQuestList();
    }

    public void CloseQuest()
    {
        for(var i = 0; i < _quest.Count; i++) Destroy(_quest[i].gameObject);
        _quest.Clear();
    }

    public void OnQuestList()
    {
        _quest_parent.localPosition = Vector3.zero;        
        for(var i = 0; i < _quest_data.Count; i++) CreateQuest(i);
    }

    public void CreateQuest(int index)
    {
        Quest obj = null;
        obj = Instantiate<Quest>(_quest_ori, _quest_parent);
        obj.transform.localScale = Vector3.one;
        obj.OnSet(_quest_data[index]);

        _quest.Add(obj);
    }
}
