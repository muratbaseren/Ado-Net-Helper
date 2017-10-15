using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
