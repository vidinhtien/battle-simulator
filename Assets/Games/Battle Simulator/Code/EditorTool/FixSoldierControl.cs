#if UNITY_EDITOR

using System.Collections.Generic;
using UnityEngine;

namespace BattleSimulatorV2.EditorTool
{
    public class FixSoldierControl : MonoBehaviour
    {
        [SerializeField] private List<GameObject> listCharacter;


        [ContextMenu("Fix")]
        public void Fix()
        {
            // foreach (var character in listCharacter)
            // {
            //     var soldierControl = character.GetComponentInChildren<SoldierControl>();
            //     if(soldierControl == null)
            //         continue;
            //
            //     soldierControl.unitTrait = soldierControl.soldierInfo;
            //     
            //     EditorUtility.SetDirty(soldierControl);
            //     PrefabUtility.SavePrefabAsset(character);
            // }
            //
            // AssetDatabase.SaveAssets();
            // AssetDatabase.Refresh();
        }
    }
}

#endif