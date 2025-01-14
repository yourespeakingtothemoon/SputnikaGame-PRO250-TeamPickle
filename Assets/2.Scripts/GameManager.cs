using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _levelText;

    private static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<GameManager>();
            }
            return _instance;
        }
    }

    public event System.Action<PlanetData> OnReload;
    public event System.Action OnGameOver;

    [SerializeField] private int _maxStartPlanetID;

    [Header("Next Planet UI")]
    [SerializeField] private Sprite _defaultSprite;
    [SerializeField] Image _nextPlanetImg;

    [Header("Game Over UI")]
    [SerializeField] private GameObject _gameOverUI;
    [SerializeField] private GameObject _congratsUI;
    [SerializeField] private float _gameOverUIDelay = 1f;

    [Header("My Dumb Shit")]
    [SerializeField] public int _gameMode;

    [Header("Level Points and Timer")]
    // [SerializeField] private float _defaultPoints = 100;
    // [SerializeField] private ScoreManager _scoreManager;
    [SerializeField] private float _defaultTimer = 5;
    [SerializeField] private float _amountToSubtractPerLevel = 0.5f;
    [SerializeField] private AsteroidField _asteroidField;
    //public float _points = 0;
    public int _level = 0;

    public bool IsGameOver => _isGameOver;

    private PlanetData _curPlanetData;
    private PlanetData _nextPlanetData;

    private bool _isGameOver = false;
    private bool _sayCongrats = false;

    private void Start()
    {
        _curPlanetData = ChoosePlanet();
        _nextPlanetData = ChoosePlanet();
        ReloadPlanet();
        UpdateTimer();
    }

    private void UpdatePlanetQueue()
    {
        _curPlanetData = _nextPlanetData;
        _nextPlanetData = ChoosePlanet();
        DisplayNextPlanet();
    }

    private PlanetData ChoosePlanet()
    {
        var id = Random.Range(0, _maxStartPlanetID + 1);
        return PlanetManager.GetPlanetData((uint)id);
    }

    private void DisplayNextPlanet()
    {
        if (_nextPlanetData.sprite != null)
        {
            _nextPlanetImg.sprite = _nextPlanetData.sprite;
        }
        else
        {
            _nextPlanetImg.sprite = _defaultSprite;
            _nextPlanetImg.color = _nextPlanetData.color;
        }

        _nextPlanetImg.rectTransform.sizeDelta = 50 * _nextPlanetData.radius * Vector2.one;
    }

    private void Awake()
    {

        UpdateLevelText();
    }

    public void ReloadPlanet()
    {
        UpdatePlanetQueue();
        OnReload?.Invoke(_curPlanetData);
    }

    public void GameOver()
    {
        if (_isGameOver) return;
        Debug.Log("Game Over");

        _isGameOver = true;
        Time.timeScale = 0f;
        SoundManager.Instance.SetMasterVolume(0.3f);
        StartCoroutine(TurnOnGameOverUI());

        OnGameOver?.Invoke();
    }

    IEnumerator TurnOnGameOverUI()
    {
        yield return new WaitForSecondsRealtime(_gameOverUIDelay);
        if (_sayCongrats)
            _congratsUI.SetActive(true);
        else
            _gameOverUI.SetActive(true);
    }

    public void ReloadScene()
    {
        Time.timeScale = 1f;
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.buildIndex);
    }

    public void AddLevel()
    {
        _level++;
        UpdateTimer();
        UpdateLevelText();
    }

    public void UpdateLevelText()
    {
        if (_gameMode == 1)
        { 
            _levelText.text = _level.ToString();
        }
    }

    public void UpdateTimer()
    {
		if (_gameMode == 1)
		{
		    _asteroidField.SpawnSpeed = _defaultTimer - (_level * _amountToSubtractPerLevel);
		}
    }

    public void SayCongrats() => _sayCongrats = true;
}
