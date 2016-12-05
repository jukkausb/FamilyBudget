namespace FamilyBudget.Next.Data
{
    public interface IAccountableEntity
    {
        int ID { get; set; }
        int AccountID { get; set; }
        decimal Summa { get; set; }
        AccountRestoreOperation AccountRestoreOperationType { get; set; }
        AccountTransactionOperation AccountTransactionOperationType { get; set; }
    }
}