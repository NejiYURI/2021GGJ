using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Messag : MonoBehaviour
{
    //[SerializeField] private AddMessage addmessage;
    private Text messagetext;
    private bool IsActive;
    private int LifeTimeCounter = 0;
    private int num;
    public List<string> messageArray;
    [SerializeField]
    public SpriteRenderer char1;
    [SerializeField]
    private SpriteRenderer char2;
    [SerializeField]
    private SpriteRenderer char3;
    [SerializeField]
    private SpriteRenderer char4;
    [SerializeField]
    private SpriteRenderer char5;
    [SerializeField]
    private SpriteRenderer char6;
    [SerializeField]
    private SpriteRenderer char71;
    [SerializeField]
    private SpriteRenderer char8;
    [SerializeField]
    private SpriteRenderer char72;
    [SerializeField]
    private SpriteRenderer char73;
    [SerializeField]
    private SpriteRenderer char74;
    [SerializeField]
    private SpriteRenderer char75;
    [SerializeField]
    private SpriteRenderer char9;

    IEnumerator ScoreAddIEnum()
    {
        /*AddMessage.MessageWriter_Static(messagetext, messageArray[0], .1f, true);
        yield return new WaitForSeconds(3f);
        AddMessage.MessageWriter_Static(messagetext, messageArray[1], .1f, true);*/

        for(int i = 0; i < messageArray.Count; i++)
        {
            if (i == 0)
            {
                char1.gameObject.SetActive(true);
                char2.gameObject.SetActive(true);
            }
            if (i == 1)
            {
                char8.gameObject.SetActive(true);
            }
            if (i == 2)
            {
                char71.gameObject.SetActive(true);
                char72.gameObject.SetActive(true);
                char73.gameObject.SetActive(true);
                char74.gameObject.SetActive(true);
                char75.gameObject.SetActive(true);
            }
            if (i == 3)
            {
                char3.gameObject.SetActive(true);
                char4.gameObject.SetActive(true);
                char5.gameObject.SetActive(true);
                char6.gameObject.SetActive(true);
            }
            if (i == 5)
            {
                char8.gameObject.SetActive(true);
            }
            if (i == 6)
            {
                char9.gameObject.SetActive(true);
            }
            AddMessage.MessageWriter_Static(messagetext, messageArray[i], .1f, true);
            yield return new WaitForSeconds(4f);
            char1.gameObject.SetActive(false);
            char2.gameObject.SetActive(false);
            char3.gameObject.SetActive(false);
            char4.gameObject.SetActive(false);
            char5.gameObject.SetActive(false);
            char6.gameObject.SetActive(false);
            char71.gameObject.SetActive(false);
            char8.gameObject.SetActive(false);
            char72.gameObject.SetActive(false);
            char73.gameObject.SetActive(false);
            char74.gameObject.SetActive(false);
            char75.gameObject.SetActive(false);
            char9.gameObject.SetActive(false);

        }

    }
    // Start is called before the first frame update
    void Awake()
    {
        messagetext = GetComponent<Text>();
        //string message = messageArray[Random.Range(0, messageArray.Length)];
        //AddMessage.MessageWriter_Static(messagetext, message, .1f, true);
        
        
    }

    void Start()
    {
        //addmessage.MessageWriter(messagetext, "早安你好", 1f);
        StartCoroutine(ScoreAddIEnum());
    }

    // Update is called once per frame
}
