using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.InputSystem;
using System.Text;
using System.IO;

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

    public IEnumerable GetData(int count)
    {
        if (data.Count < count) return data;
        return data.Skip(data.Count - count);
    }

    public IEnumerable GetData(float from, float to)
    {
        return data.Where(d => d.timestamp > from && d.timestamp < to);
    }

    public IEnumerable GetData(float duration)
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

            Vector3 globalPosition = showRelativeTo.transform.position + element.position;
            Quaternion globalRotation = element.rotation * showRelativeTo.transform.rotation;

            trackedObject.transform.SetPositionAndRotation(globalPosition, globalRotation);

            yield return new WaitForFixedUpdate();
        }
       
    }

    public void ExportData()
    {
        string[] headers = { "timestamp", "position", "rotation" };
        List<string[]> rows = new List<string[]>();

        rows.Add(headers);

        foreach(TrackedData d in data)
        {
            rows.Add(new string[] { d.timestamp.ToString(), d.position.ToString(), d.rotation.ToString() });
        }

        // Generate CSV string
        StringBuilder sb = new StringBuilder();
        foreach (string[] row in rows)
        {
            sb.AppendLine(string.Join(",", row));
        }

        // Save CSV data to a file
        string filePath = Application.dataPath + "/data.csv";
        File.WriteAllText(filePath, sb.ToString());

        Debug.Log("CSV file saved to: " + filePath);
    }

}
