using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LabelManager : MonoBehaviour
{

    public static LabelManager instance;

    public GameObject textobj;
    Text text;

    // Start is called before the first frame update

    void Awake() => instance = this;
    void Start()
    {
        text = textobj.GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetText(string input)
    {
        text.text = input;
    }
}
