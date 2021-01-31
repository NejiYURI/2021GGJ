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
    /// ��������
    /// </summary>
    public string TrapType;

    /// <summary>
    /// ��������
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
