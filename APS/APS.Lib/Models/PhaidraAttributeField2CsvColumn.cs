using System.Collections.Generic;

namespace APS.Lib
{
    public class PhaidraAttributeField2CsvColumn : PropertyChangedBase
    {
        private KeyValuePair<string, string> _field;
        private CsvColumn _selectedCsvColumn;

        public KeyValuePair<string, string> Field
        {
            get
            {
                return _field;
            }
            set
            {
                _field = value;
                RaisePropertyChanged(nameof(Field));
            }
        }
        public CsvColumn SelectedCsvColumn
        {
            get
            {
                return _selectedCsvColumn;
            }
            set
            {
                if(value == null)
                {
                    return;
                }
                _selectedCsvColumn = value;
                RaisePropertyChanged(nameof(SelectedCsvColumn));
            }
        }

        public void RaisePropertyChanged()
        {
            RaisePropertyChanged(nameof(Field));
            RaisePropertyChanged(nameof(SelectedCsvColumn));
        }
    }
}