using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class JumpArea : MonoBehaviour
{
    public Transform JumpA;
    public Transform JumpB;
    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Luna"))
        {
            LunaController lunaController= collision.transform.GetComponent<LunaController>();
            lunaController.Jump(true);
            //判断那个地点离角色更近，从近点跳到远点
            float distanceA=Vector3.Distance(lunaController.transform.position,JumpA.transform.position);
            float distanceB=Vector3.Distance(lunaController.transform.position,JumpB.transform.position);
            Transform target;
            
            if (distanceA>distanceB)
            {
                //从B跳跃到A
                target=JumpA;

            }
            else
            {
                //从A跳到B
                target=JumpB;
            }
            //这里有个注意点要注意的是，角色的跳跃动画和DoTween动画都是0.5秒两者是同步进行的
            lunaController.transform.DOMove(target.position,0.5f).OnComplete(()=>{EndJump(lunaController);});
            Transform LunaLocalTran=lunaController.transform.GetChild(0);
            Sequence sequence=DOTween.Sequence();
            sequence.Append(LunaLocalTran.transform.DOLocalMoveY(1.5f,0.25f).SetEase(Ease.InOutSine));
            sequence.Append(LunaLocalTran.transform.DOLocalMoveY(0.34f,0.25f).SetEase(Ease.InOutSine));
            sequence.Play();
        }
    }
    public void EndJump(LunaController lunaController)
    {
        lunaController.Jump(false);
    }
}
