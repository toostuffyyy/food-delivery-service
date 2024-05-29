using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Avalonia.Controls.Notifications;
using DeliveryEatsClientMobile.Models;
using DeliveryEatsClientMobile.Service;
using ReactiveUI;
using Refit;

namespace DeliveryEatsClientMobile.ViewModels;

public class ViewModelBase : ReactiveObject
{
    protected readonly IUpdateTokenService _updateTokenService;
    protected readonly INotificationService _notificationService;
    protected readonly Order _order;
    public ViewModelBase Owner { get; set; }
    
    public ViewModelBase(INotificationService notificationService, IUpdateTokenService updateTokenService, Order order)
    {
        _notificationService = notificationService;
        _updateTokenService = updateTokenService;
        _order = order;
    }
    
    protected async Task CommandExc<TParam, TResult>(Exception e, ReactiveCommand<TParam, TResult> reactiveCommand)
    {
        switch (e)
        {
            case HttpRequestException:
                _notificationService?.ShowNotification(new Notification("Ошибка", "Не удалось установить соедиенение с сервером", NotificationType.Error));
                break;
            case ApiException exception:
                switch (exception.StatusCode)
                {
                    case HttpStatusCode.Unauthorized:
                        await _updateTokenService.UpdateTokens();
                        reactiveCommand.Execute();
                        break;
                    case HttpStatusCode.NotFound:
                    case HttpStatusCode.BadRequest:
                        _notificationService?.ShowNotification(new Notification("Ошибка", exception.Content, NotificationType.Error));
                        break;
                    case HttpStatusCode.InternalServerError:
                        _notificationService?.ShowNotification(new Notification("Ошибка", "На стороне сервера произошла ошибка", NotificationType.Error));
                        break;
                    case HttpStatusCode.Forbidden:
                        _notificationService?.ShowNotification(new Notification("Ошибка", "Недостаточно прав для выполнения данной операции", NotificationType.Error));
                        break;
                }
                break;
        }
    }
}