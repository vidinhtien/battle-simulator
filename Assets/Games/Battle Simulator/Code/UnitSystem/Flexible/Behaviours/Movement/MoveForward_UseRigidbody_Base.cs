using UnityEngine;
using ZeroX.RagdollSystem;

namespace BattleSimulatorV2.Flexible
{
    public abstract class MoveForward_UseRigidbody_Base : FlexibleUnitBehaviour
    {
        public float steer = 1;
        public float acceleration = 10;
        public bool projectMoveDirectionOnGround = true;
        

        protected abstract Rigidbody RigidbodyToMove { get; }
        protected float MoveSpeed => UnitController.TraitData.MoveSpeed;
        
        

        public override void OnStateFixedUpdate()
        {
            Move();
        }

        void Move()
        {
            float angle = Vector3.Angle(UnitController.Forward, RigidbodyToMove.velocity);
            if(angle < 5 && RigidbodyToMove.velocity.magnitude > MoveSpeed)
                return;

            Vector3 moveForce = UnitController.Forward * MoveSpeed; //Lực move gốc
            Vector3 steerForce = CalculateSteerForce(angle);
            Vector3 accelerationForce = CalculateAccelerationForce(angle);
            
            //Debug.DrawRay(transform.position + Vector3.up, moveForce * 10, Color.cyan);
            //Debug.DrawRay(transform.position + Vector3.up, steerForce * 10, Color.magenta);
            //Debug.DrawRay(transform.position + Vector3.up, accelerationForce * 10, Color.red);


            Vector3 finalForce = moveForce + steerForce + accelerationForce;
            
            if (projectMoveDirectionOnGround)
            {
                float finalForceMagnitude = finalForce.magnitude;
                finalForce = Vector3.ProjectOnPlane(finalForce, Vector3.up).normalized * finalForceMagnitude; //giữ độ lớn của finalForce
            }
            
            //Debug.DrawRay(transform.position + Vector3.up, finalForce * 10, Color.green);
            
            RigidbodyToMove.AddForce(finalForce, ForceMode.Acceleration);
        }

        Vector3 CalculateSteerForce(float angle)
        {
            float forceMagnitude = Mathf.Lerp(0, MoveSpeed * steer, angle / 180);

            if (angle < 90)
            {
                Vector3 inDirection = Vector3.ProjectOnPlane(-RigidbodyToMove.velocity, UnitController.Up);
                Vector3 forceDirection = Vector3.Reflect(inDirection, UnitController.Forward).normalized;
                return forceDirection * forceMagnitude;
            }
            else
            {
                Vector3 forceDirection = Vector3.ProjectOnPlane(-RigidbodyToMove.velocity, UnitController.Up);
                return forceDirection * forceMagnitude;
            }
        }

        Vector3 CalculateAccelerationForce(float angle)
        {
            //Độ lệch giữa velocity và moveSpeed càng lớn thì càng cần add nhiều lực
            //Góc giữa vận tốc và targetDirection(forward) càng lớn thì càng add ít lực
            
            float deltaByVelocity = 1 - Mathf.Clamp01(RigidbodyToMove.velocity.magnitude / MoveSpeed);
            float deltaByAngle = 1 - Mathf.Clamp01(angle / 90);
            
            float forceMagnitude = Mathf.Lerp(0, MoveSpeed * acceleration, deltaByVelocity * deltaByAngle);
            Vector3 forceDirection = UnitController.Forward;
            return forceDirection * forceMagnitude;
        }
    }
}