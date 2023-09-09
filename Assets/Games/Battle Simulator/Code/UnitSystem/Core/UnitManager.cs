using System.Collections.Generic;
using System.Linq;

namespace BattleSimulatorV2.UnitSystem
{
    public static class UnitManager
    {
        private static List<UnitController> listUnit = new List<UnitController>();

        public static int UnitCount => listUnit.Count;
        public static List<UnitController> ListUnit => listUnit;

        
        
        
        public static void RegisterUnit(UnitController unitController)
        {
            listUnit.Add(unitController);
        }
        
        public static void UnRegisterUnit(UnitController unitController)
        {
            listUnit.Remove(unitController);
        }

        /// <summary>
        /// Lấy các unit có teamId không cùng với teamId truyền vào
        /// </summary>
        public static List<UnitController> GetUnitsAlive_NotSame_TeamId(int teamId)
        {
            return listUnit.Where(u => u.TeamId != teamId && u.UnitTrait.IsDead == false && u.gameObject.activeInHierarchy).ToList();
        }
    }
}