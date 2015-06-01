using System;
using System.ComponentModel.DataAnnotations;
using FamilyBudget.Www.App_CodeBase;

namespace FamilyBudget.Www.App_DataModel
{

    #region Account

    public class AccountMetadata
    {
        [Key]
        public int ID { get; set; }

        [Display(Name = "Остаток")]
        public decimal Balance { get; set; }

        [Display(Name = "Основной")]
        public bool IsMain { get; set; }

        [Required]
        [Display(Name = "Валюта")]
        public int CurrencyID { get; set; }

        [Required]
        [Display(Name = "Наименование счета")]
        public string Name { get; set; }
    }

    [MetadataType(typeof (AccountMetadata))]
    public partial class Account
    {
        public string DisplayName
        {
            get
            {
                if (Currency != null)
                {
                    return string.Format("({0}) {1}", Currency.Code, Name);
                }
                return Name;
            }
        }
    }

    #endregion

    #region Currency

    public class CurrencyMetadata
    {
        [Key]
        public int ID { get; set; }

        [Required]
        [StringLength(3)]
        [Display(Name = "Код валюты")]
        public string Code { get; set; }

        [Display(Name = "Наименование")]
        public string Name { get; set; }
    }

    [MetadataType(typeof (CurrencyMetadata))]
    public partial class Currency
    {
    }

    #endregion

    #region ExpenditureCategory

    public class ExpenditureCategoryMetadata
    {
        [Key]
        public int ID { get; set; }

        [Display(Name = "Имя категории")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Описание")]
        public string Description { get; set; }
    }

    [MetadataType(typeof (ExpenditureCategoryMetadata))]
    public partial class ExpenditureCategory
    {
    }

    #endregion

    #region IncomeCategory

    public class IncomeCategoryMetadata
    {
        [Key]
        public int ID { get; set; }

        [Display(Name = "Имя категории")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Описание")]
        public string Description { get; set; }
    }

    [MetadataType(typeof (IncomeCategoryMetadata))]
    public partial class IncomeCategory
    {
    }

    #endregion

    #region Expenditure

    public class ExpenditureMetadata
    {
        [Key]
        public int ID { get; set; }

        [Required]
        [Display(Name = "Дата")]
        public DateTime Date { get; set; }

        [Required]
        [Display(Name = "Счет")]
        public int AccountID { get; set; }

        [Required]
        [Display(Name = "Категория")]
        public int CategoryID { get; set; }

        [Required]
        [Display(Name = "Сумма")]
        public string Summa { get; set; }

        [Display(Name = "Описание")]
        public string Description { get; set; }
    }

    [MetadataType(typeof (ExpenditureMetadata))]
    public partial class Expenditure : IAccountableEntity
    {
        public Expenditure()
        {
            AccountTransactionOperationType = AccountTransactionOperation.Decrease;
            AccountRestoreOperationType = AccountRestoreOperation.Increase;
        }

        public AccountRestoreOperation AccountRestoreOperationType { get; set; }
        public AccountTransactionOperation AccountTransactionOperationType { get; set; }

        public static void Copy(Expenditure entity1, Expenditure entity2)
        {
            if (entity1 == null || entity2 == null)
                return;

            entity1.AccountID = entity2.AccountID;
            entity1.CategoryID = entity2.CategoryID;
            entity1.Date = entity2.Date;
            entity1.Description = entity2.Description;
            entity1.Summa = entity2.Summa;
        }
    }

    #endregion

    #region Income

    public class IncomeMetadata
    {
        [Key]
        public int ID { get; set; }

        [Required]
        [Display(Name = "Дата")]
        public DateTime Date { get; set; }

        [Required]
        [Display(Name = "Счет")]
        public int AccountID { get; set; }

        [Required]
        [Display(Name = "Категория")]
        public int CategoryID { get; set; }

        [Required]
        [Display(Name = "Сумма")]
        public string Summa { get; set; }

        [Display(Name = "Описание")]
        public string Description { get; set; }
    }

    [MetadataType(typeof (IncomeMetadata))]
    public partial class Income : IAccountableEntity
    {
        public Income()
        {
            AccountTransactionOperationType = AccountTransactionOperation.Increase;
            AccountRestoreOperationType = AccountRestoreOperation.Decrease;
        }

        public AccountRestoreOperation AccountRestoreOperationType { get; set; }
        public AccountTransactionOperation AccountTransactionOperationType { get; set; }

        public static void Copy(Income entity1, Income entity2)
        {
            if (entity1 == null || entity2 == null)
                return;

            entity1.AccountID = entity2.AccountID;
            entity1.CategoryID = entity2.CategoryID;
            entity1.Date = entity2.Date;
            entity1.Description = entity2.Description;
            entity1.Summa = entity2.Summa;
        }
    }

    #endregion
}