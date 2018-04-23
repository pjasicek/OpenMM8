using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.OpenMM8.Scripts.Data.Databases
{
    public abstract class DataDb
    {
        public bool Initialize(params string[] csvData)
        {
            foreach (string csv in csvData)
            {
                if (!CsvDataLoader.LoadRows(csv, ProcessCsvDataRow))
                {
                    return false;
                }
            }

            return true;
        }

        abstract public bool ProcessCsvDataRow(int row, string[] columns);
    }
}
