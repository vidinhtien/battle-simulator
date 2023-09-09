using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ZeroX.FsmSystem.Editors
{
    [System.Serializable]
    public class StateCloner : ScriptableObject
    {
        [SerializeReference] private List<State> listState;

        public static List<State> Clone(List<State> listState)
        {
            if (listState.Count == 0)
                return new List<State>();
            
            StateCloner originCloner = ScriptableObject.CreateInstance<StateCloner>();
            originCloner.listState = listState;
            
            StateCloner newCloner = Instantiate(originCloner);
            var newList = newCloner.listState.ToList();
            
            DestroyImmediate(originCloner);
            DestroyImmediate(newCloner);

            return newList;
        }
    }
}