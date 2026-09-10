using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Candle : MonoBehaviour
{
    public GameObject effectGo;
    public AudioClip pickClip;

    [Tooltip("引用 ItemSO 资产，决定拾取入背包时的所有数据（名称、图标、描述、最大堆叠）。")]
    public ItemSO itemSO;

    private InventoryManager inventoryManager;

    private void Start()
    {
        inventoryManager = GameObject.Find("Canvas").GetComponent<InventoryManager>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Luna"))
        {
            return;
        }

        if (itemSO == null)
        {
            Debug.LogError($"[Candle] {name} 没有配置 ItemSO！请在 Inspector 拖入。");
            return;
        }

        GameManager.Instance.candleNum++;
        GameManager.Instance.PlaySound(pickClip);
        Instantiate(effectGo, transform.position, Quaternion.identity);

        // 调用 AddItem：从 ItemSO 一次性拿到所有需要的字段
        int leftOverItems = inventoryManager.AddItem(
            itemSO.itemId,
            1,                      // 每根蜡烛拾取 1 个
            itemSO.icon,
            itemSO.description,
            itemSO.maxStack,         // ★ 每格最大堆叠数从 SO 传入
            itemSO                   // ★ 把 SO 也传过去，OnRightClick 按 itemType 分发
        );

        if (leftOverItems == 0)
        {
            Destroy(gameObject);
        }
        if (GameManager.Instance.candleNum >= 5)
        {
            GameManager.Instance.SetContentIndex();
        }
        GameManager.Instance.UpdateTaskText();
    }
}