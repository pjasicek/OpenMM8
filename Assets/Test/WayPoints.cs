using UnityEngine;

public class WayPoints : MonoBehaviour
{

    // put the points from unity interface
    public Transform[] wayPointList;

    public int currentWayPoint = 0;
    Transform targetWayPoint;

    public float speed = 4f;

    // Use this for initialization
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // check if we have somewere to walk
        if (currentWayPoint < this.wayPointList.Length)
        {
            if (targetWayPoint == null)
                targetWayPoint = wayPointList[currentWayPoint];
            walk();
        }

        //transform.rotation.eulerAngles.Set(0, transform.eulerAngles.y, transform.eulerAngles.z);
    }

    void walk()
    {
        // rotate towards the target
        Vector3 rot = Vector3.RotateTowards(transform.forward, targetWayPoint.position - transform.position, speed * Time.deltaTime, 0.0f);
        transform.forward.Set(0, rot.y, 0);

        //transform.rotation.eulerAngles.Set(0, transform.eulerAngles.y, 0);

        // move towards the target
        transform.position = Vector3.MoveTowards(transform.position, targetWayPoint.position, speed * Time.deltaTime);
        RaycastHit ray;
        if (Physics.Raycast(transform.position, -Vector3.up, out ray))
        {
            float y = ray.distance - GetComponent<CapsuleCollider>().bounds.extents.y;
            transform.position.Set(transform.position.x, y, transform.position.z);
            Debug.Log("New Y: " + y);
        }

        //GetComponent<Rigidbody>().velocity = new Vector3(2, 0, 5);

        if (transform.position.x == targetWayPoint.position.x && transform.position.z == targetWayPoint.position.z)
        {
            currentWayPoint++;
            targetWayPoint = wayPointList[currentWayPoint];
        }
    }
}