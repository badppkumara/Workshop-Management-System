using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UILAB.Models;

namespace UILAB.Interface
{
    public interface IChart
    {
        void ProductWiseSales(out string MobileCountList, out string ProductList);
    }
}