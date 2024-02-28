namespace Lingtren.Application.Common.Interfaces
{
    using Lingtren.Application.Common.Dtos;
    using Lingtren.Domain.Entities;

    public interface IMailNotificationService
        : IGenericService<MailNotification, MailNotificationBaseSearchCriteria> { }
}
