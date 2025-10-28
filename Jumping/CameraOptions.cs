using Assets.Scripts.Jumping.StaticInfo;
using System;
using System.Drawing;
using System.Runtime.CompilerServices;
using Unity.Cinemachine;
using UnityEngine;

namespace Assets.Scripts.Jumping
{
    public class CameraOptions : MonoBehaviour
    {
        [SerializeField]
        public CameraOption[] options;

        [SerializeField]
        JumperInfoGlobal jumperInfo;

        private CinemachineFollow cinemachineFollow;
        private CinemachineRotationComposer rotationComposer;
        private int type;
        private Vector3 defaultPos;
        // Start is called once before the first execution of Update after the MonoBehaviour is created

        private const int SIZE = 3;
        void OnValidate()
        {
            foreach (var option in options)
            {
                if (option.isChange)
                {
                    if (option.states.Length != SIZE)
                    {
                        Debug.LogWarning("Don't change the size (InRun/Fly/Down)!");
                        Array.Resize(ref option.states, SIZE);
                    }
                }
                else
                {
                    Array.Resize(ref option.states, 1);
                }
            }
        }

        void Start()
        {
            type = 0;
            cinemachineFollow = GetComponent<CinemachineFollow>();
            rotationComposer = GetComponent<CinemachineRotationComposer>();
            cinemachineFollow.FollowOffset = options[type].getCamera(jumperInfo.jumpState);
            rotationComposer.TargetOffset = options[type].camOffset;
            defaultPos = this.transform.position;
        }

        // Update is called once per frame
        void Update()
        {
            for (int i = 0; i < options.Length; ++i)
            {
                if (Input.GetKeyDown((KeyCode)49 + i))
                {
                    type = i;
                    rotationComposer.TargetOffset = options[type].camOffset;
                    if (!options[type].isLocation)
                    {
                        GetComponent<CinemachineFollow>().enabled = true;
                        this.transform.position = defaultPos;
                    }
                    else
                    {
                        GetComponent<CinemachineFollow>().enabled = false;
                    }
                    break;
                }
            }
            if (!options[type].isLocation)
                cinemachineFollow.FollowOffset = options[type].getCamera(jumperInfo.jumpState);
            else
            {
                this.transform.position = options[type].getCamera(jumperInfo.jumpState);
            }
        }

        public float GetWindArrowChange()
        {
            return options[type].windChangeAngle;
        }

        public void SetInfo(JumperInfoGlobal info)
        {
            jumperInfo = info;
        }
    }

    [System.Serializable]
    public class CameraOption
    {
        public bool isChange = false;
        public float windChangeAngle;
        public Vector3[] states;
        public bool isLocation = false;
        public Vector3 camOffset = new Vector3(0, 0, 0);
        private float timeOffset = 2;

        public Vector3 getCamera(JumpState state)
        {
            if (isChange)
            {
                switch (state)
                {
                    case JumpState.Idle:
                        {
                            timeOffset = 0;
                            return states[0];
                        }
                    case JumpState.Fly:
                        {
                            if (!isLocation || timeOffset > 1.5f)
                            {
                                return states[1];
                            }
                            else
                            {
                                timeOffset += Time.deltaTime;
                                return states[0];
                            }
                        }
                    case JumpState.Down:
                        return states[2];
                }
                return states[0];
            }
            else
            {
                return states[0];
            }
        }

    }
}