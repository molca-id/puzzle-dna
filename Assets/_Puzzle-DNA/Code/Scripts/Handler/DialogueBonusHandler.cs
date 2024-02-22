using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Utilities;
using TMPro;

public class DialogueBonusHandler : SingletonMonoBehaviour<DialogueBonusHandler>
{
    [SerializeField] GameObject dialoguePanel;
    [SerializeField] Image characterDialogueA;
    [SerializeField] Image characterDialogueB;
    [SerializeField] TextMeshProUGUI dialogueBoxText;
    [SerializeField] List<DialogueBonus> dialogueBonus;
    bool dialogueIsOngoing;

    public void InitDialogue(
        Sprite charA, 
        Sprite charB,
        List<DialogueBonus> datas
        )
    {   
        characterDialogueA.sprite = charA;
        characterDialogueB.sprite = charB;
        characterDialogueA.SetNativeSize();
        characterDialogueB.SetNativeSize();

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
                characterDialogueA.transform.parent.SetAsLastSibling();
                characterDialogueB.transform.parent.SetAsFirstSibling();
            }
            else
            {
                characterDialogueA.transform.parent.SetAsFirstSibling();
                characterDialogueB.transform.parent.SetAsLastSibling();
            }

            dialogueBoxText.text = datas[i].dialogueContent;
            yield return new WaitForSeconds(datas[i].dialogueDelay);
        }

        dialoguePanel.SetActive(false);
        dialogueIsOngoing = false;
        dialogue.isDone = true;
    }
}
