namespace CustomersTestApp;

public class Grpc_customerService : ICustomerService
{
    //public Customer[] get_cust()
    //{
    //    return _customers;
    //}

    #region Constructors

    public Grpc_customerService()
    {
    }

    #endregion Constructors

    private List<Customer> _customers = new List<Customer>
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
    

    public async Task<bool> AddCustomer(Customer customer)
    {
        await Task.Run(() =>
        {
            customer.Id = Guid.NewGuid();
            _customers.Add(customer);
        });
        return true;
    }
    public async Task<bool> UpdateCustomer(Customer customer)
    {
        //var existingCustomer = _customers.FirstOrDefault(c => c.Id == customer.Id);
        var existingCustomer = await GetCustomerById(customer.Id);
        if (existingCustomer != null)
        {
            await Task.Run(() =>
            {
                existingCustomer.name = customer.name;
                existingCustomer.Email = customer.Email;
                existingCustomer.Discount = customer.Discount;
                //existingCustomer.Can_Remove = customer.Can_Remove;
            });
            return true;
        }
        return false;
    }
    public async Task<bool> DeleteCustomer(Guid customerId)
    {
        var customerById = await GetCustomerById(customerId);
        if (customerById != null && customerById.Can_Remove) 
        {
            await Task.Run(() =>
            {
                _customers.Remove(customerById);
            });
            return true;
        }
        return false;
    }

    public async Task<Customer?> GetCustomerById(Guid customerId)
    {

        return await Task.Run(() =>
        {
            return _customers.FirstOrDefault(c => c.Id == customerId);
        });
    }


    public IAsyncEnumerable<Customer> GetCustomers(CancellationToken cancellationToken)
    {
        return _customers.ToAsyncEnumerable();
    }

    // todo implement an asynchronous CRUD
    // No database is needed
    // Assume we have a perfect service that is always online.
}