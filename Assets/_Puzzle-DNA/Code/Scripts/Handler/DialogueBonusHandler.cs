using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Utilities;
using TMPro;
using System;

[Serializable]
public class DialogueBonusUI
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
    [SerializeField] DialogueBonusUI playerDialogue;
    [SerializeField] DialogueBonusUI interlocutorDialogue;
    [SerializeField] Dialogue dialogues;
    bool dialogueIsOngoing;

    public void InitDialogue(Dialogue dialogue)
    {
        dialogues = dialogue;
        SetCharacterSprites(dialogues.playerIdleSprite, dialogues.interlocutorIdleSprite);
        playerDialogue.nameText.text = DataHandler.instance.GetUserDataValue().username;
        interlocutorDialogue.nameText.text = dialogue.interlocutorName;

        StartCoroutine(UIController.instance.IEOpenScreen(
            dialoguePanel.transform.parent.GetComponent<CanvasGroup>()
            ));
    }

    public void SetCharacterSprites(
        Sprite playerSprite, 
        Sprite interlocutorSprite
        )
    {
        interlocutorDialogue.charImage.sprite = interlocutorSprite;
        interlocutorDialogue.charImage.SetNativeSize();

        playerDialogue.charImage.sprite = playerSprite;
        playerDialogue.charImage.SetNativeSize();
    }

    public void StartDialogue(int score)
    {
        dialogues.dialogueBonus.ForEach(data =>
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
            SetCharacterSprites(datas[i].playerSprite, datas[i].interlocutorSprite);
            if (datas[i].playerIsTalking)
            {
                playerDialogue.dialogueParentPanel.transform.parent.SetAsLastSibling();
                interlocutorDialogue.dialogueParentPanel.transform.parent.SetAsFirstSibling();

                playerDialogue.dialoguePanel.SetActive(true);
                interlocutorDialogue.dialoguePanel.SetActive(false);

                if (DataHandler.instance.GetLanguage() == "id")
                    playerDialogue.dialogueText.text = datas[i].contentData.contentId;
                else
                    playerDialogue.dialogueText.text = datas[i].contentData.contentEn;
            }
            else
            {
                playerDialogue.dialogueParentPanel.transform.parent.SetAsFirstSibling();
                interlocutorDialogue.dialogueParentPanel.transform.parent.SetAsLastSibling();

                playerDialogue.dialoguePanel.SetActive(false);
                interlocutorDialogue.dialoguePanel.SetActive(true);

                if (DataHandler.instance.GetLanguage() == "id")
                    interlocutorDialogue.dialogueText.text = datas[i].contentData.contentId;
                else
                    interlocutorDialogue.dialogueText.text = datas[i].contentData.contentEn;
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
