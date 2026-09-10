using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Potion : MonoBehaviour
{
    public GameObject effectGo;
    public AudioClip pickSound;

    [Tooltip("引用 ItemSO 资产。")]
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
            Debug.LogError($"[Potion] {name} 没有配置 ItemSO！请在 Inspector 拖入。");
            return;
        }

        int leftOverItems = inventoryManager.AddItem(
            itemSO.itemId,
            1,
            itemSO.icon,
            itemSO.description,
            itemSO.maxStack,         // ★ 每格最大堆叠数从 SO 传入
            itemSO                   // ★ 把 SO 也传过去，OnRightClick 按 itemType 分发
        );

        // 拾取特效（与 Candle 对齐）
        if (effectGo != null)
        {
            Instantiate(effectGo, transform.position, Quaternion.identity);
        }

        GameManager.Instance.PlaySound(pickSound);

        if (leftOverItems == 0)
        {
            Destroy(gameObject);
        }
    }
}