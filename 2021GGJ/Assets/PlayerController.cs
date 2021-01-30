using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    /// <summary>
    /// ���a����
    /// </summary>
    private float Score;

    /// <summary>
    /// ���ƨC��(?)�W�[�Ѽ�
    /// </summary>
    private float ScoreAdd;

    /// <summary>
    /// ��ܤ�r(�i�����)
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
        //���o�����d�򤺥���h�֪���
        Collider2D[] TargetHit = Physics2D.OverlapCircleAll(this.transform.position, this.DashAttackRange);

        //�@�@�z�磌��
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
        //�N�\��[�J�q�\(�i�J�B���}�T���o�e�I)
        GameManager.gameManager.OnTriggerBeaconIn += BeaconIn;
        GameManager.gameManager.OnTriggerBeaconExit += BeaconExit;
        this.Score = 0;
        this.ScoreAdd = -1;
        //�}�l�]���ƭp�⾹
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
    /// ���a�i�J�T���d��
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
    /// ���a���}�T���d��
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
    /// ���ƭp�⾹�A�̳]�w�W�v�W�[�δ�֪��a������
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
    /// �p��Ĩ�N�o�ɶ�
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
    /// ���U��ܧ����d��ϥ�
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        if (this.transform != null)
        {
            Gizmos.DrawWireSphere(this.transform.position, this.DashAttackRange);
        }
    }

}
