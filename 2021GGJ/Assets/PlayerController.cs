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
    [SerializeField]
    private float ScoreAdd;

    ///// <summary>
    ///// ��ܤ�r(�i�����)
    ///// </summary>
    //public Text ScoreText;

    /// <summary>
    /// ���ƶi�ױ�
    /// </summary>
    public Image ProgressBar;

    /// <summary>
    /// ���a���A�C��
    /// </summary>
    public enum PlayerState
    {
        Normal,//�@��(���ʵ�)
        Dashing,//�Ĩ�
        Drifting,//�Q�������Ʋ�
        Pause//�Ȱ�
    }

    /// <summary>
    /// ���a���A
    /// </summary>
    [SerializeField]
    private PlayerState state;

    /// <summary>
    /// �T���ϥ�
    /// </summary>
    public SpriteRenderer SignalIcon;

    /// <summary>
    /// ��ܥΪ��T���ϥܦC��
    /// </summary>
    public List<Model_Signal> SignalSpriteList;

    /// <summary>
    /// �Q�����ɭԪ��L�T���ϥ�
    /// </summary>
    public SpriteRenderer NoSignalIcon;

    /// <summary>
    /// �p��Ĩ�^�_�ɶ�
    /// </summary>
    private float DashCounter;

    /// <summary>
    /// �Ĩ�^�_�ɶ�
    /// </summary>
    public float DashCoolDown;

    /// <summary>
    /// �Ĩ�����d��
    /// </summary>
    public float DashAttackRange;

    /// <summary>
    /// �Ĩ��V
    /// </summary>
    private Vector2 DashDir;

    /// <summary>
    /// �Ĩ�^�_��ܱ�
    /// </summary>
    public Image DashCooldownProgressBar;

    /// <summary>
    /// �O�_���b�{�{
    /// </summary>
    private bool IsFlashing;

    

    private void Start()
    {
        //�N�\��[�J�q�\(�i�J�B���}�T���o�e�I)
        GameManager.gameManager.OnTriggerBeaconIn += BeaconIn;
        GameManager.gameManager.OnTriggerBeaconExit += BeaconExit;
        GameManager.gameManager.OnTriggerGetScore += SetScore;
        GameManager.gameManager.OnTriggerGameStart += StartGame;
        GameManager.gameManager.OnTriggerBeaconReposition += BeaconReset;
        GameManager.gameManager.OnTriggerSignalIsJam += SignalIsJam;
        this.Score = 0;
        this.ScoreAdd = -1;
        //�}�l�]���ƭp�⾹
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
    /// �ˬd�O�_�i�H�Ĩ�
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
    /// �ˬd�ثe���a���A
    /// </summary>
    /// <returns></returns>
    public int CheckState()
    {
        return (int)state;
    }

    /// <summary>
    /// �Ĩ�ƥ�
    /// </summary>
    /// <param name="dir">�Ĩ��V</param>
    public void PlayerDash(Vector2 dir)
    {
        this.state = PlayerState.Dashing;
        this.DashCounter = DashCoolDown;
        this.DashDir = dir;
        StartCoroutine(DashCooldownIEum());
        StartCoroutine(DashStateEndIEum());
    }

    /// <summary>
    /// ���a�Q���᪺�}��
    /// </summary>
    public void SetDrifting()
    {
        this.state = PlayerState.Drifting;
        StartCoroutine(GetHitRecover(0));
    }

    /// <summary>
    /// �Ĩ�������I���P�w
    /// </summary>
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
                if (targetController != null && targetController.CheckState() != 2)
                {

                    PlayerMovement m_Player = this.gameObject.GetComponent<PlayerMovement>();
                    Debug.Log("Dash Hit " + this.DashDir);
                    //Ĳ�o�I�����a���q�\�ƥ�
                    GameManager.gameManager.TriggerPlayerHit(item.tag, this.DashDir);
                    m_Player.SetVelocity(Vector2.zero);
                    m_Player.stopPos();
                }
            }
        }
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
    /// �վ�O�_�Q���y�q���
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
    /// ���a���}�T���d��
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
    /// �T�������ܦ�m�A���A���m
    /// </summary>
    private void BeaconReset()
    {
        this.ScoreAdd = -0.1f;
        this.SignalIcon.enabled = false;
    }

    /// <summary>
    /// �W�Ǥ���(�C���ɶ���)
    /// </summary>
    private void SetScore()
    {
        GameManager.gameManager.UploadScore(this.tag, this.Score);
    }

    /// <summary>
    /// ���ƭp�⾹�A�̳]�w�W�v�W�[�δ�֪��a������
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
    /// �p��Ĩ�N�o�ɶ�
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
    /// �Q�����᪺�^�_�ɶ�
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
