using System.Collections.Generic;

namespace AdoNetHelper.Abstract
{
    public interface IDbOperations<T, K>
    {
        int Insert();
        int Update();
        int Delete();
        List<T> SelectAll();
        T SelectById(K id);
    }
}
