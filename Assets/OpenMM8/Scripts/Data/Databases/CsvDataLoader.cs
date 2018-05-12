using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

using LINQtoCSV;

internal class MyDataRow : List<DataRowItem>, IDataRow
{
}

public class CsvDataLoader
{
    static public bool LoadRows<T>(string csvPath, System.Func<int, string[], bool> rowProcessor, int headerRow = 1, char csvDelim = '\t')
    {
        CsvFileDescription inputFileDescription = new CsvFileDescription
        {
            FirstLineHasColumnNames = false,
            SeparatorChar = csvDelim,
            LinesToSkip = headerRow - 1
        };
        CsvContext cc = new CsvContext();

        IEnumerable<MyDataRow> pr = cc.Read<MyDataRow>(csvPath, inputFileDescription);
        
        int rowNum = 0;
        int numCols = 0;
        string[] arr = null;
        foreach (MyDataRow r in pr)
        {
            // This is the header - depending on its number of columns
            // we parse the data from next rows
            if (rowNum == 0)
            {
                numCols = r.Count;
                arr = new string[numCols];
            }

            int i = 0;
            foreach (var item in r)
            {
                if (i >= numCols)
                {
                    break;
                }

                arr[i] = r[i].Value;
                i++;
            }

            rowProcessor(rowNum, arr);
            rowNum++;
        }

        return true;
    }
}
