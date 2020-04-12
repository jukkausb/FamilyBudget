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

namespace FamilyBudget.v3.App_CodeBase.Tinkoff
{
    public interface ITinkoffInvestmentDataProvider
    {
        List<InvestmentAccount> GetInvestmentAccounts();
    }

    public class TinkoffInvestmentDataProvider : ITinkoffInvestmentDataProvider
    {
        private readonly IExpenditureRepository _expenditureRepository;
        private readonly IInvestmentRulesEtfRepository _investmentRulesEtfRepository;
        private readonly IInvestmentRulesInstrumentsRepository _investmentRulesInstrumentsRepository;

        public TinkoffInvestmentDataProvider(IExpenditureRepository expenditureRepository,
            IInvestmentRulesEtfRepository investmentRulesEtfRepository,
            IInvestmentRulesInstrumentsRepository investmentRulesInstrumentsRepository)
        {
            _expenditureRepository = expenditureRepository;
            _investmentRulesEtfRepository = investmentRulesEtfRepository;
            _investmentRulesInstrumentsRepository = investmentRulesInstrumentsRepository;
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
                InvestmentAccount investmentAccount = new InvestmentAccount()
                {
                    Id = account.BrokerAccountId,
                    Currency = Constants.CURRENCY_RUB // Инвестирую только в рублях (это Крым, детка)
                };

                if (account.BrokerAccountType == BrokerAccountType.TinkoffIis)
                {
                    investmentAccount.Name = "ИИС";
                    investmentAccount.TotalInvested = investmentsToIIS;
                    investmentAccount.IsActive = false;
                }
                else
                {
                    investmentAccount.Name = "Брокерский счет";
                    investmentAccount.TotalInvested = investmentsBrokerAccount;
                    investmentAccount.IsActive = true;
                }

                var portfolioPositions = GetPortfolioPositions(account.BrokerAccountId);
                var accountCashRub = GetPortfolioCurrencies(account.BrokerAccountId).
                    Where(c => c.Currency.ToString().ToUpper() == Constants.CURRENCY_RUB).
                    FirstOrDefault();

                var groups = portfolioPositions.GroupBy(p => p.Type).ToList();

                investmentAccount.TotalBalance = portfolioPositions.Sum(p => p.CurrentTotalInPortfolio) + accountCashRub.Balance;
                investmentAccount.TotalDelta = portfolioPositions.Sum(p => p.CurrentDelta);
                investmentAccount.TotalDeltaPercent = Math.Round(Math.Abs(investmentAccount.TotalDelta / investmentAccount.TotalInvested * 100), 2).ToString("N2");

                decimal etfTotal = 0;

                foreach (var group in groups)
                {
                    if (group.Key == InstrumentType.Bond)
                    {
                        investmentAccount.Bonds = group.OrderBy(p => p.Name).ToList();
                    }
                    if (group.Key == InstrumentType.Currency)
                    {
                        investmentAccount.Currencies = group.OrderBy(p => p.Name).ToList();
                        investmentAccount.Currencies.Add(new TinkoffPortfolioPosition
                        {
                            Name = Constants.CURRENCY_NAME_RUB,
                            Isin = Constants.CURRENCY_RUB,
                            CurrentTotalInPortfolio = accountCashRub.Balance
                        });
                    }
                    if (group.Key == InstrumentType.Etf)
                    {
                        investmentAccount.Etfs = group.OrderBy(p => p.Name).ToList();
                        etfTotal = investmentAccount.Etfs.Sum(e => e.CurrentTotalInPortfolio);
                        investmentAccount.Messages.AddRange(CheckInvestmentRulesEtf(investmentAccount.Etfs.ToDictionary(
                            e => e.Ticker,
                            e => e.CurrentTotalInPortfolio / etfTotal * 100
                            )));
                    }
                    if (group.Key == InstrumentType.Stock)
                    {
                        investmentAccount.Stocks = group.OrderBy(p => p.Name).ToList();
                    }
                }

                // Check investment rules (instruments)
                if (etfTotal == 0)
                {
                    // just refresh value
                    investmentAccount.Etfs.Sum(e => e.CurrentTotalInPortfolio);
                }

                var etfPercentOnAccount = etfTotal / investmentAccount.TotalBalance * 100;
                investmentAccount.Messages.AddRange(CheckInvestmentRulesInstrument(Constants.Investment.INSTRUMENT_TYPE_CODE_ETF, etfPercentOnAccount));

                var stocksTotal = investmentAccount.Stocks.Sum(e => e.CurrentTotalInPortfolio);
                var stocksPercentOnAccount = stocksTotal / investmentAccount.TotalBalance * 100;
                investmentAccount.Messages.AddRange(CheckInvestmentRulesInstrument(Constants.Investment.INSTRUMENT_TYPE_CODE_STOCKS, stocksPercentOnAccount));

                investmentAccounts.Add(investmentAccount);
            }

            return investmentAccounts;
        }

        private List<Message> CheckInvestmentRulesEtf(Dictionary<string, decimal> tickers)
        {
            List<Message> messages = new List<Message>();

            foreach (var ticker in tickers)
            {
                string code = ticker.Key;
                decimal currentPercentOnAccount = ticker.Value;
                var investmentRule = _investmentRulesEtfRepository.GetAll().FirstOrDefault(i => i.Ticker == ticker.Key);
                if (investmentRule == null)
                {
                    continue;
                }

                var instrumentPersentOnAccountTarget = investmentRule.PortfolioPercent;
                var instrumentPersentOnAccountDelta = investmentRule.PortfolioPercentDelta;

                string currectPercentOnAccountPresentationString = Math.Round(currentPercentOnAccount, 2).ToString();
                string instrumentPersentOnAccountTargetPresentationString = Math.Round((decimal)instrumentPersentOnAccountTarget, 2).ToString();

                if (currentPercentOnAccount > instrumentPersentOnAccountTarget + instrumentPersentOnAccountDelta)
                {
                    messages.Add(GetMessageToDecrease(code, currectPercentOnAccountPresentationString, instrumentPersentOnAccountTargetPresentationString));
                }

                if (currentPercentOnAccount < instrumentPersentOnAccountTarget - instrumentPersentOnAccountDelta)
                {
                    messages.Add(GetMessageToIncrease(code, currectPercentOnAccountPresentationString, instrumentPersentOnAccountTargetPresentationString));
                }
            }

            return messages;
        }

        private List<Message> CheckInvestmentRulesInstrument(string instrumentCode, decimal currentPercentOnAccount)
        {
            List<Message> messages = new List<Message>();

            var investmentRule = _investmentRulesInstrumentsRepository.GetAll().FirstOrDefault(i => i.Code == instrumentCode);
            var instrumentPersentOnAccountTarget = investmentRule.PortfolioPercent;
            var instrumentPersentOnAccountDelta = investmentRule.PortfolioPercentDelta;

            string currentPercentOnAccountPresentationString = Math.Round(currentPercentOnAccount, 2).ToString();
            string instrumentPersentOnAccountTargetPresentationString = Math.Round((decimal)instrumentPersentOnAccountTarget, 2).ToString();

            string instrumentName = instrumentCode;
            if (instrumentCode == Constants.Investment.INSTRUMENT_TYPE_CODE_STOCKS)
            {
                instrumentName = "акций";
            }

            if (currentPercentOnAccount > instrumentPersentOnAccountTarget + instrumentPersentOnAccountDelta)
            {
                messages.Add(GetMessageToDecrease(instrumentCode, currentPercentOnAccountPresentationString, instrumentPersentOnAccountTargetPresentationString));
            }

            if (currentPercentOnAccount < instrumentPersentOnAccountTarget - instrumentPersentOnAccountDelta)
            {
                messages.Add(GetMessageToIncrease(instrumentCode, currentPercentOnAccountPresentationString, instrumentPersentOnAccountTargetPresentationString));
            }

            return messages;
        }

        private Message GetMessageToIncrease(string code, string currentPercentOnAccountPresentationString, string instrumentPersentOnAccountTargetPresentationString)
        {
            return new Message
            {
                Type = MessageType.Warning,
                Text = $"Доля {code} (<b>{currentPercentOnAccountPresentationString}%</b>) ниже целевого значения для портфеля (<b>{instrumentPersentOnAccountTargetPresentationString}%</b>). " +
                            $"Рекомендуется, увеличить долю {code} в портфеле"
            };

        }

        private Message GetMessageToDecrease(string code, string currentPercentOnAccountPresentationString, string instrumentPersentOnAccountTargetPresentationString)
        {
            return new Message
            {
                Type = MessageType.Warning,
                Text = $"Доля {code} (<b>{currentPercentOnAccountPresentationString}</b>%) выше целевого значения для портфеля (<b>{instrumentPersentOnAccountTargetPresentationString}%</b>). " +
                            $"Рекомендуется, снизить долю {code} в портфеле"
            };
        }

    }
}