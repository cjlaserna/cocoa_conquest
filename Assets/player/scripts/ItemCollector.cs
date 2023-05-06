using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemCollector : MonoBehaviour
{
    private int coins = 0;
    [SerializeField] private TextMeshProUGUI coinsText;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log(collision);
        if (collision is BoxCollider2D) {
            Coin coin = collision.gameObject.GetComponent<Coin>();
            if (coin && !coin.HasBeenCollected)
            {
                coin.HasBeenCollected = true;
                Destroy(collision.gameObject);
                coins++;
                coinsText.text = coins.ToString();
            }
        }
    }
}
