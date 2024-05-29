using System;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using DeliveryEatsClientMobile.Models;
using DeliveryEatsClientMobile.Service;
using ReactiveUI;

namespace DeliveryEatsClientMobile.ViewModels;

public class CartViewModel : ViewModelBase
{
    public Order Order { get; set; }
    public ReactiveCommand<Unit, Unit> GoBackCommand { get; }
    public ReactiveCommand<Unit, Unit> AddOrderCommand { get; }
    public ReactiveCommand<Product, Unit> AddProductCommand { get; }
    public ReactiveCommand<Product, Unit> RemoveProductCommand { get; }
    public ReactiveCommand<Unit, Unit> ClearOrderCommand { get; }
    public CartViewModel(IOrderRepository orderRepository, INotificationService notificationService, IAccessTokenRepository accessTokenRepository,
        IUpdateTokenService updateTokenService, IClientRepository clientRepository, Order order) : base(notificationService, updateTokenService, order)
    {
        Order = order;
        
        // Отправка заказа на сервер.
        AddOrderCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            if (accessTokenRepository.GetAccessToken() == null)
                return;
            var client = await clientRepository.GetInfo(accessTokenRepository.GetAccessToken());
            Order.Street = client.Addresses[0].Street;
            Order.House = (int)client.Addresses[0].House;
            Order.ClientId = client.Id;
            await orderRepository.AddOrder(accessTokenRepository.GetAccessToken(), Order);
        });
        AddOrderCommand.ThrownExceptions.Subscribe();
        // Добавление продукта.
        AddProductCommand = ReactiveCommand.CreateFromTask<Product>(AddProduct);
        AddProductCommand.ThrownExceptions.Subscribe();
        // Удаление продукта.
        RemoveProductCommand = ReactiveCommand.CreateFromTask<Product>(RemoveProduct);
        RemoveProductCommand.ThrownExceptions.Subscribe();
        // Очистка корзины.
        ClearOrderCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            foreach (var orderProduct in Order.Products)
                orderProduct.Count = 0;
            Order.Products.Clear();
        });
        ClearOrderCommand.ThrownExceptions.Subscribe();
        // Возвращение к продуктам.
        GoBackCommand = ReactiveCommand.Create(() =>
        {
            (Owner.Owner as MainViewModel).SelectedViewModel = Owner;
        });
        GoBackCommand.ThrownExceptions.Subscribe();
    }
    
    private async Task AddProduct(Product product)
    {
        var existingItem = Order.Products.FirstOrDefault(a => a.Id == product.Id);
        if (existingItem != null)
            existingItem.Count++;
        else
        {
            product.Count = 1;
            Order.Products.Add(product);
        }
    }
    
    private async Task RemoveProduct(Product product)
    {
        var existingItem = Order.Products.FirstOrDefault(a => a.Id == product.Id);
        if (existingItem != null)
            if (existingItem.Count == 1)
            {
                existingItem.Count--;
                Order.Products.Remove(existingItem);
            }
            else
                existingItem.Count--;
    }
}