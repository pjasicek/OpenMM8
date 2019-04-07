using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.OpenMM8.Scripts
{
    public abstract class DataDb<T, KeyType> where T : DbData<KeyType>
    {
        public Dictionary<KeyType, T> Data = new Dictionary<KeyType, T>();

        public bool Initialize(string csvFile, int headerRow = 1, char csvDelim = '\t')
        {
            bool ret = CsvDataLoader.LoadRows<T>(csvFile, _ProcessCsvDataRow, headerRow, csvDelim);
            Finalize();
            return ret;
        }

        private bool _ProcessCsvDataRow(int row, string[] columns)
        {
            try
            {
                T data = ProcessCsvDataRow(row, columns);
                if (data != null)
                {
                    Data.Add(data.Id, data);
                }
            }
            catch (Exception e)
            {
                Logger.LogError("Error parsing: " + columns.ToString() + ", Exception: " + e.Message);
            }

            return true;
        }

        abstract public T ProcessCsvDataRow(int row, string[] columns);

        protected virtual void Finalize() { }

        public T Get(KeyType id)
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

        // https://stackoverflow.com/questions/667802/what-is-the-algorithm-to-convert-an-excel-column-letter-into-its-number
        protected int ColumnToNumber(string columnName)
        {
            if (string.IsNullOrEmpty(columnName)) throw new ArgumentNullException("columnName");

            columnName = columnName.ToUpperInvariant();

            int sum = 0;

            for (int i = 0; i < columnName.Length; i++)
            {
                sum *= 26;
                sum += (columnName[i] - 'A' + 1);
            }

            return sum;
        }
    }

    public abstract class DataDb<TValue> : DataDb<TValue, int> where TValue : DbData<int> { }
}
