using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CanvasController : MonoBehaviour
{
    [SerializeField] private GameObject panelRoot;
    [SerializeField] private GameObject stageRoot;
    [SerializeField] private GameObject[] emptyStars;
    [SerializeField] private GameObject[] fullStars;
    [SerializeField] private GameObject[] levels;

    [SerializeField] private int[] levelStars;

    [SerializeField] private int levelsUnlocked;
    [SerializeField] private GameObject panelRootTitle;

    [SerializeField] private GameObject rays;
    private float RaysRotateSpeed = 10;

    [SerializeField] private GameObject prizeAmount;
    private int prizeAmountTotal = 100500;
    private int prizeAmountInt = 1;
    private TextMeshProUGUI prizeAmountTF;
    private GameObject prize;
    private TextMeshProUGUI panelRootTitleText;


    private int currLevelNum = 0;


    [SerializeField] private GameObject stagePageO;

    float lerp = 0f, duration = 10f;

    private void Start()
    {
        levelsUnlocked = 6;
        foreach (var fullStar in fullStars)
        {
            fullStar.SetActive(false);
        }
        for (int i = 0; i < levels.Length; i++)
        {
            var level = levels[i];
            level.GetComponentInChildren<TextMeshProUGUI>().text = "" + (i + 1);

            if (i < levelsUnlocked)
            {
                GameObject levelBG_dis = levels[i].transform.Find("Levebg_dis").gameObject;
                levelBG_dis.SetActive(false);
            }
        }


        prizeAmountTF = prizeAmount.GetComponent<TextMeshProUGUI>();
        prize = prizeAmountTF.transform.parent.gameObject;
        panelRootTitleText = panelRootTitle.GetComponent<TextMeshProUGUI>();
    }
    private void ActivateStars(int amount)
    {
        //levelStars[currLevelNum] = amount;
        Debug.Log( " currLevelNum = " + currLevelNum + " amount = " + amount);
        for (int i = 0; i < fullStars.Length; i++)
        {
            fullStars[i].SetActive(i <= amount);
        }


    }

    void UpdateStarsinStagePopup()
    {
        for (int i = 0; i < fullStars.Length; i++)
        {
            //TODO Stars
            //Debug.Log(levels[currLevelNum].name);
            //Debug.Log(levels[currLevelNum].transform.Find("star_full_" + i).gameObject.name);
            //if (levels[currLevelNum].transform.Find("star_full_" + i).gameObject != null)
            //{
            //   levels[currLevelNum].transform.Find("star_full_" + i).gameObject.SetActive(i <= levelStars[currLevelNum]);
            //}
        }
    }
    void Update()
    {

        //stars show
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ActivateStars(0);
            PopupShow(currLevelNum);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            ActivateStars(1);
            PopupShow(currLevelNum);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            ActivateStars(2);
            PopupShow(currLevelNum);
        }

        //popup show
        if (Input.GetKeyDown(KeyCode.Space))
        {
            PopupShow(currLevelNum);
        }
        rays.transform.Rotate(-Vector3.forward * Time.deltaTime * RaysRotateSpeed);


        //Amount Animate
        lerp += Time.deltaTime / duration;
        prizeAmountInt = (int)Mathf.Lerp(prizeAmountInt, prizeAmountTotal, lerp);
        prizeAmountTF.text = prizeAmountInt.ToString();

        //Amount pingpong
        prize.transform.localScale = Vector3.one * (1f + 0.1f * Mathf.Sin(Time.time * 4f));

    }
    public void PopupShow(int num)
    {
        currLevelNum = num;
        panelRootTitleText.text = "Level " + (currLevelNum + 1) + " clear!";
        Debug.Log("PopupShow " + num);
        panelRoot.SetActive(true);
        Animation anim = panelRoot.GetComponent<Animation>();
        anim.Play("Popup_Show");
    }
    public void PopupHide()
    {
        Animation anim = panelRoot.GetComponent<Animation>();
        anim.Play("Popup_Hide");
        StageShow();
    }
    public void StageHide()
    {
        Animation anim = stageRoot.GetComponent<Animation>();
        anim.Play("Popup_Hide");
    }
    public void StageShow()
    {
        stageRoot.SetActive(true);
        UpdateStarsinStagePopup();
        Animation anim = stageRoot.GetComponent<Animation>();
        anim.Play("Popup_Show");
    }
    public void PopupHideComplete()
    {
        Animation anim = stageRoot.GetComponent<Animation>();
        anim.Play("Popup_Show");
    }
    public void StageHideComplete()
    {
        Animation anim = panelRoot.GetComponent<Animation>();
        anim.Play("Popup_Show");
    }
    public void StagePageNum(int num)
    {
        int pageW = 780;
        var newPos = -Vector2.right * num * pageW;
        var lerp = Time.deltaTime / duration;
        Debug.Log("StagePageNum" + num + " Pos = " + newPos);
        RectTransform rt = stagePageO.GetComponent<RectTransform>();
        var newPos2 = new Vector2(newPos.x, 0f);
        //rt.anchoredPosition = newPos2;
        var oldPos = rt.anchoredPosition;
        rt.anchoredPosition = Vector2.Lerp(oldPos, newPos2, 20f);
    }
}
