using desktop.Models;
using desktop.Services;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using desktop.Service;

namespace desktop.ViewModels
{
    public class ReportViewModel : ViewModelBase
    {
        private readonly IAccessTokenRepository _accessTokenRepository;
        private readonly IStatisticRepository _statisticRepository;
        private readonly IReportProductService _reportProductService;

        private readonly ObservableAsPropertyHelper<IEnumerable<ProductStatistic>> _productStatisics;
        private readonly ObservableAsPropertyHelper<bool> _isLoading;

        private readonly ObservableAsPropertyHelper<IEnumerable<(string, float)>> _relativeFrequancies;
        private readonly ObservableAsPropertyHelper<IEnumerable<float>> _accumulatedRelativeFrequancies;
        
        public DateRange DateRange { get; set; }
        public ReactiveCommand<Unit, IEnumerable<ProductStatistic>> LoadProductStatisticsCommand { get; }
        public ReactiveCommand<Unit, Unit> CreateDocReportCommand { get; }
        public IEnumerable<ProductStatistic> ProductStatistics => _productStatisics.Value;
        public bool IsLoading => _isLoading.Value;
        public IEnumerable<(string, float)> RelativeFrequancies => _relativeFrequancies.Value;
        public IEnumerable<float> AccumulatedRelativeFrequancies => _accumulatedRelativeFrequancies.Value;

        public ReportViewModel(IAccessTokenRepository accessTokenRepository, IUpdateTokenService updateTokenService, INotificationService notificationService,
            IStatisticRepository statisticRepository, IReportProductService reportProductService) : base (notificationService, updateTokenService)
        {
            Title = "Отчет по продажам продуктов";
            _accessTokenRepository = accessTokenRepository;
            _statisticRepository = statisticRepository;
            _reportProductService = reportProductService;

            DateRange = new DateRange();

            LoadProductStatisticsCommand = ReactiveCommand.CreateFromTask<IEnumerable<ProductStatistic>>(async LoadProductStatistics =>
              await _statisticRepository.GetProductStatistics(_accessTokenRepository.GetAccessToken(), DateRange));
            LoadProductStatisticsCommand.ThrownExceptions.Subscribe(async x => await CommandExc(x, LoadProductStatisticsCommand));
            LoadProductStatisticsCommand.IsExecuting.ToProperty(this, x => x.IsLoading, out _isLoading);
            _productStatisics = LoadProductStatisticsCommand.ToProperty(this, x => x.ProductStatistics);

            this.WhenAnyValue(x => x.ProductStatistics)
                .Where(x => x != null)
                .Select(p => p.ToList().ConvertAll(t => (t.Product.Name, (float)t.Count / p.Sum(x => x.Count))))
                .ToProperty(this, x => x.RelativeFrequancies, out _relativeFrequancies);
            
            this.WhenAnyValue(x => x.RelativeFrequancies)
                .Where(x => x != null)
                .Select(x =>
                {
                    var rel = x.Select(x => x.Item2).ToList();
                    var accum = new List<float>();
                    if (!rel.Any())
                        return accum;
                    accum.Add(rel[0]);
                    for (int i = 1; i < rel.Count; i++)
                        accum.Add(accum[i - 1] + rel[i]);
                    return accum;
                })
                .ToProperty(this, x => x.AccumulatedRelativeFrequancies, out _accumulatedRelativeFrequancies);
            DateRange.WhenAnyValue(x => x.StartDate, x => x.EndDate)
                .Where((x) => (x.Item1 != null && x.Item2 != null && x.Item1 <= x.Item2) || (x.Item1 == null && x.Item2 == null))
                .Subscribe(_ => LoadProductStatisticsCommand.Execute().Subscribe());
            LoadProductStatisticsCommand.Execute().Subscribe();

            CreateDocReportCommand = ReactiveCommand.CreateFromTask(async () => await _reportProductService.SaveDocument(ProductStatistics.ToArray(), DateRange));
            CreateDocReportCommand.ThrownExceptions.Subscribe(async x => await CommandExc(x, CreateDocReportCommand));
        }
    }
}
