using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class TextHandler : MonoBehaviour
{
    public TextMeshProUGUI textbox;

    // Start is called before the first frame update
    void Start()
    {
        textbox.text = "Press B to cause an explosion and make the Biters panic!";
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetText(bool textOption)
    {
        if (textOption)
        {
            textbox.text = "KABOOM! \nPress C when you want the Biters to calm down.";
        } else
        {
            textbox.text = "Press B to cause an explosion and make the Biters panic!";
        }
    }
}
