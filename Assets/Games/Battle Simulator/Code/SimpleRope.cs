using System;
using UnityEngine;

namespace BattleSimulatorV2
{
    public class SimpleRope : MonoBehaviour
    {
        [SerializeField] private Transform pointA;
        [SerializeField] private Transform pointB;


        private void Update()
        {
            Vector3 direction = pointB.position - pointA.position;
            transform.forward = direction;

            transform.position = pointA.position + direction * 0.5f;
            
            //Scale
            Vector3 localScale = transform.localScale;
            localScale.z = direction.magnitude;
            transform.localScale = localScale;
        }
    }
}