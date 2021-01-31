using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    /// <summary>
    /// 設定全域變數(用於其他物件使用)
    /// </summary>
    public static GameManager gameManager;

    /// <summary>
    /// 遊戲結束的顯示內容
    /// </summary>
    public GameObject GameOverPanel;

    [System.Serializable]
    public class ScoreData
    {
        public ScoreData()
        {
            this.PlayerName = "";
            this.Score = 0;
        }

        [SerializeField]
        public string PlayerName;

        [SerializeField]
        public float Score;
    }

    /// <summary>
    /// 訊號的生成點
    /// </summary>
    public List<Transform> BeaconPositionList;

    /// <summary>
    /// 上次的訊號生成位置(為求不重複)
    /// </summary>
    public Transform LastPos;

    /// <summary>
    /// 訊號點
    /// </summary>
    public BeaconScript beacon;

    /// <summary>
    /// 遊戲時間(倒數)
    /// </summary>
    public float TimeRemain;

    /// <summary>
    /// 遊戲總時間
    /// </summary>
    private float TotalTime;

    /// <summary>
    /// 遊戲時間文字
    /// </summary>
    public Text TimeRemainText;

    /// <summary>
    /// 遊戲結束顯示文字
    /// </summary>
    public Text GameOverText;

    /// <summary>
    /// 遊戲開始倒數文字
    /// </summary>
    public Text StartCountDown;

    /// <summary>
    /// 時鐘圖片(遊戲倒數用)
    /// </summary>
    public Image TimerImage;

    /// <summary>
    /// 結算成績用List
    /// </summary>
    [SerializeField]
    private List<ScoreData> ScoreBoard;

    /// <summary>
    /// 風吹的區域
    /// </summary>
    public GameObject WindZone;

    /// <summary>
    /// 關燈的區域
    /// </summary>
    public GameObject BlackOutZone;

    /// <summary>
    /// 事件道具列表
    /// </summary>
    public List<GameObject> TrapObject;

    public List<string> JoyConPlayerList;

    /// <summary>
    /// 音效控制
    /// </summary>
    public AudioSource SoundEffectControl;


    private void Awake()
    {
        //宣告(重要)
        gameManager = this;

        //檢查是否無連接搖桿(消除P3、P4)
        string[] JoyconList = Input.GetJoystickNames();
        for (int Index = 0; Index < this.JoyConPlayerList.Count; Index++)
        {
            if (Index > JoyconList.Count()-1)
            {
                GameObject[] objList = GameObject.FindGameObjectsWithTag(JoyConPlayerList[Index]);
                for (int j = 0; j < objList.Count(); j++)
                {
                    objList[j].SetActive(false);
                }
            }
            else if (string.IsNullOrEmpty(JoyconList[Index]))
            {
                GameObject[] objList = GameObject.FindGameObjectsWithTag(JoyConPlayerList[Index]);
                for (int j = 0; j < objList.Count(); j++)
                {
                    objList[j].SetActive(false);
                }
            }
            
        }
    }

    private void Start()
    {
        //遊戲開始倒數三秒
        StartCoroutine(GameStartCountDown(3));
        this.GameOverPanel.SetActive(false);
        this.ScoreBoard = new List<ScoreData>();
        if (this.TimerImage != null)
        {
            this.TimerImage.fillAmount = 1;
        }

        this.TotalTime = this.TimeRemain;
        this.TimeRemainText.text = this.TimeRemain.ToString("f1") + "S";
        this.WindZone.SetActive(false);
        this.BlackOutZone.SetActive(false);
    }


    /// <summary>
    /// 訂閱事件:玩家進入訊號範圍
    /// </summary>
    public event Action<Model_BeaconTrigger> OnTriggerBeaconIn;
    /// <summary>
    /// 訂閱事件觸發:玩家進入訊號範圍
    /// </summary>
    /// <param name="PlayerTag">玩家標籤</param>
    /// <param name="AddScore">欲增加分數</param>
    /// <param name="DisPer">訊號與玩家距離(百分比)</param>
    public void TriggerBeaconIn(Model_BeaconTrigger InputModel)
    {
        if (OnTriggerBeaconIn != null)
        {
            OnTriggerBeaconIn(InputModel);
        }
    }

    /// <summary>
    /// 訂閱事件:玩家離開訊號範圍
    /// </summary>
    public event Action<string> OnTriggerBeaconExit;
    /// <summary>
    /// 訂閱事件觸發:玩家離開訊號範圍
    /// </summary>
    /// <param name="PlayerTag">玩家標籤</param>
    public void TriggerBeaconExit(string PlayerTag)
    {
        if (OnTriggerBeaconExit != null)
        {
            OnTriggerBeaconExit(PlayerTag);
        }
    }

    /// <summary>
    /// 玩家受到攻擊
    /// </summary>
    public event Action<string, Vector2> OnTriggerPlayerHit;
    /// <summary>
    /// 事件觸發:玩家受到攻擊
    /// </summary>
    /// <param name="PlayerTag">玩家標籤</param>
    /// <param name="Dir">推行方向</param>
    public void TriggerPlayerHit(string PlayerTag, Vector2 Dir)
    {
        if (OnTriggerPlayerHit != null)
        {
            OnTriggerPlayerHit(PlayerTag, Dir);
        }
    }

    /// <summary>
    /// 訂閱事件:訊號源變更
    /// </summary>
    public event Action OnTriggerBeaconReposition;
    public void TriggerBeaconReposition()
    {
        if (OnTriggerBeaconReposition != null)
        {
            OnTriggerBeaconReposition();
        }
        BeaconRePosition();
    }

    /// <summary>
    /// 訂閱事件:遊戲時間到後取得玩家分數
    /// </summary>
    public event Action OnTriggerGetScore;
    public void TriggerGetScore()
    {
        if (OnTriggerGetScore != null)
        {
            OnTriggerGetScore();
        }
    }

    /// <summary>
    /// 訂閱事件:遊戲開始
    /// </summary>
    public event Action OnTriggerGameStart;
    public void TriggerGameStart()
    {
        if (OnTriggerGameStart != null)
        {
            OnTriggerGameStart();
        }
    }

    /// <summary>
    /// 訂閱事件:訊號是否有被分流(多人於訊號源中)
    /// </summary>
    public event Action<bool> OnTriggerSignalIsJam;
    public void TriggerSignalIsJam(bool IsJam)
    {
        if (OnTriggerSignalIsJam != null)
        {
            OnTriggerSignalIsJam(IsJam);
        }
    }

    /// <summary>
    /// 訊號源位置變更
    /// </summary>
    private void BeaconRePosition()
    {
        int Index = UnityEngine.Random.Range(0, this.BeaconPositionList.Count - ((this.LastPos != null) ? 1 : 0));
        this.LastPos = this.BeaconPositionList[Index];
        this.BeaconPositionList.RemoveAt(Index);
        this.BeaconPositionList.Add(LastPos);
        List<BeaconScore> scoreSet = new List<BeaconScore>();
        scoreSet.Add(new BeaconScore { Distance = 100, Score = 0 });
        scoreSet.Add(new BeaconScore { Distance = 75, Score = UnityEngine.Random.Range(0.3f, 0.5f) });
        scoreSet.Add(new BeaconScore { Distance = 50, Score = UnityEngine.Random.Range(0.5f, 0.7f) });
        scoreSet.Add(new BeaconScore { Distance = 25, Score = UnityEngine.Random.Range(0.8f, 1f) });
        this.beacon.SetBeacon(LastPos.position, scoreSet, UnityEngine.Random.Range(8, 15));
    }

    /// <summary>
    /// 開始特殊事件
    /// </summary>
    /// <param name="trap">事件名稱</param>
    /// <param name="clip">觸發音效</param>
    public void StartTrap(string trap, AudioClip clip)
    {
        switch (trap)
        {
            case "Wind":
                this.SoundEffectControl.clip = clip;
                this.SoundEffectControl.Play();
                this.WindZone.transform.Rotate(0f, 0f, UnityEngine.Random.Range(0.0f, 360.0f));
                StartCoroutine(StartTrapCour(this.WindZone, UnityEngine.Random.Range(5, 8)));
                break;
            case "BlackOut":
                this.SoundEffectControl.clip = clip;
                this.SoundEffectControl.Play();
                StartCoroutine(StartTrapCour(this.BlackOutZone, UnityEngine.Random.Range(5, 10)));
                break;
        }
    }

    /// <summary>
    /// 玩家上傳分數功能(遊戲時間到後比較分數)
    /// </summary>
    /// <param name="PlayerTag">玩家標籤</param>
    /// <param name="Score">分數</param>
    public void UploadScore(string PlayerTag, float Score = 0)
    {
        ScoreBoard.Add(new ScoreData { PlayerName = PlayerTag, Score = Score });
    }

    /// <summary>
    /// 玩家勝利(遊戲結束)
    /// </summary>
    /// <param name="PlayerName">玩家標籤(名稱)</param>
    /// <param name="WinnerSprite">沒有用到的圖片顯示</param>
    public void PlayerWin(string PlayerName, Sprite WinnerSprite)
    {
        Time.timeScale = 0;
        this.GameOverPanel.SetActive(true);
        this.GameOverText.text = PlayerName + "獲勝!!";
    }

    /// <summary>
    /// 遊戲時間到，結束遊戲事件
    /// </summary>
    public void TimeUp()
    {
        TriggerGetScore();
        Time.timeScale = 0;

        //排序分數列表
        ScoreBoard = ScoreBoard.OrderByDescending(x => x.Score).ToList();
        int WinnerCnt = 0;
        int rowCnt = 0;
        float lastScore = 0;
        string ShowTxt = "";
        //檢查分數是否有平手情形
        foreach (var item in ScoreBoard)
        {
            if (rowCnt == 0)
            {
                rowCnt++;
                WinnerCnt++;
                lastScore = item.Score;
                ShowTxt += item.PlayerName;
                continue;
            }
            if (lastScore == item.Score)
            {
                WinnerCnt++;
                ShowTxt += "、" + item.PlayerName;
            }
            else
            {
                break;
            }
        }
        if (WinnerCnt > 1)
        {
            GameOverText.text = ShowTxt + "平手!!";
        }
        else
        {
            GameOverText.text = ShowTxt + "獲勝!!";
        }
        this.GameOverPanel.SetActive(true);
    }

    /// <summary>
    /// 重新開始
    /// </summary>
    public void Restart_Game()
    {
        // Debug.Log("In");
        Time.timeScale = 1;
        SceneManager.LoadScene("PlayScene");

    }

    /// <summary>
    /// 回到主畫面
    /// </summary>
    public void Return_Game()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("TitleScene");

    }

    /// <summary>
    /// 倒數功能
    /// </summary>
    /// <returns></returns>
    IEnumerator CountDown()
    {
        yield return new WaitForSeconds(0.1f);
        this.TimeRemain -= 0.1f;
        if (TimeRemain <= 0) this.TimeRemain = 0;
        if (this.TimeRemainText != null)
            this.TimeRemainText.text = this.TimeRemain.ToString("f1") + "S";
        if (this.TimerImage != null)
        {
            this.TimerImage.fillAmount = this.TimeRemain / this.TotalTime;
        }
        if (TimeRemain <= 0) { TimeUp(); } else { StartCoroutine(CountDown()); }

    }

    /// <summary>
    /// 遊戲開始前倒數
    /// </summary>
    /// <param name="Counter"></param>
    /// <returns></returns>
    IEnumerator GameStartCountDown(int Counter)
    {
        Debug.Log(Counter);
        this.StartCountDown.text = Counter.ToString();
        if (Counter <= 0)
        {
            BeaconRePosition();
            StartCoroutine(CountDown());
            this.StartCountDown.enabled = false;
            TriggerGameStart();
            StartCoroutine(TrapSpawnCour(UnityEngine.Random.Range(5, 8)));
            yield break;
        }
        yield return new WaitForSeconds(1f);
        StartCoroutine(GameStartCountDown(Counter - 1));

    }


    /// <summary>
    /// 特殊事件啟動與結束
    /// </summary>
    /// <param name="trap"></param>
    /// <param name="Time"></param>
    /// <returns></returns>
    IEnumerator StartTrapCour(GameObject trap, int Time)
    {
        trap.SetActive(true);
        yield return new WaitForSeconds(Time);
        trap.SetActive(false);
        this.SoundEffectControl.Stop();

    }

    /// <summary>
    /// 生成事件道具
    /// </summary>
    /// <param name="Time"></param>
    /// <returns></returns>
    IEnumerator TrapSpawnCour(int Time)
    {

        yield return new WaitForSeconds(Time);
        if (TrapObject.Count > 0)
            Instantiate(TrapObject[UnityEngine.Random.Range(0, TrapObject.Count)], new Vector2(UnityEngine.Random.Range(-5, 5), UnityEngine.Random.Range(-4, 3.6f)), Quaternion.identity);

        StartCoroutine(TrapSpawnCour(UnityEngine.Random.Range(12, 20)));

    }
}
