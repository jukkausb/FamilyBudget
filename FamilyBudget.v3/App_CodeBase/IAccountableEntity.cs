namespace FamilyBudget.v3.App_CodeBase
{
    public interface IAccountableEntity
    {
        int ID { get; set; }
        /// <summary>
        /// Used to track account change
        /// </summary>
        int OldAccountID { get; set; }
        int AccountID { get; set; }
        decimal Summa { get; set; }
        AccountRestoreOperation AccountRestoreOperationType { get; set; }
        AccountTransactionOperation AccountTransactionOperationType { get; set; }
    }
}