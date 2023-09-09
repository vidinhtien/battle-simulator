using System;
using System.Collections.Generic;
using System.Linq;
using BattleSimulatorV2.UnitSystem;
using UnityEngine;

namespace BattleSimulatorV2.Flexible
{
    public class LookAtCurrentEnemy_UseTransform : FlexibleUnitBehaviour
    {
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

        
        
        public override void OnStateUpdate()
        {
            if(UnitController.CurrentEnemy == null)
                return;

            
            if (stopLookIfHasBehaviourInProcess)
            {
                if(IsAnyHaveProcess())
                    return;
            }
            
            
            Vector3 targetDirection = (UnitController.CurrentEnemy.Position - UnitController.Position).normalized;
            Vector3 smoothDirection = CalculateDirection(UnitController.Forward, targetDirection);
            smoothDirection = Vector3.ProjectOnPlane(smoothDirection, UnitController.Up);

            UnitController.Forward = smoothDirection;
        }

        private Vector3 CalculateDirection(Vector3 fromDirection, Vector3 toDirection)
        {
            switch (mode)
            {
                case ChangeValueMode.Fixed:
                {
                    return toDirection;
                }
                case ChangeValueMode.Lerp:
                {
                    return Vector3.Slerp(fromDirection, toDirection, speed * Time.deltaTime);
                }
                case ChangeValueMode.Towards:
                {
                    return Vector3.RotateTowards(fromDirection, toDirection, Mathf.Deg2Rad * speed * Time.deltaTime, 1);
                }
                case ChangeValueMode.SmoothDamp:
                {
                    return Vector3.SmoothDamp(fromDirection, toDirection, ref currentVelocity, smoothTime, speed, Time.deltaTime);
                }
                default:
                {
                    Debug.LogError("Not code for mode: " + mode);
                    return toDirection;
                }
            }
        }


        protected bool IsAnyHaveProcess()
        {
            return listIHaveProcess.Any(p => p.IsInProcess);
        }
    }
}