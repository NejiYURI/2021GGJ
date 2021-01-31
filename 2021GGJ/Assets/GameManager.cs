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

    /// <summary>
    /// �C����������ܤ��e
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
    /// �T�����ͦ��I
    /// </summary>
    public List<Transform> BeaconPositionList;

    /// <summary>
    /// �W�����T���ͦ���m(���D������)
    /// </summary>
    public Transform LastPos;

    /// <summary>
    /// �T���I
    /// </summary>
    public BeaconScript beacon;

    /// <summary>
    /// �C���ɶ�(�˼�)
    /// </summary>
    public float TimeRemain;

    /// <summary>
    /// �C���`�ɶ�
    /// </summary>
    private float TotalTime;

    /// <summary>
    /// �C���ɶ���r
    /// </summary>
    public Text TimeRemainText;

    /// <summary>
    /// �C��������ܤ�r
    /// </summary>
    public Text GameOverText;

    /// <summary>
    /// �C���}�l�˼Ƥ�r
    /// </summary>
    public Text StartCountDown;

    /// <summary>
    /// �����Ϥ�(�C���˼ƥ�)
    /// </summary>
    public Image TimerImage;

    /// <summary>
    /// ���⦨�Z��List
    /// </summary>
    [SerializeField]
    private List<ScoreData> ScoreBoard;

    /// <summary>
    /// ���j���ϰ�
    /// </summary>
    public GameObject WindZone;

    /// <summary>
    /// ���O���ϰ�
    /// </summary>
    public GameObject BlackOutZone;

    /// <summary>
    /// �ƥ�D��C��
    /// </summary>
    public List<GameObject> TrapObject;

    public List<string> JoyConPlayerList;

    /// <summary>
    /// ���ı���
    /// </summary>
    public AudioSource SoundEffectControl;


    private void Awake()
    {
        //�ŧi(���n)
        gameManager = this;

        //�ˬd�O�_�L�s���n��(����P3�BP4)
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
        //�C���}�l�˼ƤT��
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

    /// <summary>
    /// ���a�������
    /// </summary>
    public event Action<string, Vector2> OnTriggerPlayerHit;
    /// <summary>
    /// �ƥ�Ĳ�o:���a�������
    /// </summary>
    /// <param name="PlayerTag">���a����</param>
    /// <param name="Dir">�����V</param>
    public void TriggerPlayerHit(string PlayerTag, Vector2 Dir)
    {
        if (OnTriggerPlayerHit != null)
        {
            OnTriggerPlayerHit(PlayerTag, Dir);
        }
    }

    /// <summary>
    /// �q�\�ƥ�:�T�����ܧ�
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
    /// �q�\�ƥ�:�C���ɶ������o���a����
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
    /// �q�\�ƥ�:�C���}�l
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
    /// �q�\�ƥ�:�T���O�_���Q���y(�h�H��T������)
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
    /// �T������m�ܧ�
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
    /// �}�l�S��ƥ�
    /// </summary>
    /// <param name="trap">�ƥ�W��</param>
    /// <param name="clip">Ĳ�o����</param>
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
    /// ���a�W�Ǥ��ƥ\��(�C���ɶ����������)
    /// </summary>
    /// <param name="PlayerTag">���a����</param>
    /// <param name="Score">����</param>
    public void UploadScore(string PlayerTag, float Score = 0)
    {
        ScoreBoard.Add(new ScoreData { PlayerName = PlayerTag, Score = Score });
    }

    /// <summary>
    /// ���a�ӧQ(�C������)
    /// </summary>
    /// <param name="PlayerName">���a����(�W��)</param>
    /// <param name="WinnerSprite">�S���Ψ쪺�Ϥ����</param>
    public void PlayerWin(string PlayerName, Sprite WinnerSprite)
    {
        Time.timeScale = 0;
        this.GameOverPanel.SetActive(true);
        this.GameOverText.text = PlayerName + "���!!";
    }

    /// <summary>
    /// �C���ɶ���A�����C���ƥ�
    /// </summary>
    public void TimeUp()
    {
        TriggerGetScore();
        Time.timeScale = 0;

        //�ƧǤ��ƦC��
        ScoreBoard = ScoreBoard.OrderByDescending(x => x.Score).ToList();
        int WinnerCnt = 0;
        int rowCnt = 0;
        float lastScore = 0;
        string ShowTxt = "";
        //�ˬd���ƬO�_�����ⱡ��
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

    /// <summary>
    /// ���s�}�l
    /// </summary>
    public void Restart_Game()
    {
        // Debug.Log("In");
        Time.timeScale = 1;
        SceneManager.LoadScene("PlayScene");

    }

    /// <summary>
    /// �^��D�e��
    /// </summary>
    public void Return_Game()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("TitleScene");

    }

    /// <summary>
    /// �˼ƥ\��
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
    /// �C���}�l�e�˼�
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
    /// �S��ƥ�ҰʻP����
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
    /// �ͦ��ƥ�D��
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
