using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class CustomSceneLoader : MonoBehaviour
{
    public void LoadMenu()
    {
        SceneManager.LoadScene(0);
    }
    public void LoadSweetSlotsScene()
    {
        SceneManager.LoadScene(6);
    }
    public void LoadBookSlotsScene()
    {
        SceneManager.LoadScene(7);
    }
    public void LoadBigFishSlotsScene()
    {
        SceneManager.LoadScene(1);
    }
}
