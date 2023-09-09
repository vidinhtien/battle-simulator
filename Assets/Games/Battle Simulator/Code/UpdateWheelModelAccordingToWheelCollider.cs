using System;
using System.Collections.Generic;
using UnityEngine;

namespace BattleSimulatorV2
{
    public class UpdateWheelModelAccordingToWheelCollider : MonoBehaviour
    {
        [SerializeField] private List<WheelCollider> listWheelCollider = new List<WheelCollider>();
        [SerializeField] private List<Transform> listWheelModel = new List<Transform>();


        private void FixedUpdate()
        {
            for (int i = 0; i < listWheelCollider.Count; i++)
            {
                UpdateWheel(listWheelCollider[i], listWheelModel[i]);
            }
        }

        void UpdateWheel(WheelCollider wheelCollider, Transform wheelModel)
        {
            wheelCollider.GetWorldPose(out var pos, out var rot);
            wheelModel.position = pos;
            wheelModel.rotation = rot;
        }
    }
}