using UnityEngine;

public class Orbit : MonoBehaviour
{
    //https://youtu.be/bVo0YLLO43s

    Transform CameraTransform;
    Transform ParentTransform;

    Vector3 LocalRotation;
    float camDistance = 10;

    public float MouseSensitivity = 4;
    public float ScrollSensitivity = 2;
    public float OrbitDampen = 10;
    public float ScrollDampen = 6;

    public bool CameraDisabled = true;
    void Start()
    {
        CameraTransform = this.transform;
        ParentTransform = this.transform.parent;
    }

    // Update is called once per frame
    void LateUpdate() //Called after Update(), so we render after everything else is updated
    {
        if(Input.GetMouseButton(0))
        {
            CameraDisabled = false;
        }
        else
        {
            CameraDisabled = true;
        }

        if (!CameraDisabled)
        {
            //Rotate camera based on mouse position
            if (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0)
            {
                LocalRotation.y += Input.GetAxis("Mouse X") * MouseSensitivity;
                LocalRotation.x -= Input.GetAxis("Mouse Y") * MouseSensitivity;

                //Clamp y rotation
                //LocalRotation.x = Mathf.Clamp(LocalRotation.x, 0, 90);
            }
        }
            //Zoom based on scroll wheel
        if(Input.GetAxis("Mouse ScrollWheel") != 0)
        {
                float ScrollAmount = Input.GetAxis("Mouse ScrollWheel") * ScrollSensitivity;

                //camera zooms faster when further away from target
                ScrollAmount *= (camDistance * 0.3f);

                camDistance += ScrollAmount * -1f;

                //constrains camera distance to object
                camDistance = Mathf.Clamp(camDistance, 1.5f, 100f);
        }

        //Actually move camera here
        Quaternion qt = Quaternion.Euler(LocalRotation.x, LocalRotation.y, 0);

        ParentTransform.rotation = Quaternion.Lerp(ParentTransform.rotation, qt, Time.deltaTime * OrbitDampen);

        if(CameraTransform.localPosition.z != camDistance * -1)
        {
            CameraTransform.localPosition = new Vector3(0, 0, Mathf.Lerp(CameraTransform.localPosition.z, camDistance * -1, Time.deltaTime * ScrollDampen));
        }
    }
}
