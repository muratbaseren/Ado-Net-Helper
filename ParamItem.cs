using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdoNetHelper
{
    public partial class ParamItem
    {
        public string ParamName { get; set; }
        public object ParamValue { get; set; }
    }

    public partial class ParamItem<T>
    {
        public string ParamName { get; set; }
        public T ParamValue { get; set; }
    }
}
