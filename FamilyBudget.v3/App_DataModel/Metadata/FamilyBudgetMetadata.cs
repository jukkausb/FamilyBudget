using System;
using System.ComponentModel.DataAnnotations;
using FamilyBudget.v3.App_CodeBase;

namespace FamilyBudget.v3.App_DataModel
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

    [MetadataType(typeof(AccountMetadata))]
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

    [MetadataType(typeof(CurrencyMetadata))]
    public partial class Currency
    {
    }

    #endregion

    #region ExpenditureCategory

    public class ExpenditureCategoryMetadata
    {
        [Key]
        public int ID { get; set; }

        [Required]
        [Display(Name = "Имя категории")]
        public string Name { get; set; }

        [Display(Name = "Описание")]
        public string Description { get; set; }
    }

    [MetadataType(typeof(ExpenditureCategoryMetadata))]
    public partial class ExpenditureCategory : ICategoryInfo
    {
    }

    #endregion

    #region IncomeCategory

    public class IncomeCategoryMetadata
    {
        [Key]
        public int ID { get; set; }

        [Required]
        [Display(Name = "Имя категории")]
        public string Name { get; set; }

        [Display(Name = "Описание")]
        public string Description { get; set; }
    }

    [MetadataType(typeof(IncomeCategoryMetadata))]
    public partial class IncomeCategory : ICategoryInfo
    {
    }

    #endregion

    #region Expenditure

    public class ExpenditureMetadata
    {
        [Key]
        public int ID { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:d}")]
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

    [MetadataType(typeof(ExpenditureMetadata))]
    public partial class Expenditure : IAccountableEntity
    {
        public Expenditure()
        {
            AccountTransactionOperationType = AccountTransactionOperation.Decrease;
            AccountRestoreOperationType = AccountRestoreOperation.Increase;
        }

        public int OldAccountID { get; set; }
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
        [DisplayFormat(DataFormatString = "{0:d}")]
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

    [MetadataType(typeof(IncomeMetadata))]
    public partial class Income : IAccountableEntity
    {
        public Income()
        {
            AccountTransactionOperationType = AccountTransactionOperation.Increase;
            AccountRestoreOperationType = AccountRestoreOperation.Decrease;
        }

        public int OldAccountID { get; set; }
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

    #region Investment instrument

    public class InvestmentInstrumentMetadata
    {
        [Key]
        public int ID { get; set; }
        [Required]
        [Display(Name = "Код")]
        public string Code { get; set; }
        [Display(Name = "Цвет фона диаграммы")]
        public string DiagramBackgroundColor { get; set; }
        [Display(Name = "Цвет фона диаграммы в фокусе")]
        public string DiagramBackgroundHoverColor { get; set; }
        [Display(Name = "Цвет границ диаграммы в фокусе")]
        public string DiagramHoverBorderColor { get; set; }
        [Display(Name = "Целевой процент в портфеле")]
        public Nullable<int> PortfolioPercent { get; set; }
        [Display(Name = "Отклонение от целевого процента в портфеле")]
        public Nullable<int> PortfolioPercentDelta { get; set; }
        [Required]
        [Display(Name = "Тип инструмента")]
        public Nullable<int> TypeID { get; set; }
        [Display(Name = "Рынок инструмента")]
        public Nullable<int> MarketID { get; set; }
    }

    [MetadataType(typeof(InvestmentInstrumentMetadata))]
    public partial class InvestmentInstrument : IPieDiagramDataSourceItem
    {
    }

    #endregion

    #region Investment instrument market

    public class InvestmentInstrumentMarketMetadata
    {
        [Key]
        public int ID { get; set; }
        [Required]
        [Display(Name = "Код")]
        public string Code { get; set; }
        [Required]
        [Display(Name = "Наименование")]
        public string Name { get; set; }
        [Display(Name = "Цвет фона диаграммы")]
        public string DiagramBackgroundColor { get; set; }
        [Display(Name = "Цвет фона диаграммы в фокусе")]
        public string DiagramBackgroundHoverColor { get; set; }
        [Display(Name = "Цвет границ диаграммы в фокусе")]
        public string DiagramHoverBorderColor { get; set; }
        [Display(Name = "Целевой процент в портфеле")]
        public Nullable<int> PortfolioPercent { get; set; }
        [Display(Name = "Отклонение от целевого процента в портфеле")]
        public Nullable<int> PortfolioPercentDelta { get; set; }
    }

    [MetadataType(typeof(InvestmentInstrumentMarketMetadata))]
    public partial class InvestmentInstrumentMarket : IPieDiagramDataSourceItem
    {
    }

    #endregion

    #region Investment instrument type

    public class InvestmentInstrumentTypeMetadata
    {
        [Key]
        public int ID { get; set; }
        [Required]
        [Display(Name = "Код")]
        public string Code { get; set; }
        [Required]
        [Display(Name = "Наименование")]
        public string Name { get; set; }
        [Display(Name = "Цвет фона диаграммы")]
        public string DiagramBackgroundColor { get; set; }
        [Display(Name = "Цвет фона диаграммы в фокусе")]
        public string DiagramBackgroundHoverColor { get; set; }
        [Display(Name = "Цвет границ диаграммы в фокусе")]
        public string DiagramHoverBorderColor { get; set; }
        [Display(Name = "Целевой процент в портфеле")]
        public Nullable<int> PortfolioPercent { get; set; }
        [Display(Name = "Отклонение от целевого процента в портфеле")]
        public Nullable<int> PortfolioPercentDelta { get; set; }
    }

    [MetadataType(typeof(InvestmentInstrumentTypeMetadata))]

    public partial class InvestmentInstrumentType : IPieDiagramDataSourceItem
    {
    }

    #endregion
}