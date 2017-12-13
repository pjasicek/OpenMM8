using UnityEngine;
using System.Collections;


public class CameraFacingBillboard : MonoBehaviour
{

    public Camera m_Camera;
    public bool amActive = true;
    public bool autoInit = true;
    Transform parentTransform;

    private Quaternion origRotation;

    void Awake()
    {
        if (autoInit == true)
        {
            m_Camera = Camera.main;
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
        if (amActive)
        {
            transform.LookAt(parentTransform.position + m_Camera.transform.rotation * Vector3.back, m_Camera.transform.rotation * Vector3.up);
        }

        origRotation = transform.rotation;
    }

    public void MyOnPostRender(Camera cam)
    {
        transform.rotation = origRotation;
    }
}