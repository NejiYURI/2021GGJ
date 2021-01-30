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
    /// �]�w�����ܼ�(�Ω��L����ϥ�)
    /// </summary>
    public static GameManager gameManager;

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

    public List<Transform> BeaconPositionList;

    public Transform LastPos;

    public BeaconScript beacon;

    public float TimeRemain;

    public Text TimeRemainText;

    public Text GameOverText;

    [SerializeField]
    private List<ScoreData> ScoreBoard;


    private void Awake()
    {
        //�ŧi(���n)
        gameManager = this;
    }

    private void Start()
    {
        BeaconRePosition();
        StartCoroutine(CountDown());
        this.GameOverPanel.SetActive(false);
        this.ScoreBoard = new List<ScoreData>();
    }

  
    /// <summary>
    /// �q�\�ƥ�:���a�i�J�T���d��
    /// </summary>
    public event Action<Model_BeaconTrigger> OnTriggerBeaconIn;
    /// <summary>
    /// �q�\�ƥ�Ĳ�o:���a�i�J�T���d��
    /// </summary>
    /// <param name="PlayerTag">���a����</param>
    /// <param name="AddScore">���W�[����</param>
    /// <param name="DisPer">�T���P���a�Z��(�ʤ���)</param>
    public void TriggerBeaconIn(Model_BeaconTrigger InputModel)
    {
        if (OnTriggerBeaconIn != null)
        {
            OnTriggerBeaconIn(InputModel);
        }
    }

    /// <summary>
    /// �q�\�ƥ�:���a���}�T���d��
    /// </summary>
    public event Action<string> OnTriggerBeaconExit;
    /// <summary>
    /// �q�\�ƥ�Ĳ�o:���a���}�T���d��
    /// </summary>
    /// <param name="PlayerTag">���a����</param>
    public void TriggerBeaconExit(string PlayerTag)
    {
        if (OnTriggerBeaconExit != null)
        {
            OnTriggerBeaconExit(PlayerTag);
        }
    }

    public event Action<string, Vector2> OnTriggerPlayerHit;
    public void TriggerPlayerHit(string PlayerTag, Vector2 Dir)
    {
        if (OnTriggerPlayerHit != null)
        {
            OnTriggerPlayerHit(PlayerTag, Dir);
        }
    }

    public event Action OnTriggerBeaconReposition;
    public void TriggerBeaconReposition()
    {
        if (OnTriggerBeaconReposition != null)
        {
            OnTriggerBeaconReposition();
        }
        BeaconRePosition();
    }

    public event Action OnTriggerGetScore;
    public void TriggerGetScore()
    {
        if (OnTriggerGetScore != null)
        {
            OnTriggerGetScore();
        }
    }

    private void BeaconRePosition()
    {
        int Index = UnityEngine.Random.Range(0, this.BeaconPositionList.Count - ((this.LastPos != null) ? 1 : 0));
        this.LastPos = this.BeaconPositionList[Index];
        this.BeaconPositionList.RemoveAt(Index);
        this.BeaconPositionList.Add(LastPos);
        List<BeaconScore> scoreSet = new List<BeaconScore>();
        scoreSet.Add(new BeaconScore { Distance = 100, Score = 0 });
        scoreSet.Add(new BeaconScore { Distance = 75, Score = UnityEngine.Random.Range(0.1f, 0.3f) });
        scoreSet.Add(new BeaconScore { Distance = 50, Score = UnityEngine.Random.Range(0.3f, 0.5f) });
        scoreSet.Add(new BeaconScore { Distance = 25, Score = UnityEngine.Random.Range(0.3f, 0.8f) });
        this.beacon.SetBeacon(LastPos.position, scoreSet, UnityEngine.Random.Range(8, 15));
    }


    public void UploadScore(string PlayerTag,float Score=0)
    {
        ScoreBoard.Add(new ScoreData { PlayerName= PlayerTag, Score= Score });
    }
    public void PlayerWin(string PlayerName)
    {
        Time.timeScale = 0;
        this.GameOverPanel.SetActive(true);
        this.GameOverText.text = PlayerName + "���!!";
    }

    public void TimeUp()
    {
        TriggerGetScore();
        Time.timeScale = 0;
        
        ScoreBoard =ScoreBoard.OrderByDescending(x => x.Score).ToList();
        int WinnerCnt=0;
        int rowCnt = 0;
        float lastScore=0;
        string ShowTxt = "";
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
                ShowTxt += "�B" + item.PlayerName;
            }
            else
            {
                break;
            }
        }
        if (WinnerCnt > 1)
        {
            GameOverText.text = ShowTxt + "����!!";
        }
        else
        {
            GameOverText.text = ShowTxt + "���!!";
        }
        this.GameOverPanel.SetActive(true);
    }

    public void Restart_Game()
    {
       // Debug.Log("In");
        Time.timeScale = 1;
        SceneManager.LoadScene("PlayScene");
       
    }

    IEnumerator CountDown()
    {
        yield return new WaitForSeconds(0.1f);
        this.TimeRemain -= 0.1f;
        if (this.TimeRemainText != null)
            this.TimeRemainText.text = this.TimeRemain.ToString("f1");
        if (TimeRemain <= 0) { TimeUp(); } else { StartCoroutine(CountDown()); }
     
    }
}
