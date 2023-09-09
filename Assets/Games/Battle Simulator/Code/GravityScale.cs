using System;
using UnityEngine;

namespace BattleSimulatorV2
{
    public class GravityScale : MonoBehaviour
    {
        [SerializeField] private Rigidbody rigidbody;
        public float gravityFactor = 1;


        private void Reset()
        {
            rigidbody = GetComponent<Rigidbody>();
        }

        private void FixedUpdate()
        {
            Vector3 force = (gravityFactor - 1) * Physics.gravity;
            
            rigidbody.AddForce(force, ForceMode.Acceleration);
        }
    }
}