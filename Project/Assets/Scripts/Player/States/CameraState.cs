using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEditor.Rendering.LookDev;

namespace CameraSystem
{
    [System.Serializable]
    public enum CameraPositionState
    {
        Regular = 0,
        Falling,
        LookingDown,
        Dialogue,
    }
    public class CameraState : MonoBehaviour
    {
        [SerializeField] private List<CinemachineVirtualCamera> cameras = new List<CinemachineVirtualCamera>();

        [SerializeField] private CameraScriptable cameraValues;

        private CameraPositionState state;
        public CameraPositionState CameraPosState
        {
            private get => state;
            set
            {
                if (value != state)
                {
                    state = value;
                    int cameraIndex = 0;
                    float waitTime = 0;
                    switch (state)
                    {
                        case CameraPositionState.Regular:
                            cameraIndex = 0;
                            break;
                        case CameraPositionState.Falling:
                            cameraIndex = 1;
                            waitTime = cameraValues.FallingTime;
                            break;
                        case CameraPositionState.LookingDown:
                            cameraIndex = 2;
                            break;
                        case CameraPositionState.Dialogue:
                            cameraIndex = 3;
                            break;
                        default:
                            cameraIndex = 0;
                            break;
                    }
                    StopAllCoroutines();
                    StartCoroutine(CameraChangeCheck(waitTime, cameraIndex));
                }
            }
        }

        public void setNewFollowTarget(Transform newTarget)
        {
            foreach (var cam in cameras)
            {
                cam.Follow = newTarget;
            }
        }

        private IEnumerator CameraChangeCheck(float waitingAmount, int priorityIndex)
        {
            yield return new WaitForSeconds(waitingAmount);
            Debug.LogWarning(cameras[priorityIndex].name);
            foreach (var cam in cameras) { cam.Priority = 0; }
            cameras[priorityIndex].Priority = 1;
        }
    }
}