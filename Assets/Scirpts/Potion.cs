using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Potion : MonoBehaviour
{
    public GameObject effectGo;
    public AudioClip pickSound;
    private void OnTriggerEnter2D(Collider2D collision) {

        //Debug.Log("血瓶被触碰");
        //LunaController lunaController=collision.GetComponent<LunaController>();
        if (!collision.CompareTag("Luna"))
        {
            return;
        }
        if (GameManager.Instance.lunaCurrentHP < GameManager.Instance.lunaHP)
            {
                GameManager.Instance.AddOrDecreaseHP(40);
                GameManager.Instance.PlaySound(pickSound);
                Instantiate(effectGo,transform.position,Quaternion.identity);
                //销毁血瓶
                Destroy(gameObject);
            }
        
    }
}
