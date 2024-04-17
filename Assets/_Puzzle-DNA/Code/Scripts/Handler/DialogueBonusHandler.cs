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
        SetCharacterSprites(DataHandler.instance.GetPlayerSprite(ExpressionType.Netral), dialogues.interlocutorIdleSprite);
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

        if (playerSprite == null)
        {
            playerDialogue.charImage.gameObject.SetActive(false);
        }
        else
        {
            playerDialogue.charImage.gameObject.SetActive(true);
            playerDialogue.charImage.sprite = playerSprite;
        }
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
        dialogueIsOngoing = dialogue.isDone = true;
        yield return StartCoroutine(Dialogue(dialogue));
    }

    IEnumerator Dialogue(DialogueBonus dialogue)
    {
        // Pause the game
        Time.timeScale = 0;

        List<DialogueBonusData> datas = dialogue.dialogueBonusDatas;

        for (int i = 0; i < datas.Count; i++)
        {
            SetCharacterSprites(DataHandler.instance.GetPlayerSprite(datas[i].contentData.playerExpression), datas[i].contentData.interlocutorSprite);
            if (datas[i].playerIsTalking)
            {
                playerDialogue.dialoguePanel.SetActive(true);
                interlocutorDialogue.dialoguePanel.SetActive(false);

                if (DataHandler.instance.GetLanguage() == "id")
                    playerDialogue.dialogueText.text = datas[i].contentData.contentId;
                else if (DataHandler.instance.GetLanguage() == "en")
                    playerDialogue.dialogueText.text = datas[i].contentData.contentEn;
                else
                    playerDialogue.dialogueText.text = datas[i].contentData.contentMy;
            }
            else
            {
                playerDialogue.dialoguePanel.SetActive(false);
                interlocutorDialogue.dialoguePanel.SetActive(true);

                if (DataHandler.instance.GetLanguage() == "id")
                    interlocutorDialogue.dialogueText.text = datas[i].contentData.contentId;
                else if (DataHandler.instance.GetLanguage() == "en")
                    interlocutorDialogue.dialogueText.text = datas[i].contentData.contentEn;
                else
                    interlocutorDialogue.dialogueText.text = datas[i].contentData.contentMy;
            }

            // Instead of WaitForSeconds, manually handle the delay
            float timer = 0f;
            while (timer < datas[i].dialogueDelay)
            {
                timer += Time.unscaledDeltaTime; // Use unscaledDeltaTime to ensure accurate timing even when the game is paused
                yield return null;
            }
        }

        // Resume the game
        Time.timeScale = 1;

        playerDialogue.dialoguePanel.SetActive(false);
        interlocutorDialogue.dialoguePanel.SetActive(false);
        dialoguePanel.SetActive(false);
    }
}
