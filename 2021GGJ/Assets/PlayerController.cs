using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    /// <summary>
    /// ���a����
    /// </summary>
    private int Score;

    /// <summary>
    /// ���ƨC��(?)�W�[�Ѽ�
    /// </summary>
    private int ScoreAdd;

    /// <summary>
    /// ��ܤ�r(�i�����)
    /// </summary>
    public Text ScoreText;

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
    }

    private void Update()
    {
        


    }

    /// <summary>
    /// ���a�i�J�T���d��
    /// </summary>
    /// <param name="Tag"></param>
    /// <param name="Num"></param>
    private void BeaconIn(string Tag, int Num)
    {
        if (Tag.Equals(this.tag))
        {
           
            this.ScoreAdd = Num;
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
        }
    }

    /// <summary>
    /// ���ƭp�⾹�A�̳]�w�W�v�W�[�δ�֪��a������
    /// </summary>
    /// <returns></returns>
    IEnumerator ScoreAddIEnum()
    {
        yield return new WaitForSeconds(1);
        this.Score += this.ScoreAdd;
        if (this.Score <= 0)
        {
            this.Score = 0;
        }
        if (this.Score >= 100)
        {
            this.Score = 100;
        }
        this.ScoreText.text = this.Score.ToString();

        StartCoroutine(ScoreAddIEnum());
    }

}
