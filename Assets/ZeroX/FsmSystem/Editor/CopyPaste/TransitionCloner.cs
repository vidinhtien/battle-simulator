using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ZeroX.FsmSystem.Editors
{
    [System.Serializable]
    public class TransitionCloner : ScriptableObject
    {
        [SerializeField] private List<Transition> listTransition;
        
        public static List<Transition> Clone(List<Transition> listTransition)
        {
            if (listTransition.Count == 0)
                return new List<Transition>();
            
            TransitionCloner originCloner = ScriptableObject.CreateInstance<TransitionCloner>();
            originCloner.listTransition = listTransition;
            
            TransitionCloner newCloner = Instantiate(originCloner);
            var newList = newCloner.listTransition.ToList();
            
            DestroyImmediate(originCloner);
            DestroyImmediate(newCloner);

            return newList;
        }
    }
}