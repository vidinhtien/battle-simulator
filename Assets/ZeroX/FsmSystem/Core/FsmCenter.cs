using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZeroX.FsmSystem
{
    [AddComponentMenu("")]
    [DefaultExecutionOrder(int.MinValue)]
    public class FsmCenter : MonoBehaviour
    {
        internal static long frameCount;
        private static bool firstUpdate = true;
        
        internal static long fixedFrameCount;
        private static bool firstFixedUpdate = true;

        private static FsmCenter instance;

        private static readonly Dictionary<Behaviour, FsmGraph> listUpdate = new Dictionary<Behaviour, FsmGraph>();
        private static readonly Dictionary<Behaviour, FsmGraph> listLateUpdate = new Dictionary<Behaviour, FsmGraph>();
        private static readonly Dictionary<Behaviour, FsmGraph> listFixedUpdate = new Dictionary<Behaviour, FsmGraph>();
        private static readonly Dictionary<Behaviour, FsmGraph> listLateFixedUpdate = new Dictionary<Behaviour, FsmGraph>();
        


        [RuntimeInitializeOnLoadMethod]
        public static void AutoInit()
        {
            instance = new GameObject("Fsm Center").AddComponent<FsmCenter>();
            DontDestroyOnLoad(instance.gameObject);
            
            Debug.Log("Auto Initialize FsmCenter");
        }

        private void Awake()
        {
            StartCoroutine(LateFixedUpdate());
        }

        private void Update()
        {
            if (firstUpdate == false)
                frameCount++;
            else
                firstUpdate = false;

            
            
            //Update Graph
            List<Behaviour> listNeedRemove = null;
            foreach (var kv in listUpdate)
            {
                if (kv.Key == null)
                {
                    if (listNeedRemove == null)
                        listNeedRemove = new List<Behaviour>();
                    
                    listNeedRemove.Add(kv.Key);
                    continue;
                }
                
                if(kv.Key.isActiveAndEnabled)
                    kv.Value.UpdateGraph();
            }

            if (listNeedRemove != null)
            {
                foreach (var obj in listNeedRemove)
                {
                    listUpdate.Remove(obj);
                }
            }
        }

        private void FixedUpdate()
        {
            if (firstFixedUpdate == false)
                fixedFrameCount++;
            else
                firstFixedUpdate = false;
            
            
            
            //Update Graph
            List<Behaviour> listNeedRemove = null;
            foreach (var kv in listFixedUpdate)
            {
                if (kv.Key == null)
                {
                    if (listNeedRemove == null)
                        listNeedRemove = new List<Behaviour>();
                    
                    listNeedRemove.Add(kv.Key);
                    continue;
                }
                
                if(kv.Key.isActiveAndEnabled)
                    kv.Value.FixedUpdateGraph();
            }
            
            if (listNeedRemove != null)
            {
                foreach (var obj in listNeedRemove)
                {
                    listFixedUpdate.Remove(obj);
                }
            }
        }

        private void LateUpdate()
        {
            //Update Graph
            List<Behaviour> listNeedRemove = null;
            foreach (var kv in listLateUpdate)
            {
                if (kv.Key == null)
                {
                    if (listNeedRemove == null)
                        listNeedRemove = new List<Behaviour>();
                    
                    listNeedRemove.Add(kv.Key);
                    continue;
                }
                
                if(kv.Key.isActiveAndEnabled)
                    kv.Value.LateUpdateGraph();
            }
            
            if (listNeedRemove != null)
            {
                foreach (var obj in listNeedRemove)
                {
                    listLateUpdate.Remove(obj);
                }
            }
        }

        IEnumerator LateFixedUpdate()
        {
            var waitFixedUpdate = new WaitForFixedUpdate();
            while (true)
            {
                yield return waitFixedUpdate;
                
                
                //Update Graph
                List<Behaviour> listNeedRemove = null;
                foreach (var kv in listLateFixedUpdate)
                {
                    if (kv.Key == null)
                    {
                        if (listNeedRemove == null)
                            listNeedRemove = new List<Behaviour>();
                    
                        listNeedRemove.Add(kv.Key);
                        continue;
                    }
                
                    if(kv.Key.isActiveAndEnabled)
                        kv.Value.LateFixedUpdateGraph();
                }
            
                if (listNeedRemove != null)
                {
                    foreach (var obj in listNeedRemove)
                    {
                        listLateFixedUpdate.Remove(obj);
                    }
                }
            }
        }


        /// <summary>
        /// Use FsmCenter to update
        /// </summary>
        public static void RegisterFsmGraph(Behaviour owner, FsmGraph fsmGraph, bool update, bool lateUpdate, bool fixedUpdate, bool lateFixedUpdate)
        {
            if (update)
            {
                if(listUpdate.ContainsKey(owner) == false)
                    listUpdate.Add(owner, fsmGraph);
            }
            
            if (lateUpdate)
            {
                if(listLateUpdate.ContainsKey(owner) == false)
                    listLateUpdate.Add(owner, fsmGraph);
            }
            
            if (fixedUpdate)
            {
                if(listFixedUpdate.ContainsKey(owner) == false)
                    listFixedUpdate.Add(owner, fsmGraph);
            }
            
            if (lateFixedUpdate)
            {
                if(listLateFixedUpdate.ContainsKey(owner) == false)
                    listLateFixedUpdate.Add(owner, fsmGraph);
            }
        }

        public static void UnRegisterFsmGraph(Behaviour owner)
        {
            listUpdate.Remove(owner);
            listLateUpdate.Remove(owner);
            listFixedUpdate.Remove(owner);
            listLateFixedUpdate.Remove(owner);
        }
    }
}