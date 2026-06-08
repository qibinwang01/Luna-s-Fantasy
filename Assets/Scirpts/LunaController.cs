using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.Video;

public class LunaController : MonoBehaviour
{
    private Rigidbody2D rb;
    private float movespeed = 4;
    
    private Animator animator;
    private Vector2 lookDirection = new Vector2(0, -1);
    private float moveScale;
    private Vector2 move;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Application.targetFrameRate = 144;
        
        animator = GetComponentInChildren<Animator>();
        //animator.SetFloat("MoveValue",0.5f);
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.enterBattle)
        {
            return;
        }
        //如果不能控制Luna就直接返回不执行操作
        if (!GameManager.Instance.CanControlLuna)
        {
            return ;
        }
        //玩家输入监听
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        //Debug.Log(horizontal);
        move = new Vector2(horizontal, vertical);
        

        //判断Luna的朝向
        if (!Mathf.Approximately(move.x, 0) || !Mathf.Approximately(move.y, 0))
        {
            lookDirection.Set(move.x, move.y);
            lookDirection.Normalize();

        }
        //动画控制
        animator.SetFloat("LookX", lookDirection.x);
        animator.SetFloat("LookY", lookDirection.y);
        animator.SetFloat("MoveValue", move.magnitude);
        moveScale = move.magnitude;
        if (move.magnitude > 0)
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                moveScale = 2;
                movespeed = 5;
            }
            else
            {
                moveScale = 1;
                movespeed = 3;
            }
        }
        animator.SetFloat("MoveValue", moveScale);
        //按下空格进行交互
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Talk();
        }

    }

    private void FixedUpdate()
    {
        if (GameManager.Instance.enterBattle)
        {
            return ;
        }
        Vector2 position = transform.position;
        position = position + movespeed * move * Time.fixedDeltaTime;
        //transform.position = position;
        rb.MovePosition(position);
    }

    

    public void Climb(bool start)
    {
        animator.SetBool("Climb",start);
    }
    public void Jump(bool start)
    {
        animator.SetBool("Jump",start);
        rb.simulated=!start;
    }
    public void Talk()
    {
        //获取玩家位置为圆心半径为0.5f的圆内属于NPC的物体碰撞器
        Collider2D collider=Physics2D.OverlapCircle(rb.position,0.5f,LayerMask.GetMask("NPC"));
        if (collider != null)
        {
            if (collider.name == "Nala")
            {
                GameManager.Instance.CanControlLuna=false;
                //collider.GetComponent<NPCDialog>().DisplayDialog();
                NPCDialog nPCDialog=collider.GetComponent<NPCDialog>();
                UIManager.Instance.SetCurrentDialog(nPCDialog);
                nPCDialog.DisplayDialog();
            }
            else if (collider.name=="Dog"&&!GameManager.Instance.hasPetTheDog&&GameManager.Instance.dialogInfos==2)
            {
                Dog dog=collider.GetComponent<Dog>();
                if (dog != null)
                {
                    StartCoroutine(PetDogProcess(dog));
                }
            }
        }
    }
    IEnumerator PetDogProcess(Dog dog)
    {
        GameManager.Instance.CanControlLuna = false;

        PetTheDog();

        // 等 Luna 摸狗动画播放一段时间
        yield return new WaitForSeconds(1.5f);

        dog.BeHappy();

        // 这里直接恢复控制，避免依赖 Dog.cs 里的 Invoke
        GameManager.Instance.CanControlLuna = true;
    }
    public void PetTheDog()
    {
        animator.CrossFade("PetTheDog",0);
        //摸狗的时候强制指定露娜位置防止出现位置错乱
        transform.position=new Vector3(-1.42f,-7.25f,0);
    }
    
}
