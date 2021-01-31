using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    /// <summary>
    /// 玩家分數
    /// </summary>
    private float Score;

    /// <summary>
    /// 分數每秒(?)增加參數
    /// </summary>
    [SerializeField]
    private float ScoreAdd;

    ///// <summary>
    ///// 顯示文字(可能替換)
    ///// </summary>
    //public Text ScoreText;

    /// <summary>
    /// 分數進度條
    /// </summary>
    public Image ProgressBar;

    /// <summary>
    /// 玩家狀態列表
    /// </summary>
    public enum PlayerState
    {
        Normal,//一般(移動等)
        Dashing,//衝刺
        Drifting,//被撞之後飄移
        Pause//暫停
    }

    /// <summary>
    /// 玩家狀態
    /// </summary>
    [SerializeField]
    private PlayerState state;

    /// <summary>
    /// 訊號圖示
    /// </summary>
    public SpriteRenderer SignalIcon;

    /// <summary>
    /// 顯示用的訊號圖示列表
    /// </summary>
    public List<Model_Signal> SignalSpriteList;

    /// <summary>
    /// 被撞的時候的無訊號圖示
    /// </summary>
    public SpriteRenderer NoSignalIcon;

    /// <summary>
    /// 計算衝刺回復時間
    /// </summary>
    private float DashCounter;

    /// <summary>
    /// 衝刺回復時間
    /// </summary>
    public float DashCoolDown;

    /// <summary>
    /// 衝刺攻擊範圍
    /// </summary>
    public float DashAttackRange;

    /// <summary>
    /// 衝刺方向
    /// </summary>
    private Vector2 DashDir;

    /// <summary>
    /// 衝刺回復顯示條
    /// </summary>
    public Image DashCooldownProgressBar;

    /// <summary>
    /// 是否正在閃爍
    /// </summary>
    private bool IsFlashing;

    

    private void Start()
    {
        //將功能加入訂閱(進入、離開訊號發送點)
        GameManager.gameManager.OnTriggerBeaconIn += BeaconIn;
        GameManager.gameManager.OnTriggerBeaconExit += BeaconExit;
        GameManager.gameManager.OnTriggerGetScore += SetScore;
        GameManager.gameManager.OnTriggerGameStart += StartGame;
        GameManager.gameManager.OnTriggerBeaconReposition += BeaconReset;
        GameManager.gameManager.OnTriggerSignalIsJam += SignalIsJam;
        this.Score = 0;
        this.ScoreAdd = -1;
        //開始跑分數計算器
        StartCoroutine(ScoreAddIEnum());
        this.SignalIcon.enabled = false;
        this.state = PlayerState.Pause;
        this.NoSignalIcon.enabled = false;
        this.DashCooldownProgressBar.fillAmount = 0;
        this.DashCooldownProgressBar.enabled = false;
        if (this.ProgressBar != null)
        {
            this.ProgressBar.fillAmount = 0;
            this.ProgressBar.color = new Color(1, 0, 0);
        }
        this.IsFlashing = false;

    }

    private void Update()
    {

        if (this.state == PlayerState.Dashing)
            DashAttackFunc();

        if (this.ScoreAdd <= 0 && !this.IsFlashing)
        {
            this.IsFlashing = true;
            StartCoroutine(ProgressBarFlash());
        }

    }

    private void StartGame()
    {
        this.state = PlayerState.Normal;
    }

    /// <summary>
    /// 檢查是否可以衝刺
    /// </summary>
    /// <returns></returns>
    public bool CheckCanDash()
    {
        //Debug.Log(DashCounter);
        if (DashCounter > 0 || state != PlayerState.Normal)
            return false;

        return true;
    }

    /// <summary>
    /// 檢查目前玩家狀態
    /// </summary>
    /// <returns></returns>
    public int CheckState()
    {
        return (int)state;
    }

    /// <summary>
    /// 衝刺事件
    /// </summary>
    /// <param name="dir">衝刺方向</param>
    public void PlayerDash(Vector2 dir)
    {
        this.state = PlayerState.Dashing;
        this.DashCounter = DashCoolDown;
        this.DashDir = dir;
        StartCoroutine(DashCooldownIEum());
        StartCoroutine(DashStateEndIEum());
    }

    /// <summary>
    /// 玩家被撞後的漂移
    /// </summary>
    public void SetDrifting()
    {
        this.state = PlayerState.Drifting;
        StartCoroutine(GetHitRecover(0));
    }

    /// <summary>
    /// 衝刺期間的碰撞判定
    /// </summary>
    void DashAttackFunc()
    {
        //取得攻擊範圍內打到多少物件
        Collider2D[] TargetHit = Physics2D.OverlapCircleAll(this.transform.position, this.DashAttackRange);
        //一一篩選物件
        foreach (Collider2D item in TargetHit)
        {
            if (item.gameObject.layer == 6 && item.gameObject.tag != this.tag)
            {

                PlayerController targetController = item.gameObject.GetComponent<PlayerController>();
                if (targetController != null && targetController.CheckState() != 2)
                {

                    PlayerMovement m_Player = this.gameObject.GetComponent<PlayerMovement>();
                    Debug.Log("Dash Hit " + this.DashDir);
                    //觸發碰撞玩家的訂閱事件
                    GameManager.gameManager.TriggerPlayerHit(item.tag, this.DashDir);
                    m_Player.SetVelocity(Vector2.zero);
                    m_Player.stopPos();
                }
            }
        }
    }
    /// <summary>
    /// 玩家進入訊號範圍
    /// </summary>
    /// <param name="Tag"></param>
    /// <param name="Num"></param>
    private void BeaconIn(Model_BeaconTrigger _BeaconTrigger)
    {
        if (_BeaconTrigger.PlayerTag.Equals(this.tag))
        {
            this.SignalIcon.enabled = true;
            foreach (var item in this.SignalSpriteList)
            {
                if (_BeaconTrigger.Distance_Percentage <= item.Distance)
                {
                    this.SignalIcon.sprite = item.SignalIcon;
                }
                else
                {
                    break;
                }
            }
            this.ScoreAdd = _BeaconTrigger.AddScore;
        }
    }

    /// <summary>
    /// 調整是否被分流量顯示
    /// </summary>
    /// <param name="IsJam"></param>
    private void SignalIsJam(bool IsJam)
    {
        if (IsJam)
        {

            this.SignalIcon.color = new Color(1, 0.5f, 0.5f);
        }
        else
        {
            this.SignalIcon.color = new Color(1, 1, 1);
        }
    }

    /// <summary>
    /// 玩家離開訊號範圍
    /// </summary>
    /// <param name="Tag"></param>
    private void BeaconExit(string Tag)
    {
        if (Tag.Equals(this.tag))
        {
            BeaconReset();
        }
    }

    /// <summary>
    /// 訊號源改變位置，狀態重置
    /// </summary>
    private void BeaconReset()
    {
        this.ScoreAdd = -0.1f;
        this.SignalIcon.enabled = false;
    }

    /// <summary>
    /// 上傳分數(遊戲時間到)
    /// </summary>
    private void SetScore()
    {
        GameManager.gameManager.UploadScore(this.tag, this.Score);
    }

    /// <summary>
    /// 分數計算器，依設定頻率增加或減少玩家的分數
    /// </summary>
    /// <returns></returns>
    IEnumerator ScoreAddIEnum()
    {
        yield return new WaitForSeconds(0.1f);
        if (this.state != PlayerState.Drifting)
        {
            this.Score += this.ScoreAdd;
            if (this.Score <= 0)
            {
                this.Score = 0;
            }
            if (this.Score >= 100)
            {
                this.Score = 100;
                GameManager.gameManager.PlayerWin(this.tag,this.gameObject.GetComponent<SpriteRenderer>().sprite);
            }

        }
        //if (this.ScoreText != null)
        //    this.ScoreText.text = this.Score.ToString("f1");
        if (this.ProgressBar != null)
        {
            float progres_Val = this.Score / 100f;
            this.ProgressBar.fillAmount = progres_Val;
            this.ProgressBar.color = new Color(1- progres_Val, progres_Val, 0);
        }

        StartCoroutine(ScoreAddIEnum());
    }

    IEnumerator ProgressBarFlash()
    {
        this.ProgressBar.color = new Color(this.ProgressBar.color.r, this.ProgressBar.color.g, this.ProgressBar.color.b, (this.ProgressBar.color.a > 0) ? 0 : 1);
        yield return new WaitForSeconds(0.05f);
        if (this.ScoreAdd <= 0)
        {
            StartCoroutine(ProgressBarFlash());
        }
        else
        {
            this.IsFlashing = false;
        }
      
    }

    /// <summary>
    /// 計算衝刺冷卻時間
    /// </summary>
    /// <returns></returns>
    IEnumerator DashCooldownIEum()
    {
        this.DashCooldownProgressBar.enabled = true;
        yield return new WaitForSeconds(0.1f);
        DashCounter -= 0.1f;
        this.DashCooldownProgressBar.fillAmount =  (DashCounter / this.DashCoolDown);

        if (DashCounter > 0)
        {
            StartCoroutine(DashCooldownIEum());
        }
        else
        {
            this.DashCooldownProgressBar.fillAmount = 0;
            this.DashCooldownProgressBar.enabled = false;
        }
    }

    IEnumerator DashStateEndIEum()
    {
        yield return new WaitForSeconds(0.2f);
        if (this.state == PlayerState.Dashing)
            this.state = PlayerState.Normal;
    }

    /// <summary>
    /// 被撞之後的回復時間
    /// </summary>
    /// <param name="Counter"></param>
    /// <returns></returns>
    IEnumerator GetHitRecover(float Counter)
    {
        this.NoSignalIcon.enabled = true;
        yield return new WaitForSeconds(0.1f);
        Counter += 0.1f;
        this.NoSignalIcon.enabled = false;
        if (Counter < 1f)
        {
            StartCoroutine(GetHitRecover(Counter));
        }
        else
        {
            this.state = PlayerState.Normal;
        }
    }

    /// <summary>
    /// 輔助顯示攻擊範圍使用
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        if (this.transform != null)
        {
            Gizmos.DrawWireSphere(this.transform.position, this.DashAttackRange);
        }
    }

}
