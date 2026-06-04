using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TaskGuideManager : MonoBehaviour
{
    //基础对象
    public Transform luna;
    public GameObject guideArrow;
    //任务目标
    public Transform dogTarget;
    public List<Transform> candleTargets = new List<Transform>();
    public List<Transform> monsterTargets = new List<Transform>();
    //指引参数
    public float showDelay = 8f;
    public float arrowAngleOffset = -90f;

    private float timer;
    private string lastTaskState = "";
    //箭头离luna的距离
    public float arrowDistance=0.7f;
    // 箭头圆心偏移，如果你想以 Luna 脚下为圆心，可以调 Y
    public Vector2 arrowCenterOffset = new Vector2(0, -0.2f);

    // Start is called before the first frame update
    void Start()
    {
        HideArrow();
        if (GameManager.Instance != null)
        {
            lastTaskState = GetTaskStateKey();
        }
        else
        {
            lastTaskState = "";
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance == null)
        {
            HideArrow();
            return;
        }
        if (luna == null||guideArrow==null)
        {
            return ;
        }
        // 战斗中、对话中、不能控制 Luna 时，不显示任务指引
        if (GameManager.Instance.enterBattle || !GameManager.Instance.CanControlLuna)
        {
            HideArrow();
            return ;
        }
        //如果任务发生变化重新计时
        string currentState=GetTaskStateKey();
        if (currentState != lastTaskState)
        {
            lastTaskState=currentState;
            timer=0f;
            HideArrow();
            return;
        }
        Transform target=GetCurrentNearestTarget();
        if (target == null)
        {
            HideArrow();
            timer=0f;
            return;
        }
        timer+=Time.deltaTime;
        if (timer < showDelay)
        {
            HideArrow();
            return ;
        }
        ShowArrowToTarget(target);
    }
    public string GetTaskStateKey()
    {
        if (GameManager.Instance == null)
        {
            return "";
        }

        return GameManager.Instance.dialogInfos + "_" +
               GameManager.Instance.hasPetTheDog + "_" +
               GameManager.Instance.candleNum + "_" +
               GameManager.Instance.killNum;
    }
    public Transform GetCurrentNearestTarget()
    {
         if (GameManager.Instance == null)
        {
            return null;
        }

        int diologIndex=GameManager.Instance.dialogInfos;
        //小狗任务指向小狗
        if (diologIndex == 2 && !GameManager.Instance.hasPetTheDog)
        {
            return dogTarget;
        }
        //蜡烛任务
        if (diologIndex == 4 && GameManager.Instance.candleNum < 5)
        {
            return GetNearestActiveTarget(candleTargets);
        }
        //打怪任务:指向最近的怪物
        if (diologIndex == 6 && GameManager.Instance.killNum < 5)
        {
            return GetNearestActiveTarget(monsterTargets);
        }
        return null;
    }
    public Transform GetNearestActiveTarget(List<Transform> targets)
    {
        Transform nearest=null;
        float nearestDistance=float.MaxValue;
        for(int i = 0; i < targets.Count; i++)
        {
            Transform target=targets[i];
            if (target == null)
            {
                continue;
            }
            if (!target.gameObject.activeInHierarchy)
            {
                continue;
            }
            float distance=Vector2.Distance(luna.position,target.position);
            if (distance < nearestDistance)
            {
                nearestDistance=distance;
                nearest=target;
            }
        }
        return nearest;
    }
    public void ShowArrowToTarget(Transform target)
    {
        guideArrow.SetActive(true);
        Vector2 centerPos=(Vector2)luna.position+arrowCenterOffset;
        Vector2 direction=(Vector2)target.position-centerPos;
        if (direction.sqrMagnitude <= 0.001f)
        {
            HideArrow();
            return;
        }
        direction.Normalize();
        Vector2 arrowPos=centerPos+direction*arrowDistance;
        guideArrow.transform.position=new Vector3(
            arrowPos.x,
            arrowPos.y,
            guideArrow.transform.position.z
        );
        //让箭头自身朝向目标
        float angle=Mathf.Atan2(direction.y,direction.x)*Mathf.Rad2Deg+arrowAngleOffset;
        guideArrow.transform.rotation=Quaternion.Euler(0,0,angle);
    }
    public void HideArrow()
    {
        if (guideArrow != null && guideArrow.activeSelf)
        {
            guideArrow.SetActive(false);
        }
    }
}
