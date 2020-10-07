using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class GameData
{

    private static GameData instance = null;
    public static GameData Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new GameData();
            }
            return instance;
        }
    }

    private GameData()
    {

    }
    
    public int TimerChangeMinute(float timer)
    {
        int minute = 0;
        if(timer>60)
        {
            minute = (int)(timer/60);
        }
        return minute;
    }
    public int TimerChangeSecond(float timer)
    {
        int second = 0;
        second =(int)(timer%60);
        return second;
    }

    public GM _gm;
    public SoundManager _soundMN;
    public BattleMN _battleMN; 
    public ExpeditionMN _expeditionMN;           
    public CampMN _campMN;          
    public TraningMN _trainingMN;
    public ItemMN _itemMN;
    public CollectMN _collectMN;
    public UnitMN _unitMN;
    public MonsterMN _monsterMN;  
    public QuestMN _questMN;
    public StoreMN _storeMN;
    public PopupMN _popMN;

    [Serializable]
    public struct Unit
    {
        public int _index;
        public long _total;
        public long _hp;
        public long _atk;
        public float _cri;
        public long _def;
        public int _type;
        public string _grade;
        public string _name;
    }
    public Unit _setUnit;
    public Dictionary<int, Unit> _unitDataIndex = new Dictionary<int, Unit>();

    [Serializable]
    public struct MyUnit
    {
        public int _index;//대원 코드
        public int _lev;//레벨
        public int _weapon;//착용 무기
        public int _armor;//착용 갑옷
        public int _acc;//악세서리
        public int _buy_count; //구매된 순서
    }

    [Serializable]
    public struct Team
    {
        public int _lv;//원정대 레벨
        public int _map;//원정중인 맵
        public int _stage;//진행된 스테이지
        public int _round;//진행된 스테이지의 라운드        
        public int _status;//원정대 상태
        public List<int> _member;//원정대 멤버
        public int _weapon;//원정대 무기
        public int _armor;//원정대 갑옷
        public int _acc;//원정대 악세서리
        public string _time; //원정 출발 시간
    }

    [Serializable]
    public struct MonsterData
    {
        public int _index;
        public long _hp;
        public long _atk;
        public float _cri;
        public int _rate;
        public string _name;
    }
    public MonsterData _setMonster;
    public Dictionary<int, MonsterData> _monsterDataIndex = new Dictionary<int, MonsterData>();    

    [Serializable]
    public struct ItemData
    {
        public int _index;
        public int _class;
        public long _power;
        public int _type;
        public string _name;
        public string _intro;
        public long _price;
    }
    public ItemData _setItem;
    public Dictionary<int, ItemData> _itemDataIndex = new Dictionary<int, ItemData>();

    [Serializable]
    public struct MyItem
    {
        public int _index;//아이템 코드
        public int _lev;//레벨
        public int _type;//착용 무기
        public int _buy_count; //구매된 순서
    }

    [Serializable]
    public struct QuestData
    {
        public int _index;
        public string _name;
        public float _time;
        public long _reward;
        public long _upgrade;
        public long _plus;
    }
    public QuestData _setQuest;
    public Dictionary<int, QuestData> _questDataIndex = new Dictionary<int, QuestData>(); 
    
    [Serializable]
    public class PlayerData
    {
        public bool _save_data;
        public float _ver = 0;
        public Dictionary<int, MyUnit> _unit = new Dictionary<int, MyUnit>();
        public List<Team> _team = new List<Team>();
        public Dictionary<int, MyItem> _item = new Dictionary<int, MyItem>();
        public Dictionary<int, int> _quest = new Dictionary<int, int>();
        public Dictionary<int, int> _weapon = new Dictionary<int, int>();

        public long _gold;

        public List<int> _test_gold = new List<int>();


        public long _jewel;
//---------------------------------------------------
        // 게임 기본 정보에 대한 부분.

        public double _totalPlayTime = 0; // 게임 총 플레이시간.
        public long _dailyChkTime = 0;


        //게임마다 정의되어야 하는 변수들.
        public int _nowStage = 1; //게임 스테이지.
    }

    public PlayerData _playerData = new PlayerData(); // 게임에서 사용될 데이터.
    public DateTime _serverTime; 


    //오디오 믹서 정보
    public string _nowAudioMixer;

    

    //게임 저장 관련.
    public void SaveData()
    {
        // bool _saveAvailable = false;
        Debug.Log("일반 게임 데이터 저장..");

        try
        {
            // Debug.Log("플레이어 프리팹으로 저장을 시도합니다...");
            FileSaveToPrefab();
        }
        catch (System.Exception e)
        {
            // Debug.Log("플레이어 프리팹으로 저장을 시도합니다...");
            Debug.Log(e);
        }
        finally
        {
            Debug.Log("플레이어 프리팹으로 저장을 성공...");
        }
    }


    private void FileSaveToPrefab()
    {
        string _nowSaveFileName = string.Format("SaveData");

        string _nowSaveTitle = string.Format("스테이지 {0} : {1} 에 저장", GameData.instance._playerData._nowStage, System.DateTime.Now.ToString());

        var b = new BinaryFormatter();
        var m = new MemoryStream();
        b.Serialize(m , _playerData);

        PlayerPrefs.SetString(_nowSaveFileName,Convert.ToBase64String(m.GetBuffer())); 
    }


    public bool _loadChkInit = true;
    public bool _dataLoadChk = false;

    public bool _firstData = false;

    public IEnumerator LoadData()
    {
        yield return null;
        try
        {
            LoadProcess();
            _dataLoadChk = true;
        }
        catch( System.Exception e)
        {
            _dataLoadChk = false;
            Debug.Log(e.ToString());
            string _msg = string.Format("{0} : {1}", "3)Load from data", e);
        }

        yield return new WaitForSecondsRealtime(0.1f);
    }

    // 실제 데이터 로드 프로세스.
    public void LoadProcess()
    {
        Debug.Log("일반 게임 데이터 로드..");

        if(!PlayerPrefs.HasKey("SaveData"))
        {
            //아직 세이브 데이터를 만든적이 없음.
            Debug.Log("새로 시작하는 유저입니다.");
            _loadChkInit = true;
        }
        else
        {
            Debug.Log("플레이어 프리팹 세이브 데이터가 확인 됩니다.");
            string _str = PlayerPrefs.GetString("SaveData");
            Debug.Log("데이터가 유효한지 체크");

            if( _str.Length > 0)
            {
                Debug.Log( " 플레이어 데이터를 플레이어 프리팹에서 로드해줌.");

                string _tmpStr = PlayerPrefs.GetString("SaveData");
                if(!string.IsNullOrEmpty(_tmpStr)) 
                {
                    var b = new BinaryFormatter();
                    var m = new MemoryStream(Convert.FromBase64String(_tmpStr));
                    _playerData = (PlayerData) b.Deserialize(m);
                }
            }
            _loadChkInit = false;
        }
        

        if(_loadChkInit)
        {
            // 세이브 파일이 한번도 만들어진적이 없는 경우.
            _loadChkInit = true;
            Debug.Log("No data in this project, first game.");
            // GameData.instance.GameMN.SetDefaultData();//최초 사용자를 위한 데이터 셋.
        }
        else
        {
            Debug.Log("데이터 로드 성공.!!");
        }
    }


    public bool _loadSuccess = false;
    public int _errCase = 0;


    public void ConfirmCloudData( byte[] _savedat, bool fileSizeChk = false )
    {
        Debug.Log(_savedat.Length);
        if (_savedat.Length > 0)
        {
            Environment.SetEnvironmentVariable("MONO_REFLECTION_SERIALIZER", "yes");
            var memoryStream = new MemoryStream();
            BinaryFormatter bf = new BinaryFormatter();
            memoryStream.Write(_savedat, 0, _savedat.Length);
            memoryStream.Seek(0, SeekOrigin.Begin);
            object _myData = bf.Deserialize(memoryStream);
            Debug.Log("ConfirmCloudData1 " +_myData);
            PlayerData data = (PlayerData)_myData;
            Debug.Log("ConfirmCloudData2  " + data);
            
            bool process = true;
            int typeChk = 0;
            if(fileSizeChk)
            {
                if( _playerData._totalPlayTime > data._totalPlayTime )
                {
                    typeChk = 1;
                    process = false;
                }
            }

            if(process)
            {
                GameData.instance._playerData = data; // 데이터 연동.


                GameData.Instance.SaveData(); //데이터 연동.
                Debug.Log("데이터 로드가 완료되었습니다.");
            }
            else
            {
                switch(typeChk)
                {
                    case 0:
                    break;

                    case 1:
                    break;
                }
                
            }
        }
        else
        {
            _loadSuccess = false;
            _errCase = 3; // 데이터 길이가 짧다.
        }
        
        // UIMN.OpenLoading(false);
    }

    public DateTime DateTimeChange (double timestamp)
    {
        DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
        return origin.AddSeconds(timestamp);
    }

    // 일일 보상 관련.
   public bool ChkToday()
   {
    Debug.Log("일일 체크를 시작합니다.............");

    if (GameData.Instance._playerData._dailyChkTime > 0)
    {
        bool value = false;
        long _lastTickTime = GameData.Instance._playerData._dailyChkTime;
        TimeSpan spanLast = new TimeSpan( _lastTickTime );

        long _nowTickTime = GameData.instance._serverTime.Ticks;
        TimeSpan spanNow = new TimeSpan ( _nowTickTime);

        // _playerData._totalPlayDailyTime = spanNow.TotalSeconds;

        if ((int)spanLast.TotalDays != (int)spanNow.TotalDays )
        {
        // 새로운 날짜로 확인이 되면.
        GameData.Instance._playerData._dailyChkTime  = GameData.instance._serverTime.Ticks;
        Debug.Log("새로운 날이 확인 되었습니다!!일일 보상이 지급됩니다!!");
        return true; 
        }
        else
        {
            GameData.Instance._playerData._dailyChkTime  = GameData.instance._serverTime.Ticks;
        return false; 
        }

        value = ( (int)spanLast.TotalDays != (int)spanNow.TotalDays ) ? true : false;
        return value;
    }
    else 
    {
        Debug.Log("It's First Play");
        GameData.Instance._playerData._dailyChkTime  = GameData.instance._serverTime.Ticks;
        return true; 
    }
   }

    
   public int StringToInt(string st)
   {
      int v =-1;
      v = System.Convert.ToInt32(st.ToString());
      return v;
   }

   public float StringToFloat(string st)
   {
      float v = -1;
      v= float.Parse(st);
      return v;
   }

   public long StringToLong(string st)
   {
      long v =-1;
      v = System.Convert.ToInt64(st.ToString());
      return v;
   }

   public string OnType(int type)
    {
        switch(type)
        {
            case 1 : return "불";
            case 2 : return "물";
            case 3 : return "땅";
            case 4 : return "바람";
            case 5 : return "빛";
            case 6 : return "어둠";
        }

        return "속성 없음";
    }    
}