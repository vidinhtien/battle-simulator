using System;
using DG.Tweening;
using UnityEngine;

namespace ZeroX.RagdollSystem.Demo
{
    public class TestAxe : MonoBehaviour
    {
        [SerializeField] private Rigidbody rigidbody;
        [SerializeField] private float gravityScale = 1;


        private void FixedUpdate()
        {
            Vector3 force = (gravityScale - 1) * Physics.gravity;
            rigidbody.AddForce(force, ForceMode.Acceleration);
        }
    }
}