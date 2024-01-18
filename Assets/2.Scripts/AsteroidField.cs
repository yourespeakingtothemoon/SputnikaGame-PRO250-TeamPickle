using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class AsteroidField : MonoBehaviour
{
	public int MaxAsteroidSize;
	public int SpawnSpeed;
	BoxCollider2D Coll;
	Planet Asteroid;
	public Transform startpos;
	float counter = 0;
	void Start()
    {
        Coll = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
		if (counter <= 0) 
		{
			Debug.Log("owo");
			SpawnAsteroid();
			Asteroid.GetComponent<Rigidbody2D>().velocity = new Vector2 (-1, -1);
			counter = SpawnSpeed;
		}
		counter -= Time.deltaTime;
    }

    public void SpawnAsteroid()
    {
		var id = Random.Range(0, MaxAsteroidSize);
		Asteroid = PlanetManager.Spawn(PlanetManager.GetSpawnedPlanetData((uint)id), RandomPointInBounds(Coll.bounds));
	}

	public Vector2 RandomPointInBounds(Bounds bounds)
	{
		return new Vector2(
			Random.Range(bounds.min.x, bounds.max.x),
			Random.Range(bounds.min.y, bounds.max.y)
		);
	}

}
