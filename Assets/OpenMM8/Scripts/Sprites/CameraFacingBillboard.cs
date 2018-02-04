using UnityEngine;
using System.Collections;


public class CameraFacingBillboard : MonoBehaviour
{

    public Camera Camera;
    public bool amActive = true;
    public bool autoInit = true;
    Transform parentTransform;

    private Quaternion origRotation;

    void Start()
    {
        if (autoInit == true)
        {
            Camera = Camera.main;
            amActive = true;
        }

        /*myContainer = new GameObject();
        myContainer.name = "GRP_" + transform.gameObject.name;
        myContainer.transform.position = transform.position;
        transform.parent = myContainer.transform;*/

        parentTransform = GetComponentInParent<Transform>();
    }

    public void OnEnable()
    {
        Camera.onPreRender += MyOnPreRender;
        Camera.onPostRender += MyOnPostRender;
    }

    public void OnDisable()
    {
        Camera.onPreRender -= MyOnPreRender;
        Camera.onPostRender -= MyOnPostRender;
    }

    void Update()
    {
        //Debug.Log("Called");
    }

    public void MyOnPreRender(Camera cam)
    {
        origRotation = transform.rotation;

        if (amActive)
        {
            //transform.LookAt(parentTransform.position + Camera.transform.rotation * Vector3.back, Camera.transform.rotation * Vector3.up);
            transform.rotation = Quaternion.LookRotation(Camera.transform.position - transform.position);
        }
    }

    public void MyOnPostRender(Camera cam)
    {
        transform.rotation = origRotation;
    }
}