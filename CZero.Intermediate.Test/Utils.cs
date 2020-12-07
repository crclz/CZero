using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CZero.Intermediate.Test
{
    static class Utils
    {
        public static List<TEnum> GetEnumList<TEnum>() where TEnum : Enum
            => ((TEnum[])Enum.GetValues(typeof(TEnum))).ToList();
    }
}
