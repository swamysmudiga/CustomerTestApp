using CommunityToolkit.Mvvm.Messaging;
using CustomersTestApp.Messages;
using CustomersTestApp;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace CustomersTestApp.ViewModels
{
    public class MainViewModel : BaseViewModel, IDataErrorInfo
    {
        private readonly ICustomerService _customerService;
        private readonly IMessenger _messenger;
        private readonly ObservableCollection<Customer> _customers;
        private Customer _selectedCustomer;
        private string _filterText;
        private string _filterType;
        private string _newCustomerName;
        private string _newCustomerEmail;
        private int? _newCustomerDiscount;
        private bool _isAddCustomerFormVisible;

        public MainViewModel(ICustomerService customerService, IMessenger messenger)
        {
            _customerService = customerService;
            _messenger = messenger;
            _customers = new ObservableCollection<Customer>();

            CustomersCollectionView = CollectionViewSource.GetDefaultView(_customers);
            CustomersCollectionView.Filter = FilterCustomers;

            FilterType = "Name";
            FilterText = string.Empty;

            LoadCustomersCommand = new RelayCommand(async () => await LoadCustomers());
            ShowAddCustomerFormCommand = new RelayCommand(() =>
            {
                IsAddCustomerFormVisible = true;
                SelectedCustomer = null;
            });
            HideAddCustomerFormCommand = new RelayCommand(() => IsAddCustomerFormVisible = false);
            AddCustomerCommand = new RelayCommand(async () => await AddCustomer(), CanAddOrSaveCustomer);
            RemoveCustomerCommand = new RelayCommand(() => OnRemoveCustomer(), CanRemoveCustomer);
            SaveCustomerCommand = new RelayCommand(async () => await SaveCustomer(), CanAddOrSaveCustomer);
            CloseCustomerFormCommand = new RelayCommand(() =>
            {
                SelectedCustomer = null;
                IsAddCustomerFormVisible = false;
            });

            // Register to receive messages
            _messenger.Register<RemoveCustomerMessage>(this, (r, m) => RemoveCustomerById(m.CustomerId));

            // Load customers on initialization
            LoadCustomersCommand.Execute(null);

            // Set default value for new customer discount
            NewCustomerDiscount = 0;

            CustomerEditorViewModel = new CustomerEditorViewModel();
        }

        public CustomerEditorViewModel CustomerEditorViewModel { get; }

        public ICollectionView CustomersCollectionView { get; }

        public Customer SelectedCustomer
        {
            get => _selectedCustomer;
            set
            {
                _selectedCustomer = value;
                OnPropertyChanged(nameof(SelectedCustomer));
                OnPropertyChanged(nameof(IsCustomerSelected)); // Notify that IsCustomerSelected has changed
                ((RelayCommand)RemoveCustomerCommand).RaiseCanExecuteChanged();

                // Set the editing customer in the CustomerEditorViewModel
                CustomerEditorViewModel.EditingCustomer = _selectedCustomer != null ? new Customer
                {
                    Id = _selectedCustomer.Id,
                    Name = _selectedCustomer.Name,
                    Email = _selectedCustomer.Email,
                    Discount = _selectedCustomer.Discount,
                    CanRemove = _selectedCustomer.CanRemove
                } : null;

                ((RelayCommand)SaveCustomerCommand).RaiseCanExecuteChanged();

                // Hide Add Customer Form when a customer is selected
                IsAddCustomerFormVisible = _selectedCustomer == null;
            }
        }

        public string FilterText
        {
            get => _filterText;
            set
            {
                _filterText = value;
                OnPropertyChanged(nameof(FilterText));
                CustomersCollectionView.Refresh();
            }
        }

        public string FilterType
        {
            get => _filterType;
            set
            {
                _filterType = value;
                OnPropertyChanged(nameof(FilterType));
                CustomersCollectionView.Refresh();
            }
        }

        public bool IsAddCustomerFormVisible
        {
            get => _isAddCustomerFormVisible;
            set
            {
                _isAddCustomerFormVisible = value;
                OnPropertyChanged(nameof(IsAddCustomerFormVisible));
                ((RelayCommand)AddCustomerCommand).RaiseCanExecuteChanged();
            }
        }

        public bool IsCustomerSelected => SelectedCustomer != null;

        public string NewCustomerName
        {
            get => _newCustomerName;
            set
            {
                _newCustomerName = value;
                OnPropertyChanged(nameof(NewCustomerName));
                ((RelayCommand)AddCustomerCommand).RaiseCanExecuteChanged();
            }
        }

        public string NewCustomerEmail
        {
            get => _newCustomerEmail;
            set
            {
                _newCustomerEmail = value;
                OnPropertyChanged(nameof(NewCustomerEmail));
                ((RelayCommand)AddCustomerCommand).RaiseCanExecuteChanged();
            }
        }

        public int? NewCustomerDiscount
        {
            get => _newCustomerDiscount;
            set
            {
                _newCustomerDiscount = value;
                OnPropertyChanged(nameof(NewCustomerDiscount));
                ((RelayCommand)AddCustomerCommand).RaiseCanExecuteChanged();
            }
        }

        public ICommand LoadCustomersCommand { get; }
        public ICommand ShowAddCustomerFormCommand { get; }
        public ICommand HideAddCustomerFormCommand { get; }
        public ICommand AddCustomerCommand { get; }
        public ICommand RemoveCustomerCommand { get; }
        public ICommand SaveCustomerCommand { get; }
        public ICommand CloseCustomerFormCommand { get; }

        private async Task LoadCustomers()
        {
            await foreach (var customer in _customerService.GetCustomers(CancellationToken.None))
            {
                _customers.Add(customer);
            }
        }

        private async Task AddCustomer()
        {
            var newCustomer = new Customer
            {
                Id = Guid.NewGuid(),
                Name = NewCustomerName,
                Email = NewCustomerEmail,
                Discount = NewCustomerDiscount ?? 0,
                CanRemove = true
            };

            var success = await _customerService.AddCustomer(newCustomer);
            if (success)
            {
                _customers.Add(newCustomer);
                CustomersCollectionView.Refresh();
                IsAddCustomerFormVisible = false;
                NewCustomerName = string.Empty;
                NewCustomerEmail = string.Empty;
                NewCustomerDiscount = 0;
                MessageBox.Show("New customer added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void OnRemoveCustomer()
        {
            if (SelectedCustomer != null && SelectedCustomer.CanRemove)
            {
                _messenger.Send(new RemoveCustomerMessage(SelectedCustomer.Id));
            }
        }

        private bool CanRemoveCustomer()
        {
            return SelectedCustomer != null && SelectedCustomer.CanRemove;
        }

        private async Task SaveCustomer()
        {
            if (CustomerEditorViewModel.EditingCustomer != null)
            {
                // Update the original SelectedCustomer with the values from EditingCustomer
                SelectedCustomer.Name = CustomerEditorViewModel.EditingCustomer.Name;
                SelectedCustomer.Email = CustomerEditorViewModel.EditingCustomer.Email;
                SelectedCustomer.Discount = CustomerEditorViewModel.EditingCustomer.Discount;

                var success = await _customerService.UpdateCustomer(SelectedCustomer);
                if (success)
                {
                    CustomersCollectionView.Refresh();
                    MessageBox.Show("Customer details updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        private bool CanAddOrSaveCustomer()
        {
            if (IsAddCustomerFormVisible)
            {
                return IsValidCustomer(new Customer { Name = NewCustomerName, Email = NewCustomerEmail, Discount = NewCustomerDiscount ?? 0 });
            }
            return IsValidCustomer(CustomerEditorViewModel.EditingCustomer);
        }

        private bool IsValidCustomer(Customer customer)
        {
            return customer != null &&
                   !string.IsNullOrWhiteSpace(customer.Name) &&
                   !string.IsNullOrWhiteSpace(customer.Email) &&
                   customer.Discount >= 0 && customer.Discount <= 30;
        }

        private void RemoveCustomerById(Guid customerId)
        {
            var customer = _customers.FirstOrDefault(c => c.Id == customerId);
            if (customer != null && customer.CanRemove)
            {
                _customers.Remove(customer);
                CustomersCollectionView.Refresh();
            }
        }

        private bool FilterCustomers(object obj)
        {
            if (obj is Customer customer)
            {
                if (string.IsNullOrEmpty(FilterText))
                {
                    return true;
                }

                return FilterType switch
                {
                    "Name" => customer.Name.Contains(FilterText, StringComparison.OrdinalIgnoreCase),
                    "Email" => customer.Email.Contains(FilterText, StringComparison.OrdinalIgnoreCase),
                    _ => true
                };
            }
            return false;
        }

        // IDataErrorInfo implementation for new customer
        public string this[string columnName]
        {
            get
            {
                string result = null;

                switch (columnName)
                {
                    case nameof(NewCustomerName):
                        if (string.IsNullOrWhiteSpace(NewCustomerName))
                        {
                            result = "Name cannot be empty.";
                        }
                        break;
                    case nameof(NewCustomerEmail):
                        if (string.IsNullOrWhiteSpace(NewCustomerEmail))
                        {
                            result = "Email cannot be empty.";
                        }
                        break;
                    case nameof(NewCustomerDiscount):
                        if (NewCustomerDiscount == null)
                        {
                            result = "Discount cannot be null.";
                        }
                        else if (NewCustomerDiscount < 0 || NewCustomerDiscount > 30)
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
