using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraController : MonoBehaviour {

    public float followSpeed = 20f;
    public float zoom;
    public float zoomSpeed = 2f;
    public float maxZoom = 50f;
    public float minZoom = 10f;

    List<Transform> followTransforms;
    Camera camera;

    public void Init()
    {
        followTransforms = new List<Transform>();

        foreach(GameObject obj in GameObject.FindGameObjectsWithTag("Player"))
        {
            followTransforms.Add(obj.transform);
        }

        followTransforms.Add(GameObject.FindGameObjectWithTag("Ball").transform);
        camera = GetComponent<Camera>();
    }

    void LateUpdate()
    {
        Vector3 averagePosition = Vector3.zero;

        for(int i = 0; i < followTransforms.Count; i++)
        {
            averagePosition += followTransforms[i].position;
        }

        averagePosition =  averagePosition / followTransforms.Count;
        Vector3 targetPos = new Vector3(averagePosition.x, averagePosition.y, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, targetPos, followSpeed * Time.deltaTime);

        Vector3 furthestPos = averagePosition;
        float furthestPosMagnitude = 0f;

        for(int i = 0; i < followTransforms.Count; i++)
        {
            float thisMagnitude = Vector3.SqrMagnitude(averagePosition - followTransforms[i].position);

            if (thisMagnitude > Vector3.SqrMagnitude(averagePosition - furthestPos))
            {
                furthestPos = followTransforms[i].position;
                furthestPosMagnitude = thisMagnitude;
            }
        }

        float targetZoom = Mathf.Clamp(furthestPosMagnitude * zoom, minZoom, maxZoom);
        camera.orthographicSize = Mathf.Lerp(camera.orthographicSize, targetZoom, zoomSpeed * Time.deltaTime);
    }
}
