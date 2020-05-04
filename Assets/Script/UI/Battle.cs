using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Assets.HeroEditor.Common.CharacterScripts;

public class Battle : MonoBehaviour
{
    public Slider _team_hp_guage;
    public GameObject _obj_tent;
    public Transform _team;
    public List<Character> _obj_team_member = new List<Character>();
    public List<Transform> _team_pos = new List<Transform>();

    public Transform _parent_monster;
    public Transform _now_monster;
    public Transform _death_monster;
    public List<Monster> _monster = new List<Monster>();
    public long _monster_hp;
    public long _monster_hp_max;
    public float _monster_hp_percent;
    public long _monster_atk;
    public Slider _monster_hp_guage;

    public bool _rest = false;
    public bool _map_move = false;
    public string _tweenId;

    public Image _map;
    public Image _fade;
    private Material thisMaterial;
    Vector2 newOffset;
    
    void Start()
    {
        _map.material = Instantiate<Material>(GameData.Instance._battleMN._map_ori);
        thisMaterial = _map.material;
        newOffset = thisMaterial.mainTextureOffset;
    }
        
    public int _monster_obj_code = 0;

    public TeamData _team_data;

    public void OnTeamSetting(TeamData team)//파티 세팅
    {
        _team_data = team;
        OnTeamInfoUpdate();

        switch(_team_data._status) 
        {  
            case 0 :
                OnMaintenance();
            break;

            case 1 :
                OnBattleStart();
            break;

            case 2 :
                OnRest();
            break;
        }
    }

    public void OnTeamInfoUpdate()
    {
        for(var i = 0; i < _obj_team_member.Count; i++) Destroy(_obj_team_member[i].gameObject);//기존 팀원 오브젝트 삭제
        _obj_team_member.Clear();

        for(var i = 0; i < _team_data._member.Count; i++)
        {
            if(_team_data._member[i] == null) continue;
            Character _obj = Instantiate<Character>(GameData.Instance._unitMN._unit_prafab[_team_data._member[i]._unit_index-1], _team_pos[_obj_team_member.Count]);
            _obj.transform.localPosition = Vector3.zero;
            _obj.transform.localScale = new Vector3(50, 50, 1);
                      
            _obj_team_member.Add(_obj);
        }

        //hp바 위치 설정
        switch(_obj_team_member.Count) 
        {  
            case 0 :
                if(_team_data._status > 0) 
                {
                    OnBattleStop();
                    OnMaintenance();                    
                    _team_data.OnTeamStatus(0);
                }
            break;

            case 1 :                            
                if(_team_data._status == 2) break;
                if(_team_data._status == 0) OnBattleStart();    
                _team_hp_guage.transform.localPosition = new Vector3(0, -50, 0);
            break;

            case 2 :
                _obj_team_member[1].transform.SetParent(_team_pos[2]);
                _obj_team_member[1].transform.localPosition = Vector3.zero;

                if(_team_data._status == 2) break;
                _team_hp_guage.transform.localPosition = new Vector3(-25, -50, 0);
            break;

            case 3 :
                if(_team_data._status == 2) break;
                _team_hp_guage.transform.localPosition = new Vector3(-25, -50, 0);
            break;

            case 4 :
                if(_team_data._status == 2) break;
                _team_hp_guage.transform.localPosition = new Vector3(-45, -50, 0);
            break;

            case 5 :
                if(_team_data._status == 2) break;
                _team_hp_guage.transform.localPosition = new Vector3(-55, -50, 0);
            break;
        }
    }

    public void OnMonsterSetting()//몬스터 세팅 
    {
        if(!_map_move) _map_move = true;

        if(_monster_obj_code >= _monster.Count) _monster_obj_code = 0;
        
        //몬스터 수치 적용
        _monster_hp = 100;//GameData.Instance._monsterDataIndex[1]._hp;//몬스터 체력
        _monster_hp_max = 100;//GameData.Instance._monsterDataIndex[1]._hp;//몬스터 체력
        _monster_hp_percent = 1f/_monster_hp_max;
        _monster_atk = GameData.Instance._monsterDataIndex[1]._atk;//몬스터 공격력

        if(_monster.Count < 1)
        {
            for(var i = 0; i < _parent_monster.childCount; i++)
            {
                _monster.Add(_parent_monster.GetChild(i).GetComponent<Monster>());
            }
        }

        _monster_hp_guage = _monster[_monster_obj_code]._hp;
        _monster_hp_guage.value = 1;

        _now_monster = _monster[_monster_obj_code].transform;
        _monster[_monster_obj_code].OnSet(1);
        
        _tweenId = "0" + _team_data._data_code;                              
        _now_monster.DOLocalMove(Vector3.zero, 2f).SetEase(Ease.Linear).SetId(_tweenId).OnComplete(() =>
        {
            OnBattle();
        });

        _monster_obj_code++;
        /*
        for(var i = 0; i < _monster.Count; i++)
        {
            if(_monster[i].gameObject.activeSelf) continue;//오브젝트가 활성화 되어있으면 사용 다음 오브젝트로 넘긴다.
            else
            {
                //몬스터 수치 적용
                _monster_hp = GameData.Instance._monsterDataIndex[1]._hp;//몬스터 체력
                _monster_hp_max = GameData.Instance._monsterDataIndex[1]._hp;//몬스터 체력
                _monster_atk = GameData.Instance._monsterDataIndex[1]._atk;//몬스터 공격력

                _now_monster = _monster[i].transform;
                _now_monster.localPosition = new Vector3(1000, 0, 0);                 
                _now_monster.gameObject.SetActive(true);                                
                _now_monster.DOLocalMove(Vector3.zero, 2f).SetEase(Ease.Linear).OnComplete(() =>
                {
                    OnBattle();
                });
                break;
            }
        }
        */
    }

    public void OnBattleStart()
    {
        if(_obj_team_member.Count < 1) return;//배치된 멤버가 없을 시에는 원정 못함

        if(_team_data._status == 2) OnRecoveryComplete();//체력 회복 상태에서 되돌림

        if(!_team_hp_guage.gameObject.activeSelf) _team_hp_guage.gameObject.SetActive(true);
        _team_hp_guage.value = (_team_data._hp*_team_data._hp_percent);

        _team_data.OnTeamStatus(1);
        _team.gameObject.SetActive(true);
        _obj_tent.SetActive(false);

        OnMonsterSetting();        
    }

    public void OnBattleStop()
    {
        _map_move = false;

        if(_now_monster == null) return;

        DOTween.Kill(_tweenId);
        
        _now_monster.localPosition = new Vector3(1000, 0, 0);
        _now_monster.gameObject.SetActive(false);     
    }

    public void OnBattlePause()
    {
        _map_move = false;

        if(_now_monster == null) return;

        DOTween.Pause(_tweenId);     
    }

    public void OnBattleRestart()
    {
        if(_tweenId == "0" + _team_data._data_code) _map_move = true;
        DOTween.Play(_tweenId);
    }

    public void OnRest()
    {            
        _map_move = false;

        _team_data.OnTeamStatus(2);
        _rest = true;
        
        _team.gameObject.SetActive(false);
        _obj_tent.SetActive(true);

        _team_hp_guage.transform.localPosition = new Vector3(0, -80, 0);
    }

    public void OnMaintenance()//정비 세팅
    {
        OnBattleStop();
        
        if(_rest) _rest = false;

        DOTween.Kill("fade");
        _fade.DOFade(0, 0);

        _team.gameObject.SetActive(false);
        //_obj_tent.SetActive(true);
        _obj_tent.SetActive(false);

        _team_hp_guage.gameObject.SetActive(false); 
    }

    public void OnBattle()
    {      
        if(_now_monster == null) return;

        _team_data.OnDamage(_monster_atk);
        _monster_hp -= _team_data._atk;

        _team_hp_guage.value = (_team_data._hp*_team_data._hp_percent); 
        _monster_hp_guage.value = (_monster_hp*_monster_hp_percent);  

        if(_team_data._hp <= 0)
        {
            //원정대 패배            
            _team_data.OnStage(false);
            OnLose();
            
            Debug.Log("-----------------------------------------패배");
        }
        else
        {
            if(_monster_hp <= 0)
            {
                //몬스터 격파
                _team_data.OnStage(true);
                OnWin();
                
                Debug.Log("남은 체력-----------------------------------------" + _team_data._hp);
            }
            else
            {
                //누구도 죽지않는 상황                
                OnDraw();
            }
        }     
    }

    void OnWin()//전투 승리
    {
        if(_now_monster == null) return;

        _death_monster = _now_monster;
        _death_monster.DOLocalJump(new Vector3(1000, 0, 0), 350f, 1, 0.5f).SetEase(Ease.Linear).OnComplete(() =>
        {
            _death_monster.gameObject.SetActive(false);                      
        });
        OnMonsterSetting();  
    }

    void OnLose()//전투 패배
    {
        if(_now_monster == null) return;
        //몬스터 위치 초기화
        _now_monster.localPosition = new Vector3(1000, 0, 0);
        _now_monster.gameObject.SetActive(false);

        //OnRest();//회복
        _fade.DOFade(1, 0.25f).SetEase(Ease.Linear).SetId("fade").OnComplete(() =>
        {
            _fade.DOFade(0, 0.25f).SetEase(Ease.Linear).SetId("fade").OnComplete(() =>
            {
                _team_data._hp = _team_data._hp_max;
                _team_hp_guage.value = 1;
                OnBattleStart();
            });
        });
        
    }

    void OnDraw()//교착
    {
        _map_move = false;
        //파티 넉백
        _team.DOLocalJump(new Vector3(-100, 0, 0), 10f, 1, 0.1f).SetEase(Ease.Linear).OnComplete(() =>
        {
            _team.DOLocalMove(Vector3.zero, 0.1f).SetEase(Ease.Linear);
        });
        //몬스터 넉백
        if(_now_monster == null) return;

        _tweenId = "1" + _team_data._data_code;  
        _now_monster.DOLocalJump(new Vector3(100, 0, 0), 20f, 1, 0.1f).SetEase(Ease.Linear).SetId(_tweenId).OnComplete(() =>
        {
            _tweenId = "2" + _team_data._data_code;  
            _now_monster.DOLocalMove(Vector3.zero, 0.1f).SetEase(Ease.Linear).SetId(_tweenId).OnComplete(() =>
            {
                OnBattle();
            });
        });
    }

    public float _time = 0;
    void Update()
    {
        if(_rest)
        {
            _time += GameData.Instance._gm._timeRate;
            if(_time >= 1)
            {
                OnRecovery();
                _time = 0;
            }
        }

        if(_map_move)
        {
            OnMoveMap();
            OnUnitAnimation();
        }
    }

    public void OnRecovery()//원정대 체력 회복
    {
        if(_team_data == null) return;
        _team_data._hp += _team_data._rcv;
        if(_team_data._hp >= _team_data._hp_max) 
        {
            OnRecoveryComplete();// 체력 회복 완료  
            OnBattleStart();//원정 시작
        }            
        _team_hp_guage.value = (_team_data._hp*_team_data._hp_percent); 
    }

    public void OnRecoveryComplete()
    {
        _team_data._hp = _team_data._hp_max;
        _team_hp_guage.value = 1;
        _rest = false;

        switch(_obj_team_member.Count) 
        {  
            case 1 :
                _team_hp_guage.transform.localPosition = new Vector3(0, -50, 0);
            break;

            case 2 :
                _team_hp_guage.transform.localPosition = new Vector3(-25, -50, 0);
            break;

            case 3 :
                _team_hp_guage.transform.localPosition = new Vector3(-25, -50, 0);
            break;

            case 4 :
                _team_hp_guage.transform.localPosition = new Vector3(-45, -50, 0);
            break;

            case 5 :
                _team_hp_guage.transform.localPosition = new Vector3(-55, -50, 0);
            break;
        }
    }

    public float scrollSpeed = 0.8f;//맵이동 속도
    public void OnMoveMap()//맵 이동 연출
    {
        newOffset = thisMaterial.mainTextureOffset;
        newOffset.Set(newOffset.x + (scrollSpeed * GameData.Instance._gm._timeRate), 0);
        thisMaterial.mainTextureOffset = newOffset;
    }

    public void OnUnitAnimation()
    {
        for(var i = 0; i < _obj_team_member.Count; i++)
        {
            if(_obj_team_member != null) _obj_team_member[i].Animator.SetBool("Run", true);
        }
    }
}
