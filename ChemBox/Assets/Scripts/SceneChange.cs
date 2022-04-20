using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChange : MonoBehaviour
{
    public void SetMol(string mol)
    {
        StaticClass.CrossSceneInformation = mol;
    }
    public void LoadScene(string sname)
    {
        SceneManager.LoadScene(sname);
    }
}