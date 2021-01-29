using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeaconScript : MonoBehaviour
{
    /// <summary>
    /// ���T���W�[���ƶq
    /// </summary>
    public int ScoreAdd;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //�ˬd�I������h�ŬO�_���s��6(���aLayer)
        if (collision.gameObject.layer == 6)
        {
            Debug.Log(collision.tag+" In!!");
            //Ĳ�o�q�\�ƥ�
            GameManager.gameManager.TriggerBeaconIn(collision.tag, ScoreAdd);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        //�ˬd�I������h�ŬO�_���s��6(���aLayer)
        if (collision.gameObject.layer == 6)
        {
            Debug.Log(collision.tag + " Exit!!");
            //Ĳ�o�q�\�ƥ�
            GameManager.gameManager.TriggerBeaconExit(collision.tag);
        }
    }
}
