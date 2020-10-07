using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LitJson;
using System.Linq;

public class ItemMN : MonoBehaviour
{
    //데이터 불러오는 영역
    private JsonData _jsonList;

    public List<int> _weapon = new List<int>();
    public List<int> _armor = new List<int>();

	public IEnumerator SetItemMNData()
    {        
        yield return StartCoroutine(LoadItemIndexData());
    }

    public IEnumerator LoadItemIndexData()
	{
		TextAsset t = (TextAsset)Resources.Load("item", typeof(TextAsset));
		yield return t;
		yield return StartCoroutine(SetDatUnitIndex(t.text));
	}

    public IEnumerator SetDatUnitIndex(string jsonString)
	{
        GameData.Instance._itemDataIndex.Clear();
		_jsonList = JsonMapper.ToObject(jsonString);
		for(var i = 0; i< _jsonList.Count;i++)
        {            
            GameData.Instance._setItem._index = System.Convert.ToInt32(_jsonList[i]["index"].ToString());
            GameData.Instance._setItem._class = System.Convert.ToInt32(_jsonList[i]["class"].ToString());
            GameData.Instance._setItem._power = System.Convert.ToInt64(_jsonList[i]["power"].ToString());
            GameData.Instance._setItem._name = _jsonList[i]["name"].ToString();
            GameData.Instance._setItem._intro = _jsonList[i]["intro"].ToString();
            GameData.Instance._setItem._price = System.Convert.ToInt64(_jsonList[i]["price"].ToString());

			GameData.Instance._itemDataIndex.Add(GameData.Instance._setItem._index, GameData.Instance._setItem); 

            if(GameData.Instance._setItem._class == 0) _weapon.Add(GameData.Instance._setItem._index);
            else if(GameData.Instance._setItem._class == 1) _armor.Add(GameData.Instance._setItem._index);
        }
        yield return null;  
    }

    void Awake()
    {
        GameData.Instance._itemMN = this;
    }

    public void OpenItem()
    {
        OnLoadItemUI();
        if(_item_data.Count > 0 && _sellect_item != null) OnSellectItem(_sellect_item);
        
        GameData.Instance._popMN.OnEquipPop(1);
        GameData.Instance._popMN.OpenEquipPop();
    }

    public bool _equip;

    public void OnEquipItem()
    {
        if(_sellect_item == null) return;

        _equip = true;
        GameData.Instance._unitMN.OnUnitList();
        GameData.Instance._popMN.OnEquipPop(2);        
    }

    public void OnEquipCancel()
    {
        _equip = false;
        GameData.Instance._popMN.CancelEquip();
        GameData.Instance._unitMN.ClearUnitObjectAll();
    }

    public void UnEquipItem()
    {
         if(_sellect_item == null) return;
         if(_sellect_item._equip_unit == null) return;   
         _sellect_item._equip_unit.UnEquipItem(_sellect_item);

         OnSellectItem(_sellect_item);
    }    

    public List<Text> _item_text = new List<Text>();
    public List<GameObject> _obj = new List<GameObject>();
    public List<GameObject> _obj_btn = new List<GameObject>();

    public ItemData _item_data_ori;   
    public List<ItemData> _item_data = new List<ItemData>();
    public ItemData _sellect_item;
    GameData.MyItem _base_item;    

    public Item _obj_item_ori;
    public List<Item> _obj_item = new List<Item>();
    public Transform _item_parent;

    public void BuyWeapon()
    {
        int code = Random.Range(0, _weapon.Count);
        int type = Random.Range(1, 7);

        GetItem(_weapon[code], type);
    }

    public void BuyArmor()
    {
        int code = Random.Range(0, _armor.Count);
        int type = Random.Range(1, 7);

        GetItem(_armor[code], type);
    }

    public void GetItem(int _item_index, int type)
    {
        //뽑은 직원 능력치 초기화
        _base_item._index = _item_index;
        _base_item._lev = 1;
        _base_item._type = type;

        //빈 데이터 여부 확인
        int _item_data_check = -1;
        for(var i = 0; i < GameData.Instance._playerData._unit.Count; i++)
        {
            if(GameData.Instance._playerData._unit[i]._lev <= 0)
            {
                _item_data_check = i;
                break;
            }
        }
        if(_item_data_check < 0) //빈 데이터가 없을 경우
        {
            _item_data_check = GameData.Instance._playerData._item.Count;
            GameData.Instance._playerData._item.Add(GameData.Instance._playerData._item.Count, _base_item);
        }                
        else //빈 데이터가 있을 경우
            GameData.Instance._playerData._item[_item_data_check] = _base_item;

        CreateItemData(_item_data_check);//대원 데이터 프리팹 생성

        ClearItemObjectAll();
        OnLoadItemUI();
        OnSellectItem(_sellect_item);
    }

    public void OnLoadItemData()//아이템 데이터 불러오기
    {
        for(var i = 0; i < GameData.Instance._playerData._item.Count; i++)
        {
            if(GameData.Instance._playerData._item[i]._lev < 1) continue;
            CreateItemData(i);
        }
    }
    public void CreateItemData(int index)
    {
        ItemData obj = null;
        obj = Instantiate<ItemData>(_item_data_ori, transform);
        obj.transform.localScale = Vector3.one;
        obj.transform.localPosition = Vector3.zero;
        obj.OnSet(index);

        _item_data.Add(obj);
        _sellect_item = obj;
    }

    public void OnSellectItem(ItemData sellect)
    {
        _sellect_item = sellect;

        for(var i = 0; i < _obj_item.Count; i++)
        {
            if(_obj_item[i]._item_info != _sellect_item) 
                _obj_item[i]._sellect.SetActive(false);
            else
            {
                _obj_item[i]._sellect.SetActive(true);

                _item_text[0].text = GameData.Instance._itemDataIndex[_sellect_item._item_index]._name;//이름                               
                _item_text[7].text = GameData.Instance._itemDataIndex[_sellect_item._item_index]._intro;//설명

                _item_text[1].text = string.Format("Lv.{0}", _sellect_item._lev);//레벨
                _item_text[5].text = GameData.Instance.OnType(_sellect_item._type);//속성 

                if(_sellect_item._class == 0)//무기
                {
                    _obj[0].SetActive(true);
                    _obj[1].SetActive(false);
                    _item_text[2].text = "무기";//종류
                    _item_text[3].text = _sellect_item._power.ToString();//공격력
                
                }
                else//방어구
                {
                    _obj[0].SetActive(false);
                    _obj[1].SetActive(true);
                    _item_text[2].text = "방어구";//종류
                    _item_text[4].text = _sellect_item._power.ToString();//방어력

                }


                if(sellect._equip_unit == null) 
                {
                    _item_text[6].text = "장착 안됨";//상태
                    _obj_btn[0].SetActive(true);
                    _obj_btn[1].SetActive(false);
                }
                else 
                {                    
                    _item_text[6].text = string.Format("<{0}> 장착", (GameData.Instance._unitDataIndex[sellect._equip_unit._unit_index]._name));//상태
                    _obj_btn[0].SetActive(false);
                    _obj_btn[1].SetActive(true);
                }
            }
        }
    }

    public void ClearItemObjectAll()
    {
        for(var i = 0; i < _obj_item.Count; i++) Destroy(_obj_item[i].gameObject);
        _obj_item.Clear();
    }

    public void CreateItemObject(ItemData data)
    {
        Item obj = null;
        obj = Instantiate<Item>(_obj_item_ori, _item_parent);
        obj.transform.localScale = Vector3.one;
        obj.transform.localPosition = Vector3.zero;
        obj.OnSet(data);

        _obj_item.Add(obj);
    }

    public void OnLoadItemUI()//대원 목록 UI 불러오기
    {
        if(_sellect_item == null)
        {
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
        }
        
        for(var i = 0; i < _item_data.Count; i++)
        {
            CreateItemObject(_item_data[i]);
        }
        
    }

    public void DelItem()//대원 해고
    {
        if(_sellect_item == null) return;

        if(_sellect_item._item_obj != null)//UI 삭제
        {
            _obj_item.Remove(_sellect_item._item_obj);
            Destroy(_sellect_item._item_obj.gameObject);
        }

        _sellect_item.OnDel();

        _item_data.Remove(_sellect_item);
        Destroy(_sellect_item.gameObject);

        for(var i = 0; i < _item_data.Count; i++)
        {
            if(_item_data[i]._lev <= 0) continue;
            _sellect_item = _item_data[i];
            break;
        }
        if( _obj_item.Count > 0) OnSellectItem(_sellect_item);
        else
        {
            _item_text[0].text = "";//이름
            _item_text[1].text = "";//레벨
            _item_text[2].text = "";//종류
            _item_text[3].text = "";//공격력
            _item_text[4].text = "";//방어력
            _item_text[5].text = "";//속성
            _item_text[6].text = "";//상태
            _item_text[7].text = "";//설명

            _obj[0].SetActive(true);
            _obj[1].SetActive(false);

            _obj_btn[0].SetActive(true);
            _obj_btn[1].SetActive(false);
        }            
    }

    public long _weapon_power; //원정대 무기 공격력
    public long _weapon_price; //원정대 무기 강화 가격

    public WeaponData _weapon_data_ori;
    public Dictionary<int, WeaponData> _weapon_data = new Dictionary<int, WeaponData>();

    public Transform _weapon_parent;
    public Weapon _weapon_ori;
    public Dictionary<int, Weapon> _weapon_list = new Dictionary<int, Weapon>();

    public void OnLoadWeaponData()
    {
        for(var i = 1; i < GameData.Instance._playerData._weapon.Count+1; i++)
        {
            CreateWeaponData(i);
        }
    }

    public void CreateWeaponData(int index)
    {
        WeaponData obj = null;
        obj = Instantiate<WeaponData>(_weapon_data_ori, transform);
        obj.transform.localScale = Vector3.one;
        obj.transform.localPosition = Vector3.zero;
        obj.OnSet(index);

        _weapon_data.Add(index, obj);
    }
    
    public void OpenWeapon()
    {
        if(!GameData.Instance._popMN.OnMainPop(2)) return;

        for(var i = 1; i < GameData.Instance._itemDataIndex.Count+1; i++)
        {
            if(GameData.Instance._playerData._weapon.ContainsKey(i)) CreateWeaponObject(i, _weapon_data[i]);
            else CreateWeaponObject(i, null);
        }

        _weapon_parent.localPosition = new Vector3(0, 145*(GameData.Instance._playerData._weapon.Count-1),0);
    }

    public void CreateWeaponObject(int index, WeaponData data)
    {
        Weapon obj = null;
        obj = Instantiate<Weapon>(_weapon_ori, _weapon_parent);
        obj.transform.localScale = Vector3.one;
        obj.transform.localPosition = Vector3.zero;
       
        if(data == null) obj.OnNullSet(index);
        else obj.OnSet(data);
        
        _weapon_list.Add(index, obj);
    }

    public void ClearWeapon()
    {
        for(var i = 1; i < _weapon_list.Count+1; i++)
        {
            if(_weapon_list.ContainsKey(i)) Destroy(_weapon_list[i].gameObject);
        }       

        _weapon_list.Clear();
    }

    public void OnAddWeapon(int index)
    {
        GameData.Instance._playerData._weapon.Add(index, 1);
        CreateWeaponData(index);

        if(_weapon_list.ContainsKey(index)) _weapon_list[index].OnSet(_weapon_data[index]);
    }
}
