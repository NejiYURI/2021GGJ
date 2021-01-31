using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TrapItemScript : MonoBehaviour
{

    private void Start()
    {
        Destroy(this.gameObject,10);
    }

    /// <summary>
    /// 陷阱類型
    /// </summary>
    public string TrapType;

    /// <summary>
    /// 陷阱音效
    /// </summary>
    public AudioClip clip;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 6)
        {
            GameManager.gameManager.StartTrap(TrapType, clip);
            Destroy(this.gameObject);
        }
    }

    
}
