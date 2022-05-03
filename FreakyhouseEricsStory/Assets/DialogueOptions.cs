using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueOptions : MonoBehaviour
{
    public GameObject template;
    public static GameObject BoxTemplate;
    public float optionSeparation = 10;
    public string[] dialogueChoices;

    // Start is called before the first frame update
    void Start()
    {
        if (BoxTemplate != null) Debug.LogError("MULTIPLE TEMPLATES IN PLAY!! BAD BAD BAD");

        BoxTemplate = template;


        for(int i = 0; i < dialogueChoices.Length; i++)
        {
            string c = dialogueChoices[i];
            new DialogueOption(c, true).GenerateBox(GetComponent<RectTransform>(), optionSeparation*i);

        }

        //this.gameObject.SetActive(false);

    }

    public static void PlayDialogueFor(int convoID)
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

public class DialogueOption
{
    string text = "UNASSIGNED";
    bool active = false;
    static int globalId = 0;
    public readonly int id;

    DialogueOption[] leadsTo;

    public DialogueOption(string text, bool startsActive) : this(text, startsActive, null)
    {}

    public DialogueOption(string text, bool startsActive, params DialogueOption [] leadsTo)
    {
        this.text = text;
        this.active = startsActive;
        this.leadsTo = leadsTo;
        this.id = globalId;
        globalId++;
    }


    public GameObject GenerateBox(RectTransform parent, float y_offset)
    {
        GameObject dialogueBox = GameObject.Instantiate(DialogueOptions.BoxTemplate, parent);
        dialogueBox.GetComponentInChildren<Text>().text = this.text;
        dialogueBox.name = "DB" + id;
        dialogueBox.GetComponent<RectTransform>().anchoredPosition += new Vector2(0, y_offset);
        return dialogueBox;
    }
}
