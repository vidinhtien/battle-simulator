using UnityEngine;
using ZeroX.DataTableSystem.SoTableSystem;

namespace ZeroX.DataTableSystem.Demo
{
    [CreateAssetMenu(menuName = "ZeroX/Data Table System/Demo/Test Table")]
    public class TestTable : SoTableOneId<TestTable, TestRow, string>
    {
        protected override string MainPath => "ZeroX/DataTableSystem Demo/Test";

        protected override string GetRowId(TestRow row)
        {
            return row.id;
        }
    }
}