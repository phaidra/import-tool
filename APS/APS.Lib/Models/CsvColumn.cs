using System;
using System.Collections.Generic;
using System.Text;

namespace APS.Lib
{
    public class CsvColumn : PropertyChangedBase
    {
        public static CsvColumn Empty;
        public string Name { get; set; }
        public KeyValuePair<int, string> Header { get; set; }

        private string _value;

        public string Value
        {
            get { return _value; }
            set
            {
                _value = value;
                RaisePropertyChanged(nameof(Value));
                RaisePropertyChanged(nameof(ValueTrunc));
            }
        }

        public string ValueTrunc => Value != null && Value.Length > 50 ? $"{Value.Substring(0, 50)}..." : Value;


        public void SetValue(Dictionary<int, string> row)
        {
            if (this != Empty && row != null)
            {
                if (row.TryGetValue(Header.Key, out string val))
                {
                    Value = val;
                    return;
                }
            }
            Value = null;
        }

        static CsvColumn()
        {
            Empty = new CsvColumn() { Name = "-" };
        }

        public override string ToString()
        {
            return $"{Name}";
        }
    }
}
