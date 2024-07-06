namespace CustomersTestApp;

public interface ICustomerService
{
    //Implementig crud operation

    //

    Task<bool> AddCustomer(Customer customer);
    Task<bool> UpdateCustomer(Customer customer);
    //Task<bool> RemoveCustomer(Customer customer);
    Task<bool> DeleteCustomer(Guid customerId);
    Task<Customer?> GetCustomerById(Guid customerId);

    IAsyncEnumerable<Customer> GetCustomers(CancellationToken cancellationToken);
}
