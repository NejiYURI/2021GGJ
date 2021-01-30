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
    private float ScoreAdd;

    /// <summary>
    /// 顯示文字(可能替換)
    /// </summary>
    public Text ScoreText;

    public enum PlayerState
    {
        Normal,
        Dashing,
        Drifting
    }

    [SerializeField]
    private PlayerState state;

    public SpriteRenderer SignalIcon;

    public List<Model_Signal> SignalSpriteList;

    private float DashCounter;

    public float DashCoolDown;

    public float DashAttackRange;




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
                if (targetController != null && targetController.CheckState() != 3)
                {
                    Debug.Log("Dash Hit " + item.gameObject.tag);
                    PlayerMovement m_Player = this.gameObject.GetComponent<PlayerMovement>();
                    GameManager.gameManager.TriggerPlayerHit(item.tag, m_Player.GetVelocity());
                }
            }

        }
    }

    private void Start()
    {
        //將功能加入訂閱(進入、離開訊號發送點)
        GameManager.gameManager.OnTriggerBeaconIn += BeaconIn;
        GameManager.gameManager.OnTriggerBeaconExit += BeaconExit;
        this.Score = 0;
        this.ScoreAdd = -1;
        //開始跑分數計算器
        StartCoroutine(ScoreAddIEnum());
        this.SignalIcon.enabled = false;
        this.state = PlayerState.Normal;
    }

    private void Update()
    {

        if (this.state == PlayerState.Dashing)
            DashAttackFunc();

    }

    public bool CheckCanDash()
    {
        //Debug.Log(DashCounter);
        if (DashCounter > 0 || state != PlayerState.Normal)
            return false;

        return true;
    }

    public int CheckState()
    {
        return (int)state;
    }

    public void PlayerDash()
    {
        this.state = PlayerState.Dashing;
        this.DashCounter = DashCoolDown;
        StartCoroutine(DashCooldownIEum());
        StartCoroutine(DashStateEndIEum());
    }

    public void SetDrifting()
    {
        this.state = PlayerState.Drifting;
        StartCoroutine(GetHitRecover(0));
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
    /// 玩家離開訊號範圍
    /// </summary>
    /// <param name="Tag"></param>
    private void BeaconExit(string Tag)
    {
        if (Tag.Equals(this.tag))
        {
            this.ScoreAdd = -0.1f;
            this.SignalIcon.enabled = false;
        }
    }

    /// <summary>
    /// 分數計算器，依設定頻率增加或減少玩家的分數
    /// </summary>
    /// <returns></returns>
    IEnumerator ScoreAddIEnum()
    {
        yield return new WaitForSeconds(0.1f);
        this.Score += this.ScoreAdd;
        if (this.Score <= 0)
        {
            this.Score = 0;
        }
        if (this.Score >= 100)
        {
            this.Score = 100;
        }
        if (this.ScoreText != null)
            this.ScoreText.text = this.Score.ToString("f1");

        StartCoroutine(ScoreAddIEnum());
    }

    /// <summary>
    /// 計算衝刺冷卻時間
    /// </summary>
    /// <returns></returns>
    IEnumerator DashCooldownIEum()
    {
        yield return new WaitForSeconds(0.1f);
        DashCounter -= 0.1f;

        if (DashCounter > 0)
        {
            StartCoroutine(DashCooldownIEum());
        }
    }

    IEnumerator DashStateEndIEum()
    {
        yield return new WaitForSeconds(0.5f);
        if (this.state == PlayerState.Dashing)
            this.state = PlayerState.Normal;
    }

    IEnumerator GetHitRecover(float Counter)
    {
        yield return new WaitForSeconds(0.1f);
        Counter += 0.1f;

        if (Counter < 2)
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
