using UnityEngine;
using System.Collections;
using uk.ac.cam.ch.wwmm.opsin;


public class OpsinInteract : MonoBehaviour
{
    private uk.ac.cam.ch.wwmm.opsin.NameToStructure nts;
    public string testInput;
    public string testOutput;
    void Start()
    {
        nts = NameToStructure.getInstance();
        //Debug.Log(nts.parseToSmiles("acetonitrile")); // C(C)#N
        //Debug.Log(nts.parseToSmiles("2,4,6-trinitrotoluene")); // [N+](=O)([O-])C1=C(C)C(=CC(=C1)[N+](=O)[O-])[N+](=O)[O-]
        //Debug.Log(nts.parseToSmiles("1-Hexanoyl-4-carbazolecarbaldehyde")); // C(CCCCC)(=O)C1=CC=C(C=2C3=CC=CC=C3NC12)C=O
    }

    // Update is called once per frame
    void LateUpdate() //Called after Update(), so we render after everything else is updated
    {
        if (testInput.Length > 0)
        {
            testOutput = ptoSmile(testInput);
        }
    }

    public string ptoSmile(string input)
    {
        nts = NameToStructure.getInstance();
        string sout = nts.parseToSmiles(input);
        if (sout == null)
        {
            return "Error";
        } else
        {
            return sout;
        }
    }
}
