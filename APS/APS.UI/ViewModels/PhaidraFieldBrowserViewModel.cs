using APS.Lib;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Input;
using System.Linq;

namespace APS.UI
{
    public class PhaidraFieldBrowserViewModel : VMBase
    {
        private string _filterText;

        public string FilterText
        {
            get { return _filterText; }
            set
            {
                _filterText = value;
                RaisePropertyChanged(nameof(FilterText));
                FilterFields();
            }
        }

        private bool _isSystemOnly;

        public bool IsSystemOnly
        {
            get { return _isSystemOnly; }
            set
            {
                _isSystemOnly = value;
                RaisePropertyChanged(nameof(IsSystemOnly));
                FilterFields();
            }
        }

        private void FilterFields()
        {
            Fields.Clear();

            foreach (var field in _phaidraAttributeFields)
            {
                if (FilterText == "" || field.DisplayName.ToLower().Contains(FilterText.ToLower()))
                {
                    if (IsSystemOnly == false || IsSystemOnly == true && field.PhaidraAttribute?.IsSystem == true)
                    {
                        Fields.Add(field);
                    }
                }
            }
        }

        public ObservableCollection<PhaidraAttributeField> Fields { get; set; }

        private PhaidraAttributeField _selectedField;
        private ObservableCollection<PhaidraAttributeField> _phaidraAttributeFields;

        public PhaidraAttributeField SelectedField
        {
            get { return _selectedField; }
            set
            {
                _selectedField = value;
                RaisePropertyChanged(nameof(SelectedField));
            }
        }

        public string CsvColumnName => $"'{_phaidraAttributeField2CsvColumn2?.CsvColumn?.Name}'";

        public Command SelectCommand { get; set; }
        public Command ClearCommand { get; set; }
        public Command DuplicatePhaidraAttributeCommand { get; set; }

        public Action CloseAction { get; internal set; }

        private PhaidraAttributeField2CsvColumn2 _phaidraAttributeField2CsvColumn2;

        public PhaidraFieldBrowserViewModel(PhaidraAttributeField2CsvColumn2 phaidraAttributeField2CsvColumn2, ObservableCollection<PhaidraAttributeField> phaidraAttributeFields)
        {
            SelectCommand = new Command(SelectExecute, SelectCanExecute);
            ClearCommand = new Command(ClearExecute, ClearCanExecute);
            DuplicatePhaidraAttributeCommand = new Command(DuplicatePhaidraAttributeExecute);
            Fields = new ObservableCollection<PhaidraAttributeField>();
            _phaidraAttributeField2CsvColumn2 = phaidraAttributeField2CsvColumn2;
            _phaidraAttributeFields = phaidraAttributeFields;
            FilterText = "";
            SelectedField = _phaidraAttributeField2CsvColumn2.Field;
        }

        private void SelectExecute(object obj)
        {
            if (SelectedField != null)
            {
                _phaidraAttributeField2CsvColumn2.Field = SelectedField;
                CloseAction();
            }
        }

        private bool SelectCanExecute(object arg)
        {
            return true;
        }

        private void ClearExecute(object obj)
        {
            _phaidraAttributeField2CsvColumn2.Field = PhaidraAttributeField.Empty;
            CloseAction();
        }

        private bool ClearCanExecute(object arg)
        {
            return true;
        }

        private void DuplicatePhaidraAttributeExecute(object obj)
        {
            if (SelectedField != null)
            {
                var maxIndex = _phaidraAttributeFields.Where(i => i.PhaidraAttribute == SelectedField.PhaidraAttribute).Max(i => i.Set.Index);
                var set = new PhaidraMetadataSet(maxIndex + 1);

                int foundIndexForInsertion = -1;

                for (int i = _phaidraAttributeFields.Count - 1; i >= 0; i--)
                {
                    if (_phaidraAttributeFields[i].PhaidraAttribute == SelectedField.PhaidraAttribute)
                    {
                        foundIndexForInsertion = i;
                        break;
                    }
                }

                foreach (var field in SelectedField.PhaidraAttribute.Fields)
                {
                    var attrField = new PhaidraAttributeField()
                    {
                        PhaidraAttribute = SelectedField.PhaidraAttribute,
                        Field = new KeyValuePair<string, string>(field.Key, field.Value),
                        Set = set
                    };
                    if (foundIndexForInsertion > -1)
                    {
                        _phaidraAttributeFields.Insert(foundIndexForInsertion + 1, attrField);
                        foundIndexForInsertion++;
                    }
                    else
                    {
                        _phaidraAttributeFields.Add(attrField);
                    }
                }
                FilterFields();
            }
        }
    }
}
