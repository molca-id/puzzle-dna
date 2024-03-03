using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Utilities;
using TMPro;
using System;

[Serializable]
public class DialogueUI
{
    public Image charImage;
    public GameObject dialogueParentPanel;
    public GameObject dialoguePanel;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI dialogueText;
}

public class DialogueBonusHandler : SingletonMonoBehaviour<DialogueBonusHandler>
{
    [SerializeField] GameObject dialoguePanel;
    [SerializeField] DialogueUI playerDialogue;
    [SerializeField] DialogueUI interlocutorDialogue;
    [SerializeField] List<DialogueBonus> dialogueBonus;
    bool dialogueIsOngoing;

    public void InitDialogue(
        Sprite playerSprite, 
        Sprite interlocutorSprite,
        string interlocutorName,
        List<DialogueBonus> datas
        )
    {   
        playerDialogue.nameText.text = DataHandler.instance.GetUserDataValue().username;
        playerDialogue.charImage.sprite = playerSprite;
        playerDialogue.charImage.SetNativeSize();

        interlocutorDialogue.nameText.text = interlocutorName;
        interlocutorDialogue.charImage.sprite = interlocutorSprite;
        interlocutorDialogue.charImage.SetNativeSize();

        StartCoroutine(UIController.instance.IEOpenScreen(
            dialoguePanel.transform.parent.GetComponent<CanvasGroup>()
            ));

        dialogueBonus = datas;
    }

    public void StartDialogue(int score)
    {
        dialogueBonus.ForEach(data =>
        {
            if (!data.isDone && data.scoreTrigger <= score)
            {
                dialoguePanel.SetActive(true);
                StartCoroutine(SetupDialogue(data));
            }
        });
    }

    IEnumerator SetupDialogue(DialogueBonus dialogue)
    {
        yield return new WaitUntil(() => !dialogueIsOngoing);
        StartCoroutine(Dialogue(dialogue));
        dialogueIsOngoing = true;
    }

    IEnumerator Dialogue(DialogueBonus dialogue)
    {
        List<DialogueBonusData> datas = dialogue.dialogueBonusDatas;
        for (int i = 0; i < datas.Count; i++)
        {
            if (datas[i].playerIsTalking)
            {
                playerDialogue.dialogueParentPanel.transform.parent.SetAsLastSibling();
                interlocutorDialogue.dialogueParentPanel.transform.parent.SetAsFirstSibling();

                playerDialogue.dialoguePanel.SetActive(true);
                interlocutorDialogue.dialoguePanel.SetActive(false);
                playerDialogue.dialogueText.text = datas[i].dialogueContent;
            }
            else
            {
                playerDialogue.dialogueParentPanel.transform.parent.SetAsFirstSibling();
                interlocutorDialogue.dialogueParentPanel.transform.parent.SetAsLastSibling();

                playerDialogue.dialoguePanel.SetActive(false);
                interlocutorDialogue.dialoguePanel.SetActive(true);
                interlocutorDialogue.dialogueText.text = datas[i].dialogueContent;
            }

            yield return new WaitForSeconds(datas[i].dialogueDelay);
        }

        playerDialogue.dialoguePanel.SetActive(false);
        interlocutorDialogue.dialoguePanel.SetActive(false);
        dialoguePanel.SetActive(false);
        dialogueIsOngoing = false;
        dialogue.isDone = true;
    }
}
