using System;
using System.Collections.Generic;
using System.Text;

namespace APS.Lib
{
    public class PhaidraAttributeField
    {
        public string DisplayName
        {
            get
            {
                if (PhaidraAttribute != null)
                {
                    return $"{PhaidraAttribute.Name}{Set}->{Field.Value}";
                }
                return "-";
            }
        }
        public PhaidraAttribute PhaidraAttribute { get; set; }
        public KeyValuePair<string, string> Field { get; set; }
        public PhaidraMetadataSet Set { get; set; }

        public static PhaidraAttributeField Empty;

        static PhaidraAttributeField()
        {
            Empty = new PhaidraAttributeField();
        }

        public override string ToString()
        {
            return DisplayName;
        }
    }
}
