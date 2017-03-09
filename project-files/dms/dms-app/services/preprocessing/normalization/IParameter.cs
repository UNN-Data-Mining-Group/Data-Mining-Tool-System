using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dms.services.preprocessing.normalization
{
    interface IParameter
    {
        string Type { get; }
        int CountNumbers { get; set; }

        float GetNormalizedFloat(string value);
        int GetNormalizedInt(string value);
    }
}
