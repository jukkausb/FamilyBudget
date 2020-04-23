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
        private readonly IInvestmentInstrumentRepository _investmentInstrumentRepository;

        public TinkoffInvestmentDataProvider(IExpenditureRepository expenditureRepository,
            IInvestmentInstrumentRepository investmentInstrumentRepository)
        {
            _expenditureRepository = expenditureRepository;
            _investmentInstrumentRepository = investmentInstrumentRepository;
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

                var positionGroups = portfolioPositions.GroupBy(p => p.Type).ToList();

                investmentAccount.TotalBalance = portfolioPositions.Sum(p => p.CurrentTotalInPortfolio) + accountCashRub.Balance;
                investmentAccount.TotalDelta = portfolioPositions.Sum(p => p.CurrentDelta);
                investmentAccount.TotalDeltaType = BusinessHelper.GetDeltaType(investmentAccount.TotalDelta);
                investmentAccount.TotalDeltaPercent = Math.Round(Math.Abs(investmentAccount.TotalDelta / investmentAccount.TotalInvested * 100), 2).ToString("N2");

                decimal stocksTotal = 0;
                decimal etfTotal = 0;
                decimal bondsTotal = 0;
                decimal currenciesTotal = 0;

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
                        groupData.Add(new TinkoffPortfolioPosition
                        {
                            Name = Constants.CURRENCY_NAME_RUB,
                            AvatarImageLink = TinkoffStaticUrlResolver.ResolveAvatarImageLink(Constants.CURRENCY_RUB, Constants.CURRENCY_RUB),
                            CurrentTotalInPortfolio = accountCashRub.Balance
                        });

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
                var totalAccountBalance = investmentAccount.TotalBalance;

                // Check investment instrument rules on account
                investmentAccount.Messages.AddRange(CheckInvestmentInstrumentRulesOnAccount(allInstruments, totalAccountBalance));

                // Check investment instrument type rules on account
                var etfPercentOnAccount = etfTotal / investmentAccount.TotalBalance * 100;
                investmentAccount.Messages.AddRange(CheckInvestmentInstrumentTypeOnAccount(Constants.InstrumentType.INSTRUMENT_TYPE_CODE_ETF, etfPercentOnAccount));
                var bondsPercentOnAccount = bondsTotal / investmentAccount.TotalBalance * 100;
                investmentAccount.Messages.AddRange(CheckInvestmentInstrumentTypeOnAccount(Constants.InstrumentType.INSTRUMENT_TYPE_CODE_BONDS, bondsPercentOnAccount));
                var stocksPercentOnAccount = stocksTotal / investmentAccount.TotalBalance * 100;
                investmentAccount.Messages.AddRange(CheckInvestmentInstrumentTypeOnAccount(Constants.InstrumentType.INSTRUMENT_TYPE_CODE_STOCKS, stocksPercentOnAccount));
                var currenciesPercentOnAccount = currenciesTotal / investmentAccount.TotalBalance * 100;
                investmentAccount.Messages.AddRange(CheckInvestmentInstrumentTypeOnAccount(Constants.InstrumentType.INSTRUMENT_TYPE_CODE_CURRENCIES, currenciesPercentOnAccount));

                investmentAccounts.Add(investmentAccount);
            }

            return investmentAccounts;
        }

        private TinkoffPortfolioGroup BuildGroup(List<TinkoffPortfolioPosition> instrumentsInGroup,
            string groupCode,
            string groupTitle)
        {
            TinkoffPortfolioGroup group = new TinkoffPortfolioGroup();
            group.Positions = instrumentsInGroup;
            group.Code = groupCode;
            group.Title = groupTitle;

            decimal total = instrumentsInGroup.Sum(e => e.CurrentTotalInPortfolio);
            group.CurrentTotalInPortfolio = total;
            group.Messages.AddRange(CheckInvestmentInstrumentRulesInGroup(group, groupTitle));

            return group;
        }

        private List<Message> CheckInvestmentInstrumentRulesInGroup(TinkoffPortfolioGroup group, string groupTitle)
        {
            List<Message> messages = new List<Message>();
            if (group == null)
            {
                return messages;
            }

            var tickers = group.Positions
                .Where(g => !string.IsNullOrEmpty(g.Ticker))
                .ToDictionary(
                    e => e.Ticker,
                    e => e.CurrentTotalInPortfolio / group.CurrentTotalInPortfolio * 100
                );

            foreach (var ticker in tickers)
            {
                string code = ticker.Key;
                decimal currentPercentInGroup = ticker.Value;
                var investmentInstrument = _investmentInstrumentRepository.FindBy(i => i.Code == ticker.Key).FirstOrDefault();
                if (investmentInstrument == null)
                {
                    continue;
                }

                var instrumentPersentInGroupTarget = investmentInstrument.GroupPercent;
                var instrumentPersentInGroupDelta = investmentInstrument.GroupPercentDelta;
                if (!instrumentPersentInGroupTarget.HasValue || !instrumentPersentInGroupDelta.HasValue)
                {
                    // Do not check if rule values are not specified
                    continue;
                }

                string currectPercentInGroupPresentationString = Math.Round(currentPercentInGroup, 2).ToString();
                string instrumentPersentInGroupTargetPresentationString = Math.Round((decimal)instrumentPersentInGroupTarget, 2).ToString();

                if (currentPercentInGroup > instrumentPersentInGroupTarget + instrumentPersentInGroupDelta)
                {
                    messages.Add(GetMessageToDecreaseInstrumentInGroup(code, groupTitle, currectPercentInGroupPresentationString, instrumentPersentInGroupTargetPresentationString));
                }

                if (currentPercentInGroup < instrumentPersentInGroupTarget - instrumentPersentInGroupDelta)
                {
                    messages.Add(GetMessageToIncreaseInstrumentInGroup(code, groupTitle, currectPercentInGroupPresentationString, instrumentPersentInGroupTargetPresentationString));
                }
            }

            return messages;
        }

        private List<Message> CheckInvestmentInstrumentRulesOnAccount(List<TinkoffPortfolioPosition> instruments, decimal totalAccountBalance)
        {
            List<Message> messages = new List<Message>();
            if (instruments == null)
            {
                return messages;
            }

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

                var currentPercentOnAccount = instrument.CurrentTotalInPortfolio / totalAccountBalance * 100;
                string currentPercentOnAccountPresentationString = Math.Round(currentPercentOnAccount, 2).ToString();
                string instrumentPersentOnAccountTargetPresentationString = Math.Round((decimal)instrumentPersentOnAccountTarget, 2).ToString();

                if (currentPercentOnAccount > instrumentPersentOnAccountTarget + instrumentPersentOnAccountDelta)
                {
                    messages.Add(GetMessageToDecreaseInstrumentInPortfolio(instrument.Ticker, currentPercentOnAccountPresentationString, instrumentPersentOnAccountTargetPresentationString));
                }

                if (currentPercentOnAccount < instrumentPersentOnAccountTarget - instrumentPersentOnAccountDelta)
                {
                    messages.Add(GetMessageToIncreaseInstrumentInPortfolio(instrument.Ticker, currentPercentOnAccountPresentationString, instrumentPersentOnAccountTargetPresentationString));
                }
            }

            return messages;
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