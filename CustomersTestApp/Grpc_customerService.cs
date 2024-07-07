using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Net.Client;
using IPC;
using static IPC.CustomerService;

namespace CustomersTestApp
{
    public class Grpc_customerService : ICustomerService
    {
        private readonly CustomerServiceClient _client;

        public Grpc_customerService()
        {
            var channel = GrpcChannel.ForAddress("https://localhost:5001");
            _client = new CustomerServiceClient(channel);
        }

        public async Task<bool> AddCustomer(Customer customer)
        {
            var request = new AddCustomerRequest
            {
                Customer = new IPC.Customer
                {
                    Id = customer.Id.ToString(),
                    Name = customer.name,
                    Email = customer.Email,
                    Discount = customer.Discount,
                    CanRemove = customer.Can_Remove
                }
            };

            var response = await _client.AddCustomerAsync(request);
            return response.Success;
        }

        public async Task<bool> UpdateCustomer(Customer customer)
        {
            var request = new UpdateCustomerRequest
            {
                Customer = new IPC.Customer
                {
                    Id = customer.Id.ToString(),
                    Name = customer.name,
                    Email = customer.Email,
                    Discount = customer.Discount,
                    CanRemove = customer.Can_Remove
                }
            };

            var response = await _client.UpdateCustomerAsync(request);
            return response.Success;
        }

        public async Task<bool> DeleteCustomer(Guid customerId)
        {
            var request = new DeleteCustomerRequest { Id = customerId.ToString() };
            var response = await _client.DeleteCustomerAsync(request);
            return response.Success;
        }

        public async Task<Customer?> GetCustomerById(Guid customerId)
        {
            var request = new GetCustomerRequest { Id = customerId.ToString() };
            var response = await _client.GetCustomerAsync(request);

            return response.Customer == null ? null : new Customer
            {
                Id = Guid.Parse(response.Customer.Id),
                name = response.Customer.Name,
                Email = response.Customer.Email,
                Discount = response.Customer.Discount,
                Can_Remove = response.Customer.CanRemove
            };
        }

        public async IAsyncEnumerable<Customer> GetCustomers([EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            var request = new GetCustomersRequest();
            using var call = _client.GetCustomers(request, cancellationToken: cancellationToken);

            await foreach (var response in call.ResponseStream.ReadAllAsync(cancellationToken))
            {
                yield return new Customer
                {
                    Id = Guid.Parse(response.Customer.Id),
                    name = response.Customer.Name,
                    Email = response.Customer.Email,
                    Discount = response.Customer.Discount,
                    Can_Remove = response.Customer.CanRemove
                };
            }
        }

        private Customer[] _List1 =
        {
            new Customer
            {
                Id = Guid.NewGuid(),
                name = "Microsoft",
                Email = "microsoft@microsoft.com",
                Discount = 10,
                Can_Remove = false
            },
            new Customer
            {
                Id = Guid.NewGuid(),
                name = "Google",
                Email = "google@google.com",
                Discount = 5,
                Can_Remove = false
            },
            new Customer
            {
                Id = Guid.NewGuid(),
                name = "Amazon",
                Email = "amazon@amazon.com",
                Discount = 0,
                Can_Remove = true
            }
        };
    }
}