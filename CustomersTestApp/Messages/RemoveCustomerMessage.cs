public class RemoveCustomerMessage
{
    public Guid CustomerId { get; }

    public RemoveCustomerMessage(Guid customerId)
    {
        CustomerId = customerId;
    }
}
