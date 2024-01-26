using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlanetData
{
    public string name;
    [HideInInspector] public uint id;
    public Sprite sprite;
    public Color color = Color.white;
    public float radius;
    public float mass;
    public int mergeScore;
    public int spawned;
    public GameObject particles;
}

public class Planet : MonoBehaviour
{
    [SerializeField] private float _mergeForce = 1f;
    private PlanetData _data;
    private GravityField _gravityField;
    private GameObject _gameManager;
    private bool _isColliding = false;

    private void Start()
    {
        _gravityField = PlanetManager.GravityField;
        IsInGravityField = _gravityField.IsIn(this);
        if (_data.particles)
        { 
            Instantiate(_data.particles, gameObject.transform);
        }
    }

    public void SetData(PlanetData data)
    {
        _data = data;
        GetComponent<SpriteRenderer>().sortingOrder = (int)data.id;

        if (data.sprite != null) 
            GetComponent<SpriteRenderer>().sprite = data.sprite;
        else
            GetComponent<SpriteRenderer>().color = data.color;

        transform.localScale = Vector3.one * data.radius * 2f;
        GetComponent<Rigidbody2D>().mass = data.mass;
    }
    public PlanetData GetData()
    {
        return _data;
    }
    public PlanetData GetNextData()
    {
        return PlanetManager.GetPlanetData(_data.id + 1);
    }

    private bool _isMerging = false;
    // when planet gets out of the gravity field while playing, game over
    private bool _isPlaying = false;
    // is reload called after spawn
    private bool _calledReload = false;
    
    private bool _isInGravityField = false;
    private bool IsInGravityField
    {
        get
        {
            return _isInGravityField;
        }
        set
        {
            if (value)
            {
                if (!_isInGravityField)
                {
                    _isInGravityField = value;
                    CameraController.Instance.ResetCamTargetSize();
                    if (!_calledReload && _data.spawned == 0)
                    {
                        GameManager.Instance.ReloadPlanet();
                        _calledReload = true;
                    }
                }
            }
            else
            {
                if (_isInGravityField)
                {
                    _isInGravityField = value;
                    if (_gameManager.GetComponent<GameManager>()._gameMode == 0)
                    {
                        if (_isPlaying && !_isMerging)
                        {
                            Debug.Log(gameObject.name + " called GameOver");
                            GameManager.Instance.GameOver();
                        }
                    }
                    else
                    {
                        if (_isPlaying && !_isMerging && _isColliding)
                        {
							Debug.Log(gameObject.name + " called GameOver");
							GameManager.Instance.GameOver();
						}
                    }
                }
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        IsInGravityField = _gravityField.IsIn(this);
        if (!_calledReload && _data.spawned == 0)
        {
            GameManager.Instance.ReloadPlanet();
            _calledReload = true;
        }

        if (_isMerging) return;

        if (collision.gameObject.CompareTag("Planet"))
        {
            _isPlaying = true;
            _isColliding = true;

            Planet otherPlanet = collision.gameObject.GetComponent<Planet>();
            if (otherPlanet._isMerging) return;

            if (FindObjectOfType<GameManager>()._gameMode == 0)
            {
                // Merge
                if (otherPlanet._data.name == _data.name && !(otherPlanet._data.spawned == 1 && _data.spawned == 1))
                {
                    Debug.Log("Merge");

                    ScoreManager.Instance.AddScore(GetData().mergeScore);
                    SoundManager.Instance.PlayMergeSound();

                    if (PlanetManager.IsLastPlanet(GetNextData())) GameManager.Instance.SayCongrats();

                    otherPlanet._isMerging = true;
                    _isMerging = true;

                    var nextPlanetData = GetNextData();
                    var nextPlanet = PlanetManager.Spawn(nextPlanetData, (transform.position + otherPlanet.transform.position) / 2);
                    ApplyForceToOther((transform.position + otherPlanet.transform.position) / 2, nextPlanetData);

                    nextPlanet._isPlaying = true;
                    nextPlanet._calledReload = true;

                    Destroy(gameObject);
                    Destroy(otherPlanet.gameObject);
                }
                else if (!IsInGravityField)
                {
                    Debug.Log(gameObject.name + " called GameOver");
                    GameManager.Instance.GameOver();
                    return;
                }
            }
            else 
            {
                if (otherPlanet._data.name == _data.name && !(otherPlanet._data.spawned == 1 && _data.spawned == 1))
                {
					ScoreManager.Instance.AddScore(GetData().mergeScore);
					SoundManager.Instance.PlayMergeSound();
					Destroy(gameObject);
					Destroy(otherPlanet.gameObject);
				}
				else if (!IsInGravityField)
				{
					Debug.Log(gameObject.name + " called GameOver");
					GameManager.Instance.GameOver();
					return;
				}
			}

        }
    }

	private void OnCollisionExit2D(Collision2D collision)
	{
        if (collision.gameObject.CompareTag("Planet"))
        {
            _isColliding = false;
        }
	}

	private void ApplyForceToOther(Vector2 center, PlanetData data)
    {
        var overlappingPlanets = Physics2D.OverlapCircleAll(center, data.radius);
        foreach (var planetCol in overlappingPlanets)
        {
            if (planetCol.gameObject == gameObject) continue;
            if (!planetCol.gameObject.CompareTag("Planet")) continue;

            Planet otherPlanet = planetCol.gameObject.GetComponent<Planet>();
            if (otherPlanet._isMerging) continue;
            if (otherPlanet._data == _data) continue;

            var planetRb = planetCol.GetComponent<Rigidbody2D>();

            var dir = (Vector2)otherPlanet.transform.position - center;
            
            var dist = dir.magnitude;
            dist -= data.radius + otherPlanet.GetData().radius;

            planetRb.AddForce(-dir.normalized * dist * Mathf.Sqrt(data.mass) *_mergeForce, ForceMode2D.Impulse);
        }
    }

    private void Update()
    {
        if(!_isPlaying)
            CameraController.Instance.SetPlanetDistance(transform);
        
        if(IsInGravityField && _isPlaying)
        {
             _gravityField.SetDistanceFromCenter(this);
        }
    }
    private void FixedUpdate()
    {
        IsInGravityField = _gravityField.IsIn(this);
    }
}
