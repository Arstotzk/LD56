using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hook : MonoBehaviour
{
    public float distance = 10f;
    public float minDistance = 1f;
    public float currentDistance = 0f;


    private Vector3 target;
    private RaycastHit2D raycast;
    public LineRenderer line;
    public DistanceJoint2D joint;
    public DistanceJoint2D jointPlayer;
    public LayerMask mask;
    private Vector3 startedPosition;

    public bool isEnabled;

    void Start()
    {
        startedPosition = this.transform.localPosition;
        line = GetComponent<LineRenderer>();
        joint.enabled = false;
        jointPlayer.enabled = false;
        line.enabled = false;
        isEnabled = false;
        this.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
    }


    void Update()
    {
        if (isEnabled)
            UpdateHook();
        else
            this.transform.localPosition = startedPosition;
    }

    public bool SetHook() 
    {
        target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        target.z = 0;
        
        var raycasts = Physics2D.RaycastAll(transform.position, target - transform.position, distance);
        foreach (var rayFirstObject in raycasts)
        {
            if (rayFirstObject.collider.gameObject.tag == "Player" && rayFirstObject.collider.gameObject.layer == LayerMask.NameToLayer("Grabbing"))
                break;
            if (rayFirstObject.collider.gameObject.tag == "Player")
                continue;
            if (rayFirstObject.collider.gameObject.layer != LayerMask.NameToLayer("Grabbing"))
                return false;
            else
                break;
        }

        raycast = Physics2D.Raycast(transform.position, target - transform.position, distance, mask);

        if (raycast.collider != null)
        {
            this.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
            joint.enabled = true;
            jointPlayer.enabled = true;
            joint.connectedBody = raycast.collider.gameObject.GetComponent<Rigidbody2D>();
            joint.connectedAnchor = raycast.point - new Vector2(raycast.collider.transform.position.x, raycast.collider.transform.position.y);
            joint.distance = Vector2.Distance(transform.position, raycast.point);

            line.enabled = true;
            line.SetPosition(0, transform.position);
            line.SetPosition(1, raycast.point);

            isEnabled = true;
            return true;
        }
        return false;
    }
    public void UpdateHook()
    {
        line.SetPosition(0, transform.position);
        line.SetPosition(1, joint.connectedAnchor + new Vector2(raycast.collider.transform.position.x, raycast.collider.transform.position.y));
    }
    public void UnsetHook() 
    {
        this.transform.localPosition = startedPosition;
        this.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
        joint.enabled = false;
        jointPlayer.enabled = false;
        line.enabled = false;
        isEnabled = false;
    }
    public void Climb(float value)
    {
        if (isEnabled)
        {
            joint.distance += value;
            if (joint.distance > distance)
            {
                joint.distance = distance;
            }
            if (joint.distance < minDistance)
            {
                joint.distance = minDistance;
            }
        }
    }
}
