using System;
using UnityEngine;

namespace BattleSimulatorV2.UnitSystem
{
    public class UnitTrait : MonoBehaviour
    {
        //Serialize Fields
        [SerializeField] private int teamId;
        [SerializeField] private UnitTraitData traitData = new UnitTraitData();
        

        public int TeamId
        {
            get => teamId;
            set => teamId = value;
        }
        
        [field: SerializeField] public bool IsDead { get; protected set; }
        [field: SerializeField] public float CurrentHp { get; protected set; }
        
        
        //Properties
        public UnitTraitData TraitData => traitData;
        
        
        //Local Fields
        private bool setupTraitFirst = false;





        #region Unity Method
        
        protected virtual void Awake()
        {
            
        }
        
        protected virtual void Start()
        {
            if (setupTraitFirst == false) //Nếu chưa setup trait lần nào thì tự setup bằng data của chính nó
            {
                Setup(traitData);
            }
        }

        #endregion
       


        public virtual void Setup(UnitTraitData traitData)
        {
            setupTraitFirst = true;
            
            this.traitData = traitData;
            this.CurrentHp = this.traitData.HealthPoint;
        }


        public void MinusHp(float value)
        {
            CurrentHp -= value;
            
            if (CurrentHp <= 0)
            {
                CurrentHp = 0;
                IsDead = true;
            }
        }
    }
}