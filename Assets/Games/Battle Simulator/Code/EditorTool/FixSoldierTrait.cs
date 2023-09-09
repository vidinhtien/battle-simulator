#if UNITY_EDITOR

using System.Collections.Generic;
using UnityEngine;

namespace BattleSimulatorV2.EditorTool
{
    public class FixSoldierTrait : MonoBehaviour
    {
        [SerializeField] private List<GameObject> listCharacter;


        [ContextMenu("Fix")]
        public void Fix()
        {
            // foreach (var character in listCharacter)
            // {
            //     var soldierTrait = character.GetComponentInChildren<SoldierTrait>();
            //     if(soldierTrait == null)
            //         continue;
            //     
            //     string json = JsonUtility.ToJson(soldierTrait.defaultDataInfo);
            //     JsonUtility.FromJsonOverwrite(json, soldierTrait.traitData);
            //     
            //     EditorUtility.SetDirty(soldierTrait);
            //     PrefabUtility.SavePrefabAsset(character);
            // }
            //
            // AssetDatabase.SaveAssets();
            // AssetDatabase.Refresh();
        }
    }
}

#endif