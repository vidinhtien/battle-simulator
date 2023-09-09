using System;
using System.Collections.Generic;
using UnityEngine;


namespace ZeroX.RagdollSystem
{
    public class BoundRagdollTransformSyncer : RagdollTransformSyncer
    {
        public enum SyncMode
        {
            Fixed, Lerp, MoveTowards, SmoothDamp
        }
        
        public class OldPosData
        {
            public Transform transform;
            public Vector3 pos;
        }
        
        
        
        [SerializeField] private RagdollAnimator ragdollAnimator;
        [SerializeField] private Transform syncTarget;
        
        [Tooltip("Danh sách điểm để tạo thành bound. Các điểm này nên đặt ở 2 chân, 2 tay, đầu, xương chậu, ngực")]
        [SerializeField] private List<Transform> listPoint = new List<Transform>();
        
        [Tooltip("Danh sách các object sẽ được giữ world position khi syncTarget được dịch chuyển")]
        [SerializeField] private List<Transform> listIgnoreSync = new List<Transform>();

        [SerializeField] private SyncMode syncMode = SyncMode.Fixed;
        [SerializeField] private float syncSpeed = 20;
        [SerializeField] private float syncSmoothTime = 1;
        
        
        
        


        private bool isSyncing = false;
        public override bool IsSyncing => isSyncing;
        public RagdollAnimator RagdollAnimator => ragdollAnimator;
        public List<Transform> ListPoint => listPoint;
        public List<Transform> ListIgnoreSync => listIgnoreSync;

        Vector3 smoothDamp_CurrentVelocity = Vector3.zero;

        private List<OldPosData> listIgnoreSyncOldPos = new List<OldPosData>();



        #region Unity Method

        private void Reset()
        {
            ragdollAnimator = GetComponent<RagdollAnimator>();
            if (ragdollAnimator != null)
                syncTarget = ragdollAnimator.transform;
        }
        
        
        private void LateUpdate()
        {
            if(isSyncing == false)
                return;

            
            //Record Old Pos
            Vector3 oldPosOfRootFollowBone = ragdollAnimator.RootFollowBone.transform.position;
            RecordOldPosOfListIgnoreSync();
            
            
            
            Vector3 newPos = CalculatePosition();
            
            switch (syncMode)
            {
                case SyncMode.Fixed:
                {
                    Sync_Fixed(newPos);
                    break;
                }
                case SyncMode.Lerp:
                {
                    Sync_Lerp(newPos);
                    break;
                }
                case SyncMode.MoveTowards:
                {
                    Sync_MoveTowards(newPos);
                    break;
                }
                case SyncMode.SmoothDamp:
                {
                    Sync_SmoothDamp(newPos);
                    break;
                }
                default:
                {
                    Debug.LogError("Not code for sync mode: " + syncMode);
                    break;
                }
            }

            ragdollAnimator.RootFollowBone.transform.position = oldPosOfRootFollowBone;
            RestoreOldPosOfListIgnoreSync();
        }

        #endregion


        private void RecordOldPosOfListIgnoreSync()
        {
            //Nếu size của 2 list không giống nhau thì cần điều chỉnh lại
            if (listIgnoreSyncOldPos.Count != listIgnoreSync.Count)
            {
                if (listIgnoreSyncOldPos.Count > listIgnoreSync.Count)
                {
                    listIgnoreSyncOldPos.RemoveRange(listIgnoreSync.Count, listIgnoreSyncOldPos.Count - listIgnoreSync.Count);
                }
                else
                {
                    int add = listIgnoreSync.Count - listIgnoreSyncOldPos.Count;
                    for (int i = 0; i < add; i++)
                    {
                        listIgnoreSyncOldPos.Add(new OldPosData());
                    }
                }
            }
            
            //Ghi lại old pos
            for (int i = 0; i < listIgnoreSync.Count; i++)
            {
                OldPosData oldPosData = listIgnoreSyncOldPos[i];
                oldPosData.transform = listIgnoreSync[i];
                
                if (oldPosData.transform != null)
                    oldPosData.pos = oldPosData.transform.position;
            }
        }

        private void RestoreOldPosOfListIgnoreSync()
        {
            foreach (var oldPosData in listIgnoreSyncOldPos)
            {
                if(oldPosData.transform == null)
                    continue;
                
                oldPosData.transform.position = oldPosData.pos;
            }
        }


        public override void KeepSync()
        {
            isSyncing = true;
        }

        public override void LostSync()
        {
            isSyncing = false;
        }

        

        Vector3 CalculatePosition()
        {
            if (listPoint.Count == 0)
                return ragdollAnimator.RootFollowBone.transform.position;
            
            
            
            Bounds bounds = new Bounds(listPoint[0].position, Vector3.zero);
            
            foreach (var point in listPoint)
            {
                bounds.Encapsulate(point.position);
            }

            Vector3 pos = bounds.center;
            pos.y -= bounds.size.y * 0.5f;
            return pos;
        }

        void Sync_Fixed(Vector3 newPos)
        {
            syncTarget.position = newPos;
        }
        
        void Sync_Lerp(Vector3 newPos)
        {
            syncTarget.position = Vector3.Lerp(syncTarget.position, newPos, syncSpeed * Time.deltaTime);
        }

        void Sync_MoveTowards(Vector3 newPos)
        {
            syncTarget.position = Vector3.MoveTowards(syncTarget.position, newPos, syncSpeed * Time.deltaTime);
        }

        void Sync_SmoothDamp(Vector3 newPos)
        {
            syncTarget.position = Vector3.SmoothDamp(syncTarget.position, newPos, ref smoothDamp_CurrentVelocity, syncSmoothTime, syncSpeed, Time.deltaTime);
        }

        private void OnDrawGizmosSelected()
        {
            if (listPoint.Count == 0)
                return;

            bool boundsInitialized = false;
            Bounds bounds = new Bounds();
            
            foreach (var point in listPoint)
            {
                if(point == null)
                    continue;

                if (boundsInitialized == false)
                {
                    boundsInitialized = true;
                    bounds.center = point.position;
                    bounds.size = Vector3.zero;
                }
                
                bounds.Encapsulate(point.position);
            }
            
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(bounds.center, bounds.size);
            
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(bounds.center + Vector3.up * bounds.size.y * 0.5f, 0.02f);
            Gizmos.DrawSphere(bounds.center - Vector3.up * bounds.size.y * 0.5f, 0.02f);
        }
    }
}