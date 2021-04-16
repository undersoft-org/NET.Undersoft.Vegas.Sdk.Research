using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Data;

namespace System.Instants.Mathline
{      
    public class Dataline
    {
        public int RowCount;
        public int RowOffset;

        public Dataline()
        {            
        }
        public Dataline(IMultemic table)
        {
            Data = table;
        }
        public Dataline(IMultemic table, int rowOffset, int rowCount)
        {
            RowCount = rowCount;
            RowOffset = rowOffset;
            Data = table;
        }       

        public double this[int rowid, int cellid]
        {
            get
            {
                return Convert.ToDouble(Data[rowid, cellid]);
            }
            set
            {
                Data[rowid, cellid] = value;
            }
        }

        public IMultemic Data;

        public bool Enabled = false;
        public int Count
        { get; set; }
    }
}
