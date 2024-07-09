using Grpc.Core;
using IPC;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomerService.Services
{
    public class CustomerService : IPC.CustomerService.CustomerServiceBase
    {
        private readonly ILogger<CustomerService> _logger;
        private static readonly List<Customer> _customers = new List<Customer>();
        private static readonly object _lock = new object();
        private static bool _isInitialized = false;

        public CustomerService(ILogger<CustomerService> logger)
        {
            _logger = logger;

            // Ensure the customer list is initialized only once
            if (!_isInitialized)
            {
                lock (_lock)
                {
                    if (!_isInitialized)
                    {
                        // Initialize with some predefined customers
                        _customers.Add(new Customer { Id = Guid.NewGuid().ToString(), Name = "John Doe", Email = "john.doe@example.com", Discount = 10, CanRemove = true });
                        _customers.Add(new Customer { Id = Guid.NewGuid().ToString(), Name = "Jane Smith", Email = "jane.smith@example.com", Discount = 20, CanRemove = true });
                        _customers.Add(new Customer { Id = Guid.NewGuid().ToString(), Name = "AshDoe", Email = "jodfghn.doe@example.com", Discount = 10, CanRemove = false });
                        _customers.Add(new Customer { Id = Guid.NewGuid().ToString(), Name = "gfgdf Smith", Email = "jfgane.smith@example.com", Discount = 20, CanRemove = false });

                        _isInitialized = true;
                    }
                }
            }
        }

        public override Task<GetCustomerResponse> GetCustomer(GetCustomerRequest request, ServerCallContext context)
        {
            var customer = _customers.FirstOrDefault(c => c.Id == request.Id);
            return Task.FromResult(new GetCustomerResponse { Customer = customer });
        }

        public override async Task GetCustomers(GetCustomersRequest request, IServerStreamWriter<GetCustomerResponse> responseStream, ServerCallContext context)
        {
            foreach (var customer in _customers)
            {
                await responseStream.WriteAsync(new GetCustomerResponse { Customer = customer });
            }
        }

        public override Task<AddCustomerResponse> AddCustomer(AddCustomerRequest request, ServerCallContext context)
        {
            _customers.Add(request.Customer);
            return Task.FromResult(new AddCustomerResponse { Success = true });
        }

        public override Task<UpdateCustomerResponse> UpdateCustomer(UpdateCustomerRequest request, ServerCallContext context)
        {
            var index = _customers.FindIndex(c => c.Id == request.Customer.Id);
            if (index != -1)
            {
                _customers[index] = request.Customer;
                return Task.FromResult(new UpdateCustomerResponse { Success = true });
            }
            return Task.FromResult(new UpdateCustomerResponse { Success = false });
        }

        public override Task<DeleteCustomerResponse> DeleteCustomer(DeleteCustomerRequest request, ServerCallContext context)
        {
            var customer = _customers.FirstOrDefault(c => c.Id == request.Id);
            if (customer != null)
            {
                _customers.Remove(customer);
                return Task.FromResult(new DeleteCustomerResponse { Success = true });
            }
            return Task.FromResult(new DeleteCustomerResponse { Success = false });
        }
    }
}
