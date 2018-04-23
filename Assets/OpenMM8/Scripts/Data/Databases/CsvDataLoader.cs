using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CsvDataLoader
{
    static public bool LoadRows(string csvPath, System.Func<int, string[], bool> rowProcessor, char csvDelim = ';')
    {
        using (StreamReader reader = new StreamReader(csvPath))
        {
            if (reader == null || reader.Peek() == -1)
            {
                return false;
            }

            int rowNum = 1;
            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                string[] columns = line.Split(csvDelim);
                rowProcessor(rowNum, columns);
                rowNum++;
            }
        }

        return true;
    }
}
