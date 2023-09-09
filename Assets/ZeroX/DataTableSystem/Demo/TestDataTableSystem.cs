using System;
using UnityEngine;

namespace ZeroX.DataTableSystem.Demo
{
    public class TestDataTableSystem : MonoBehaviour
    {
        [SerializeField] private TestTable testTable;
        private void Start()
        {
            TestTable.SetMain(testTable);
            Debug.Log("Lan 1: " + TestTable.GetRowByIdWithLog("enemy_10"));
            TestTable.Rows.RemoveAt(0);
            Debug.Log("Lan 2: " + TestTable.GetRowByIdWithLog("enemy_10"));
            TestTable.UpdateAfterListRowChange();
            Debug.Log("Lan 3: " + TestTable.GetRowByIdWithLog("enemy_10"));
        }
    }
}