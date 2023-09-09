using System.Collections.Generic;
using System.Linq;
using BattleSimulatorV2.UnitSystem;
using UnityEngine;

namespace BattleSimulatorV2.Flexible
{
    public class LookAtCurrentEnemy_UseRigidbody : FlexibleUnitBehaviour
    {
        public Rigidbody rigidbodyToRotate;
        public ChangeValueMode mode = ChangeValueMode.Lerp;
        public float speed = 5;
        public float smoothTime = 1;
        public bool stopLookIfHasBehaviourInProcess = false;

        private Vector3 currentVelocity = Vector3.zero;
        private List<IHaveProcess> listIHaveProcess = new List<IHaveProcess>();
        
        
        
        public override void OnStateAwake()
        {
            base.OnStateAwake();

            
            //Lấy list have process
            foreach (var behaviour in State.listBehaviour)
            {
                if (behaviour is IHaveProcess p)
                {
                    listIHaveProcess.Add(p);
                }
            }
        }
        

        public override void OnStateFixedUpdate()
        {
            if(UnitController.CurrentEnemy == null)
                return;
            
            
            if (stopLookIfHasBehaviourInProcess)
            {
                if(IsAnyHaveProcess())
                    return;
            }
            

            Vector3 groundNormal = Vector3.up; //Để đơn giản, lấy groundNormal là Vector3.up;

            //Tính direction đến target và chiếu lên ground
            Vector3 directionToTarget = UnitController.CurrentEnemy.Position - rigidbodyToRotate.position;
            directionToTarget = Vector3.ProjectOnPlane(directionToTarget, groundNormal); //Chiếu lên mặt phẳng của mặt đất để xoay cho chính xác
            
            Quaternion destRot = Quaternion.LookRotation(directionToTarget);
            
            
            //Smooth
            //Quaternion newRot = Quaternion.Lerp(rigidbodyToRotate.rotation, destRot, rotateSpeed * Time.fixedDeltaTime);
            Quaternion smoothRot = CalculateRotation(rigidbodyToRotate.rotation, destRot);
            rigidbodyToRotate.MoveRotation(smoothRot);
        }

        private Quaternion CalculateRotation(Quaternion fromRot, Quaternion toRot)
        {
            switch (mode)
            {
                case ChangeValueMode.Fixed:
                {
                    return toRot;
                }
                case ChangeValueMode.Lerp:
                {
                    return Quaternion.Slerp(fromRot, toRot, speed * Time.deltaTime);
                }
                case ChangeValueMode.Towards:
                {
                    return Quaternion.RotateTowards(fromRot, toRot, Mathf.Deg2Rad * speed * Time.deltaTime);
                }
                case ChangeValueMode.SmoothDamp:
                {
                    Vector3 fromDirection = fromRot * Vector3.forward;
                    Vector3 toDirection = toRot * Vector3.forward;
                    Vector3 resultDirection = Vector3.SmoothDamp(fromDirection, toDirection, ref currentVelocity, smoothTime, speed, Time.deltaTime);
                    return Quaternion.FromToRotation(Vector3.forward, resultDirection);
                }
                default:
                {
                    Debug.LogError("Not code for mode: " + mode);
                    return toRot;
                }
            }
        }
        
        protected bool IsAnyHaveProcess()
        {
            return listIHaveProcess.Any(p => p.IsInProcess);
        }
    }
}