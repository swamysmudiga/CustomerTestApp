using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CustomersTestApp.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private readonly ICustomerService _customerService;
        private ObservableCollection<Customer> _customers;
        private Customer _selectedCustomer;

        public MainViewModel(ICustomerService customerService)
        {
            _customerService = customerService;
            LoadCustomersCommand = new RelayCommand(async () => await LoadCustomers());
            AddCustomerCommand = new RelayCommand(async () => await AddCustomer());
            RemoveCustomerCommand = new RelayCommand(async () => await RemoveCustomer());
            SaveCustomerCommand = new RelayCommand(async () => await SaveCustomer());
        }

        public ObservableCollection<Customer> Customers
        {
            get => _customers;
            set
            {
                _customers = value;
                OnPropertyChanged(nameof(Customers));
            }
        }

        public Customer SelectedCustomer
        {
            get => _selectedCustomer;
            set
            {
                _selectedCustomer = value;
                OnPropertyChanged(nameof(SelectedCustomer));
            }
        }

        public ICommand LoadCustomersCommand { get; }
        public ICommand AddCustomerCommand { get; }
        public ICommand RemoveCustomerCommand { get; }
        public ICommand SaveCustomerCommand { get; }

        private async Task LoadCustomers()
        {
            var customers = new ObservableCollection<Customer>();

            await foreach (var customer in _customerService.GetCustomers(CancellationToken.None))
            {
                customers.Add(customer);
            }

            Customers = customers;
        }

        private async Task AddCustomer()
        {
            var newCustomer = new Customer
            {
                Id = Guid.NewGuid(),
                name = "New Customer",
                Email = "new.customer@example.com",
                Discount = 5,
                Can_Remove = true
            };

            var success = await _customerService.AddCustomer(newCustomer);
            if (success)
            {
                Customers.Add(newCustomer);
            }
        }

        private async Task RemoveCustomer()
        {
            if (SelectedCustomer != null && SelectedCustomer.Can_Remove)
            {
                var success = await _customerService.DeleteCustomer(SelectedCustomer.Id);
                if (success)
                {
                    Customers.Remove(SelectedCustomer);
                }
            }
        }

        private async Task SaveCustomer()
        {
            if (SelectedCustomer != null)
            {
                var success = await _customerService.UpdateCustomer(SelectedCustomer);
                if (success)
                {
                    // Optionally, refresh the customer list
                }
            }
        }
    }
}