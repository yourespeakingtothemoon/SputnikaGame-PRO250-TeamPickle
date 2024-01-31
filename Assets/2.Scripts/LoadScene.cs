using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadScene : MonoBehaviour
{
    // I am so sorry for this code I couldn't figure the other way :(
    [SerializeField] public string SceneName;
    [SerializeField] int setSpawnSpeedE,setMaxSizeE;    
    [SerializeField] int setSpawnSpeedM,setMaxSizeM;    
    [SerializeField] int setSpawnSpeedH,setMaxSizeH;    
    AsteroidField asteroidField;


    public void SelectMode() 
    {
        if (SceneName == null) Debug.Log("No Mode Selected");
                   
        SceneManager.LoadScene(SceneName); 
    }

    public void SetDifficultyE()
    {
        asteroidField.MaxAsteroidSize = setMaxSizeE;
        asteroidField.SpawnSpeed = setSpawnSpeedE;
    }

    public void SetDifficultyM()
    {
        asteroidField.MaxAsteroidSize = setMaxSizeM;
        asteroidField.SpawnSpeed = setSpawnSpeedM;
    }
    public void SetDifficultyH()
    {
        asteroidField.MaxAsteroidSize = setMaxSizeH;
        asteroidField.SpawnSpeed = setSpawnSpeedH;
    }
}
