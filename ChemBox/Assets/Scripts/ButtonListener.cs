using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonListener : MonoBehaviour
{
    public bool labelsVisible = false;

    public void ToggleLabels()
    {
        labelsVisible = !labelsVisible;
    }

}
