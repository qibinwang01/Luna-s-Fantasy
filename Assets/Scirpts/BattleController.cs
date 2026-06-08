using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Video;

public class BattleController : MonoBehaviour
{
    public Animator LunaAnimator;
    public Transform LunaTran;
    public Transform MonsterTran;
    private Vector3 MonsterInitPos;
    private Vector3 LunaInitPos;
    public SpriteRenderer MonsterSR;
    public SpriteRenderer LunaSR;
    public GameObject skillEffectGo;
    public GameObject recoverHPEffectGo;
    public AudioClip attackSound;
    public AudioClip lunaAttackSound;
    public AudioClip monsterSound;
    public AudioClip skillSound;
    public AudioClip recoverHPSound;
    public AudioClip hitSound;
    public AudioClip deadSound;
    public AudioClip monsterDieSound;
    public bool isAction;

    private void Awake()
    {
        MonsterInitPos = MonsterTran.localPosition;
        LunaInitPos = LunaTran.localPosition;
    }
    private void OnEnable()
    {
        isAction = false;
        MonsterSR.DOFade(1, 0.1f);
        LunaSR.DOFade(1, 0.1f);
        LunaTran.localPosition = LunaInitPos;
        MonsterTran.localPosition = MonsterInitPos;
        //StartCoroutine(MonsterAttack());

    }

    // Update is called once per frame
    void Update()
    {

    }
    public void LunaAttack()
    {
        if (!TryStartAction())
        {
            return;
        }
        StartCoroutine(PerformAttackLogic());
    }
    IEnumerator PerformAttackLogic()
    {
        UIManager.Instance.ShowOrHideBattlePanle(false);
        LunaAnimator.SetBool("MoveState", true);
        LunaAnimator.SetFloat("MoveValue", -1);
        //将luna移动到怪兽前方，移动结束后执行lambda函数里面的逻辑
        LunaTran.DOLocalMove(MonsterInitPos + new Vector3(1, 0, 0), 0.5f).OnComplete(
            () =>
            {
                GameManager.Instance.PlaySound(attackSound);
                GameManager.Instance.PlaySound(lunaAttackSound);
                LunaAnimator.SetBool("MoveState", false);
                LunaAnimator.SetFloat("MoveValue", 0);
                LunaAnimator.CrossFade("Attack", 0);
                MonsterSR.DOFade(0.4f, 0.2f).OnComplete(() => { JudgeMonsterHP(-20); });
            }
        );
        yield return new WaitForSeconds(1.167f);
        LunaAnimator.SetBool("MoveState", true);
        LunaAnimator.SetFloat("MoveValue", 1);
        LunaTran.DOLocalMove(LunaInitPos, 0.5f).OnComplete(() => { LunaAnimator.SetBool("MoveState", false); });
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(MonsterAttack());
    }
    IEnumerator MonsterAttack()
    {
        //怪兽移动到Luna身前
        MonsterTran.DOLocalMove(LunaInitPos - new Vector3(1.5f, 0, 0), 0.5f);
        yield return new WaitForSeconds(0.5f);
        MonsterTran.DOLocalMove(LunaInitPos, 0.1f).OnComplete(
            () =>
        {
            GameManager.Instance.PlaySound(monsterSound);
            //怪兽短距离冲刺一下
            MonsterTran.DOLocalMove(LunaInitPos - new Vector3(1.5f, 0, 0), 0.1f);
            LunaAnimator.CrossFade("Hit", 0);
            GameManager.Instance.PlaySound(hitSound);
            LunaSR.DOFade(0.4f, 0.2f).OnComplete(() =>
            {
                LunaSR.DOFade(1, 0.2f);
            });
            JugeLunaHP(-20);
        });
        yield return new WaitForSeconds(0.2f);
        MonsterTran.DOLocalMove(MonsterInitPos, 0.5f).OnComplete(
            () =>
            {
                //UIManager.Instance.ShowOrHideBattlePanle(true);
                FinishAction();
            });
    }
    IEnumerator PerformDefendLogic()
    {
        UIManager.Instance.ShowOrHideBattlePanle(false);
        LunaAnimator.SetBool("Defend", true);
        MonsterTran.DOLocalMove(LunaInitPos - new Vector3(1.5f, 0, 0), 0.5f);
        yield return new WaitForSeconds(0.5f);
        MonsterTran.DOLocalMove(LunaInitPos, 0.1f).OnComplete(
            () =>
        {
            MonsterTran.DOLocalMove(LunaInitPos - new Vector3(1.5f, 0, 0), 0.1f);
            //防御时受击有一个后退效果
            LunaTran.DOLocalMove(LunaInitPos + new Vector3(0.5f, 0, 0), 0.2f).OnComplete(
                () =>
                {
                    LunaTran.DOLocalMove(LunaInitPos, 0.2f);
                    LunaAnimator.SetBool("Defend", false);
                });
        });
        yield return new WaitForSeconds(0.2f);
        MonsterTran.DOLocalMove(MonsterInitPos, 0.5f).OnComplete(
            () =>
            {
                //UIManager.Instance.ShowOrHideBattlePanle(true);
                FinishAction();
                GameManager.Instance.PlaySound(monsterSound);
            });
    }
    IEnumerator PerformSkillLogic()
    {
        UIManager.Instance.ShowOrHideBattlePanle(false);
        LunaAnimator.CrossFade("Skill", 0);
        GameManager.Instance.AddOrDecreaseMP(-30);
        yield return new WaitForSeconds(0.25f);
        GameObject go = Instantiate(skillEffectGo, MonsterTran);
        go.transform.localPosition = Vector3.zero;
        GameManager.Instance.PlaySound(lunaAttackSound);
        GameManager.Instance.PlaySound(skillSound);
        yield return new WaitForSeconds(0.4F);
        MonsterSR.DOFade(0.4f, 0.2f).OnComplete(
            () => { JudgeMonsterHP(-40); });
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(MonsterAttack());

    }
    IEnumerator PerformRecoverHPLogic()
    {
        UIManager.Instance.ShowOrHideBattlePanle(false);
        LunaAnimator.CrossFade("RecoverHP", 0);
        GameManager.Instance.AddOrDecreaseMP(-50);
        GameManager.Instance.PlaySound(lunaAttackSound);
        GameManager.Instance.PlaySound(recoverHPSound);
        yield return new WaitForSeconds(0.1f);
        GameObject go = Instantiate(recoverHPEffectGo, LunaTran);
        go.transform.localPosition = Vector3.zero;
        GameManager.Instance.AddOrDecreaseHP(40);
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(MonsterAttack());

    }
    public void lunaRecoverHP()
    {
        if (!GameManager.Instance.HasEnoughMP(50))
        {
            return;
        }
        if (!TryStartAction())
        {
            return;
        }
        StartCoroutine(PerformRecoverHPLogic());
    }
    /// <summary>
    /// 跟玩家血量相关
    /// </summary>
    /// <param name="value"></param>
    private void JugeLunaHP(int value)
    {
        GameManager.Instance.AddOrDecreaseHP(value);
        if (GameManager.Instance.lunaCurrentHP <= 0)
        {
            LunaAnimator.CrossFade("Dead", 0);
            GameManager.Instance.PlaySound(deadSound);
            LunaSR.DOFade(0, 0.75f).OnComplete(() =>
            {
                isAction = false;
                GameManager.Instance.EnterOrExitBattle(false);
            });
        }
    }
    /// <summary>
    /// 露娜使用技能
    /// </summary>
    public void LunaUseSkill()
    {

        if (!GameManager.Instance.HasEnoughMP(30))
        {
            return;
        }
        if (!TryStartAction())
        {
            return;
        }
        StartCoroutine(PerformSkillLogic());

    }
    /// <summary>
    /// 跟敌人血量相关
    /// </summary>
    /// <param name="value"></param>
    private void JudgeMonsterHP(int value)
    {
        if (GameManager.Instance.MonsterHPDecrease(value) == 0)
        {
            GameManager.Instance.PlaySound(monsterDieSound);
            MonsterSR.DOFade(0, 0.4f).OnComplete(
                () =>
                {
                    isAction = false;
                    GameManager.Instance.EnterOrExitBattle(false, 1);
                    StopCoroutine(MonsterAttack());
                });
        }
        else
        {
            MonsterSR.DOFade(1, 0.2f);
        }
    }

    public void LunaDefend()
    {
        if (!TryStartAction())
        {
            return;
        }
        StartCoroutine(PerformDefendLogic());
    }
    /// <summary>
    /// luna逃跑
    /// </summary>
    public void LunaEscape()
    {
        if (!TryStartAction())
        {
            return;
        }
        UIManager.Instance.ShowOrHideBattlePanle(false);
        LunaAnimator.SetBool("MoveState", true);
        LunaAnimator.SetFloat("MoveValue", 1);
        LunaTran.DOLocalMove(LunaInitPos + new Vector3(2.5f, 0, 0), 0.5f).OnComplete(() =>
        {
            LunaAnimator.SetBool("MoveState", false);
            LunaAnimator.SetFloat("MoveValue", 0);
            GameManager.Instance.StartBattleEnterCooldown();
            GameManager.Instance.EnterOrExitBattle(false);
            isAction = false;
        });


    }
    /// <summary>
    /// 用来判断当前能不能开始一个动作
    /// </summary>
    /// <returns></returns>
    public bool TryStartAction()
    {
        if (isAction)
        {
            return false;
        }
        isAction = true;
        return true;
    }
    /// <summary>
    /// 用来一轮动作结束后解除锁定,重新显示战斗按钮
    /// </summary>
    public void FinishAction()
    {
        isAction = false;
        if (GameManager.Instance.enterBattle)
        {
            UIManager.Instance.ShowOrHideBattlePanle(true);
        }
    }
}
