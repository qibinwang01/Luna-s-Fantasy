using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class EnemyController : MonoBehaviour
{
    //轴向控制
    public bool vertical;
    private float speed = 3;
    private Rigidbody2D rigidbody2d;
    //方向控制
    private int direction=1;
    //方向改变时间间隔
    private float changetime = 2;
    //计时器
    private float timer;
    //动画控制
    private Animator animator;
    private void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        timer = changetime;
        animator = GetComponent<Animator>();
    }
    private void FixedUpdate()
    {
        if (GameManager.Instance.enterBattle)
        {
            return ;
        }
        Vector3 pos = rigidbody2d.position;
        if (vertical)
        {
            //垂直移动
            animator.SetFloat("LookX",0);
            animator.SetFloat("LookY",direction);
            pos.y = pos.y + speed * direction * Time.fixedDeltaTime;
        }
        else
        {
            //水平移动
            animator.SetFloat("LookX",direction);
            animator.SetFloat("LookY",0);
            pos.x = pos.x + speed * direction * Time.fixedDeltaTime;
        }
        rigidbody2d.MovePosition(pos);
    }
    private void Update()
    {
        //实现怪物的来回移动
        if (GameManager.Instance.enterBattle)
        {
            return ;
        }
        timer -= Time.deltaTime;
        if (timer < 0)
        {
            direction = -direction;
            timer = changetime;
        }
    }
    private void OnCollisionEnter2D(Collision2D collision) {
        if (GameManager.Instance.enterBattle)
        {
            return;
        }
        if (!GameManager.Instance.CanEnterBattleNow())
        {
            return;
        }
        if (collision.transform.CompareTag("Luna"))
        {
            GameManager.Instance.SetMonster(gameObject);
            GameManager.Instance.EnterOrExitBattle(true);     
        }
    }
}
