using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadScene : MonoBehaviour
{
    [SerializeField] public string SceneName;
    int setSpawnSpeed;    
    AsteroidField asteroidField;


    public void SelectMode() 
    {
        if (SceneName == null) Debug.Log("No Mode Selected");
                   
        SceneManager.LoadScene(SceneName); 
    }



    public void SetDifficulty(int setSpawnSpeed, int setAsteroidSize)
    {
        asteroidField.MaxAsteroidSize = setAsteroidSize;
        asteroidField.SpawnSpeed = setSpawnSpeed;
    }
}
