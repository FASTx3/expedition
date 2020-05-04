using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Assets.HeroEditor.Common.CharacterScripts;
public class Monster : MonoBehaviour
{
    public Slider _hp;
    public Transform _parent;
    public Character _monster;

    public void OnSet(int index)
    {
        if(_monster != null) Destroy(_monster.gameObject);
        Character _obj = Instantiate<Character>(GameData.Instance._monsterMN._monster_prafab[index-1], _parent);
        _obj.transform.localPosition = Vector3.zero;
        _obj.transform.localScale = new Vector3(-50, 50, 1);

        _monster = _obj;

        transform.localPosition = new Vector3(1000, 0, 0);                 
        gameObject.SetActive(true);  
    }

    void Update()
    {
        if(_monster != null)
        {
            _monster.Animator.SetBool("Run", true);
        }
    }
}
