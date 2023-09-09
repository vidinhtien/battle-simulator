using System.Collections;
using UnityEngine;

namespace BattleSimulatorV2.Flexible
{
    public abstract class AttackPerSecondsBehaviour : AttackBehaviour
    {
        [Tooltip("Nếu true, attack sẽ ko hoạt động ngay mà đợi attackPerSeconds vào lần đầu attack")]
        public bool waitForFirstAttack = false;
        
        [Tooltip("Khi chuyển state có reset time để attack được ngay hay sẽ đợi đủ attackPerSeconds mới attack")]
        public bool resetTimeWhenEnterState = false;
        
        public override bool IsInAttackProcess => crAttackProcess != null;

        
        private bool isFirstAttack = true;
        protected float lastTimeEndAttackProcess = -999;
        protected Coroutine crLoopAttack;
        protected Coroutine crAttackProcess;


        public float AttackPerSeconds => UnitController.TraitData.AttackPerSeconds;
        
        
        
        public override void OnStateEnter()
        {
            if(resetTimeWhenEnterState)
                lastTimeEndAttackProcess = -999;
            
            StopLoopAttack();
            StopAttackProcess();
            crLoopAttack = StartCoroutine(LoopAttack());
        }

        public override void OnStateExit()
        {
            StopLoopAttack();
            StopAttackProcess();
        }

        void StopLoopAttack()
        {
            if (crLoopAttack != null)
            {
                StopCoroutine(crLoopAttack);
                crLoopAttack = null;
            }
        }
        
        void StopAttackProcess()
        {
            if (crAttackProcess != null)
            {
                StopCoroutine(crAttackProcess);
                crAttackProcess = null;
            }
        }
        

        protected IEnumerator LoopAttack()
        {
            if (isFirstAttack)
            {
                isFirstAttack = false;
                if (waitForFirstAttack)
                    yield return new WaitForSeconds(AttackPerSeconds);
            }

            
            float timePassed = Time.time - lastTimeEndAttackProcess;
            if (timePassed < AttackPerSeconds)
            {
                yield return new WaitForSeconds(AttackPerSeconds - timePassed);
            }
            
            
            while (true)
            {
                crAttackProcess = StartCoroutine(AttackProcess());
                yield return crAttackProcess;
                
                crAttackProcess = null;
                lastTimeEndAttackProcess = Time.time;
                
                yield return new WaitForSeconds(AttackPerSeconds);
            }
        }

        protected abstract IEnumerator AttackProcess();
    }
}