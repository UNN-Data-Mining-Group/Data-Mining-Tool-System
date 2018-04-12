using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.Serialization;

namespace dms.services.preprocessing.normalization
{
    public interface IParameter
    {
        string Type { get; }
        int CountNumbers { get; set; }

        float GetLinearNormalizedFloat(string value);
        float GetNonlinearNormalizedFloat(string value);
        int GetNormalizedInt(string value);
        
        string GetFromNormalized(int value);
        string GetFromLinearNormalized(float value);
        string GetFromNonlinearNormalized(float value);

        void setRange(float left, float right);
        void setParam(float a);
    }
}