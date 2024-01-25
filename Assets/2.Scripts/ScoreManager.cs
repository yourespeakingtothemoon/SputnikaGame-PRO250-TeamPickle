using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class ScoreManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _scoreText;
    [SerializeField] private GameManager _gm;
    //singleton
    private static ScoreManager _instance;
    public static ScoreManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<ScoreManager>();
            }
            return _instance;
        }
    }

    private int _score;
    [SerializeField] private int _nextLevel = 100;
    [SerializeField] private int _levelRequirementInclement = 100;
    private void Awake()
    {
        _score = 0;
        UpdateScoreText();
    }

    public void AddScore(int score)
    {
        _score += score;
        if (_score >= _nextLevel)
        {
            _gm.AddLevel();
            _nextLevel += _levelRequirementInclement;
        }
        UpdateScoreText();
    }

    private void UpdateScoreText()
    {
        _scoreText.text = _score.ToString();
    }

    public int getScore()
    {
        return _score;
    }
}
