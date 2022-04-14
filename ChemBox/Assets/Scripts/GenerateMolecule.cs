using System.Collections;
using System.Collections.Generic;
using UnityEngine;

struct atom
{
    public int connected;
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
    public int i, startpar, endpara;
    public UnityEngine.Vector3 vect;
}


public class GenerateMolecule : MonoBehaviour
{
    public Transform parent;
    public GameObject sphere;
    public GameObject dummy;
    public GameObject bond;
    public OpsinInteract opsin;
    private molecule mol;
    public string molname;
    // Start is called before the first frame update
    void Start()
    {
        mol = new molecule();
        mol.rotate = new List<int>();
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

    // Update is called once per frame
    void Update()
    {

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

        Debug.Log(mol.smiles);
        mol.size = 0;

        // DFS
        DFSstep start = new DFSstep();
        Stack<DFSstep> stk = new Stack<DFSstep>();
        UnityEngine.Vector3 vect = new UnityEngine.Vector3(0, 0, 0);
        mol.selectorArr[0] = Instantiate(sphere, vect, roto(0, 0, 0), parent);
        mol.selectorArr[0].name = (mol.smiles[0]).ToString();
        start.vect = vect;
        start.i = 1;
        start.startpar = 0;
        start.GameObjectLoc = -1;
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
                        if (mol.smiles[start.i] != '=' && mol.smiles[start.i] != '#') //ignore for now
                        {
                            start.vect += new UnityEngine.Vector3(1.5f / 2f, 0, 0);
                            if (start.GameObjectLoc == -1)
                            {
                                mol.selectorArr[start.i * 2] = Instantiate(sphere, start.vect, roto(0, 0, 0), parent);
                            }
                            else
                            {
                                mol.selectorArr[start.i * 2] = Instantiate(sphere, start.vect, roto(0, 0, 0), mol.selectorArr[start.GameObjectLoc * 2].transform);
                            }
                            mol.selectorArr[start.i * 2].name = (mol.smiles[start.i]).ToString();
                            mol.size++;
                            // TOOD: Bonds
                            //start.vect += new UnityEngine.Vector3(1.5f/2f, 0, 0);
                            /* if (start.i + 1 < mol.smiles.Length)
                             {
                                 mol.selectorArr[start.i * 2 +1]=Instantiate(bond, mol.vect, roto(0, 0, 1), parent);
                                 mol.vect += new UnityEngine.Vector3(1.5f, 0, 0);
                             }*/
                        }
                        start.i++;
                    }
                    else
                    {
                        if (mol.smiles[start.i] == '(')
                        {
                            DFSstep step2 = start;
                            step2.GameObjectLoc = start.i - 1;
                            mol.atoms[(start.i - 1) * 2].connected++;
                            mol.selectorArr[start.i * 2] = Instantiate(dummy, start.vect, roto(0, 0, 0), mol.selectorArr[(start.i - 1) * 2].transform);
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
                                if (c == '(')
                                {
                                    st.Push(')');
                                }
                                else if (c == ')')
                                {
                                    st.Pop();
                                }
                                if (st.Count == 0)
                                // if (c == ')') // TODO: for other bracket types
                                {
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }
        foreach (int i in mol.rotate)
        {
            mol.selectorArr[i * 2].transform.rotation = roto(0, (mol.atoms[(i - 1) * 2].connected)--, 0);
        }
    }
}