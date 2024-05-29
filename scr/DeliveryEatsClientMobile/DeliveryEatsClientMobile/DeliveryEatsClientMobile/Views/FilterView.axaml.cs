using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using DeliveryEatsClientMobile.ViewModels;

namespace DeliveryEatsClientMobile.Views;

public partial class FilterView : UserControl
{
    private bool _isPressed;
    private double _shift;
    private DispatcherTimer _timer;
    private FilterViewModel _viewModel;

    public FilterView()
    {
        InitializeComponent();
        _timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(100) };
        _timer.Tick += (sender, e) => { _isPressed = true; _timer.Stop(); };
    }

    private void StyledElement_OnDataContextChanged(object? sender, EventArgs e)
    {
        _viewModel = DataContext as FilterViewModel;
    }
    
    private void BottomSheet_OnLoaded(object? sender, RoutedEventArgs e)
    {
        double desiredHeight = TopLevel.GetTopLevel(this).Height * 0.975;
        BottomSheet.Height = desiredHeight;
        BottomSheet.Margin = new Thickness(0, 0, 0, -desiredHeight);
        _viewModel.MarginBottomSheet = new Thickness(0, 0, 0, -desiredHeight);
    }

    private void InputElement_OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        _timer.Start();
    }
    
    private void InputElement_OnPointerMoved(object? sender, PointerEventArgs e)
    {
        if (BottomSheet.Opacity == 0)
            BottomSheet.Opacity = 1;
        if (_isPressed)
        {
            var point = e.GetPosition(this);
            _shift = TopLevel.GetTopLevel(this).Height - point.Y;
            if (_shift > 25)
            {
                double newBottom = -BottomSheet.Height + _shift;
                if (newBottom > 0) 
                    newBottom = 0;
                BottomSheet.Margin = new Thickness(0, 0, 0, newBottom);
            }
        }
    }
    
    private void BottomSheet_PointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        _timer.Stop();
        if (_isPressed == false)
            return;
        
        var isShiftLargeEnough = _shift >= BottomSheet.Height / 2.5;
        _isPressed = false;
        BottomSheet.Classes.Clear();
        BottomSheet.Classes.Add(isShiftLargeEnough ? "up" : "down");
        _viewModel.BottomSheetClass = isShiftLargeEnough;
    }
    
    private void NextButton_OnClick(object? sender, RoutedEventArgs e)
    {
        ProductImagesCarousel.Next();
    }

    private void PreviousButton_OnClick(object? sender, RoutedEventArgs e)
    {
        ProductImagesCarousel.Previous();
    }
}