using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.OpenMM8.Scripts
{
    public abstract class DataDb<T> where T : DbData
    {
        public Dictionary<int, T> Data = new Dictionary<int, T>();

        public bool Initialize(string csvFile, int headerRow = 1, char csvDelim = '\t')
        {
            return CsvDataLoader.LoadRows<T>(csvFile, _ProcessCsvDataRow, headerRow, csvDelim);
        }

        private bool _ProcessCsvDataRow(int row, string[] columns)
        {
            T data = ProcessCsvDataRow(row, columns);
            if (data != null)
            {
                Data.Add(data.Id, data);
            }

            return true;
        }

        abstract public T ProcessCsvDataRow(int row, string[] columns);

        public T Get(int id)
        {
            if (Data.ContainsKey(id))
            {
                return Data[id];
            }
            else
            {
                Logger.LogError(this.GetType().Name + ": Failed data for id: " + id);
            }

            return default(T);
        }
    }
}
