using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Candle : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject effectGo;
    public AudioClip pickClip;

    private void OnTriggerEnter2D(Collider2D collision) {
        if (!collision.CompareTag("Luna"))
        {
            return;
        }
        GameManager.Instance.candleNum++;
        GameManager.Instance.PlaySound(pickClip);
        Instantiate(effectGo,transform.position,Quaternion.identity);
        //销毁蜡烛
        Destroy(gameObject);
        if (GameManager.Instance.candleNum >= 5)
        {
            GameManager.Instance.SetContentIndex();
        }
        GameManager.Instance.UpdateTaskText();
    }
}
