using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Threading;
using DeliveryEatsClientMobile.ViewModels;

namespace DeliveryEatsClientMobile.Views;

public partial class ProductsView : UserControl
{
    private bool _isPressed;
    private double _shift;
    private DispatcherTimer _timer;
    private ProductsViewModel _viewModel;

    public ProductsView()
    {
        InitializeComponent();
        _timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(100) };
        _timer.Tick += (sender, e) => { _isPressed = true; _timer.Stop(); };
    }

    private void StyledElement_OnDataContextChanged(object? sender, EventArgs e)
    {
        _viewModel = DataContext as ProductsViewModel;
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

    private void ScrollViewer_OnScrollChanged(object? sender, ScrollChangedEventArgs e)
    {
        var scrollViewer = (ScrollViewer)sender;
        BorderSearch.BoxShadow = scrollViewer.Offset.Y > 15
            ? new BoxShadows(
                new BoxShadow
                {
                    OffsetX = 0,
                    OffsetY = 5,
                    Blur = 10,
                    Color = new Color(19, 00, 00, 00)
                })
            : new BoxShadows(
                new BoxShadow());
    }

    private void NextButton_OnClick(object? sender, RoutedEventArgs e)
    {
        if (ProductImagesCarousel.SelectedIndex == ProductImagesCarousel.ItemCount - 1)
            ProductImagesCarousel.SelectedIndex = 0;
        else
            ProductImagesCarousel.SelectedIndex++;
    }

    private void PreviousButton_OnClick(object? sender, RoutedEventArgs e)
    {
        if (ProductImagesCarousel.SelectedIndex == 0)
            ProductImagesCarousel.SelectedIndex = ProductImagesCarousel.ItemCount - 1;
        else
            ProductImagesCarousel.SelectedIndex--;
    }
}