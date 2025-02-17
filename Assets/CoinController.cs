using UnityEngine;

public class Coin : MonoBehaviour
{
    public float rotationSpeed = 100f; // 旋转速度
    public int scoreValue = 1;        // 硬币的分数值

    void Update()
    {
        // 使硬币绕 Y 轴旋转
        transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
    }

    void OnTriggerEnter(Collider other)
    {
        // 检测玩家是否接触到硬币
        if (other.CompareTag("Player"))
        {
            // 调用玩家的收集方法
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                player.CollectCoin(scoreValue);
            }

            // 销毁硬币
            Destroy(gameObject);
        }
    }
}