﻿using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CustomersTestApp.ViewModels
{
    public class CustomerEditorViewModel : BaseViewModel, IDataErrorInfo
    {
        private Customer _editingCustomer;

        public Customer EditingCustomer
        {
            get => _editingCustomer;
            set
            {
                _editingCustomer = value;
                OnPropertyChanged(nameof(EditingCustomer));
                OnPropertyChanged(nameof(Name));
                OnPropertyChanged(nameof(Email));
                OnPropertyChanged(nameof(Discount));
                OnPropertyChanged(nameof(Id));
            }
        }

        public string Name
        {
            get => EditingCustomer?.Name;
            set
            {
                if (EditingCustomer != null)
                {
                    EditingCustomer.Name = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Email
        {
            get => EditingCustomer?.Email;
            set
            {
                if (EditingCustomer != null)
                {
                    EditingCustomer.Email = value;
                    OnPropertyChanged();
                }
            }
        }

        public int? Discount
        {
            get => EditingCustomer?.Discount;
            set
            {
                if (EditingCustomer != null)
                {
                    EditingCustomer.Discount = value ?? 0;
                    OnPropertyChanged();
                }
            }
        }

        public Guid Id
        {
            get => EditingCustomer?.Id ?? Guid.Empty;
        }

        // IDataErrorInfo implementation
        public string this[string columnName]
        {
            get
            {
                string result = null;

                switch (columnName)
                {
                    case nameof(Name):
                        if (string.IsNullOrWhiteSpace(Name))
                        {
                            result = "Name cannot be empty.";
                        }
                        break;
                    case nameof(Email):
                        if (string.IsNullOrWhiteSpace(Email))
                        {
                            result = "Email cannot be empty.";
                        }
                        break;
                    case nameof(Discount):
                        if (Discount == null)
                        {
                            result = "Discount cannot be null.";
                        }
                        else if (Discount < 0 || Discount > 30)
                        {
                            result = "Discount must be between 0 and 30.";
                        }
                        break;
                }

                return result;
            }
        }

        public string Error => null;
    }
}
