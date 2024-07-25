// <copyright file="DepartmentController.cs" company="Vurilo Nepal Pvt. Ltd.">
// Copyright (c) Vurilo Nepal Pvt. Ltd.. All rights reserved.
// </copyright>

namespace Lingtren.Api.Controllers
{
    using FluentValidation;
    using Lingtren.Api.Common;
    using Lingtren.Application.Common.Dtos;
    using Lingtren.Application.Common.Exceptions;
    using Lingtren.Application.Common.Interfaces;
    using Lingtren.Application.Common.Models.RequestModels;
    using Lingtren.Application.Common.Models.ResponseModels;
    using Lingtren.Domain.Entities;
    using Lingtren.Infrastructure.Localization;
    using LinqKit;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Localization;

    public class MailNotificationController : BaseApiController
    {
        private readonly IMailNotificationService mailNotificationService;
        private readonly IValidator<MailNotificationRequestModel> validator;
        private readonly IGeneralSettingService generalSettingService;
        protected readonly IStringLocalizer<ExceptionLocalizer> localizer;
        private readonly IEmailService emailService;

        public MailNotificationController(
            IMailNotificationService mailNotificationService,
            IValidator<MailNotificationRequestModel> validator,
            IGeneralSettingService generalSettingService,
            IStringLocalizer<ExceptionLocalizer> localizer,
            IEmailService emailService
        )
        {
            this.mailNotificationService = mailNotificationService;
            this.validator = validator;
            this.generalSettingService = generalSettingService;
            this.localizer = localizer;
            this.emailService = emailService;
        }

        /// <summary>
        ///     get Mail api.
        /// </summary>
        /// <returns> the list of <see cref="MailNotificationResponseModel" /> .</returns>
        [HttpGet]
        public async Task<SearchResult<MailNotificationResponseModel>> SearchAsync(
            [FromQuery] MailNotificationBaseSearchCriteria searchCriteria
        )
        {
            var searchResult = await mailNotificationService
                .SearchAsync(searchCriteria, false)
                .ConfigureAwait(false);
            var response = new SearchResult<MailNotificationResponseModel>
            {
                Items = new List<MailNotificationResponseModel>(),
                CurrentPage = searchResult.CurrentPage,
                PageSize = searchResult.PageSize,
                TotalCount = searchResult.TotalCount,
                TotalPage = searchResult.TotalPage,
            };

            searchResult.Items.ForEach(p =>
                response.Items.Add(new MailNotificationResponseModel(p))
            );

            return response;
        }

        /// <summary>
        /// This api helps to create a mail template of different type as required
        /// </summary>
        /// <param name="model">the Instance of <see cref="MailNotificationRequestModel" /> .</param>
        /// <returns> the instance of <see cref="MailNotificationResponseModel" /> .</returns>
        [HttpPost]
        public async Task<MailNotificationResponseModel> CreateAsync(
            MailNotificationRequestModel model
        )
        {
            IsSuperAdminOrAdmin(CurrentUser.Role);

            await validator
                .ValidateAsync(model, options => options.ThrowOnFailures())
                .ConfigureAwait(false);
            var currentTimeStamp = DateTime.UtcNow;
            var entity = new MailNotification
            {
                Id = Guid.NewGuid(),
                Subject = model.MailSubject,
                Name = model.MailName,
                Message = model.MailMessage,
                MailType = model.MailType,
                IsActive = model.IsActive,
                CreatedOn = currentTimeStamp,
                CreatedBy = CurrentUser.Id,
            };
            var response = await mailNotificationService
                .CreateAsync(entity, false)
                .ConfigureAwait(false);
            return new MailNotificationResponseModel(response);
        }

        /// <summary>
        /// update Mail Notification api.
        /// </summary>
        /// <param name="identity"> id or slug. </param>
        /// <param name="model"> the instance of <see cref="MailNotificationRequestModel" />. </param>
        /// <returns> the instance of <see cref="MailNotificationResponseModel" /> .</returns>
        [HttpPut("{identity}")]
        public async Task<MailNotificationResponseModel> UpdateAsync(
            Guid identity,
            MailNotificationRequestModel model
        )
        {
            IsSuperAdminOrAdmin(CurrentUser.Role);

            await validator
                .ValidateAsync(model, options => options.ThrowOnFailures())
                .ConfigureAwait(false);

            var existing = await mailNotificationService
                .GetAsync(identity, CurrentUser.Id, false)
                .ConfigureAwait(false);

            var currentTimeStamp = DateTime.UtcNow;

            existing.Subject = model.MailSubject;
            existing.Name = model.MailName;
            existing.Message = model.MailMessage;
            existing.IsActive = model.IsActive;
            existing.UpdatedBy = CurrentUser.Id;
            existing.UpdatedOn = currentTimeStamp;

            var savedEntity = await mailNotificationService
                .UpdateAsync(existing, false)
                .ConfigureAwait(false);

            return new MailNotificationResponseModel(savedEntity);
        }

        /// <summary>
        /// Gets the mail by id
        /// </summary>
        /// <param name="mailId"></param>
        /// <returns></returns>The html to show in the preview <summary>

        [HttpGet("{mailId}")]
        public async Task<IActionResult> Get(Guid mailId)
        {
            IsSuperAdminOrAdmin(CurrentUser.Role);

            var getExistingMail = await mailNotificationService
                .GetByIdOrSlugAsync(mailId.ToString(), CurrentUser.Id, false)
                .ConfigureAwait(false);
            var company = await generalSettingService
                .GetFirstOrDefaultAsync()
                .ConfigureAwait(false);

            var html = $"<br><br>{getExistingMail.Message}";
            // html += $"<br> {company.EmailSignature}";

            return Content(html, "text/html");
        }

        /// <summary>
        /// Api to check the Email is being send or not
        /// </summary>
        /// <param name="identity">Id of the Mail Template </param>
        /// <param name="model">the instance of <see cref="MailSendRequestModel" />.</param>
        /// <returns></returns> <summary>
        [HttpPatch("{identity}/checkSendEmail")]
        public async Task<IActionResult> CheckSendMail(Guid identity, MailSendRequestModel model)
        {
            try
            {
                IsSuperAdminOrAdmin(CurrentUser.Role);

                var company = await generalSettingService
                    .GetFirstOrDefaultAsync()
                    .ConfigureAwait(false);
                var existingMail = await mailNotificationService
                    .GetAsync(identity, null, false)
                    .ConfigureAwait(false);

                var html = existingMail.Message;
                var mail = new EmailRequestDto
                {
                    To = model.EmailAddress,
                    Subject = existingMail.Subject,
                    Message = html
                };
                await emailService.SendMailWithHtmlBodyAsync(mail).ConfigureAwait(false);
                return Ok(
                    new CommonResponseModel { Success = true, Message = "Email Send successfully." }
                );
            }
            catch (Exception ex)
            {
                throw new ServiceException(ex.Message);
            }
        }

        /// <summary>
        /// Deletes the mail template
        /// </summary>
        /// <param name="identity"></param>
        /// <returns></returns>
        [HttpDelete("{identity}")]
        public async Task<IActionResult> DeleteAsync(string identity)
        {
            IsSuperAdminOrAdmin(CurrentUser.Role);

            await mailNotificationService
                .DeleteAsync(identity, CurrentUser.Id)
                .ConfigureAwait(false);
            return Ok(
                new CommonResponseModel()
                {
                    Success = true,
                    Message = localizer.GetString("MailTemplateRemoved")
                }
            );
        }
    }
}
