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

    private void Awake()
    {
        //取得此物件的碰撞器
        this.rb = this.gameObject.GetComponent<Rigidbody2D>();
        this.playerController = this.gameObject.GetComponent<PlayerController>();
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
            this.rb.velocity=(PushDir*1.2f);
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
            this.rb.AddForce(this.movement * MoveSpeed * 1.5f);
        }
    }

    public Vector2 GetVelocity()
    {
        return this.rb.velocity;
    }

    private void FixedUpdate()
    {
        if (playerController.CheckState() != 0) return;

        //依照移動參數*速度決定移動
        this.rb.velocity = this.movement * MoveSpeed * Time.deltaTime;
    }
}
