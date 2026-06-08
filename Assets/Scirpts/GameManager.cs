using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public float lunaHP;//最大生命值
    public float lunaCurrentHP;//当前生命值
    public float lunaMP;
    public float lunaCurrentMP;
    public int monsterCurrentHP;
    public GameObject battleGo;//战斗场景
    public int dialogInfos;
    public bool CanControlLuna = true;
    public bool hasPetTheDog;
    public int candleNum;
    public int killNum;
    public GameObject monsterGo;
    public NPCDialog npc;
    public bool enterBattle;
    public GameObject battleMonsterGo;
    public AudioSource audioSource;
    public AudioClip normalClip;
    public AudioClip battleClip;
    public float battleEnterCooldown = 1.5f;
    private float nextCanEnterBattleTime = 0f;
    public GameObject nalaHighlight;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        lunaHP = 100;
        lunaMP = 100;
        lunaCurrentHP = lunaHP;
        lunaCurrentMP = lunaMP;
        monsterCurrentHP = 50;
    }
    private void Start()
    {
        UpdateTaskText();
    }
    private void Update()
    {
        if (!enterBattle)
        {
            if (lunaCurrentMP < 100)
            {
                AddOrDecreaseMP(Time.deltaTime);
            }
            if (lunaCurrentHP < 100)
            {
                AddOrDecreaseHP(Time.deltaTime);
            }
        }
    }

    public void EnterOrExitBattle(bool enter = true, int addKillNum = 0)
    {
        UIManager.Instance.ShowOrHideBattlePanle(enter);
        battleGo.SetActive(enter);
        if (!enter)
        {
            killNum += addKillNum;
            if (addKillNum > 0)
            {
                DestroyMonster();
                if (killNum >= 5 && dialogInfos == 6)
                {
                    CompleteKillTask();
                }
            }
            monsterCurrentHP = 60;
            //战斗结束播放普通音乐
            PlayMusic(normalClip);
            //Luna死亡
            if (lunaCurrentHP <= 0)
            {
                lunaCurrentHP = 100;
                lunaCurrentMP = 0;
                battleMonsterGo.transform.position += new Vector3(0, 2, 0);
            }
        }
        else
        {
            PlayMusic(battleClip);
        }
        enterBattle = enter;
        UpdateTaskText();
    }
    /// <summary>
    /// 改变当前露娜血量方法
    /// </summary>
    /// <param name="value"></param>
    public void AddOrDecreaseHP(float value)
    {
        lunaCurrentHP += value;
        if (lunaCurrentHP >= lunaHP)
        {
            lunaCurrentHP = lunaHP;
        }
        if (lunaCurrentHP < 0)
        {
            lunaCurrentHP = 0;
        }
        UIManager.Instance.SetHPValue((float)lunaCurrentHP / lunaHP);
    }
    public void AddOrDecreaseMP(float value)
    {
        lunaCurrentMP += value;
        if (lunaCurrentMP >= lunaMP)
        {
            lunaCurrentMP = lunaMP;
        }
        if (lunaCurrentMP < 0)
        {
            lunaCurrentMP = 0;
        }
        UIManager.Instance.SetMPValue((float)lunaCurrentMP / lunaMP);
    }
    /// <summary>
    /// 是否可以使用相关技能
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public bool HasEnoughMP(int value)
    {
        return lunaCurrentMP >= value;
    }
    public int MonsterHPDecrease(int value)
    {
        monsterCurrentHP += value;
        if (monsterCurrentHP <= 0)
        {
            monsterCurrentHP = 0;
        }
        return monsterCurrentHP;
    }
    /// <summary>
    /// 显示怪物
    /// </summary>
    public void ShowMonsters()
    {
        if (!monsterGo.activeSelf)
        {
            monsterGo.SetActive(true);
        }
    }
    /// <summary>
    /// 显示完成索引
    /// </summary>
    public void SetContentIndex()
    {
        npc.SetContentIndex();
    }
    public void DestroyMonster()
    {
        Destroy(battleMonsterGo);
    }
    public void SetMonster(GameObject go)
    {
        battleMonsterGo = go;
    }
    public void PlayMusic(AudioClip audioClip)
    {
        if (audioSource.clip != audioClip)
        {
            audioSource.clip = audioClip;
            audioSource.Play();
        }
    }
    public void PlaySound(AudioClip audioClip)
    {
        if (audioClip)
        {
            audioSource.PlayOneShot(audioClip);
        }
    }
    public void UpdateTaskText()
    {
        if (UIManager.Instance == null)
        {
            return;
        }

        string taskContent = "";

        if (dialogInfos <= 1)
        {
            taskContent = "当前任务：与 Nala 对话";
        }
        else if (dialogInfos == 2)
        {
            if (!hasPetTheDog)
            {
                taskContent = "当前任务：安抚小狗";
            }
            else
            {
                taskContent = "当前任务：回去找 Nala";
            }
        }
        else if (dialogInfos == 3)
        {
            taskContent = "当前任务：与 Nala 对话";
        }
        else if (dialogInfos == 4)
        {
            if (candleNum < 5)
            {
                taskContent = "当前任务：收集蜡烛 " + candleNum + "/5";
            }
            else
            {
                taskContent = "当前任务：回去找 Nala";
            }
        }
        else if (dialogInfos == 5)
        {
            taskContent = "当前任务：领取 Nala 的奖励";
        }
        else if (dialogInfos == 6)
        {
            if (killNum < 5)
            {
                taskContent = "当前任务：清理怪物 " + killNum + "/5";
            }
            else
            {
                taskContent = "当前任务：回去找 Nala";
            }
        }
        else if (dialogInfos == 7)
        {
            taskContent = "当前任务：向 Nala 汇报";
        }
        else
        {
            taskContent = "当前任务：全部完成";
        }

        UIManager.Instance.SetTaskText(taskContent);
        //新增高亮显示
        UpdateGuideHighlight();
    }
    //防止频繁发生战斗设置冷却时间
    public void StartBattleEnterCooldown()
    {
        nextCanEnterBattleTime = Time.time + battleEnterCooldown;
    }

    public bool CanEnterBattleNow()
    {
        return Time.time >= nextCanEnterBattleTime;
    }
    /// <summary>
    /// NPC的高亮显示
    /// </summary>
    public void UpdateGuideHighlight()
    {
        if (nalaHighlight == null)
        {
            return;
        }

        bool shouldShowNalaHighlight = false;

        // 游戏开始阶段，需要找 Nala
        if (dialogInfos <= 1)
        {
            shouldShowNalaHighlight = true;
        }
        // 小狗安抚完成后，需要回去找 Nala
        else if (dialogInfos == 2 && hasPetTheDog)
        {
            shouldShowNalaHighlight = true;
        }
        // 蜡烛任务完成后，需要回去找 Nala
        else if (dialogInfos == 4 && candleNum >= 5)
        {
            shouldShowNalaHighlight = true;
        }
        // 打怪任务完成后，需要回去找 Nala
        else if (dialogInfos == 6 && killNum >= 5)
        {
            shouldShowNalaHighlight = true;
        }
        // 某些剧情阶段本身就是找 Nala 继续对话
        else if (dialogInfos == 3 || dialogInfos == 5 || dialogInfos == 7)
        {
            shouldShowNalaHighlight = true;
        }

        nalaHighlight.SetActive(shouldShowNalaHighlight);
    }
    public void UpdateNalaHighlight()
    {
        if (nalaHighlight == null)
        {
            return;
        }

        bool show = false;

        // 开局阶段：需要找 Nala
        if (dialogInfos <= 1)
        {
            show = true;
        }
        // 小狗任务完成后：需要回去找 Nala
        else if (dialogInfos == 2 && hasPetTheDog)
        {
            show = true;
        }
        // 蜡烛任务开始前的对话阶段
        else if (dialogInfos == 3)
        {
            show = true;
        }
        // 蜡烛收集完成后：需要回去找 Nala
        else if (dialogInfos == 4 && candleNum >= 5)
        {
            show = true;
        }
        // 领取奖励阶段
        else if (dialogInfos == 5)
        {
            show = true;
        }
        // 怪物清理完成后：需要回去找 Nala
        else if (dialogInfos == 6 && killNum >= 5)
        {
            show = true;
        }
        // 最终汇报阶段
        else if (dialogInfos == 7)
        {
            show = true;
        }

        nalaHighlight.SetActive(show);
    }
    public void CompleteKillTask()
    {
        if (npc != null)
        {
            npc.CompleteKillTask();
        }
        UpdateTaskText();
    }
}

