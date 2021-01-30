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
    /// �I����
    /// </summary>
    private Rigidbody2D rb;

    private PlayerController playerController;

    private void Awake()
    {
        //���o�����󪺸I����
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
        //���ʰѼƤ�ӨϥΪ̿�J�����e
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

        //�̷Ӳ��ʰѼ�*�t�רM�w����
        this.rb.velocity = this.movement * MoveSpeed * Time.deltaTime;
    }
}
