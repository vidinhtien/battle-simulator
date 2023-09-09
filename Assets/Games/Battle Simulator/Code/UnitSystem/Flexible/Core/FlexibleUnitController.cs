using System;
using BattleSimulatorV2.DamageSystem;
using BattleSimulatorV2.UnitSystem;
using UnityEngine;
using UnityEngine.Events;
using ZeroX.FsmSystem;
using ZeroX.RagdollSystem;

namespace BattleSimulatorV2.Flexible
{
    public class FlexibleUnitController : UnitController, IDamageable
    {
        [SerializeField] private FsmGraph mainGraph = new FsmGraph();
        [SerializeField] private bool autoStartWorkOnStart = false;
        
        [Header("Events")]
        public UnityEvent<DamageResult> onTakeDamage = new UnityEvent<DamageResult>();
        public UnityEvent<DamageResult> onDead = new UnityEvent<DamageResult>();


        [NonSerialized]
        private UnitController currentEnemy;

        [NonSerialized]
        private RagdollAnimator ragdollAnimator;

        public UnitController CurrentEnemy
        {
            get
            {
                if (currentEnemy == null)
                    return null;
                
                if (currentEnemy.gameObject.activeInHierarchy == false ||
                    currentEnemy.UnitTrait.IsDead)
                {
                    currentEnemy = null;
                    return null;
                }

                return currentEnemy;
            }
            set
            {
                currentEnemy = value;
            }
        }

        public RagdollAnimator RagdollAnimator
        {
            get
            {
                if (ragdollAnimator == null)
                {
                    ragdollAnimator = GetComponentInChildren<RagdollAnimator>();
                }

                return ragdollAnimator;
            }
        }


        protected override void Awake()
        {
            base.Awake();
            
            InitGraph();
        }

        protected override void Start()
        {
            base.Start();
            
            if(autoStartWorkOnStart)
                StartWork();
        }


        protected virtual void InitGraph()
        {
            FsmCenter.RegisterFsmGraph(this, mainGraph, true, true, true, true);
        }
        
        public override void StartWork()
        {
            mainGraph.StartGraph();
        }

        public override void StopWork()
        {
            mainGraph.StopGraph();
        }

        public GameObject GetDamageableObject()
        {
            return gameObject;
        }

        public DamageResult TakeDamage(DamageMessage damageMessage)
        {
            DamageResult damageResult = new DamageResult();
            damageResult.damageMessage = damageMessage;
            damageResult.physicDamageTaken = damageMessage.physicDamage;

            //Nếu đã dead rồi thì thôi ko làm gì và cũng ko bắn event nữa
            if (UnitTrait.IsDead)
            {
                return damageResult;
            }

            
            
            //Trừ máu. Trong trường hợp đơn giản này thì số máu mất bằng số damage
            UnitTrait.MinusHp(damageMessage.physicDamage);
            
            //Events
            onTakeDamage.Invoke(damageResult);

            if (UnitTrait.IsDead)
            {
                OnDead();
                onDead.Invoke(damageResult);
            }
                

            return damageResult;
        }


        protected virtual void OnDead()
        {
            mainGraph.SetTrigger("dead");
        }
    }
}