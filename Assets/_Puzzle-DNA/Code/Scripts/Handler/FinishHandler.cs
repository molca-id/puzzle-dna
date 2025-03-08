using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class ResultValueData
{
    public int id_talent;
    public string nama_talent;
    public int score_talent;
    public int ranking_talent;
}

[Serializable]
public class ResultData
{
    public string survey_code;
    public List<ResultValueData> hasil_isi;
}

public class FinishHandler : MonoBehaviour
{
    public static FinishHandler instance;
    public GameObject parentPanel;
    public GameObject finalFadePanel;
    public GameObject finalResultPanel;
    public GameObject finalSubmitPanel;
    public GameObject finalResultParentPanel;
    public List<Sprite> perkIconsColorful;
    public List<Sprite> perkIconsWhite;

    [Header("Character UI")]
    public Image charReplaceSprite;
    public ExpressionType expressionType;

    [Header("Perks UI Parent Ranking")]
    public GameObject parent5;
    public GameObject parent10;
    public GameObject parent4565;

    [Header("Perks UI Ranking")]
    public List<GameObject> top5PerksObject;
    public List<GameObject> topJust10PerksObject;
    public List<GameObject> top10PerksObject;
    public List<GameObject> bottom5PerksObject;

    [Header("Perks Ranking")]
    public ResultData resultData;

    private void Awake()
    {
        instance = this;
    }

    public void InitFinalSubmitPanel()
    {
        parentPanel.SetActive(true);
        finalSubmitPanel.SetActive(true);
    }

    public void CalculateFinalResult()
    {
        StartCoroutine(IEOpenScreen(finalFadePanel.GetComponent<CanvasGroup>(), () => { }));
        StartCoroutine(IECloseScreen(finalResultPanel.GetComponent<CanvasGroup>(), () => { }));

        resultData.survey_code = DataHandler.instance.GetUserDataValue().game_url;
        //resultData.hasil_isi = datas.OrderBy(x => x.ranking_talent).ToList();

        string json = JsonUtility.ToJson(resultData);
        StartCoroutine(
                APIManager.instance.PostDataWithTokenCoroutine(
                    APIManager.instance.SetupSendResultUrl(), json,
                    res => 
                    {
#if UNITY_EDITOR
                        string path = Path.Combine(Application.persistentDataPath, "TalentRanking.json");
                        File.WriteAllText(path, json);
                        Debug.Log($"JSON saved to: {path}");
#endif

                        StartCoroutine(IEOpenScreen(finalResultPanel.GetComponent<CanvasGroup>(), () => { }));
                        StartCoroutine(IECloseScreen(finalFadePanel.GetComponent<CanvasGroup>(), () => { }));
                        Debug.Log(res);
                    }));
    }

    #region OpenClosePanel
    IEnumerator IEOpenScreen(CanvasGroup screen, Action executeAfter = null)
    {
        while (screen.alpha < 1)
        {
            screen.alpha += Time.deltaTime * 2;
        }

        yield return null;
        executeAfter?.Invoke();
    }

    IEnumerator IECloseScreen(CanvasGroup screen, Action executeAfter = null)
    {
        while (screen.alpha > 0)
        {
            screen.alpha -= Time.deltaTime * 2;
        }

        yield return null;
        executeAfter?.Invoke();
    }
    #endregion
}
