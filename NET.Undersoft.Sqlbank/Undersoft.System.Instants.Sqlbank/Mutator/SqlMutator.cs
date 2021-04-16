using System.Multemic;

namespace System.Instants.Sqlbank
{
    public class SqlMutator
    {
        private InstantSql sqaf;

        public SqlMutator()
        {
        }
        public SqlMutator(InstantSql insql)
        {
            sqaf = insql;
        }

        public Album<Album<IFigure>> SetBank(string SqlConnectString, IMultemic cards, bool Renew)
        {
            try
            {
                if(sqaf == null)
                    sqaf = new InstantSql(SqlConnectString);

                try
                {
                    bool buildmap = true;
                    if (cards.Count > 0)
                    {                       
                        BulkPrepareType prepareType = BulkPrepareType.Drop;

                        if (Renew)
                            prepareType = BulkPrepareType.Trunc;

                        var ds = sqaf.Update(cards, Renew, buildmap, true, null, prepareType);
                        if (ds != null)
                        {
                            IMultemic im = (IMultemic)Summon.New(cards.GetType());
                            im.Rubrics = cards.Rubrics;
                            im.FigureType = cards.FigureType;
                            im.FigureSize = cards.FigureSize;
                            im.Add(ds["Failed"].AsValues());
                            return sqaf.Insert(im, Renew, false, prepareType);
                        }
                        else
                            return null;
                    }
                    return null;
                }
                catch (SqlException ex)
                {
                    throw ex;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }        
    }
}
