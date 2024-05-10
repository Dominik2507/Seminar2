using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.UI;
using System.Text;
using System.IO;
public class TrackRightController : MonoBehaviour
{
    // Start is called before the first frame update

    public TransformDataTracker tracker;

    public List<TrackedData[]> trackingDataLists = new ();

    public Image RecordingImage;

    public InputDevice device;

    public bool PrimaryButtonHolder = false;
    void Start()
    {
        List<InputDevice> rightHandDevices = new();
        UnityEngine.XR.InputDevices.GetDevicesAtXRNode(UnityEngine.XR.XRNode.RightHand, rightHandDevices);
        
        if(rightHandDevices.Count == 1)
            device = rightHandDevices[0];
    }

    // Update is called once per frame
    void Update()
    {
        bool primaryValue;
        if (device != null && device.TryGetFeatureValue(UnityEngine.XR.CommonUsages.primaryButton, out primaryValue) && primaryValue)
        {
            Debug.Log("Primary button is pressed.");
            if (tracker.isCollectingData) EndRecording();
            else StartRecording();
        }
        if (PrimaryButtonHolder)
        {
            if (tracker.isCollectingData)
            {
                EndRecording();
                //tracker.StartCoroutine(tracker.ReplayData());
            }
            else StartRecording();
            PrimaryButtonHolder = false;
        }

        //Debug.Log(InputTracking.GetLocalPosition(XRNode.RightHand));
        //Debug.Log(InputTracking.GetLocalRotation(XRNode.RightHand));
    }

    void StartRecording()
    {
        tracker.StartRecording();
        RecordingImage.gameObject.active = true;
    }

    void EndRecording()
    {
        tracker.EndRecording();
        TrackedData[] data = new TrackedData[tracker.data.Count];
        tracker.data.CopyTo(data, 0);
        trackingDataLists.Add(data);
        RecordingImage.gameObject.active = false;
    }

    public void ExportData()
    {
        string[] headers = { "recordingNumber", "timestamp", "position", "rotation" };
        List<string[]> rows = new List<string[]>();

        rows.Add(headers);

        int i = 0;

        foreach(TrackedData[] list in trackingDataLists)
        {
            foreach (TrackedData d in list)
            {
                rows.Add(new string[] { i.ToString(), d.timestamp.ToString(), d.position.ToString(), d.rotation.ToString() });
            }

            i++;
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
