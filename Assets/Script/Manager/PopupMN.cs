using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupMN : MonoBehaviour
{
    void Awake()
    {
        GameData.Instance._popMN = this;        
    }

    public List<GameObject> _main = new List<GameObject>();
    
    public void OnPopReset()
    {
        for(var i = 0; i < _main.Count; i++)
        {
            _main[i].SetActive(false);
            _main[i].transform.localPosition = Vector3.zero;
        }
    }
    
    public bool OnMainPop(int code)
    {
        if(_main[code].activeSelf) return false;

        for(var i = 0; i < _main.Count; i++)
        {
            if(i == code) _main[i].SetActive(true);
            else 
            {
                _main[i].SetActive(false);
                
                switch(i)
                {  
                    case 0 :
                        GameData.Instance._questMN.CloseQuest();
                    break;
                }
            }   
        }

        return true; 
    }


    public List<GameObject> _equip_pop = new List<GameObject>();

    public void OpenEquipPop()
    {
        if(_equip_pop[0].activeSelf) return;
        _equip_pop[0].SetActive(true);
    }
    public void CloseEquipPop()
    {
        if(!_equip_pop[0].activeSelf) return;
        _equip_pop[0].SetActive(false);
    }
    public void OnEquipPop(int code)
    {       
        switch(code)
        {
            case 0 : //유닛창
                _equip_pop[1].SetActive(true);
                _equip_pop[3].SetActive(true);
                _equip_pop[5].SetActive(true);

                _equip_pop[2].SetActive(false);
                _equip_pop[4].SetActive(false);
                _equip_pop[6].SetActive(false);

                _equip_pop[7].SetActive(false);
            break;

            case 1 : //아이템창
                _equip_pop[2].SetActive(true);
                _equip_pop[4].SetActive(true);
                _equip_pop[6].SetActive(true);

                _equip_pop[1].SetActive(false);
                _equip_pop[3].SetActive(false);
                _equip_pop[5].SetActive(false);

                _equip_pop[7].SetActive(false);
            break;

            case 2 : //아이템 창에서 유닛 장착
                _equip_pop[5].SetActive(true);
                _equip_pop[6].SetActive(false);

                _equip_pop[7].SetActive(true);
            break;

            case 3 : //유닛 창에서 아이템 장착
                _equip_pop[5].SetActive(false);
                _equip_pop[6].SetActive(true);

                _equip_pop[7].SetActive(true);
            break;
        }
    }

    public void CancelEquip()
    {
        _equip_pop[7].SetActive(false);

        if(_equip_pop[5].activeSelf)//아이템 창으로 복귀
        {
            _equip_pop[5].SetActive(false);
            _equip_pop[6].SetActive(true);
        }
        else//유닛 창으로 복귀
        {
            _equip_pop[5].SetActive(true);
            _equip_pop[6].SetActive(false);
        }     
    }
}
