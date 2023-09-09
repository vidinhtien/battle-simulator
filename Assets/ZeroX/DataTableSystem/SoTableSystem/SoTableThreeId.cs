using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ZeroX.DataTableSystem.SoTableSystem
{
    public abstract class SoTableThreeId<TTable, TRow, TId1, TId2, TId3> : SoTable<TTable, TRow>
        where TTable : SoTableThreeId<TTable, TRow, TId1, TId2, TId3>, new()
        where TRow : new()
    {
        
        
        
        //Fields
        protected Dictionary<TId1, Dictionary<TId2, Dictionary<TId3, TRow>>> dictRow;
        
        
        
        //Non Public
        protected abstract void GetRowId(TRow row, out TId1 id1, out TId2 id2, out TId3 id3);

        private void GenerateDictRowFromListRow()
        {
            if (dictRow == null)
                dictRow = new Dictionary<TId1, Dictionary<TId2, Dictionary<TId3, TRow>>>();
            else
                dictRow.Clear();
            
            
            
            foreach (var row in RowsI)
            {
                GetRowId(row, out TId1 id1, out TId2 id2, out TId3 id3);
                
                if (dictRow.TryGetValue(id1, out var dict2) == false)
                {
                    dict2 = new Dictionary<TId2, Dictionary<TId3, TRow>>();
                    dictRow.Add(id1, dict2);
                }

                if (dict2.TryGetValue(id2, out var dict3) == false)
                {
                    dict3 = new Dictionary<TId3, TRow>();
                    dict2.Add(id2, dict3);
                }
                
                dict3.Add(id3, row);
            }
        }
        
        
        
        //Public
        public Dictionary<TId1, Dictionary<TId2, Dictionary<TId3, TRow>>> DictRowI
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
        public TRow GetRowByIdI(TId1 id1, TId2 id2, TId3 id3)
        {
            if (DictRowI.TryGetValue(id1, out var dict2) == false)
                return default;

            if (dict2.TryGetValue(id2, out var dict3) == false)
                return default;

            if (dict3.TryGetValue(id3, out TRow row) == false)
                return default;

            return row;
        }
        
        public TRow GetRowByIdWithLogI(TId1 id1, TId2 id2, TId3 id3)
        {
            if (DictRowI.TryGetValue(id1, out var dict2) == false)
            {
                Debug.LogError("Id1 not exist: " + id1);
                return default;
            }

            if (dict2.TryGetValue(id2, out var dict3) == false)
            {
                Debug.LogError("Id2 not exist: " + id2);
                return default;
            }

            if (dict3.TryGetValue(id3, out TRow row) == false)
            {
                Debug.LogError("Id3 not exist: " + id3);
                return default;
            }
            
            return row;
        }
        
        public Dictionary<TId2, Dictionary<TId3, TRow>> GetDictRowByIdI(TId1 id1)
        {
            if (DictRowI.TryGetValue(id1, out var dict2) == false)
                return null;

            return dict2;
        }
        
        public Dictionary<TId2, Dictionary<TId3, TRow>> GetDictRowByIdWithLogI(TId1 id1)
        {
            if (DictRowI.TryGetValue(id1, out var dict2) == false)
            {
                Debug.LogError("Id1 not exist: " + id1);
                return null;
            }

            return dict2;
        }
        
        public Dictionary<TId3, TRow> GetDictRowByIdI(TId1 id1, TId2 id2)
        {
            if (DictRowI.TryGetValue(id1, out var dict2) == false)
                return null;

            if (dict2.TryGetValue(id2, out var dict3) == false)
                return null;

            return dict3;
        }
        
        public Dictionary<TId3, TRow> GetDictRowByIdWithLogI(TId1 id1, TId2 id2)
        {
            if (DictRowI.TryGetValue(id1, out var dict2) == false)
            {
                Debug.LogError("Id1 not exist: " + id1);
                return null;
            }
            
            if (dict2.TryGetValue(id2, out var dict3) == false)
            {
                Debug.LogError("Id2 not exist: " + id2);
                return null;
            }

            return dict3;
        }
        
        public List<TRow> GetListRowByIdI(TId1 id1)
        {
            var dict2 = GetDictRowByIdI(id1);
            if(dict2 == null)
                return null;

            List<TRow> listRow = new List<TRow>();
            foreach (var dict3 in dict2.Values)
            {
                listRow.AddRange(dict3.Values.ToList());
            }
            
            return listRow;
        }
        
        public List<TRow> GetListRowByIdI(TId1 id1, TId2 id2)
        {
            var dict3 = GetDictRowByIdI(id1, id2);
            if(dict3 == null)
                return null;

            List<TRow> listRow = dict3.Values.ToList();
            return listRow;
        }
        
        
        
        
        
        
        
        
        
        
        
        //Static
        public Dictionary<TId1, Dictionary<TId2, Dictionary<TId3, TRow>>> DictRow => Main.DictRowI;
        
        
        
        //Get
        public static TRow GetRowById(TId1 id1, TId2 id2, TId3 id3)
        {
            if (Main.DictRowI.TryGetValue(id1, out var dict2) == false)
                return default;

            if (dict2.TryGetValue(id2, out var dict3) == false)
                return default;

            if (dict3.TryGetValue(id3, out TRow row) == false)
                return default;

            return row;
        }
        
        public static TRow GetRowByIdWithLog(TId1 id1, TId2 id2, TId3 id3)
        {
            if (Main.DictRowI.TryGetValue(id1, out var dict2) == false)
            {
                Debug.LogError("Id1 not exist: " + id1);
                return default;
            }

            if (dict2.TryGetValue(id2, out var dict3) == false)
            {
                Debug.LogError("Id2 not exist: " + id2);
                return default;
            }

            if (dict3.TryGetValue(id3, out TRow row) == false)
            {
                Debug.LogError("Id3 not exist: " + id3);
                return default;
            }
            
            return row;
        }
        
        public static Dictionary<TId2, Dictionary<TId3, TRow>> GetDictRowById(TId1 id1)
        {
            if (Main.DictRowI.TryGetValue(id1, out var dict2) == false)
                return null;

            return dict2;
        }
        
        public static Dictionary<TId2, Dictionary<TId3, TRow>> GetDictRowByIdWithLog(TId1 id1)
        {
            if (Main.DictRowI.TryGetValue(id1, out var dict2) == false)
            {
                Debug.LogError("Id1 not exist: " + id1);
                return null;
            }

            return dict2;
        }
        
        public static Dictionary<TId3, TRow> GetDictRowById(TId1 id1, TId2 id2)
        {
            if (Main.DictRowI.TryGetValue(id1, out var dict2) == false)
                return null;

            if (dict2.TryGetValue(id2, out var dict3) == false)
                return null;

            return dict3;
        }
        
        public static Dictionary<TId3, TRow> GetDictRowByIdWithLog(TId1 id1, TId2 id2)
        {
            if (Main.DictRowI.TryGetValue(id1, out var dict2) == false)
            {
                Debug.LogError("Id1 not exist: " + id1);
                return null;
            }
            
            if (dict2.TryGetValue(id2, out var dict3) == false)
            {
                Debug.LogError("Id2 not exist: " + id2);
                return null;
            }

            return dict3;
        }
        
        public static List<TRow> GetListRowById(TId1 id1)
        {
            var dict2 = Main.GetDictRowByIdI(id1);
            if(dict2 == null)
                return null;

            List<TRow> listRow = new List<TRow>();
            foreach (var dict3 in dict2.Values)
            {
                listRow.AddRange(dict3.Values.ToList());
            }
            
            return listRow;
        }
        
        public static List<TRow> GetListRowById(TId1 id1, TId2 id2)
        {
            var dict3 = Main.GetDictRowByIdI(id1, id2);
            if(dict3 == null)
                return null;

            List<TRow> listRow = dict3.Values.ToList();
            return listRow;
        }
    }
}