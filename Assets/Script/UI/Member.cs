using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Member : MonoBehaviour
{
    public int _index;
    
    void Start()
    {
        GameData.Instance._expeditionMN._member.Add(_index, this);        
    }

    public GameObject _obj_face; //캐릭터 이미지 오브젝트
    public Image _img_face; //캐릭터 얼굴 이미지

    public Text _text_rank; //캐릭터 랭크
    public Text _text_level; //캐릭터 레벨
    public Text _text_name; //캐릭터 이름

    public void OnNull()//슬롯에 배치되지 않은 상태
    {
        _text_rank.text = "";
        _text_level.text = "";
        _text_name.text = "";
    }

    public void OnUnitData(UnitData _unit)
    {
        if(_unit == null) return;

        _text_rank.text = GameData.Instance._unitDataIndex[_unit._unit_index]._grade;
        _text_level.text = string.Format("Lv.{0}", _unit._lev);
        _text_name.text = GameData.Instance._unitDataIndex[_unit._unit_index]._name;
    }

    public void OnLevelUp()
    {
        
    }
}
