using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class NPCDialog : MonoBehaviour
{
    private List<DialogInfo[]> dialogInfos;
    public int contentIndex;
    public Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        dialogInfos=new List<DialogInfo[]>()
        {
            //0
            new DialogInfo[]
            {
                new DialogInfo(){name="Luna",content="Helloヾ(•ω•`)o，我是Luna,你可以通过W，A，S，D来控制我的移动按住shift可以进行加速,空格键来进行NPC对话,战斗环节只需要进行点击相应按钮就能做出相应指令。祝您玩的愉快。"},
            },
            //1
            new DialogInfo[]
            {
                new DialogInfo(){name="Nala",content="好久不见,小Luna~(｡･∀･)ﾉﾞ嗨"},
                new DialogInfo(){name="Luna",content="好久不见，Nala，你还是那么的有活力，哈哈ヾ(^▽^*)))"},
                new DialogInfo(){name="Nala",content="还好吧~"},
                new DialogInfo(){name="Nala",content="你现在有空吗？我的狗一直在叫，但是我现在忙不过来，你能帮我安抚一下它吗？"},
                new DialogInfo(){name="Luna",content="啊？w(ﾟДﾟ)w"},
                new DialogInfo(){name="Nala",content="摸摸它就行了"},
                new DialogInfo(){name="Nala",content="你别看它一直在叫，其实它只是想吸引别人的注意"},
                new DialogInfo(){name="Luna",content="可是。。。"},
                new DialogInfo(){name="Luna",content="我是猫女郎啊(￣_,￣ )"},
                new DialogInfo(){name="Nala",content="放心好啦，我的狗很乖的不会咬人的，去吧去吧"}
            },
            //2
            new DialogInfo[]
            {
                new DialogInfo(){name="Nala",content="它还在叫呢"}

            },
            //3
            new DialogInfo[]
            {
                new DialogInfo(){name="Nala",content="感谢你Lunaヾ(≧▽≦*)o，你还是那么的可靠"},
                new DialogInfo(){name="Nala",content="我想再请你帮个忙好吗？"},
                new DialogInfo(){name="Nala",content="说起来这个事还是怨我"},
                new DialogInfo(){name="Nala",content="我今天睡过头了，出门比较匆忙"},
                new DialogInfo(){name="Nala",content="然后装蜡烛的袋子没有封好！/(ㄒoㄒ)/~~"},
                new DialogInfo(){name="Nala",content="结果就是我的蜡烛基本全部丢了"},
                new DialogInfo(){name="Luna",content="Nala你还是老样子，哈哈o(￣▽￣)ｄ"},
                new DialogInfo(){name="Nala",content="所以Luna请你帮帮忙，帮我把蜡烛找回来"},
                new DialogInfo(){name="Nala",content="如果你能帮我找回来全部的5根蜡烛，我就送你一个礼物"},
                new DialogInfo(){name="Luna",content="礼物，Luna最喜欢礼物了o((>ω< ))o"},
                new DialogInfo(){name="Nala",content="是的，我感觉你一定会喜欢这个礼物，加油哦"}
            },
            //4
            new DialogInfo[]
            {
                new DialogInfo(){name="Nala",content="你还没有收集完所有的蜡烛呢，小猫娘"}
            },
            //5
            new DialogInfo[]
            {
                new DialogInfo(){name="Nala",content="太帮了，你成功一个不落的全部收集回来了太感谢了ㄟ(≧◇≦)ㄏ"},
                new DialogInfo(){name="Luna",content="Nala你还是老样子喜欢到处逛，这些蜡烛散落到了各处真是不好找,好累啊！(╬▔皿▔)╯"},
                new DialogInfo(){name="Nala",content=",,ԾㅂԾ,,辛苦了辛苦了"},
                new DialogInfo(){name="Nala",content="这是给你的奖励"},
                new DialogInfo(){name="Nala",content="蓝纹火焰锤,传说中的神器"},
                new DialogInfo(){name="Nala",content="我相信你一定会喜欢的"},
                new DialogInfo(){name="Luna",content="获得蓝纹火焰锤~~（遇到怪物即可出发战斗）"},
                new DialogInfo(){name="Luna",content="哇，谢谢你Nala(≧∀≦)ゞ"},
                new DialogInfo(){name="Nala",content="正好，最近山里面出现了一些怪物，你可以清理他们为民除害"},
                new DialogInfo(){name="Luna",content="啊？(＃°Д°)，这才是你的真实目的吧"},
                new DialogInfo(){name="Nala",content="拜托啦拜托啦，你知道的我一路上过来真的很不方便的,,ԾㅂԾ,,"},
                new DialogInfo(){name="Luna",content="(lll￢ω￢)"},
                new DialogInfo(){name="Nala",content="求求你了"},
                new DialogInfo(){name="Luna",content="哎，行吧。谁让你是我的好朋友呢"},
                new DialogInfo(){name="Nala",content="谢谢你Luna，祝你顺利"},
            },
            //6
            new DialogInfo[]
            {
                new DialogInfo(){name="Nala",content="我想如果你能清理干净的话，村民和我一定会万分感激的（＾∀＾●）ﾉｼ"},
            },
            //7
            new DialogInfo[]
            {
                new DialogInfo(){name="Nala",content="Luna,你真是一个大英雄，周围的村名这下可以安心出行了"},
                new DialogInfo(){name="Luna",content="( •̀ ω •́ )y，Luna是大英雄"}
            },
            //8
            new DialogInfo[]
            {
                new DialogInfo(){name="Nala",content="改天见"},
                new DialogInfo(){name="Luna",content="改天见"},
            }

        };
        GameManager.Instance.dialogInfos=1;
        contentIndex=0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void DisplayDialog()
    {
        if (GameManager.Instance.dialogInfos >=9)
        {
            return;
        }
        if (contentIndex >= dialogInfos[GameManager.Instance.dialogInfos].Length)
        {
            if (GameManager.Instance.dialogInfos == 2 && !GameManager.Instance.hasPetTheDog)
            {
                
            }
            else if(GameManager.Instance.dialogInfos==4&&GameManager.Instance.candleNum<5)
            {
                
            }
            else if (GameManager.Instance.dialogInfos==6&&GameManager.Instance.killNum<5)
            {
                
            }
            else
            {
                GameManager.Instance.dialogInfos++;
            }
            if (GameManager.Instance.dialogInfos == 6)
            {
                GameManager.Instance.ShowMonsters();
            }
            //当前对话结束
            contentIndex=0;
            UIManager.Instance.ShowDialog();
            GameManager.Instance.CanControlLuna=true;
            GameManager.Instance.UpdateTaskText();
        }
        else
        {
            DialogInfo dialogInfo=dialogInfos[GameManager.Instance.dialogInfos][contentIndex];
            UIManager.Instance.ShowDialog(dialogInfo.content,dialogInfo.name);
            contentIndex++;
            animator.SetTrigger("Talk");
        }
    }
    public void SetContentIndex()
    {
        contentIndex=dialogInfos[GameManager.Instance.dialogInfos].Length;
    }
    public void CompleteKillTask()
    {
        // 6 表示“清理怪物中的等待对话”
        // 7 表示“清理怪物完成后的汇报对话”
        if (GameManager.Instance.dialogInfos == 6)
        {
            GameManager.Instance.dialogInfos = 7;
            contentIndex = 0;
        }
    }
}

/// <summary>
/// 类外结构体
/// </summary>
public struct DialogInfo
{
    public string name; // 说话人
    public string content;  // 对话内容
    // 你可以自己加头像、音效等字段
}
