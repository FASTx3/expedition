using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LitJson;
using System.Linq;

public class ItemMN : MonoBehaviour
{
    void Awake()
    {
        GameData.Instance._itemMN = this;
    }

    public void OpenItem()
    {
        OnLoadItemUI();
        
        GameData.Instance._popMN.OnEquipPop(1);
        GameData.Instance._popMN.OpenEquipPop();
    }

    public void OnEquipItem()
    {
        GameData.Instance._popMN.OnEquipPop(3);        
    }

    public void OnEquipCancel()
    {
        GameData.Instance._popMN.CancelEquip();
    }

    public List<Text> _item_text = new List<Text>();
    public List<GameObject> _obj = new List<GameObject>();
    public List<GameObject> _obj_btn = new List<GameObject>();

    public void OnLoadItemUI()//대원 목록 UI 불러오기
    {
        //if(_sellect_unit == null)
        //{
            _item_text[0].text = "";//이름
            _item_text[1].text = "";//레벨
            _item_text[2].text = "";//종류
            _item_text[3].text = "";//공격력
            _item_text[4].text = "";//방어력
            _item_text[5].text = "";//속성
            _item_text[6].text = "";//상태
            _item_text[7].text = "";//설명

            _obj_btn[0].SetActive(true);
            _obj_btn[1].SetActive(false);
        //}

        /*
        for(var i = 0; i < _unit_data.Count; i++)
        {
            CreateUnitObject(_unit_data[i]);
        }
        */
    }

}
