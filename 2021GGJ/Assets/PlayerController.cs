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


    public SpriteRenderer SignalIcon;

    public List<Model_Signal> SignalSpriteList;

    private void OnTriggerStay2D(Collider2D collision)
    {

        if (collision.gameObject.tag == "Beacon")
        {

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
    }

    private void Update()
    {



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
            this.ScoreAdd = -1;
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

}
