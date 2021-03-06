﻿using FamilyBudget.v3.App_CodeBase.Tinkoff.Models;
using FamilyBudget.v3.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System;
using FamilyBudget.v3.App_Helpers;
using FamilyBudget.v3.Models.Repository.Interfaces;
using FamilyBudget.v3.App_DataModel;

namespace FamilyBudget.v3.App_CodeBase.Tinkoff
{
    public interface ITinkoffInvestmentDataProvider
    {
        List<InvestmentAccount> GetInvestmentAccounts();
        double GetTotalInvested();
    }

    public class TinkoffInvestmentDataProvider : ITinkoffInvestmentDataProvider
    {
        private readonly IExpenditureRepository _expenditureRepository;
        private readonly IInvestmentInstrumentRepository _investmentInstrumentRepository;
        private readonly IInvestmentInstrumentTypeRepository _investmentInstrumentTypeRepository;
        private readonly IInvestmentInstrumentMarketRepository _investmentInstrumentMarketRepository;
        private readonly ICurrencyProvider _currencyProvider;

        public TinkoffInvestmentDataProvider(IExpenditureRepository expenditureRepository,
            IInvestmentInstrumentRepository investmentInstrumentRepository,
            IInvestmentInstrumentMarketRepository investmentInstrumentMarketRepository,
            IInvestmentInstrumentTypeRepository investmentInstrumentTypeRepository,
            ICurrencyProvider currencyProvider)
        {
            _expenditureRepository = expenditureRepository;
            _investmentInstrumentRepository = investmentInstrumentRepository;
            _investmentInstrumentMarketRepository = investmentInstrumentMarketRepository;
            _investmentInstrumentTypeRepository = investmentInstrumentTypeRepository;
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

        private List<CurrencyPosition> GetPortfolioCurrencies(string brokerAccountId)
        {
            ITinkoffPortfolioDataRetriever tinkoffPortfolioDataRetriever = new TinkoffPortfolioDataRetriever(GetToken());
            List<CurrencyPosition> positions = tinkoffPortfolioDataRetriever.GetTinkoffPortfolioCurrencies(brokerAccountId);
            return positions;
        }

        public double GetTotalInvested()
        {
            double investmentsToIIS = (double)BusinessHelper.GetIISExpenditures(_expenditureRepository).Sum(e => e.Summa);
            double investmentsBrokerAccount = (double)BusinessHelper.GetBrokerAccountExpenditures(_expenditureRepository).Sum(e => e.Summa);
            double totalInvestments = investmentsToIIS + investmentsBrokerAccount;
            return totalInvestments;
        }

        public List<InvestmentAccount> GetInvestmentAccounts()
        {
            double investmentsToIIS = (double)BusinessHelper.GetIISExpenditures(_expenditureRepository).Sum(e => e.Summa);
            double investmentsBrokerAccount = (double)BusinessHelper.GetBrokerAccountExpenditures(_expenditureRepository).Sum(e => e.Summa);
            double totalInvestments = investmentsToIIS + investmentsBrokerAccount;

            ITinkoffUserAccountDataRetriever tinkoffUserAccountDataRetriever = new TinkoffUserAccountDataRetriever(GetToken());
            List<UserAccount> accounts = tinkoffUserAccountDataRetriever.GetTinkoffUserAccounts();

            List<InvestmentAccount> investmentAccounts = new List<InvestmentAccount>();

            foreach (var account in accounts)
            {
                double totalInvested = 0;
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

        private InvestmentAccount BuildInvestmentAccount(UserAccount account, string name, double totalInvested)
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

            var accountCashRub = GetPortfolioCurrencies(account.BrokerAccountId).
                Where(c => c.Currency.ToString().ToUpper() == Constants.CURRENCY_RUB).
                FirstOrDefault();

            TryUpdateCurrencyRates(portfolioPositions);

            double investmentAccountTotalBalance = GetPositionsTotalInPortfolio(portfolioPositions) + accountCashRub.Balance;
            double investmentAccountTotalDelta = GetInvestmentAccountTotalDelta(portfolioPositions);

            investmentAccount.Totals = new MoneyWithDeltaModel(investmentAccountTotalBalance, Constants.CURRENCY_RUB, investmentAccountTotalDelta);

            foreach (var portfolioPosition in portfolioPositions)
            {
                portfolioPosition.CurrentPercentInPortfolio = (portfolioPosition.CurrentTotalInPortfolio / investmentAccountTotalBalance * 100).Round();
                var investmentInstrument = _investmentInstrumentRepository.FindBy(i => i.Code == portfolioPosition.Ticker).FirstOrDefault();
                ApplyTinkoffPortfolioPositionCustomAttributes(portfolioPosition, investmentInstrument);
            }

            double stocksTotal = 0;
            double etfTotal = 0;
            double bondsTotal = 0;
            double currenciesTotal = 0;

            var positionGroups = portfolioPositions.GroupBy(p => p.Type).ToList();

            foreach (var positionGroup in positionGroups)
            {
                TinkoffPortfolioTableGroup group = null;
                var groupData = positionGroup.OrderBy(p => p.Name).ToList();

                if (positionGroup.Key == InstrumentType.Bond)
                {
                    group = BuildGroup(groupData,
                        Constants.InstrumentType.INSTRUMENT_TYPE_CODE_BONDS,
                        Constants.InstrumentType.INSTRUMENT_TYPE_TITLE_BONDS);
                    bondsTotal = GetPositionsTotalInPortfolio(groupData);
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

                    ApplyTinkoffPortfolioPositionCustomAttributes(rublePortfolioPosition, null);

                    groupData.Add(rublePortfolioPosition);

                    group = BuildGroup(groupData,
                        Constants.InstrumentType.INSTRUMENT_TYPE_CODE_CURRENCIES,
                        Constants.InstrumentType.INSTRUMENT_TYPE_TITLE_CURRENCIES);
                    currenciesTotal = GetPositionsTotalInPortfolio(groupData);
                }
                if (positionGroup.Key == InstrumentType.Etf)
                {
                    group = BuildGroup(groupData,
                        Constants.InstrumentType.INSTRUMENT_TYPE_CODE_ETF,
                        Constants.InstrumentType.INSTRUMENT_TYPE_TITLE_ETF);
                    etfTotal = GetPositionsTotalInPortfolio(groupData);
                }
                if (positionGroup.Key == InstrumentType.Stock)
                {
                    group = BuildGroup(groupData,
                        Constants.InstrumentType.INSTRUMENT_TYPE_CODE_STOCKS,
                        Constants.InstrumentType.INSTRUMENT_TYPE_TITLE_STOCKS);
                    stocksTotal = GetPositionsTotalInPortfolio(groupData);
                }

                if (group != null)
                {
                    investmentAccount.TableGroups.Add(group);
                }
            }

            AddMessages(investmentAccount);
            AddTypeGroups(investmentAccount, portfolioPositions, accountCashRub.Balance);
            AddMarketGroups(investmentAccount, portfolioPositions);

            return investmentAccount;
        }

        private void AddMessages(InvestmentAccount investmentAccount)
        {
            var allInstruments = investmentAccount.TableGroups.SelectMany(g => g.Items).Select(p => p.Position).ToList();
            double totalAccountBalance = investmentAccount.Totals.Value;

            // Check investment instrument rules
            // investmentAccount.MessageGroups.AddRange(CheckInvestmentInstrumentRules(allInstruments, totalAccountBalance));
            investmentAccount.MessageGroups.AddRange(CheckInvestmentInstrumentTypeRules(allInstruments, totalAccountBalance));
            investmentAccount.MessageGroups.AddRange(CheckInvestmentInstrumentMarketRules(allInstruments, totalAccountBalance));
        }

        private void AddTypeGroups(InvestmentAccount investmentAccount,
            List<TinkoffPortfolioPosition> portfolioPositions,
            double rubleCashBalance)
        {
            if (investmentAccount == null || portfolioPositions == null)
            {
                return;
            }

            var positionMarketGroups = portfolioPositions
                .Where(p => p.CustomType != null)
                .GroupBy(p => p.CustomType.Code)
                .Where(p => !string.IsNullOrEmpty(p.Key))
                .ToList();

            var total = GetPositionsTotalInPortfolio(portfolioPositions);

            foreach (var group in positionMarketGroups)
            {
                string typeCode = group.Key;
                var investmentInstrumentType = _investmentInstrumentTypeRepository.FindBy(i => i.Code == typeCode).FirstOrDefault();
                var currentTotalInPortfolio = GetPositionsTotalInPortfolio(group.ToList());

                PortfolioDiagramGroupItem portfolioDiagramGroupItem = new PortfolioDiagramGroupItem
                {
                    Name = investmentInstrumentType.Name,
                    CurrentTotalInPortfolio = currentTotalInPortfolio,
                    CurrentPercentInPortfolio = (currentTotalInPortfolio / total * 100).Round()
                };

                if (typeCode == Constants.InstrumentMarket.INSTRUMENT_MARKET_CODE_CASH)
                {
                    portfolioDiagramGroupItem.CurrentTotalInPortfolio += rubleCashBalance;
                }

                DiagramHelper.ApplyPieDiagramColors(portfolioDiagramGroupItem, investmentInstrumentType);
                investmentAccount.TypeGroupItems.Add(portfolioDiagramGroupItem);
            }
        }

        private void AddMarketGroups(InvestmentAccount investmentAccount,
            List<TinkoffPortfolioPosition> portfolioPositions)
        {
            if (investmentAccount == null || portfolioPositions == null)
            {
                return;
            }

            var positionMarketGroups = portfolioPositions
                .Where(p => p.CustomMarket != null)
                .GroupBy(p => p.CustomMarket.Code)
                .Where(p => !string.IsNullOrEmpty(p.Key))
                .ToList();

            var total = GetPositionsTotalInPortfolio(portfolioPositions);

            foreach (var group in positionMarketGroups)
            {
                string marketCode = group.Key;
                var investmentInstrumentMarket = _investmentInstrumentMarketRepository.FindBy(i => i.Code == marketCode).FirstOrDefault();
                double currentTotalInPortfolio = GetPositionsTotalInPortfolio(group.ToList());

                PortfolioDiagramGroupItem portfolioDiagramGroupItem = new PortfolioDiagramGroupItem
                {
                    Name = investmentInstrumentMarket.Name,
                    CurrentTotalInPortfolio = currentTotalInPortfolio,
                    CurrentPercentInPortfolio = (currentTotalInPortfolio / total * 100).Round()
                };

                DiagramHelper.ApplyPieDiagramColors(portfolioDiagramGroupItem, investmentInstrumentMarket);
                investmentAccount.MarketGroupItems.Add(portfolioDiagramGroupItem);
            }
        }

        private void ApplyTinkoffPortfolioPositionCustomAttributes(TinkoffPortfolioPosition portfolioPosition, InvestmentInstrument investmentInstrument)
        {
            if (portfolioPosition == null)
            {
                return;
            }

            if (investmentInstrument != null)
            {
                portfolioPosition.CustomType = investmentInstrument.InvestmentInstrumentType;
                portfolioPosition.CustomMarket = investmentInstrument.InvestmentInstrumentMarket;
            }

            portfolioPosition.AvatarImageLink = TinkoffStaticUrlResolver.ResolveAvatarImageLink(portfolioPosition, investmentInstrument);
            portfolioPosition.TickerPageLink = TinkoffStaticUrlResolver.ResolveExternalPageId(portfolioPosition, investmentInstrument);

            DiagramHelper.ApplyPieDiagramColors(portfolioPosition, investmentInstrument);
        }

        private double GetPositionsTotalInPortfolio(List<TinkoffPortfolioPosition> portfolioPositions)
        {
            double total = 0;

            foreach (var portfolioPosition in portfolioPositions)
            {
                if (string.IsNullOrEmpty(portfolioPosition.Currency) || portfolioPosition.Currency.ToUpper() == Constants.CURRENCY_RUB)
                {
                    total += portfolioPosition.CurrentTotalInPortfolio;
                }
                else
                {
                    double rate = _currencyProvider.GetSellCurrencyRate(portfolioPosition.Currency.ToUpper(), Constants.CURRENCY_RUB);
                    total += portfolioPosition.CurrentTotalInPortfolio * rate;
                }
            }

            return total.Round();
        }

        private double GetInvestmentAccountTotalDelta(List<TinkoffPortfolioPosition> portfolioPositions)
        {
            double total = 0;

            foreach (var portfolioPosition in portfolioPositions)
            {
                if (string.IsNullOrEmpty(portfolioPosition.Currency) || portfolioPosition.Currency.ToUpper() == Constants.CURRENCY_RUB)
                {
                    total += portfolioPosition.CurrentDelta;
                }
                else
                {
                    double rate = _currencyProvider.GetSellCurrencyRate(portfolioPosition.Currency.ToUpper(), Constants.CURRENCY_RUB);
                    total += portfolioPosition.CurrentDelta * rate;
                }
            }

            return total;
        }

        private TinkoffPortfolioTableGroup BuildGroup(List<TinkoffPortfolioPosition> positions,
            string groupCode,
            string groupTitle)
        {
            double totalBalanceOfPositions = GetPositionsTotalInPortfolio(positions);

            TinkoffPortfolioTableGroup group = new TinkoffPortfolioTableGroup();

            foreach (var position in positions)
            {
                var groupItem = new TinkoffPortfolioTableGroupItem
                {
                    Code = position.Ticker,
                    Name = position.Name,
                    Position = position,
                    CurrentTotalInPortfolio = GetPositionCurrentTotalInPortfolio(position),
                    CurrentPercentInPortfolio = GetPositionCurrentPercentInPortfolio(position, totalBalanceOfPositions)
                };

                DiagramHelper.ApplyPieDiagramColors(groupItem, position);

                group.Items.Add(groupItem);
            }

            group.Code = groupCode;
            group.Name = groupTitle;
            group.CurrentTotalInPortfolio = totalBalanceOfPositions;

            var investmentInstrument = _investmentInstrumentRepository.FindBy(i => i.Code == groupCode).FirstOrDefault();

            DiagramHelper.ApplyPieDiagramColors(group, investmentInstrument);

            return group;
        }

        /// <summary>
        /// Checks investment instrument type rules
        /// </summary>
        private List<MessageGroup> CheckInvestmentInstrumentRules(List<TinkoffPortfolioPosition> instruments,
            double totalAccountBalance)
        {
            List<MessageGroup> messageGroups = new List<MessageGroup>();
            if (instruments == null)
            {
                return messageGroups;
            }

            MessageGroup msg = new MessageGroup()
            {
                Name = Constants.InstrumentType.INSTRUMENT_TYPE_TITLE_BY_INSTRUMENT_DETAILS
            };

            foreach (var instrument in instruments)
            {
                string ticker = instrument.Ticker;
                if (string.IsNullOrEmpty(ticker))
                {
                    continue;
                }

                var investmentInstrument = _investmentInstrumentRepository.FindBy(i => i.Code == ticker).FirstOrDefault();
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

                double currentPercentOnAccount = GetPositionCurrentPercentInPortfolio(instrument, totalAccountBalance);
                string currentPercentOnAccountPresentationString = Math.Round(currentPercentOnAccount, 2).ToString();
                string instrumentPersentOnAccountTargetPresentationString = Math.Round((decimal)instrumentPersentOnAccountTarget, 2).ToString();

                if (currentPercentOnAccount > instrumentPersentOnAccountTarget + instrumentPersentOnAccountDelta)
                {
                    msg.Messages.Add(GetMessageToDecreaseInstrumentInPortfolio(instrument.Ticker, currentPercentOnAccountPresentationString, instrumentPersentOnAccountTargetPresentationString));
                }

                if (currentPercentOnAccount < instrumentPersentOnAccountTarget - instrumentPersentOnAccountDelta)
                {
                    msg.Messages.Add(GetMessageToIncreaseInstrumentInPortfolio(instrument.Ticker, currentPercentOnAccountPresentationString, instrumentPersentOnAccountTargetPresentationString));
                }
            }

            messageGroups.Add(msg);

            return messageGroups;
        }

        /// <summary>
        /// Checks investment instrument type rules
        /// </summary>
        private List<MessageGroup> CheckInvestmentInstrumentTypeRules(List<TinkoffPortfolioPosition> instruments, double totalAccountBalance)
        {
            List<MessageGroup> messageGroups = new List<MessageGroup>();
            if (instruments == null)
            {
                return messageGroups;
            }

            var allTypes = _investmentInstrumentTypeRepository.GetAll().ToList();
            foreach (var type in allTypes)
            {
                MessageGroup msg = new MessageGroup()
                {
                    Name = type.Name
                };

                var instrumentsOfType = instruments.Where(p => p.CustomType != null && p.CustomType.Code == type.Code);

                var instrumentPersentOnAccountTarget = type.PortfolioPercent;
                var instrumentPersentOnAccountDelta = type.PortfolioPercentDelta;
                if (!instrumentPersentOnAccountTarget.HasValue || !instrumentPersentOnAccountTarget.HasValue)
                {
                    // Do not check if rule values are not specified
                    continue;
                }

                double currentGroupTotalInPortfolio = instrumentsOfType.Sum(p => GetPositionCurrentTotalInPortfolio(p));
                double currentPercentOnAccount = currentGroupTotalInPortfolio / totalAccountBalance * 100;
                string currentPercentOnAccountPresentationString = Math.Round(currentPercentOnAccount, 2).ToString();
                string instrumentPersentOnAccountTargetPresentationString = Math.Round((decimal)instrumentPersentOnAccountTarget, 2).ToString();

                if (currentPercentOnAccount > instrumentPersentOnAccountTarget + instrumentPersentOnAccountDelta)
                {
                    msg.Messages.Add(GetMessageToDecreaseInstrumentTypeInPortfolio(type, currentPercentOnAccountPresentationString, instrumentPersentOnAccountTargetPresentationString));
                }

                if (currentPercentOnAccount < instrumentPersentOnAccountTarget - instrumentPersentOnAccountDelta)
                {
                    msg.Messages.Add(GetMessageToIncreaseInstrumentTypeInPortfolio(type, currentPercentOnAccountPresentationString, instrumentPersentOnAccountTargetPresentationString));
                }

                messageGroups.Add(msg);
            }

            return messageGroups;
        }

        /// <summary>
        /// Checks investment instrument market rules
        /// </summary>
        private List<MessageGroup> CheckInvestmentInstrumentMarketRules(List<TinkoffPortfolioPosition> instruments, double totalAccountBalance)
        {
            List<MessageGroup> messageGroups = new List<MessageGroup>();
            if (instruments == null)
            {
                return messageGroups;
            }

            var allMarkets = _investmentInstrumentMarketRepository.GetAll().ToList();
            foreach (var market in allMarkets)
            {
                MessageGroup msg = new MessageGroup()
                {
                    Name = market.Name
                };

                var instrumentsOnMarket = instruments.Where(p => p.CustomMarket != null && p.CustomMarket.Code == market.Code);

                var instrumentPersentOnAccountTarget = market.PortfolioPercent;
                var instrumentPersentOnAccountDelta = market.PortfolioPercentDelta;
                if (!instrumentPersentOnAccountTarget.HasValue || !instrumentPersentOnAccountTarget.HasValue)
                {
                    // Do not check if rule values are not specified
                    continue;
                }

                double currentGroupTotalInPortfolio = instrumentsOnMarket.Sum(p => GetPositionCurrentTotalInPortfolio(p));
                double currentPercentOnAccount = currentGroupTotalInPortfolio / totalAccountBalance * 100;
                string currentPercentOnAccountPresentationString = Math.Round(currentPercentOnAccount, 2).ToString();
                string instrumentPersentOnAccountTargetPresentationString = Math.Round((decimal)instrumentPersentOnAccountTarget, 2).ToString();

                if (currentPercentOnAccount > instrumentPersentOnAccountTarget + instrumentPersentOnAccountDelta)
                {
                    msg.Messages.Add(GetMessageToDecreaseMarketInPortfolio(market, currentPercentOnAccountPresentationString, instrumentPersentOnAccountTargetPresentationString));
                }

                if (currentPercentOnAccount < instrumentPersentOnAccountTarget - instrumentPersentOnAccountDelta)
                {
                    msg.Messages.Add(GetMessageToIncreaseMarketInPortfolio(market, currentPercentOnAccountPresentationString, instrumentPersentOnAccountTargetPresentationString));
                }

                messageGroups.Add(msg);
            }

            return messageGroups;
        }





        private double GetPositionCurrentPercentInPortfolio(TinkoffPortfolioPosition position, double total)
        {
            double currentPercentOnAccount = 0;

            if (position == null)
            {
                return currentPercentOnAccount;
            }

            if (string.IsNullOrEmpty(position.Currency) || position.Currency.ToUpper() == Constants.CURRENCY_RUB)
            {
                currentPercentOnAccount = position.CurrentTotalInPortfolio / total * 100;
            }
            else
            {
                double rate = _currencyProvider.GetSellCurrencyRate(position.Currency.ToUpper(), Constants.CURRENCY_RUB);
                currentPercentOnAccount = position.CurrentTotalInPortfolio * rate / total * 100;
            }
            return currentPercentOnAccount.Round();
        }

        private double GetPositionCurrentTotalInPortfolio(TinkoffPortfolioPosition position)
        {
            double currentPercentOnAccount = 0;

            if (position == null)
            {
                return currentPercentOnAccount;
            }

            if (string.IsNullOrEmpty(position.Currency) || position.Currency.ToUpper() == Constants.CURRENCY_RUB)
            {
                currentPercentOnAccount = position.CurrentTotalInPortfolio;
            }
            else
            {
                double rate = _currencyProvider.GetSellCurrencyRate(position.Currency.ToUpper(), Constants.CURRENCY_RUB);
                currentPercentOnAccount = position.CurrentTotalInPortfolio * rate;
            }
            return currentPercentOnAccount.Round();
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

        private Message GetMessageToIncreaseInstrumentTypeInPortfolio(InvestmentInstrumentType investmentInstrumentType,
            string currentPercentInPortfolioPresentationString,
            string instrumentPersentInPortfolioTargetPresentationString)
        {
            if (investmentInstrumentType == null)
            {
                return null;
            }

            return new Message
            {
                Type = MessageType.Warning,
                Text = $"Доля '{investmentInstrumentType.Name}' (<b>{currentPercentInPortfolioPresentationString}%</b>) " +
                       $"ниже целевого значения (<b>{instrumentPersentInPortfolioTargetPresentationString}%</b>) в портфеле. " +
                       $"Рекомендуется увеличить долю '{investmentInstrumentType.Name}' в портфеле"
            };

        }

        private Message GetMessageToDecreaseInstrumentTypeInPortfolio(InvestmentInstrumentType investmentInstrumentType,
            string currentPercentInPortfolioPresentationString,
            string instrumentPersentInPortfolioTargetPresentationString)
        {
            if (investmentInstrumentType == null)
            {
                return null;
            }

            return new Message
            {
                Type = MessageType.Warning,
                Text = $"Доля '{investmentInstrumentType.Name}' (<b>{currentPercentInPortfolioPresentationString}%</b>) " +
                       $"выше целевого значения (<b>{instrumentPersentInPortfolioTargetPresentationString}%</b>) в портфеле. " +
                       $"Рекомендуется снизить долю '{investmentInstrumentType.Name}' в портфеле"
            };
        }

        private Message GetMessageToIncreaseMarketInPortfolio(InvestmentInstrumentMarket investmentInstrumentMarket,
            string currentPercentInPortfolioPresentationString,
            string instrumentPersentInPortfolioTargetPresentationString)
        {
            if (investmentInstrumentMarket == null)
            {
                return null;
            }

            return new Message
            {
                Type = MessageType.Warning,
                Text = $"Доля '{investmentInstrumentMarket.Name}' (<b>{currentPercentInPortfolioPresentationString}%</b>) " +
                       $"ниже целевого значения (<b>{instrumentPersentInPortfolioTargetPresentationString}%</b>) в портфеле. " +
                       $"Рекомендуется увеличить долю '{investmentInstrumentMarket.Name}' в портфеле"
            };
        }

        private Message GetMessageToDecreaseMarketInPortfolio(InvestmentInstrumentMarket investmentInstrumentMarket,
            string currentPercentInPortfolioPresentationString,
            string instrumentPersentInPortfolioTargetPresentationString)
        {
            if (investmentInstrumentMarket == null)
            {
                return null;
            }

            return new Message
            {
                Type = MessageType.Warning,
                Text = $"Доля '{investmentInstrumentMarket.Name}' (<b>{currentPercentInPortfolioPresentationString}%</b>) " +
                       $"выше целевого значения (<b>{instrumentPersentInPortfolioTargetPresentationString}%</b>) в портфеле. " +
                       $"Рекомендуется снизить долю '{investmentInstrumentMarket.Name}' в портфеле"
            };
        }

        private void TryUpdateCurrencyRates(List<TinkoffPortfolioPosition> portfolioPositions)
        {
            if (portfolioPositions == null)
            {
                return;
            }

            var usdPosition = portfolioPositions.FirstOrDefault(x => x.Figi == Constants.USD_FIGI);
            if (usdPosition != null)
            {
                var usdRate = usdPosition.CurrentPriceInMarket;
                _currencyProvider.SetCurrencyRate(Constants.CURRENCY_USD, Constants.CURRENCY_RUB, usdRate);
            }

            var eurPosition = portfolioPositions.FirstOrDefault(x => x.Figi == Constants.EUR_FIGI);
            if (eurPosition != null)
            {
                var eurRate = eurPosition.CurrentPriceInMarket;
                _currencyProvider.SetCurrencyRate(Constants.CURRENCY_EUR, Constants.CURRENCY_RUB, eurRate);
            }
        }
    }
}