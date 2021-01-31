using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���a����
/// </summary>
public class PlayerMovement : MonoBehaviour
{
    /// <summary>
    /// �ϥΪ�Input�W��(������V)
    /// </summary>
    public string InputHorizontal;

    /// <summary>
    /// �ϥΪ�Input�W��(������V)
    /// </summary>
    public string InputVertical;

    /// <summary>
    /// �ϥΪ�Input�W��(�Ĩ���s)
    /// </summary>
    public string InputDash;

    /// <summary>
    /// ���ʰѼ�
    /// </summary>
    private Vector2 movement;

    /// <summary>
    /// ���ʳt��
    /// </summary>
    public float MoveSpeed;

    /// <summary>
    /// �t�ץ[��
    /// </summary>
    public float SpeedPara;

    /// <summary>
    /// �I����
    /// </summary>
    private Rigidbody2D rb;

    /// <summary>
    /// ���a�\��script
    /// </summary>
    private PlayerController playerController;

    /// <summary>
    /// ���a�ϥ�
    /// </summary>
    public SpriteRenderer PlayerImage;

    /// <summary>
    /// ���a�n������
    /// </summary>
    private PlayerAudioController playerAudio;

    private void Awake()
    {
        //���o�����󪺸I����
        this.rb = this.gameObject.GetComponent<Rigidbody2D>();
        this.playerController = this.gameObject.GetComponent<PlayerController>();
        this.playerAudio = this.gameObject.GetComponent<PlayerAudioController>();
    }
    private void Start()
    {
        GameManager.gameManager.OnTriggerPlayerHit += GetAttack;
        this.SpeedPara = 1;
    }

    /// <summary>
    /// ���a�������(�q�\�ƥ�Ĳ�o)
    /// </summary>
    /// <param name="PlayerTag"></param>
    /// <param name="PushDir"></param>
    private void GetAttack(string PlayerTag, Vector2 PushDir)
    {
        Debug.Log(PlayerTag + " Get Attack");
        if (PlayerTag == this.playerController.tag)
        {
            this.playerController.SetDrifting();
            //this.rb.velocity = (PushDir * 1.2f);
            this.rb.AddForce(PushDir * 2f);
            if (this.playerAudio != null)
            {
                this.playerAudio.PlayDamageAudio();
            }
        }
    }


    void Update()
    {
        if (playerController.CheckState() == 2 || playerController.CheckState() == 3) return;
        //���ʰѼƤ�ӨϥΪ̿�J�����e
        movement.x = Input.GetAxis(InputHorizontal);
        movement.y = Input.GetAxis(InputVertical);
        if (Input.GetAxisRaw(InputDash) != 0 && playerController.CheckCanDash() && movement!=Vector2.zero)
        {
            Debug.Log("Dash");
            playerController.PlayerDash(this.rb.velocity);
            StartCoroutine(DashingIEum());
            //this.rb.AddForce(this.movement * 300f);
            if (this.playerAudio != null)
            {
                this.playerAudio.PlayDashAudio();
            }
        }
    }

    /// <summary>
    /// ���a�ثe�[�t��(�L��)
    /// </summary>
    /// <returns></returns>
    public Vector2 GetVelocity()
    {
        return this.rb.velocity;
    }

    /// <summary>
    /// �]�w�[�t��
    /// </summary>
    /// <param name="Dir"></param>
    public void SetVelocity(Vector2 Dir)
    {
        this.rb.velocity = Dir;
    }

    /// <summary>
    /// ���a���٨�(=w=)
    /// </summary>
    public void stopPos()
    {
        this.rb.position = this.rb.position;
    }

    private void FixedUpdate()
    {
        if (this.playerAudio != null)
        {
            if (this.movement != Vector2.zero)
            {
                playerAudio.PlayRunAudio();
            }
            else
            {
                playerAudio.StopRunAudio();
            }
        }
        //�p�G���a�B���Ʋ��B�Ȱ��B�άO�L��J���ܡA�N�����沾�ʥ\��
        if (playerController.CheckState() == 2 || playerController.CheckState() == 3 || this.movement==Vector2.zero) return;
        //�̷Ӳ��ʰѼ�*�t�רM�w����
        this.rb.MovePosition(this.rb.position + this.movement * MoveSpeed * SpeedPara * Time.deltaTime);
        


        if (this.movement.x > 0)
        {
            this.PlayerImage.flipX = true;
        }
        else if (this.movement.x < 0)
        {
            this.PlayerImage.flipX = false;
        }
    }

    /// <summary>
    /// �Ĩ�\��(���ܳt�װѼ�)
    /// </summary>
    /// <returns></returns>
    IEnumerator DashingIEum()
    {
        this.SpeedPara = 3f;
        yield return new WaitForSeconds(0.1f);
        this.SpeedPara = 1f;
    }
}
