using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ZeroX.DataTableSystem.SoTableSystem
{
    public abstract class SoTableTwoId<TTable, TRow, TId1, TId2> : SoTable<TTable, TRow>
        where TTable : SoTableTwoId<TTable, TRow, TId1, TId2>, new()
        where TRow : new()
    {
        
        
        
        //Fields
        protected Dictionary<TId1, Dictionary<TId2, TRow>> dictRow;
        
        
        
        //Non Public
        protected abstract void GetRowId(TRow row, out TId1 id1, out TId2 id2);
        
        private void GenerateDictRowFromListRow()
        {
            if (dictRow == null)
                dictRow = new Dictionary<TId1, Dictionary<TId2, TRow>>();
            else
                dictRow.Clear();

            
            
            foreach (var row in RowsI)
            {
                GetRowId(row, out TId1 id1, out TId2 id2);
                
                if (dictRow.TryGetValue(id1, out var dictRow2) == false)
                {
                    dictRow2 = new Dictionary<TId2, TRow>();
                    dictRow.Add(id1, dictRow2);
                }
                
                dictRow2.Add(id2, row);
            }
        }
        
        
        
        //Public
        public Dictionary<TId1, Dictionary<TId2, TRow>> DictRowI
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
        public TRow GetRowByIdI(TId1 id1, TId2 id2)
        {
            if (DictRowI.TryGetValue(id1, out var dict2) == false)
                return default;

            if (dict2.TryGetValue(id2, out TRow row) == false)
                return default;

            return row;
        }
        
        public TRow GetRowByIdWithLogI(TId1 id1, TId2 id2)
        {
            if (DictRowI.TryGetValue(id1, out var dict2) == false)
            {
                Debug.LogError("Id1 not exist: " + id1);
                return default;
            }

            if (dict2.TryGetValue(id2, out TRow row) == false)
            {
                Debug.LogError("Id2 not exist: " + id2);
                return default;
            }
            
            return row;
        }
        
        public Dictionary<TId2, TRow> GetDictRowByIdI(TId1 id1)
        {
            if (DictRowI.TryGetValue(id1, out var dict) == false)
                return null;

            return dict;
        }
        
        public Dictionary<TId2, TRow> GetDictRowByIdWithLogI(TId1 id1)
        {
            if (DictRowI.TryGetValue(id1, out var dict) == false)
            {
                Debug.LogError("Id1 not exist: " + id1);
                return null;
            }

            return dict;
        }
        
        public List<TRow> GetListRowByIdI(TId1 id1)
        {
            var dict2 = GetDictRowByIdI(id1);
            if(dict2 == null)
                return null;

            return dict2.Values.ToList();
        }
        
        public List<TRow> GetListRowByIdWithLogI(TId1 id1)
        {
            var dict2 = GetDictRowByIdI(id1);
            if (dict2 == null)
            {
                Debug.LogError("Id1 not exist: " + id1);
                return null;
            }

            return dict2.Values.ToList();
        }
        
        
        
        
        
        
        
        
        
        
        //Static
        public static Dictionary<TId1, Dictionary<TId2, TRow>> DictRow => Main.DictRowI;
        
        
        
        //Get
        public static TRow GetRowById(TId1 id1, TId2 id2)
        {
            if (Main.DictRowI.TryGetValue(id1, out var dict2) == false)
                return default;

            if (dict2.TryGetValue(id2, out TRow row) == false)
                return default;

            return row;
        }
        
        public static TRow GetRowByIdWithLog(TId1 id1, TId2 id2)
        {
            if (Main.DictRowI.TryGetValue(id1, out var dict2) == false)
            {
                Debug.LogError("Id1 not exist: " + id1);
                return default;
            }

            if (dict2.TryGetValue(id2, out TRow row) == false)
            {
                Debug.LogError("Id2 not exist: " + id2);
                return default;
            }
            
            return row;
        }
        
        public static Dictionary<TId2, TRow> GetDictRowById(TId1 id1)
        {
            if (Main.DictRowI.TryGetValue(id1, out var dict) == false)
                return null;

            return dict;
        }
        
        public static Dictionary<TId2, TRow> GetDictRowByIdWithLog(TId1 id1)
        {
            if (Main.DictRowI.TryGetValue(id1, out var dict) == false)
            {
                Debug.LogError("Id1 not exist: " + id1);
                return null;
            }

            return dict;
        }
        
        public static List<TRow> GetListRowById(TId1 id1)
        {
            var dict2 = Main.GetDictRowByIdI(id1);
            if(dict2 == null)
                return null;

            return dict2.Values.ToList();
        }
        
        public static List<TRow> GetListRowByIdWithLog(TId1 id1)
        {
            var dict2 = Main.GetDictRowByIdI(id1);
            if (dict2 == null)
            {
                Debug.LogError("Id1 not exist: " + id1);
                return null;
            }

            return dict2.Values.ToList();
        }
    }
}