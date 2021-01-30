using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AddMessage : MonoBehaviour
{
    private List<TextWriterSingle> textWriteSingleList;
    private static AddMessage instance;

    private void Awake()
    {
        instance = this;
        textWriteSingleList = new List<TextWriterSingle>();
    }
    // Start is called before the first frame update
    public static void MessageWriter_Static(Text uitext, string textwrite, float timePerCharacter, bool invisableCharacter)
    {
       instance.MessageWriter(uitext, textwrite, timePerCharacter, invisableCharacter);
    }

    public void MessageWriter(Text uitext, string textwrite, float timePerCharacter, bool invisableCharacter)
    {
        textWriteSingleList.Add(new TextWriterSingle(uitext, textwrite, timePerCharacter, invisableCharacter));
    }
    private void Update()
    {
        for (int i = 0; i < textWriteSingleList.Count; i++)
        {
            bool destroyInstance = textWriteSingleList[i].Update();
            if (destroyInstance)
            {
                textWriteSingleList.RemoveAt(i);
                i--;
            }
        }
    }

    public class TextWriterSingle
    {
        private Text uitext;
        private string textwrite;
        private int character;
        private float timePerCharacter;
        private float timer;
        private bool invisableCharacter;

        public TextWriterSingle(Text uitext, string textwrite, float timePerCharacter, bool invisableCharacter)
        {
            this.uitext = uitext;
            this.textwrite = textwrite;
            this.timePerCharacter = timePerCharacter;
            this.invisableCharacter = invisableCharacter;
            character = 0;

        }
        public bool Update()
        {           
                timer -= Time.deltaTime;
                while (timer <= 0f)
                {
                    timer += timePerCharacter;
                    character++;
                    string text = textwrite.Substring(0, character);
                    if (invisableCharacter)
                    {
                        text += "<color=#00000000>" + textwrite.Substring(character) + "</color>";
                    }
                    uitext.text = text;
                    if (character >= textwrite.Length)
                    {
                        return true;
                    }
                }
                return false;
        }
    }
    
}
