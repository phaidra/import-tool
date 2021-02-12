using APS.Lib;
using System;
using System.Collections.Generic;
using System.Text;

namespace APS.UI.Helpers
{
    public class PhaidraMetadataGenerator
    {
        private List<PhaidraMapping> _mappings;

        public PhaidraMetadataGenerator(List<PhaidraMapping> mappings)
        {
            this._mappings = mappings;
        }

        public void SetMetadata(Dictionary<int, string> row, PhaidraFile file)
        {
            foreach (PhaidraMapping mapping in _mappings)
            {
                var values = new Dictionary<string, string>();
                foreach (var col in mapping.PhaidraAttributeFields2CsvColumns)
                {
                    if (col.SelectedCsvColumn != CsvColumn.Empty)
                    {
                        values[col.Field.Key] = row[col.SelectedCsvColumn.Header.Key];
                    }
                }
                mapping.PhaidraAttribute.WriteAttributeContent(file, values);
            }
        }

        public void WriteAttributeContentToCSV(PhaidraFile file)
        {
            foreach (PhaidraMapping mapping in _mappings)
            {
                var values = new Dictionary<string, string>();
                mapping.PhaidraAttribute.WriteAttributeContentToCSV(file, values);
                foreach (var col in mapping.PhaidraAttributeFields2CsvColumns)
                {
                    if (col.SelectedCsvColumn != CsvColumn.Empty)
                    {
                        if (values.TryGetValue(col.Field.Key, out string value))
                        {
                            int colIndex = col.SelectedCsvColumn.Header.Key;
                            file.CsvRow[colIndex] = value;
                        }
                    }
                }
            }
        }
    }
}
