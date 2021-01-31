using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 玩家控制
/// </summary>
public class PlayerMovement : MonoBehaviour
{
    /// <summary>
    /// 使用的Input名稱(水平方向)
    /// </summary>
    public string InputHorizontal;

    /// <summary>
    /// 使用的Input名稱(垂直方向)
    /// </summary>
    public string InputVertical;

    /// <summary>
    /// 使用的Input名稱(衝刺按鈕)
    /// </summary>
    public string InputDash;

    /// <summary>
    /// 移動參數
    /// </summary>
    private Vector2 movement;

    /// <summary>
    /// 移動速度
    /// </summary>
    public float MoveSpeed;

    /// <summary>
    /// 速度加成
    /// </summary>
    public float SpeedPara;

    /// <summary>
    /// 碰撞器
    /// </summary>
    private Rigidbody2D rb;

    /// <summary>
    /// 玩家功能script
    /// </summary>
    private PlayerController playerController;

    /// <summary>
    /// 玩家圖示
    /// </summary>
    public SpriteRenderer PlayerImage;

    /// <summary>
    /// 玩家聲音控制
    /// </summary>
    private PlayerAudioController playerAudio;

    private void Awake()
    {
        //取得此物件的碰撞器
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
    /// 玩家受到攻擊(訂閱事件觸發)
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
        //移動參數比照使用者輸入的內容
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
    /// 取地目前加速度(無用)
    /// </summary>
    /// <returns></returns>
    public Vector2 GetVelocity()
    {
        return this.rb.velocity;
    }

    /// <summary>
    /// 設定加速度
    /// </summary>
    /// <param name="Dir"></param>
    public void SetVelocity(Vector2 Dir)
    {
        this.rb.velocity = Dir;
    }

    /// <summary>
    /// 玩家緊急煞車(=w=)
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
        //如果玩家處於飄移、暫停、或是無輸入的話，就不執行移動功能
        if (playerController.CheckState() == 2 || playerController.CheckState() == 3 || this.movement==Vector2.zero) return;
        //依照移動參數*速度決定移動
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
    /// 衝刺功能(改變速度參數)
    /// </summary>
    /// <returns></returns>
    IEnumerator DashingIEum()
    {
        this.SpeedPara = 3f;
        yield return new WaitForSeconds(0.1f);
        this.SpeedPara = 1f;
    }
}
