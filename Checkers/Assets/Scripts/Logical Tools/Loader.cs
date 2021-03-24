using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Loader : MonoBehaviour
{
    public void LoadScene(string name)
    {
        SceneManager.LoadScene(name);
    }
    public void RestartTheScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
