using System.Collections.Generic;
using UnityEngine;

namespace ZeroX.DataTableSystem.SoTableSystem
{
    public abstract class SoTableOneId<TTable, TRow, TId> : SoTable<TTable, TRow>
        where TTable : SoTableOneId<TTable, TRow, TId>, new()
        where TRow : new()
    {
        
        
        
        //Fields
        private Dictionary<TId, TRow> dictRow;
        
        
        
        //Non Public
        protected abstract TId GetRowId(TRow row);
        
        private void GenerateDictRowFromListRow()
        {
            if (dictRow == null)
                dictRow = new Dictionary<TId, TRow>();
            else
                dictRow.Clear();

            
            
            foreach (var row in RowsI)
            {
                dictRow.Add(GetRowId(row), row);
            }
        }
        
        
        
        //Public
        public Dictionary<TId, TRow> DictRowI
        {
            get
            {
                if(dictRow == null)
                    GenerateDictRowFromListRow();

                return dictRow;
            }
        }
        
        public override void UpdateAfterListRowChangeI()
        {
            GenerateDictRowFromListRow();
        }
        
        
        
        //Public - Get
        public TRow GetRowByIdI(TId id)
        {
            DictRowI.TryGetValue(id, out TRow row);
            return row;
        }
        
        public TRow GetRowByIdWithLogI(TId id)
        {
            if (DictRowI.TryGetValue(id, out TRow row))
                return row;
            else
            {
                Debug.LogError("Cannot find row with id: " + id);
                return default;
            }
        }
        
        
        
        
        
        
        
        
        
        //Static------------------------------------------------------------------------------
        public static Dictionary<TId, TRow> DictRow => Main.DictRowI;
        
        public static TRow GetRowById(TId id)
        {
            Main.DictRowI.TryGetValue(id, out TRow row);
            return row;
        }
        
        public static TRow GetRowByIdWithLog(TId id)
        {
            if (Main.DictRowI.TryGetValue(id, out TRow row))
                return row;
            else
            {
                Debug.LogError("Cannot find row with id: " + id);
                return default;
            }
        }
    }
}