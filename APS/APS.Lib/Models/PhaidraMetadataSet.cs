using System;
using System.Collections.Generic;
using System.Text;

namespace APS.Lib
{
    public class PhaidraMetadataSet
    {
        public PhaidraMetadataSet(int index = 0)
        {
            Index = index;
        }

        public int Index { get; }

        public override string ToString()
        {
            return Index > 0 ? $"[{Index}]" : "";
        }
    }
}
