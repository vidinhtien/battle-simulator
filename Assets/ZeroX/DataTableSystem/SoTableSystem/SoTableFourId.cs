using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ZeroX.DataTableSystem.SoTableSystem
{
    public abstract class SoTableFourId<TTable, TRow, TId1, TId2, TId3, TId4> : SoTable<TTable, TRow>
        where TTable : SoTableFourId<TTable, TRow, TId1, TId2, TId3, TId4>, new()
        where TRow : new()
    {
        
        
        
        //Fields
        protected Dictionary<TId1, Dictionary<TId2, Dictionary<TId3, Dictionary<TId4, TRow>>>> dictRow;
        
        
        
        //Non Public
        protected abstract void GetRowId(TRow row, out TId1 id1, out TId2 id2, out TId3 id3, out TId4 id4);
        
        private void GenerateDictRowFromListRow()
        {
            if (dictRow == null)
                dictRow = new Dictionary<TId1, Dictionary<TId2, Dictionary<TId3, Dictionary<TId4, TRow>>>>();
            else
                dictRow.Clear();
            
            
            
            foreach (var row in RowsI)
            {
                GetRowId(row, out TId1 id1, out TId2 id2, out TId3 id3, out TId4 id4);
                
                if (dictRow.TryGetValue(id1, out var dict2) == false) //Nếu trong dict chưa có id1
                {
                    dict2 = new Dictionary<TId2, Dictionary<TId3, Dictionary<TId4, TRow>>>();
                    dictRow.Add(id1, dict2);
                }

                if (dict2.TryGetValue(id2, out var dict3) == false)
                {
                    dict3 = new Dictionary<TId3, Dictionary<TId4, TRow>>();
                    dict2.Add(id2, dict3);
                }
                
                if (dict3.TryGetValue(id3, out var dict4) == false)
                {
                    dict4 = new Dictionary<TId4, TRow>();
                    dict3.Add(id3, dict4);
                }
                
                dict4.Add(id4, row);
            }
        }
        
        
        
        //Public
        public Dictionary<TId1, Dictionary<TId2, Dictionary<TId3, Dictionary<TId4, TRow>>>> DictRowI
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
        public Dictionary<TId2, Dictionary<TId3, Dictionary<TId4, TRow>>> GetDictRowByIdI(TId1 id1)
        {
            if (DictRowI.TryGetValue(id1, out var dict2) == false)
                return null;

            return dict2;
        }
        
        public Dictionary<TId2, Dictionary<TId3, Dictionary<TId4, TRow>>> GetDictRowByIdWithLogI(TId1 id1)
        {
            if (DictRowI.TryGetValue(id1, out var dict2) == false)
            {
                Debug.LogError("Id1 not exist: " + id1);
                return null;
            }

            return dict2;
        }
        
        public Dictionary<TId3, Dictionary<TId4, TRow>> GetDictRowByIdI(TId1 id1, TId2 id2)
        {
            if (DictRowI.TryGetValue(id1, out var dict2) == false)
                return null;

            if (dict2.TryGetValue(id2, out var dict3) == false)
                return null;

            return dict3;
        }
        
        public Dictionary<TId3, Dictionary<TId4, TRow>> GetDictRowByIdWithLogI(TId1 id1, TId2 id2)
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
        
        public Dictionary<TId4, TRow> GetDictRowByIdI(TId1 id1, TId2 id2, TId3 id3)
        {
            if (DictRowI.TryGetValue(id1, out var dict2) == false)
                return null;

            if (dict2.TryGetValue(id2, out var dict3) == false)
                return null;

            if (dict3.TryGetValue(id3, out var dict4) == false)
                return null;

            return dict4;
        }
        
        public Dictionary<TId4, TRow> GetDictRowByIdWithLogI(TId1 id1, TId2 id2, TId3 id3)
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
            
            if (dict3.TryGetValue(id3, out var dict4) == false)
            {
                Debug.LogError("Id3 not exist: " + id2);
                return null;
            }

            return dict4;
        }
        
        public List<TRow> GetListRowByIdI(TId1 id1)
        {
            var dict2 = GetDictRowByIdI(id1);
            if(dict2 == null)
                return new List<TRow>();

            List<TRow> listRow = new List<TRow>();
            foreach (var dict3 in dict2.Values)
            {
                foreach (var dict4 in dict3.Values)
                {
                    listRow.AddRange(dict4.Values.ToList());
                }
            }
            
            return listRow;
        }
        
        public List<TRow> GetListRowByIdI(TId1 id1, TId2 id2)
        {
            var dict3 = GetDictRowByIdI(id1, id2);
            if(dict3 == null)
                return new List<TRow>();

            List<TRow> listRow = new List<TRow>();
            foreach (var dict4 in dict3.Values)
            {
                listRow.AddRange(dict4.Values.ToList());
            }
            
            return listRow;
        }
        
        public List<TRow> GetListRowByIdI(TId1 id1, TId2 id2, TId3 id3)
        {
            var dict4 = GetDictRowByIdI(id1, id2, id3);
            if(dict4 == null)
                return new List<TRow>();

            List<TRow> listRow = dict4.Values.ToList();
            return listRow;
        }
        
        
        
        
        
        
        
        //Static
        public Dictionary<TId1, Dictionary<TId2, Dictionary<TId3, Dictionary<TId4, TRow>>>> DictRow => Main.DictRowI;
        
        
        
        //Get
        public static Dictionary<TId2, Dictionary<TId3, Dictionary<TId4, TRow>>> GetDictRowById(TId1 id1)
        {
            if (Main.DictRowI.TryGetValue(id1, out var dict2) == false)
                return null;

            return dict2;
        }
        
        public static Dictionary<TId2, Dictionary<TId3, Dictionary<TId4, TRow>>> GetDictRowByIdWithLog(TId1 id1)
        {
            if (Main.DictRowI.TryGetValue(id1, out var dict2) == false)
            {
                Debug.LogError("Id1 not exist: " + id1);
                return null;
            }

            return dict2;
        }
        
        public static Dictionary<TId3, Dictionary<TId4, TRow>> GetDictRowById(TId1 id1, TId2 id2)
        {
            if (Main.DictRowI.TryGetValue(id1, out var dict2) == false)
                return null;

            if (dict2.TryGetValue(id2, out var dict3) == false)
                return null;

            return dict3;
        }
        
        public static Dictionary<TId3, Dictionary<TId4, TRow>> GetDictRowByIdWithLog(TId1 id1, TId2 id2)
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
        
        public static Dictionary<TId4, TRow> GetDictRowById(TId1 id1, TId2 id2, TId3 id3)
        {
            if (Main.DictRowI.TryGetValue(id1, out var dict2) == false)
                return null;

            if (dict2.TryGetValue(id2, out var dict3) == false)
                return null;

            if (dict3.TryGetValue(id3, out var dict4) == false)
                return null;

            return dict4;
        }
        
        public static Dictionary<TId4, TRow> GetDictRowByIdWithLog(TId1 id1, TId2 id2, TId3 id3)
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
            
            if (dict3.TryGetValue(id3, out var dict4) == false)
            {
                Debug.LogError("Id3 not exist: " + id2);
                return null;
            }

            return dict4;
        }
        
        public static List<TRow> GetListRowById(TId1 id1)
        {
            var dict2 = Main.GetDictRowByIdI(id1);
            if(dict2 == null)
                return new List<TRow>();

            List<TRow> listRow = new List<TRow>();
            foreach (var dict3 in dict2.Values)
            {
                foreach (var dict4 in dict3.Values)
                {
                    listRow.AddRange(dict4.Values.ToList());
                }
            }
            
            return listRow;
        }
        
        public static List<TRow> GetListRowById(TId1 id1, TId2 id2)
        {
            var dict3 = Main.GetDictRowByIdI(id1, id2);
            if(dict3 == null)
                return new List<TRow>();

            List<TRow> listRow = new List<TRow>();
            foreach (var dict4 in dict3.Values)
            {
                listRow.AddRange(dict4.Values.ToList());
            }
            
            return listRow;
        }
        
        public static List<TRow> GetListRowById(TId1 id1, TId2 id2, TId3 id3)
        {
            var dict4 = Main.GetDictRowByIdI(id1, id2, id3);
            if(dict4 == null)
                return new List<TRow>();

            List<TRow> listRow = dict4.Values.ToList();
            return listRow;
        }
    }
}