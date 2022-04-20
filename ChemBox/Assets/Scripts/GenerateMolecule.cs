using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Linq;
using System.Text;
using UnityEngine;

struct atom
{
    public int connected;
    public int rotateHead;
    public int rotateLeft;
    public UnityEngine.Quaternion finalQuant;
}

struct molecule
{
    public string smiles;
    public string name;
    public GameObject[] selectorArr;
    public atom[] atoms;
    public List<int> rotate;
    public int size; // unused
}

struct DFSstep
{
    public int GameObjectLoc;
    public int lastObj;
    public int i, startpar, endpara;
    public UnityEngine.Vector3 vect;
}


public class GenerateMolecule : MonoBehaviour
{
    public Transform parent;
    public GameObject atomObj;
    public GameObject dummy;
    public GameObject SingleBond;
    public GameObject DoubleBond;
    public GameObject TripleBond;
    public OpsinInteract opsin;
    public bool bonds;
    private molecule mol;
    public string molname;

    // Start is called before the first frame update
    void Start()
    {
        mol = new molecule();
        mol.rotate = new List<int>();
        if(StaticClass.CrossSceneInformation != null)
        {
            molname = StaticClass.CrossSceneInformation;
        }
        mol.name = molname;
        mol.smiles = opsin.ptoSmile(mol.name);
        //mol.smiles = "C";
        if (mol.smiles == "Error")
        {
            mol.smiles = "";
        }
        else
        {
            mol.selectorArr = new GameObject[(mol.smiles.Length + 1) * 4];
            mol.atoms = new atom[(mol.smiles.Length + 1) * 4];
            SpawnObject(mol);
        }
    }
    private float timeCount = 0.0f;
    private Vector3 startVect;
    private Vector3 endVect;
    private float startTime;

    // Update is called once per frame
    void Update()
    {
        float fracComplete = (Time.time - startTime) / 0.45f;
        parent.position = Vector3.Slerp(startVect, endVect, fracComplete);
        // Rotate subgroups off each other animation
        foreach (int i in mol.rotate)
        {
            mol.selectorArr[i * 2].transform.rotation = Quaternion.Slerp(mol.selectorArr[i * 2].transform.rotation, mol.atoms[i * 2].finalQuant, timeCount * 0.04f);
            timeCount = timeCount + Time.deltaTime;
        }
    }

    // Ripped from stack overflow
    //Change the Quaternion values depending on the values of the Sliders
    private static UnityEngine.Quaternion roto(float x, float y, float z)
    {
        UnityEngine.Quaternion newQuaternion = new UnityEngine.Quaternion();
        newQuaternion.Set(x, y, z, 1);
        //Return the new Quaternion
        return newQuaternion;
    }

    void SpawnObject(molecule mol)
    {
        LabelManager.instance.SetText(mol.name);

        Debug.Log(mol.smiles);
        mol.size = 0;

        // DFS
        DFSstep start = new DFSstep();
        Stack<DFSstep> stk = new Stack<DFSstep>();
        UnityEngine.Vector3 vect = new UnityEngine.Vector3(0, 0, 0);
        mol.selectorArr[0] = Instantiate(atomObj, vect, roto(0, 0, 0), parent);
        mol.selectorArr[0].name = (mol.smiles[0]).ToString();
        start.vect = vect;
        start.i = 1;
        start.startpar = 0;
        start.GameObjectLoc = 0;
        start.lastObj = 0;
        stk.Push(start);
        while (stk.Count != 0)
        {
            start = stk.Peek();
            stk.Pop();
            start.vect = mol.selectorArr[start.startpar * 2].transform.position;
            for (int i = start.i; i < mol.smiles.Length; i++)
            {
                if (start.i < mol.smiles.Length)
                {
                    if (mol.smiles[start.i] != '(' && mol.smiles[start.i] != ')')
                    {
                        if (mol.smiles[start.i] != '=' && mol.smiles[start.i] != '#' && char.IsLetter(mol.smiles[start.i])) //ignore for now
                        {
                            string namea = (mol.smiles[start.i]).ToString();
                            string bondname = "Single Bond";
                            if (start.i != 0) // check behind to see if bracket or bond type (ugly but identical big O)
                            {
                                if (mol.smiles[start.i - 1] == '[')
                                {
                                    if (start.i > 1) // check behind to see if bracket or bond type (ugly but identical big O)
                                    {
                                        if (mol.smiles[start.i - 2] == '=')
                                        {
                                            bondname = "Double Bond";
                                        }
                                        else if (mol.smiles[start.i - 2] == '#')
                                        {
                                            bondname = "Triple Bond";
                                        }
                                    }
                                    for (int j = start.i + 1; j < mol.smiles.Length; j++)
                                    {
                                        if (mol.smiles[j] == ']')
                                        {
                                            break;
                                        }
                                        namea += (mol.smiles[j]).ToString();
                                        start.i++;
                                    }
                                }
                                else if (mol.smiles[start.i - 1] == '=')
                                {
                                    bondname = "Double Bond";
                                }
                                else if (mol.smiles[start.i - 1] == '#')
                                {
                                    bondname = "Triple Bond";
                                }
                            }
                            if (namea.Length == 1 && start.i + 1 < mol.smiles.Length) // peak ahead to see if two letter element
                            {
                                if (char.IsLetter(mol.smiles[start.i + 1]) && char.IsLower(mol.smiles[start.i + 1]))
                                {
                                    namea += (mol.smiles[start.i + 1]).ToString();
                                    start.i++;
                                }
                            }

                            if (bonds)
                            {
                                if (start.i != 0) // Bonds
                                {
                                    start.vect += new UnityEngine.Vector3(1.5f / 2f, 0, 0);
                                    if (start.GameObjectLoc == 0)
                                    {
                                        if (bondname == "Single Bond")
                                        {
                                            mol.selectorArr[start.i * 2 + 1] = Instantiate(SingleBond, start.vect, roto(0, 0, 1), parent);  // TODO: change if = or #
                                        }
                                        else if (bondname == "Double Bond")
                                        {
                                            mol.selectorArr[start.i * 2 + 1] = Instantiate(DoubleBond, start.vect, roto(0, 0, 1), parent);  // TODO: change if = or #

                                        }
                                        else if (bondname == "Triple Bond")
                                        {
                                            mol.selectorArr[start.i * 2 + 1] = Instantiate(TripleBond, start.vect, roto(0, 0, 1), parent);  // TODO: change if = or #

                                        }
                                    }
                                    else
                                    {
                                        if (bondname == "Single Bond")
                                        {
                                            mol.selectorArr[start.i * 2 + 1] = Instantiate(SingleBond, start.vect, roto(0, 0, 1), mol.selectorArr[start.GameObjectLoc * 2].transform); // TODO: change if = or #
                                        }
                                        else if (bondname == "Double Bond")
                                        {
                                            mol.selectorArr[start.i * 2 + 1] = Instantiate(DoubleBond, start.vect, roto(0, 0, 1), mol.selectorArr[start.GameObjectLoc * 2].transform); // TODO: change if = or #

                                        }
                                        else if (bondname == "Triple Bond")
                                        {
                                            mol.selectorArr[start.i * 2 + 1] = Instantiate(TripleBond, start.vect, roto(0, 0, 1), mol.selectorArr[start.GameObjectLoc * 2].transform); // TODO: change if = or #

                                        }
                                    }
                                    mol.selectorArr[start.i * 2 + 1].name = bondname;
                                }
                            }

                            start.vect += new UnityEngine.Vector3(1.5f / 2f, 0, 0);
                            if (start.GameObjectLoc == 0)
                            {
                                mol.selectorArr[start.i * 2] = Instantiate(atomObj, start.vect, roto(0, 0, 0), parent);
                            }
                            else
                            {
                                mol.selectorArr[start.i * 2] = Instantiate(atomObj, start.vect, roto(0, 0, 0), mol.selectorArr[start.GameObjectLoc * 2].transform);
                            }
                            mol.selectorArr[start.i * 2].name = namea;
                            mol.size++;
                            start.lastObj = start.i;
                        }
                        start.i++;
                    }
                    else
                    {
                        if (mol.smiles[start.i] == '(')
                        {
                            DFSstep step2 = start;
                            step2.GameObjectLoc = start.lastObj;
                            mol.atoms[step2.GameObjectLoc * 2].rotateHead++;
                            mol.atoms[(start.i - 1) * 2].connected = step2.GameObjectLoc;
                            mol.selectorArr[start.i * 2] = Instantiate(dummy, start.vect, roto(0, 0, 0), mol.selectorArr[step2.GameObjectLoc * 2].transform);
                            mol.selectorArr[start.i * 2].name = "dummy";
                            mol.rotate.Add(start.i);
                            step2.GameObjectLoc = start.i;
                            step2.startpar = start.i;
                            step2.i++;
                            stk.Push(step2);
                            Stack<char> st = new Stack<char>();
                            for (int j = start.i; j < mol.smiles.Length; j++)
                            {
                                start.i++;
                                char c = mol.smiles[j];
                                if (c == '(') // TODO: for other bracket types
                                {
                                    st.Push(')');
                                }
                                else if (c == st.Peek())
                                {
                                    st.Pop();
                                }
                                if (st.Count == 0)
                                {
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }
        // Color each atom based off name
        Vector3 meanVector = Vector3.zero;
        int total = 0;
        foreach (GameObject i in mol.selectorArr)
        {
            if (i != null && i.name != "dummy" && i.name != "Single Bond" && i.name != "Double Bond" && i.name != "Triple Bond")
            {
                meanVector += i.transform.position;
                total++;
                // Hash name of atom and then use as color values
                var render = i.GetComponent<Renderer>();
                byte[] encoded = SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(i.name));
                var value = System.BitConverter.ToUInt32(encoded, 0);
                float b = value % 100 / 100f;
                float g = value % 1000000000 / 1000000000f;
                float r = value % 1000000 / 1000000f;
                Color autogenColor = new Color(r, g, b, 1f);
                render.material.color = autogenColor;
            }
        }
        startVect = parent.position;
        endVect = (-meanVector / total); // Center Molecule

        // Rotate subgroups off each other calculation
        foreach (int i in mol.rotate)
        {
            float leftToRotate = ++mol.atoms[mol.atoms[(i - 1) * 2].connected * 2].rotateLeft;
            float rotateHead = (mol.atoms[mol.atoms[(i - 1) * 2].connected * 2].rotateHead);
            float spin = (leftToRotate) / (rotateHead + 1);
            //UnityEngine.Quaternion tmpQuaternion = mol.selectorArr[i * 2].transform.rotation;
            //mol.selectorArr[i * 2].transform.rotation = roto(-((spin * 3f) - 1f), -((spin * 4f) - 1f), ((spin * 2f) - 1f));
            //transform.rotation = Quaternion.FromToRotation(tmpQuaternion, transform.forward);
            mol.atoms[i * 2].finalQuant = roto(-((spin * 3f) - 1f), -((spin * 4f) - 1f), ((spin * 2f) - 1f));
        }
        startTime = Time.time;
    }
}