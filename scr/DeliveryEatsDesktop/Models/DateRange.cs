using System;
using ReactiveUI;

namespace desktop.Models;

public class DateRange : ReactiveObject
{
    private DateTime? _startDate;
    private DateTime? _endDate;

    public DateTime? StartDate
    {
        get => _startDate;
        set => this.RaiseAndSetIfChanged(ref _startDate, value);
    }
    public DateTime? EndDate
    {
        get => _endDate;
        set => this.RaiseAndSetIfChanged(ref _endDate, value);
    }
}