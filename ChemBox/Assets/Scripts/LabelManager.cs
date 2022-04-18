using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LabelManager : MonoBehaviour
{

    public static LabelManager instance;

    public bool labelsVisible = true;
    private string label;

    public void ToggleLabels()
    {
        labelsVisible = !labelsVisible;
    }

    public GameObject textobj;
    Text text;

    // Start is called before the first frame update

    void Awake() => instance = this;
    void Start()
    {
        text = textobj.GetComponent<Text>();
        label = text.text;
    }

    // Update is called once per frame
    void Update()
    {
        if (!labelsVisible)
        {
            SetText(" ");
        } else
        {
            text = textobj.GetComponent<Text>();
            if (text.text.Length > 1){
                label = text.text;
            }
            SetText(label);
        }
    }

    public void SetText(string input)
    {
        text.text = input;
    }
}
