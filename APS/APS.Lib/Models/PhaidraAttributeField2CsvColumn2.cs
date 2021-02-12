using System.Collections.Generic;

namespace APS.Lib
{
    public class PhaidraAttributeField2CsvColumn2 : PropertyChangedBase
    {
        private PhaidraAttributeField _field;
        private CsvColumn _csvColumn;

        public PhaidraAttributeField Field
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
        public CsvColumn CsvColumn
        {
            get
            {
                return _csvColumn;
            }
            set
            {
                if(value == null)
                {
                    return;
                }
                _csvColumn = value;
                RaisePropertyChanged(nameof(CsvColumn));
            }
        }

        public void RaisePropertyChanged()
        {
            RaisePropertyChanged(nameof(Field));
            RaisePropertyChanged(nameof(CsvColumn));
        }
    }
}