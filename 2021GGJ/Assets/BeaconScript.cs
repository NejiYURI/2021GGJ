using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeaconScript : MonoBehaviour
{

    /// <summary>
    /// �Z���P���Ƴ]�w
    /// </summary>
    [SerializeField]
    public List<BeaconScore> ScoreDisList;

    /// <summary>
    /// �I��collider
    /// </summary>
    private CircleCollider2D thiscol;

    private int LifeTime;

    private int LifeTimeCounter;

    [SerializeField]
    private bool IsActive;

    private void Start()
    {
        this.thiscol = this.gameObject.GetComponent<CircleCollider2D>();
        this.LifeTime = 0;
        this.IsActive = false;
    }
    /// <summary>
    /// ���T���W�[���ƶq
    /// </summary>
    [SerializeField]
    private int ScoreAdd;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (this.IsActive)
        {
            //�ˬd�I������h�ŬO�_���s��6(���aLayer)
            if (collision.gameObject.layer == 6)
            {

                float dis = Vector2.Distance(this.transform.position, collision.transform.position);
                Debug.Log(collision.tag + " In!! dis:" + dis);
                foreach (var item in this.ScoreDisList)
                {
                    if (((dis / this.thiscol.radius) * 100) <= item.Distance)
                    {
                        ScoreAdd = item.Score;
                    }
                    else
                    {
                        break;
                    }
                }
                //Ĳ�o�q�\�ƥ�
                GameManager.gameManager.TriggerBeaconIn(collision.tag, ScoreAdd);
            }
        }

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (this.IsActive)
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

    public void SetBeacon(Vector2 Pos, List<BeaconScore> ScoreSet, int LifeTime)
    {
        this.transform.position = Pos;
        this.ScoreDisList = ScoreSet;
        this.LifeTime = LifeTime;
        this.LifeTimeCounter = 0;
        this.IsActive = true;
        StartCoroutine(ScoreAddIEnum());
    }

    IEnumerator ScoreAddIEnum()
    {
        yield return new WaitForSeconds(1f);
        this.LifeTimeCounter++;
        if (this.LifeTimeCounter >= this.LifeTime)
        {
            this.IsActive = false;
        }
        else
        {
            StartCoroutine(ScoreAddIEnum());
        }

    }

}



