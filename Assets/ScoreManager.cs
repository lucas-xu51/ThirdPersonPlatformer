using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    public TextMeshProUGUI scoreText; // 分数显示文本
    private int score = 0; // 初始分数

    void Start()
    {
        // 初始化分数显示为 "Score: 0"
        AddScore(0);
    }


    public void AddScore(int amount)
    {
        score += amount; // 增加分数
        scoreText.text = $"Score: {score}"; // 更新 UI
    }
}