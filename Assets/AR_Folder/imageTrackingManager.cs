using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class imageTrackingManager : MonoBehaviour
{
    ARTrackedImageManager _manager;
    GameObject _model;
    
    // Start is called before the first frame update
    void Start()
    {
        _manager = GetComponent<ARTrackedImageManager>();
        _manager.trackedImagesChanged += OnTrackedImage;        
        _model = Resources.Load<GameObject>("Model_01");
    }

    private void OnTrackedImage(ARTrackedImagesChangedEventArgs obj)
    {
        foreach (var trackedImage in obj.added)
        {
            ShowModel(trackedImage);
            Debug.Log("Image Detected: " + trackedImage.referenceImage.name);
        }
    }
    
    private void ShowModel(ARTrackedImage trackedImage)
    {
        StartCoroutine(ShowModelCoroutine(trackedImage));
    }
        
    IEnumerator ShowModelCoroutine(ARTrackedImage trackedImage)
    {
        while (trackedImage.trackingState==UnityEngine.XR.ARSubsystems.TrackingState.None)
        {
            yield return null;
        }
        
        GameObject cloneModel = Instantiate(_model);
        cloneModel.transform.position = trackedImage.transform.position;
        cloneModel.transform.rotation = trackedImage.transform.rotation;
        cloneModel.transform.localScale *= trackedImage.referenceImage.size.x;
        cloneModel.GetComponent<imageTrackingController>().SetTarget(trackedImage.transform);
    }
}