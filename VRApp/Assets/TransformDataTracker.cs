using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.InputSystem;

public struct TrackedData
{
    public float timestamp;
    public Vector3 position;
    public Quaternion rotation;

    public TrackedData(Vector3 p, Quaternion r, float start_timestamp)
    {
        timestamp = Time.time - start_timestamp;
        position = p;
        rotation = r;
    }
    public TrackedData(Transform go, float start_timestamp)
    {
        timestamp = Time.time - start_timestamp;
        position = go.position;
        rotation = go.rotation;
    }

    public TrackedData(GameObject go, float start_timestamp)
    {
        timestamp = Time.time - start_timestamp;
        position = go.transform.position;
        rotation = go.transform.rotation;
    }

    public TrackedData(float start_timestamp)
    {
        timestamp = start_timestamp;
        position = Vector3.zero;
        rotation = Quaternion.identity;
    }

    public bool IsInitialized()
    {
        return timestamp >= 0;
    }
}

public class TrackedDataComparer : IComparer<TrackedData>
{
    public int Compare(TrackedData x, TrackedData y)
    {
        return x.timestamp.CompareTo(y.timestamp);
    }
}

public class TransformDataTracker : MonoBehaviour
{
    GameObject trackedObject;
    

    public GameObject showRelativeTo;
    public GameObject collectRelativeTo;
    public TrackedData collectRelativeToData;

    public float tracking_start_timestamp;
    public bool isCollectingData = false;

    public SortedSet<TrackedData> data;

    private void Start()
    {
        if (!showRelativeTo) showRelativeTo = gameObject;
        if (!collectRelativeTo) collectRelativeToData = new TrackedData(gameObject, Time.time);

        SubscribeToObject(gameObject, showRelativeTo, gameObject.transform);
    }

    private void Update()
    {
        if (isCollectingData) CollectData();

        if (Keyboard.current.rKey.wasPressedThisFrame)
        {
            Debug.Log("R pressed");
            StartCoroutine(ReplayData());
        }
    }

    public void SubscribeToObject(GameObject go)
    {
        trackedObject = go;
        showRelativeTo = go;
        collectRelativeToData = new TrackedData(gameObject, Time.time);

        data = new(new TrackedDataComparer());
        isCollectingData = true;
        tracking_start_timestamp = Time.time;
    }

    public void SubscribeToObject(GameObject go, Transform relativeTo)
    {
        trackedObject = go;
        showRelativeTo = go;
        collectRelativeToData = new TrackedData(relativeTo, Time.time);

        data = new(new TrackedDataComparer());
        isCollectingData = true;
        tracking_start_timestamp = Time.time;
    }

    public void SubscribeToObject(GameObject go, GameObject relativeShow)
    {
        trackedObject = go;
        showRelativeTo = relativeShow;
        collectRelativeToData = new TrackedData(go, Time.time);

        data = new(new TrackedDataComparer());
        isCollectingData = true;
        tracking_start_timestamp = Time.time;
    }

    public void SubscribeToObject(GameObject go, GameObject relativeShow, Transform relativeTo)
    {
        trackedObject = go;
        showRelativeTo = relativeShow;
        collectRelativeToData = new TrackedData(relativeTo, Time.time);

        data = new(new TrackedDataComparer());
        isCollectingData = true;
        tracking_start_timestamp = Time.time;
    }

    public void RestartTracking()
    {
        data = new(new TrackedDataComparer());
        isCollectingData = true;
        tracking_start_timestamp = Time.time;
    }

    public IEnumerable getData(int count)
    {
        if (data.Count < count) return data;
        return data.Skip(data.Count - count);
    }

    public IEnumerable getData(float from, float to)
    {
        return data.Where(d => d.timestamp > from && d.timestamp < to);
    }

    public IEnumerable getData(float duration)
    {
        return data.Where(d => d.timestamp <= duration);
    }

    private void CollectData()
    {
        Vector3 relativePosition = trackedObject.transform.position - collectRelativeToData.position;
        Quaternion relativeRotation = trackedObject.transform.rotation * Quaternion.Inverse(collectRelativeToData.rotation);

        data.Add(new TrackedData(relativePosition, relativeRotation, tracking_start_timestamp));
    }

    public IEnumerator ReplayData()
    {
        isCollectingData = false;

        
        foreach (TrackedData element in data)
        {

            Debug.Log(element.position);
            Debug.Log(element.rotation);

            Vector3 globalPosition = showRelativeTo.transform.position + element.position;
            Quaternion globalRotation = element.rotation * showRelativeTo.transform.rotation;

            Debug.Log(globalPosition);
            Debug.Log(globalRotation);

            trackedObject.transform.position = globalPosition;
            trackedObject.transform.rotation = globalRotation;

            yield return new WaitForFixedUpdate();
        }
       
    }

}
