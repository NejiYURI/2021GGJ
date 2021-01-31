using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemReset : MonoBehaviour
{
    [SerializeField]
    private Vector2 Ori_Pos;

    [SerializeField]
    private bool WaitForReset;
    void Start()
    {
        this.Ori_Pos = this.transform.position;
        this.WaitForReset = false;
    }

    // Update is called once per frame
    void Update()
    {
        if ((Vector2)this.transform.position != this.Ori_Pos && !WaitForReset)
        {
            this.WaitForReset = true ;
            StartCoroutine(PosCheck(this.transform.position,0));
        }
    }
    IEnumerator PosCheck(Vector2 Last_Pos, int TimeCount)
    {
        yield return new WaitForSeconds(1f);
        TimeCount++;
        if ((Vector2)this.transform.position != Last_Pos)
        {
            StartCoroutine(PosCheck((Vector2)this.transform.position, 0));
        }
        else
        {
            if (TimeCount >= 3)
            {
                ResetPos();
            }
            else
            {
                StartCoroutine(PosCheck((Vector2)this.transform.position, TimeCount));
            }
        }

    }

    private void ResetPos()
    {
        this.transform.position = this.Ori_Pos;
        transform.rotation = Quaternion.identity;
        this.WaitForReset = false;
    }

}
