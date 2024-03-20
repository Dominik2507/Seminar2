using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
public class TrackRightController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(InputTracking.GetLocalPosition(XRNode.RightHand));
        Debug.Log(InputTracking.GetLocalRotation(XRNode.RightHand));
    }
}
