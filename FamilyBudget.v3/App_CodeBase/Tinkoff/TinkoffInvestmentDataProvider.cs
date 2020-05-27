using FamilyBudget.v3.App_CodeBase.Tinkoff.Models;
using FamilyBudget.v3.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Tinkoff.Trading.OpenApi.Models;
using System;
using FamilyBudget.v3.App_Helpers;
using FamilyBudget.v3.Models.Repository.Interfaces;
using static FamilyBudget.v3.App_CodeBase.Tinkoff.Models.PortfolioCurrenciesExtended;
using FamilyBudget.v3.App_DataModel;

namespace FamilyBudget.v3.App_CodeBase.Tinkoff
{
    public interface ITinkoffInvestmentDataProvider
    {
        List<InvestmentAccount> GetInvestmentAccounts();
        decimal GetTotalInvested();
    }

    public class TinkoffInvestmentDataProvider : ITinkoffInvestmentDataProvider
    {
        private readonly IExpenditureRepository _expenditureRepository;
        private readonly IInvestmentInstrumentRepository _investmentInstrumentRepository;
        private readonly ICurrencyProvider _currencyProvider;

        public TinkoffInvestmentDataProvider(IExpenditureRepository expenditureRepository,
            IInvestmentInstrumentRepository investmentInstrumentRepository, ICurrencyProvider currencyProvider)
        {
            _expenditureRepository = expenditureRepository;
            _investmentInstrumentRepository = investmentInstrumentRepository;
            _currencyProvider = currencyProvider;
        }

        private string GetToken()
        {
            string tokenFileName = System.Web.Configuration.WebConfigurationManager.AppSettings["TinkoffTokenFileName"];
            ITinkoffOpenApiTokenReader tinkoffOpenApiTokenReader = new TinkoffOpenApiTokenReader();
            string token = tinkoffOpenApiTokenReader.ReadToken(tokenFileName);
            return token;
        }

        private List<TinkoffPortfolioPosition> GetPortfolioPositions(string brokerAccountId)
        {
            ITinkoffPortfolioDataRetriever tinkoffPortfolioDataRetriever = new TinkoffPortfolioDataRetriever(GetToken());
            List<TinkoffPortfolioPosition> positions = tinkoffPortfolioDataRetriever.GetTinkoffPortfolioPositions(brokerAccountId);
            return positions;
        }

        private List<PortfolioCurrencyExtended> GetPortfolioCurrencies(string brokerAccountId)
        {
            ITinkoffPortfolioDataRetriever tinkoffPortfolioDataRetriever = new TinkoffPortfolioDataRetriever(GetToken());
            List<PortfolioCurrencyExtended> positions = tinkoffPortfolioDataRetriever.GetTinkoffPortfolioCurrencies(brokerAccountId);
            return positions;
        }

        public decimal GetTotalInvested()
        {
            decimal investmentsToIIS = BusinessHelper.GetIISExpenditures(_expenditureRepository).Sum(e => e.Summa);
            decimal investmentsBrokerAccount = BusinessHelper.GetBrokerAccountExpenditures(_expenditureRepository).Sum(e => e.Summa);
            decimal totalInvestments = investmentsToIIS + investmentsBrokerAccount;
            return totalInvestments;
        }

        public List<InvestmentAccount> GetInvestmentAccounts()
        {
            decimal investmentsToIIS = BusinessHelper.GetIISExpenditures(_expenditureRepository).Sum(e => e.Summa);
            decimal investmentsBrokerAccount = BusinessHelper.GetBrokerAccountExpenditures(_expenditureRepository).Sum(e => e.Summa);
            decimal totalInvestments = investmentsToIIS + investmentsBrokerAccount;

            ITinkoffUserAccountDataRetriever tinkoffUserAccountDataRetriever = new TinkoffUserAccountDataRetriever(GetToken());
            List<TinkoffBrokerAccount> accounts = tinkoffUserAccountDataRetriever.GetTinkoffUserAccounts();

            List<InvestmentAccount> investmentAccounts = new List<InvestmentAccount>();

            foreach (var account in accounts)
            {
                decimal totalInvested = 0;
                string accountName = "";
                if (account.BrokerAccountType == BrokerAccountType.TinkoffIis)
                {
                    totalInvested = investmentsToIIS;
                    accountName = "ИИС";
                }
                else
                {
                    totalInvested = investmentsBrokerAccount;
                    accountName = "Брокерский счет";
                }

                InvestmentAccount investmentAccount = BuildInvestmentAccount(account, accountName, totalInvested);
                investmentAccounts.Add(investmentAccount);
            }

            return investmentAccounts;
        }

        private InvestmentAccount BuildInvestmentAccount(TinkoffBrokerAccount account, string name, decimal totalInvested)
        {
            InvestmentAccount investmentAccount = new InvestmentAccount()
            {
                Id = account.BrokerAccountId,
                Name = name,
                Type = account.BrokerAccountType,
                TotalInvested = totalInvested,
                Currency = Constants.CURRENCY_RUB // Инвестирую только в рублях (это Крым, детка)
            };

            var portfolioPositions = GetPortfolioPositions(account.BrokerAccountId);

            foreach (var portfolioPosition in portfolioPositions)
            {
                var investmentInstrument = _investmentInstrumentRepository.FindBy(i => i.Code == portfolioPosition.Ticker).FirstOrDefault();
                ApplyPortfolioPositionCustomAttributes(portfolioPosition, investmentInstrument);
            }

            var accountCashRub = GetPortfolioCurrencies(account.BrokerAccountId).
                Where(c => c.Currency.ToString().ToUpper() == Constants.CURRENCY_RUB).
                FirstOrDefault();

            decimal investmentAccountTotalBalance = GetInvestmentAccountTotalBalance(portfolioPositions);
            decimal investmentAccountTotalDelta = GetInvestmentAccountTotalDelta(portfolioPositions);

            investmentAccount.Totals = new MoneyWithDeltaModel(investmentAccountTotalBalance, Constants.CURRENCY_RUB, investmentAccountTotalDelta);

            decimal stocksTotal = 0;
            decimal etfTotal = 0;
            decimal bondsTotal = 0;
            decimal currenciesTotal = 0;

            var positionGroups = portfolioPositions.GroupBy(p => p.Type).ToList();

            foreach (var positionGroup in positionGroups)
            {
                TinkoffPortfolioGroup group = null;
                var groupData = positionGroup.OrderBy(p => p.Name).ToList();

                if (positionGroup.Key == InstrumentType.Bond)
                {
                    group = BuildGroup(groupData,
                        Constants.InstrumentType.INSTRUMENT_TYPE_CODE_BONDS,
                        Constants.InstrumentType.INSTRUMENT_TYPE_TITLE_BONDS);
                    bondsTotal = groupData.Sum(g => g.CurrentTotalInPortfolio);
                }
                if (positionGroup.Key == InstrumentType.Currency)
                {
                    var rublePortfolioPosition = new TinkoffPortfolioPosition
                    {
                        Name = Constants.CURRENCY_NAME_RUB,
                        Ticker = Constants.CURRENCY_RUB,
                        Isin = Constants.CURRENCY_RUB,
                        IsStatic = true,
                        CurrentTotalInPortfolio = accountCashRub.Balance
                    };

                    ApplyPortfolioPositionCustomAttributes(rublePortfolioPosition, null);

                    groupData.Add(rublePortfolioPosition);

                    group = BuildGroup(groupData,
                        Constants.InstrumentType.INSTRUMENT_TYPE_CODE_CURRENCIES,
                        Constants.InstrumentType.INSTRUMENT_TYPE_TITLE_CURRENCIES);
                    currenciesTotal = groupData.Sum(g => g.CurrentTotalInPortfolio);
                }
                if (positionGroup.Key == InstrumentType.Etf)
                {
                    group = BuildGroup(groupData,
                        Constants.InstrumentType.INSTRUMENT_TYPE_CODE_ETF,
                        Constants.InstrumentType.INSTRUMENT_TYPE_TITLE_ETF);
                    etfTotal = groupData.Sum(g => g.CurrentTotalInPortfolio);
                }
                if (positionGroup.Key == InstrumentType.Stock)
                {
                    group = BuildGroup(groupData,
                        Constants.InstrumentType.INSTRUMENT_TYPE_CODE_STOCKS,
                        Constants.InstrumentType.INSTRUMENT_TYPE_TITLE_STOCKS);
                    stocksTotal = groupData.Sum(g => g.CurrentTotalInPortfolio);
                }

                if (group != null)
                {
                    investmentAccount.Groups.Add(group);
                }
            }

            var allInstruments = investmentAccount.Groups.SelectMany(g => g.Positions).ToList();
            var totalAccountBalance = investmentAccount.Totals.Value;

            // Check investment instrument rules on account
            investmentAccount.MessageGroups.AddRange(CheckInvestmentInstrumentRulesOnAccount(allInstruments, totalAccountBalance));

            MessageGroup mgCommon = new MessageGroup() { Name = Constants.InstrumentType.INSTRUMENT_TYPE_TITLE_COMMON };

            // Check investment instrument type rules on account
            var etfPercentOnAccount = etfTotal / totalAccountBalance * 100;
            mgCommon.Messages.AddRange(CheckInvestmentInstrumentTypeOnAccount(Constants.InstrumentType.INSTRUMENT_TYPE_CODE_ETF, etfPercentOnAccount));
            var bondsPercentOnAccount = bondsTotal / totalAccountBalance * 100;
            mgCommon.Messages.AddRange(CheckInvestmentInstrumentTypeOnAccount(Constants.InstrumentType.INSTRUMENT_TYPE_CODE_BONDS, bondsPercentOnAccount));
            var stocksPercentOnAccount = stocksTotal / totalAccountBalance * 100;
            mgCommon.Messages.AddRange(CheckInvestmentInstrumentTypeOnAccount(Constants.InstrumentType.INSTRUMENT_TYPE_CODE_STOCKS, stocksPercentOnAccount));
            var currenciesPercentOnAccount = currenciesTotal / totalAccountBalance * 100;
            mgCommon.Messages.AddRange(CheckInvestmentInstrumentTypeOnAccount(Constants.InstrumentType.INSTRUMENT_TYPE_CODE_CURRENCIES, currenciesPercentOnAccount));

            investmentAccount.MessageGroups.Insert(0, mgCommon);

            // Add market groups
            var positionMarketGroups = portfolioPositions.GroupBy(p => p.Market).Where(p => !string.IsNullOrEmpty(p.Key)).ToList();
            foreach (var group in positionMarketGroups)
            {
                string marketGroupCode = Constants.InstrumentMarket.INSTRUMENT_MARKET_CODE_PREFIX + group.Key;
                TinkoffPortfolioGroup portfolioMarketGroup = new TinkoffPortfolioGroup
                {
                    Code = marketGroupCode,
                    Name = group.Key,
                    CurrentTotalInPortfolio = group.Sum(p => p.CurrentTotalInPortfolio)
                };

                var investmentInstrument = _investmentInstrumentRepository.FindBy(i => i.Code == marketGroupCode).FirstOrDefault();
                ApplyPortfolioGroupCustomAttributes(portfolioMarketGroup, investmentInstrument);

                investmentAccount.MarketGroups.Add(portfolioMarketGroup);
            }

            return investmentAccount;
        }

        private void ApplyPortfolioPositionCustomAttributes(TinkoffPortfolioPosition portfolioPosition, InvestmentInstrument investmentInstrument)
        {
            if (portfolioPosition == null)
            {
                return;
            }

            if (investmentInstrument != null)
            {
                portfolioPosition.Market = investmentInstrument.Market;
            }

            portfolioPosition.AvatarImageLink = TinkoffStaticUrlResolver.ResolveAvatarImageLink(portfolioPosition, investmentInstrument);
            portfolioPosition.TickerPageLink = TinkoffStaticUrlResolver.ResolveExternalPageId(portfolioPosition, investmentInstrument);

            DiagramHelper.ApplyPieDiagramColors(portfolioPosition, investmentInstrument);
        }

        private void ApplyPortfolioGroupCustomAttributes(TinkoffPortfolioGroup portfolioGroup, InvestmentInstrument investmentInstrument)
        {
            if (portfolioGroup == null)
            {
                return;
            }

            DiagramHelper.ApplyPieDiagramColors(portfolioGroup, investmentInstrument);
        }

        private decimal GetInvestmentAccountTotalBalance(List<TinkoffPortfolioPosition> portfolioPositions)
        {
            decimal total = 0;

            foreach (var portfolioPosition in portfolioPositions)
            {
                string positionCurrency = portfolioPosition.Currency.ToUpper();
                if (positionCurrency == Constants.CURRENCY_RUB)
                {
                    total += portfolioPosition.CurrentTotalInPortfolio;
                }
                else
                {
                    decimal rate = _currencyProvider.GetSellCurrencyRate(positionCurrency, Constants.CURRENCY_RUB);
                    total += portfolioPosition.CurrentTotalInPortfolio * rate;
                }
            }

            return total;
        }

        private decimal GetInvestmentAccountTotalDelta(List<TinkoffPortfolioPosition> portfolioPositions)
        {
            decimal total = 0;

            foreach (var portfolioPosition in portfolioPositions)
            {
                string positionCurrency = portfolioPosition.Currency.ToUpper();
                if (positionCurrency == Constants.CURRENCY_RUB)
                {
                    total += portfolioPosition.CurrentDelta;
                }
                else
                {
                    decimal rate = _currencyProvider.GetSellCurrencyRate(positionCurrency, Constants.CURRENCY_RUB);
                    total += portfolioPosition.CurrentDelta * rate;
                }
            }

            return total;
        }

        private TinkoffPortfolioGroup BuildGroup(List<TinkoffPortfolioPosition> instrumentsInGroup,
            string groupCode,
            string groupTitle)
        {
            decimal total = instrumentsInGroup.Sum(e => e.CurrentTotalInPortfolio);

            TinkoffPortfolioGroup group = new TinkoffPortfolioGroup();
            group.Positions = instrumentsInGroup;
            group.Code = groupCode;
            group.Name = groupTitle;
            group.CurrentTotalInPortfolio = total;

            var investmentInstrument = _investmentInstrumentRepository.FindBy(i => i.Code == groupCode).FirstOrDefault();

            ApplyPortfolioGroupCustomAttributes(group, investmentInstrument);

            return group;
        }

        private List<MessageGroup> CheckInvestmentInstrumentRulesOnAccount(List<TinkoffPortfolioPosition> instruments, decimal totalAccountBalance)
        {
            List<MessageGroup> messageGroups = new List<MessageGroup>();
            if (instruments == null)
            {
                return messageGroups;
            }

            MessageGroup mgStocks = new MessageGroup() { Name = Constants.InstrumentType.INSTRUMENT_TYPE_TITLE_STOCKS };
            MessageGroup mgBonds = new MessageGroup() { Name = Constants.InstrumentType.INSTRUMENT_TYPE_TITLE_BONDS };
            MessageGroup mgEtf = new MessageGroup() { Name = Constants.InstrumentType.INSTRUMENT_TYPE_TITLE_ETF };

            MessageGroup targetGroup = null;

            foreach (var instrument in instruments)
            {
                if (string.IsNullOrEmpty(instrument.Ticker))
                {
                    continue;
                }

                var investmentInstrument = _investmentInstrumentRepository.FindBy(i => i.Code == instrument.Ticker).FirstOrDefault();
                if (investmentInstrument == null)
                {
                    continue;
                }

                var instrumentPersentOnAccountTarget = investmentInstrument.PortfolioPercent;
                var instrumentPersentOnAccountDelta = investmentInstrument.PortfolioPercentDelta;
                if (!instrumentPersentOnAccountTarget.HasValue || !instrumentPersentOnAccountTarget.HasValue)
                {
                    // Do not check if rule values are not specified
                    continue;
                }

                if (instrument.Type == InstrumentType.Stock)
                {
                    targetGroup = mgStocks;
                }
                else if (instrument.Type == InstrumentType.Bond)
                {
                    targetGroup = mgBonds;
                }
                else if (instrument.Type == InstrumentType.Etf)
                {
                    targetGroup = mgEtf;
                }

                var currentPercentOnAccount = instrument.CurrentTotalInPortfolio / totalAccountBalance * 100;
                string currentPercentOnAccountPresentationString = Math.Round(currentPercentOnAccount, 2).ToString();
                string instrumentPersentOnAccountTargetPresentationString = Math.Round((decimal)instrumentPersentOnAccountTarget, 2).ToString();

                if (currentPercentOnAccount > instrumentPersentOnAccountTarget + instrumentPersentOnAccountDelta)
                {
                    targetGroup.Messages.Add(GetMessageToDecreaseInstrumentInPortfolio(instrument.Ticker, currentPercentOnAccountPresentationString, instrumentPersentOnAccountTargetPresentationString));
                }

                if (instrument.Type == InstrumentType.Stock)
                {
                    // Do not encourage to increase Stocks share in portfolio
                    // No need to introduce more risk (more stocks more risk)
                    continue;
                }

                if (currentPercentOnAccount < instrumentPersentOnAccountTarget - instrumentPersentOnAccountDelta)
                {
                    targetGroup.Messages.Add(GetMessageToIncreaseInstrumentInPortfolio(instrument.Ticker, currentPercentOnAccountPresentationString, instrumentPersentOnAccountTargetPresentationString));
                }
            }

            messageGroups.Add(mgStocks);
            messageGroups.Add(mgBonds);
            messageGroups.Add(mgEtf);

            return messageGroups;
        }

        private List<Message> CheckInvestmentInstrumentTypeOnAccount(string instrumentTypeCode, decimal currentInstrumentTypePercentOnAccount)
        {
            List<Message> messages = new List<Message>();
            if (string.IsNullOrEmpty(instrumentTypeCode))
            {
                return messages;
            }

            var investmentInstrumentType = _investmentInstrumentRepository.FindBy(i => i.Code == instrumentTypeCode).FirstOrDefault();
            if (investmentInstrumentType == null)
            {
                return messages;
            }

            var instrumentTypePersentOnAccountTarget = investmentInstrumentType.PortfolioPercent;
            var instrumentTypePersentOnAccountDelta = investmentInstrumentType.PortfolioPercentDelta;
            if (!instrumentTypePersentOnAccountDelta.HasValue || !instrumentTypePersentOnAccountDelta.HasValue)
            {
                // Do not check if rule values are not specified
                return messages;
            }

            MessageGroup mgCommon = new MessageGroup() { Name = Constants.InstrumentType.INSTRUMENT_TYPE_TITLE_COMMON };

            string currentPercentOnAccountPresentationString = Math.Round(currentInstrumentTypePercentOnAccount, 2).ToString();
            string instrumentTypePersentOnAccountTargetPresentationString = Math.Round((decimal)instrumentTypePersentOnAccountTarget, 2).ToString();

            string instrumentName = instrumentTypeCode;
            if (instrumentTypeCode == Constants.InstrumentType.INSTRUMENT_TYPE_CODE_STOCKS)
            {
                instrumentName = "акций";
            }
            else if (instrumentTypeCode == Constants.InstrumentType.INSTRUMENT_TYPE_CODE_BONDS)
            {
                instrumentName = "облигаций";
            }
            else if (instrumentTypeCode == Constants.InstrumentType.INSTRUMENT_TYPE_CODE_ETF)
            {
                instrumentName = "ETF";
            }
            else if (instrumentTypeCode == Constants.InstrumentType.INSTRUMENT_TYPE_CODE_CURRENCIES)
            {
                instrumentName = "валюты";
            }

            if (currentInstrumentTypePercentOnAccount > instrumentTypePersentOnAccountTarget + instrumentTypePersentOnAccountDelta)
            {
                messages.Add(GetMessageToDecreaseInstrumentInPortfolio(instrumentName, currentPercentOnAccountPresentationString, instrumentTypePersentOnAccountTargetPresentationString));
            }

            if (currentInstrumentTypePercentOnAccount < instrumentTypePersentOnAccountTarget - instrumentTypePersentOnAccountDelta)
            {
                messages.Add(GetMessageToIncreaseInstrumentInPortfolio(instrumentName, currentPercentOnAccountPresentationString, instrumentTypePersentOnAccountTargetPresentationString));
            }

            return messages;
        }

        private Message GetMessageToIncreaseInstrumentInPortfolio(string code,
            string currentPercentOnAccountPresentationString,
            string instrumentPersentOnAccountTargetPresentationString)
        {
            return new Message
            {
                Type = MessageType.Warning,
                Text = $"Доля {code} (<b>{currentPercentOnAccountPresentationString}%</b>) ниже целевого значения для портфеля (<b>{instrumentPersentOnAccountTargetPresentationString}%</b>). " +
                            $"Рекомендуется увеличить долю {code} в портфеле"
            };

        }

        private Message GetMessageToDecreaseInstrumentInPortfolio(string code,
            string currentPercentOnAccountPresentationString,
            string instrumentPersentOnAccountTargetPresentationString)
        {
            return new Message
            {
                Type = MessageType.Warning,
                Text = $"Доля {code} (<b>{currentPercentOnAccountPresentationString}</b>%) выше целевого значения для портфеля (<b>{instrumentPersentOnAccountTargetPresentationString}%</b>). " +
                            $"Рекомендуется снизить долю {code} в портфеле"
            };
        }

        private Message GetMessageToIncreaseInstrumentInGroup(string code, string groupCode,
            string currentPercentInGroupPresentationString,
            string instrumentPersentInGroupTargetPresentationString)
        {
            return new Message
            {
                Type = MessageType.Warning,
                Text = $"Доля {code} (<b>{currentPercentInGroupPresentationString}%</b>) ниже целевого значения для группы (<b>{instrumentPersentInGroupTargetPresentationString}%</b>). " +
                            $"Рекомендуется увеличить долю {code} в группе {groupCode}"
            };

        }

        private Message GetMessageToDecreaseInstrumentInGroup(string code, string groupTitle,
            string currentPercentInGroupPresentationString,
            string instrumentPersentInGroupTargetPresentationString)
        {
            return new Message
            {
                Type = MessageType.Warning,
                Text = $"Доля {code} (<b>{currentPercentInGroupPresentationString}</b>%) выше целевого значения для группы (<b>{instrumentPersentInGroupTargetPresentationString}%</b>) для группы '{groupTitle}'. " +
                            $"Рекомендуется снизить долю {code} в группе '{groupTitle}'"
            };
        }

    }
}