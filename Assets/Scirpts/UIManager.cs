using System.Collections;
using System.Collections.Generic;
//using Unity.PlasticSCM.Editor.WebApi;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    public Image imageMaskHP;
    public Image imageMaskMP;
    private float originalSize;
    public GameObject PanleBattleGO;
    public GameObject TalkPanelGO;
    public Image characterImage;
    public Sprite[] characterSprites;
    public Text contentText;
    public Text nameText;
    private Coroutine startTalkPanelCoroutine;
    private NPCDialog currentDialog;
    private Button talkPanelButton;
    //当前任务提示文本
    public Text taskText;
    private void Awake()
    {
        Instance = this;
        originalSize = imageMaskHP.rectTransform.rect.width;
        //SetHPValue(0.5f);
    }
    public void Start()
    {
        // 游戏一开始，如果 TalkPanelGO 是显示状态，则 3 秒后自动关闭
        // if (TalkPanelGO != null && TalkPanelGO.activeSelf)
        // {
        //     startTalkPanelCoroutine = StartCoroutine(HideStartTalkPanel());
        // }
         // 给对话面板上的 Button 添加点击事件
        if (TalkPanelGO != null)
        {
            talkPanelButton = TalkPanelGO.GetComponent<Button>();

            if (talkPanelButton != null)
            {
                talkPanelButton.onClick.AddListener(OnTalkPanelClick);
            }
        }

    }
    IEnumerator HideStartTalkPanel(float seconds)
    {
        yield return new WaitForSeconds(seconds);

        if (TalkPanelGO != null)
        {
            TalkPanelGO.SetActive(false);
        }

        startTalkPanelCoroutine = null;
    }
    /// <summary>
    /// 血条填充显示
    /// </summary>
    /// <param name="fillPercent">填充百分比</param>
    public void SetHPValue(float fillPercent)
    {
        imageMaskHP.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, fillPercent * originalSize);
    }
    /// <summary>
    /// 蓝条填充
    /// </summary>
    /// <param name="fillPercent">填充百分比</param>
    public void SetMPValue(float fillPercent)
    {
        imageMaskMP.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, fillPercent * originalSize);
    }
    public void ShowOrHideBattlePanle(bool show)
    {
        PanleBattleGO.SetActive(show);
    }
    /// <summary>
    /// 显示对话内容(包含对话角色的更换)
    /// </summary>
    /// <param name="content"></param>
    /// <param name="name"></param>
    public void ShowDialog(string content = null, string name = null)
    {
        // 如果玩家在开场 3 秒内触发了正式对话，就取消开场自动关闭逻辑
        if (startTalkPanelCoroutine != null)
        {
            StopCoroutine(startTalkPanelCoroutine);
            startTalkPanelCoroutine = null;
        }
        if (content == null)
        {
            TalkPanelGO.SetActive(false);
            currentDialog=null;
        }
        else
        {
            TalkPanelGO.SetActive(true);
            if (name != null)
            {
                if (name == "Luna")
                {
                    characterImage.sprite = characterSprites[0];
                }
                else
                {
                    characterImage.sprite = characterSprites[1];
                }
                characterImage.SetNativeSize();
            }
            contentText.text = content;
            nameText.text = name;
        }
    }
    public void SetCurrentDialog(NPCDialog dialog)
    {
        currentDialog=dialog;
    }
    private void OnTalkPanelClick()
    {
        // 如果当前没有正在对话的 NPC，就不处理
        // 比如游戏开场的提示面板，只显示 3 秒，不需要点击推进
        if (currentDialog == null)
        {
            return;
        }

        currentDialog.DisplayDialog();
    }
    public void SetTaskText(string content)
    {
        if (taskText != null)
        {
            taskText.text=content;
        }
    }
    public void ShowStartTipForSeconds(float seconds)
    {
        if (startTalkPanelCoroutine != null)
        {
            StopCoroutine(startTalkPanelCoroutine);
            startTalkPanelCoroutine = null;
        }

        if (TalkPanelGO != null)
        {
            TalkPanelGO.SetActive(true);
            startTalkPanelCoroutine = StartCoroutine(HideStartTalkPanel(seconds));
        }
    }
    public void QuitGame()
    {
        Application.Quit();
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}
