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
    /// 碰撞器
    /// </summary>
    private Rigidbody2D rb;

    private PlayerController playerController;

    public SpriteRenderer PlayerImage;

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
    }

    private void GetAttack(string PlayerTag, Vector2 PushDir)
    {
        Debug.Log(PlayerTag + " Get Attack");
        if (PlayerTag == this.playerController.tag)
        {
            this.playerController.SetDrifting();
            this.rb.velocity = (PushDir * 1.2f);
            if (this.playerAudio != null)
            {
                this.playerAudio.PlayDamageAudio();
            }
        }
    }


    void Update()
    {
        //移動參數比照使用者輸入的內容
        movement.x = Input.GetAxis(InputHorizontal);
        movement.y = Input.GetAxis(InputVertical);
        if (Input.GetAxisRaw(InputDash) != 0 && playerController.CheckCanDash())
        {
            Debug.Log("Dash");
            playerController.PlayerDash();
            this.rb.AddForce(this.movement * 300f);
            if (this.playerAudio != null)
            {
                this.playerAudio.PlayDashAudio();
            }
        }
    }

    public Vector2 GetVelocity()
    {
        return this.rb.velocity;
    }

    public void SetVelocity(Vector2 Dir)
    {
        this.rb.velocity = Dir;
    }

    private void FixedUpdate()
    {
        if (playerController.CheckState() != 0) return;

        //依照移動參數*速度決定移動
        this.rb.velocity = this.movement * MoveSpeed * Time.deltaTime;
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


        if (this.movement.x > 0)
        {
            this.PlayerImage.flipX = true;
        }
        else if (this.movement.x < 0)
        {
            this.PlayerImage.flipX = false;
        }
    }
}
