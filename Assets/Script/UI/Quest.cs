using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Quest : MonoBehaviour
{
    public Image _icon; 
    public List<Text> _text = new List<Text>();
    public Slider _time_guage;
    public GameObject _btn;

    public QuestData _quest;
    public long _gold;

    public bool _play_quest;//퀘스트 진행
    public bool _auto;//퀘스트 자동 진행
    
    public void OnSet(QuestData quest)
    {
        _quest = quest;
        _quest._quest_ui = this;
        
        _text[0].text = GameData.Instance._questDataIndex[quest._index]._name;//퀘스트 이름
        var ts = TimeSpan.FromSeconds(GameData.Instance._questDataIndex[quest._index]._time);
        _text[3].text = string.Format("{0:00}:{1:00}:{2:00}", ts.Hours, ts.Minutes, ts.Seconds);//퀘스트 남음 시간
        _text[4].text = "+ " + GameData.Instance._questDataIndex[quest._index]._reward;//레벨업후 추가로 얻게될 골드양

        _time_guage.value = quest._time*quest._time_percent;

        OnInfoUpdate();
    }

    public void OnInfoUpdate()
    {
        _text[1].text =  string.Format("Lv.{0}", _quest._lev);
        

        _text[1].text =  string.Format("Lv.{0}", _quest._lev);//퀘스트 레벨
        //_text[3].text = "";//퀘스트 남음 시간
        if(_quest._lev >= 30)
        {
            if(_btn.activeSelf) _btn.SetActive(false);
        }
        else
        {
            if(!_btn.activeSelf) _btn.SetActive(true);
        }

        _text[2].text = _quest._reward_gold.ToString();
        _text[5].text = _quest._upgrade_gold.ToString(); 
    }

    public void OnQuest()
    {        
        _quest.OnQuest();
    }

    public void OnLevelUp()
    {      
        _quest.OnLevelUp();
    }
}
