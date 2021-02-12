using System;
using System.Collections.Generic;
using System.Text;

namespace APS.Lib
{
    public class PhaidraIdPhaidraAttribute : PhaidraAttribute
    {
        private const string CONST_phaidraid = "phaidraid";

        public override string Name => "Phaidra id";

        public override Dictionary<string, string> Fields
        {
            get
            {
                return new Dictionary<string, string>
                {
                    { CONST_phaidraid, "Phaidra id" }
                };
            }
        }

        public override void WriteAttributeContent(PhaidraFile file, Dictionary<string, string> values)
        {
            string phaidraId = GetStringValue(values, CONST_phaidraid);
            if (!string.IsNullOrEmpty(phaidraId))
            {
                file.PhaidraId = phaidraId;
            }
        }

        public override void WriteAttributeContentToCSV(PhaidraFile file, Dictionary<string, string> values)
        {
            values[CONST_phaidraid] = file.PhaidraId;
        }

        public override bool ValidateMappings(PhaidraMapping mapping)
        {
            bool hasPhaidraIdMapping = false;

            foreach (var m in mapping.PhaidraAttributeFields2CsvColumns)
            {
                switch (m.Field.Key)
                {
                    case CONST_phaidraid:
                        if (m.SelectedCsvColumn != CsvColumn.Empty)
                        {
                            hasPhaidraIdMapping = true;
                        }
                        break;
                }
            }

            mapping.IsValid = hasPhaidraIdMapping;
            return mapping.IsValid;
        }

        public override bool OnlyOneAllowed => true;

        public override bool IsRequired => true;

        public override bool IsSystem => true;
    }
}
