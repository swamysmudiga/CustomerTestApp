namespace CustomersTestApp;

public class Customer
{
    public Customer()
    {
        CanRemove = true;
    }

    public Guid Id { get; set; } // auto generated with random, without repeating, read-only
    public string Name { get; set; }
    public string Email { get; set; }
    public int Discount { get; set; } // min = 0, max = 30
    public bool CanRemove { get; set; }
}