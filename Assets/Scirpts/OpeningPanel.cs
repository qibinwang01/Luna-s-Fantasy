using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OpeningPanel : MonoBehaviour
{
    public GameObject openingPanel;
    public float showTime=3f;
    public bool clickToSkip = true;
    private bool hasClosed;
    private bool oldAudioPauseState;
    // Start is called before the first frame update
    void Start()
    {
         if (openingPanel == null)
        {
            openingPanel = gameObject;
        }
        openingPanel.SetActive(true);
        // 记录原本的音频暂停状态
        oldAudioPauseState = AudioListener.pause;
        // 开场图显示期间，暂停所有游戏声音
        AudioListener.pause = true;
        // 开场图显示时，禁止 Luna 移动
        GameManager.Instance.CanControlLuna = false;
        // 避免原来的 talk_panel 在开场图背后提前显示和倒计时
        if (UIManager.Instance != null && UIManager.Instance.TalkPanelGO != null)
        {
            UIManager.Instance.TalkPanelGO.SetActive(false);
        }

        if (clickToSkip)
        {
            Button button = openingPanel.GetComponent<Button>();

            if (button != null)
            {
                button.onClick.AddListener(CloseOpeningPanel);
            }
        }

        StartCoroutine(AutoCloseOpeningPanel());
    }
    IEnumerator AutoCloseOpeningPanel()
    {
        yield return new WaitForSeconds(showTime);
        CloseOpeningPanel();
    }
    public void CloseOpeningPanel()
    {
        if (hasClosed)
        {
            return;
        }

        hasClosed = true;

        StopAllCoroutines();

        openingPanel.SetActive(false);
        // 开场图关闭后，恢复原本的音频状态
        AudioListener.pause = oldAudioPauseState;

        // 开场图关闭后，恢复 Luna 控制
        GameManager.Instance.CanControlLuna = true;

        // 开场图结束后，再显示原来的开场提示 3 秒
        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowStartTipForSeconds(3f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
