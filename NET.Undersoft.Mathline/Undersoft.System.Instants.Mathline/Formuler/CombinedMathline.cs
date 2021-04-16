
namespace System.Instants.Mathline
{
	/// Class for the Generated Code
	public abstract class CombinedReckoner
    { 
        public int ParametersCount = 0;

        public IMultemic[] DataParameters = new IMultemic[1];

        public abstract void Reckon();

        public void SetParams(IMultemic[] p, int paramCount)
        {
            DataParameters = p;
            ParametersCount = paramCount;
        }
        public bool SetParams(IMultemic p, int index)
        {
            if (index < ParametersCount)
            {
                if (ReferenceEquals(DataParameters[index], p))
                    return false;
                else
                    DataParameters[index] = p;
            }
            return false;
        }
        public void SetParams(IMultemic p)
        {
            Put(p);
        }

        public int Put(IMultemic v)
        {
            int index = GetIndexOf(v);
            if (index < 0)
            {
                DataParameters[ParametersCount] = v;
                return 1 + ParametersCount++;
            }
            else
            {
                DataParameters[index] = v;
            }
            return index;
        }

        public int GetIndexOf(IMultemic v)
        {
            for (int i = 0; i < ParametersCount; i++)
                if (DataParameters[i] == v) return 1 + i;
            return -1;
        }
    
        public int GetRowCount(int paramid)
        {            
            return DataParameters[paramid].Count;
        }

        public int GetColumnCount(int paramid)
        {
            return DataParameters[paramid].Rubrics.Count;
        }

    }
}
