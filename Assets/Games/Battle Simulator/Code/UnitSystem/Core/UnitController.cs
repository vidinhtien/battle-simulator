using UnityEngine;

namespace BattleSimulatorV2.UnitSystem
{
    public abstract class UnitController : MonoBehaviour
    {
        //Serialize Fields
        [SerializeField] private UnitTrait unitTrait;



        //Properties
        public UnitTrait UnitTrait => unitTrait;
        public UnitTraitData TraitData => unitTrait.TraitData;
        public int TeamId => unitTrait.TeamId;




        #region Unity Method

        protected virtual void Awake()
        {
            UnitManager.RegisterUnit(this);
        }
        
        protected virtual void OnEnable()
        {
            
        }
        
        protected virtual void Start()
        {
            
        }
        
        protected virtual void OnDisable()
        {
            
        }

        protected virtual void OnDestroy()
        {
            UnitManager.UnRegisterUnit(this);
        }

        protected virtual void Reset()
        {
            unitTrait = GetComponent<UnitTrait>();
        }

        #endregion




        #region Transform

        public virtual Vector3 Position
        {
            get => transform.position;
            set => transform.position = value;
        }

        public virtual Vector3 Up
        {
            get => transform.up;
            set => transform.up = value;
        }
        
        public virtual Vector3 Down
        {
            get => -transform.up;
            set => transform.up = -value;
        }
        
        public virtual Vector3 Forward
        {
            get => transform.forward;
            set => transform.forward = value;
        }

        #endregion
        
        
        
        
        //Methods
        public virtual void SetupTrait(UnitTraitData traitData)
        {
            unitTrait.Setup(traitData);
        }



        public abstract void StartWork();

        public abstract void StopWork();

        public virtual void DestroySelf()
        {
            Destroy(gameObject);
        }
    }
}