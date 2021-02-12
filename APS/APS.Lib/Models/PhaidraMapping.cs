using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace APS.Lib
{
    public class PhaidraMapping : PropertyChangedBase
    {
        private bool _isValid;

        public PhaidraAttribute PhaidraAttribute { get; set; }

        public ObservableCollection<PhaidraAttributeField2CsvColumn> PhaidraAttributeFields2CsvColumns { get; set; }

        public bool IsValid
        {
            get => _isValid; 
            set
            {
                _isValid = value;
                RaisePropertyChanged(nameof(IsValid));
                RaisePropertyChanged(nameof(IsValidDisplayName));
            }
        }

        public string IsValidDisplayName
        {
            get
            {
                return IsValid ? "" : "*";
            }
        }

        public PhaidraMapping()
        {
            IsValid = true;
            PhaidraAttributeFields2CsvColumns = new ObservableCollection<PhaidraAttributeField2CsvColumn>();
        }

        public override string ToString()
        {
            return $"{PhaidraAttribute?.Name} ({PhaidraAttribute?.Fields.Count}){(IsValid ? "" : "*")}";
        }
    }
}
