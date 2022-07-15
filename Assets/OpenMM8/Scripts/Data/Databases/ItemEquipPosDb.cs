using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Assets.OpenMM8.Scripts.Gameplay.Items;
using Assets.OpenMM8.Scripts.Data;

namespace Assets.OpenMM8.Scripts.Gameplay.Data
{
    public class ItemEquipPosDb : DataDb<ItemEquipPosData>
    {
        override public ItemEquipPosData ProcessCsvDataRow(int row, string[] columns)
        {
            if (row < 2)
            {
                return null;
            }

            if (columns[0].Trim().Equals(""))
            {
                return null;
            }

            ItemEquipPosData data = new ItemEquipPosData();
            data.Id = int.Parse(columns[1]);
            data.Name = columns[2];
            data.MaleItemPos.Set(int.Parse(columns[4]), -1 * int.Parse(columns[5]));
            data.FemaleItemPos.Set(int.Parse(columns[6]), -1 * int.Parse(columns[7]));
            data.MinotaurItemPos.Set(int.Parse(columns[8]), -1 * int.Parse(columns[9]));
            data.TrollItemPos.Set(int.Parse(columns[10]), -1 * int.Parse(columns[11]));

            //Logger.LogDebug("ItemEqPos ID: " + data.Id);

            return data;
        }
    }
}
