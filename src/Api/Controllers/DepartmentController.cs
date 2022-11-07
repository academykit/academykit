﻿namespace Lingtren.Api.Controllers
{
    using FluentValidation;
    using Lingtren.Api.Common;
    using Lingtren.Application.Common.Dtos;
    using Lingtren.Application.Common.Exceptions;
    using Lingtren.Application.Common.Interfaces;
    using Lingtren.Application.Common.Models.RequestModels;
    using Lingtren.Application.Common.Models.ResponseModels;
    using Lingtren.Domain.Entities;
    using Lingtren.Domain.Enums;
    using LinqKit;
    using Microsoft.AspNetCore.Mvc;

    public class DepartmentController : BaseApiController
    {
        private readonly IDepartmentService _departmentService;
        private readonly IValidator<DepartmentRequestModel> _validator;
        private readonly ILogger<DepartmentController> _logger;
        public DepartmentController(
            IDepartmentService departmentService,
            IValidator<DepartmentRequestModel> validator,
            ILogger<DepartmentController> logger)
        {
            _departmentService = departmentService;
            _validator = validator;
            _logger = logger;
        }

        /// <summary>
        /// get department api
        /// </summary>
        /// <returns> the list of <see cref="DepartmentResponseModel" /> .</returns>
        [HttpGet]
        public async Task<SearchResult<DepartmentResponseModel>> SearchAsync([FromQuery]DepartmentBaseSearchCriteria searchCriteria)
        {
            var searchResult = await _departmentService.SearchAsync(searchCriteria).ConfigureAwait(false);

            var response = new SearchResult<DepartmentResponseModel>
            {
                Items = new List<DepartmentResponseModel>(),
                CurrentPage = searchResult.CurrentPage,
                PageSize = searchResult.PageSize,
                TotalCount = searchResult.TotalCount,
                TotalPage = searchResult.TotalPage,
            };

            searchResult.Items.ForEach(p =>
                 response.Items.Add(new DepartmentResponseModel(p))
             );
            return response;
        }

        /// <summary>
        /// create department api
        /// </summary>
        /// <param name="model"> the instance of <see cref="DepartmentRequestModel" />. </param>
        /// <returns> the instance of <see cref="DepartmentRequestModel" /> .</returns>
        [HttpPost]
        public async Task<DepartmentResponseModel> CreateAsync(DepartmentRequestModel model)
        {
            if (CurrentUser.Role != UserRole.Admin)
            {
                _logger.LogWarning("User with Id : {userId} is not allowed to create department having user role : {role}", CurrentUser.Id, CurrentUser.Role.ToString());
                throw new ForbiddenException("Only user with admin role is allowed to create department");
            }
            await _validator.ValidateAsync(model, options => options.ThrowOnFailures()).ConfigureAwait(false);
            var currentTimeStamp = DateTime.UtcNow;
            var entity = new Department
            {
                Id = Guid.NewGuid(),
                Name = model.Name,
                IsActive = model.IsActive,
                CreatedOn = currentTimeStamp,
                CreatedBy = CurrentUser.Id,
                UpdatedOn = currentTimeStamp,
                UpdatedBy = CurrentUser.Id
            };
            var response = await _departmentService.CreateAsync(entity).ConfigureAwait(false);
            return new DepartmentResponseModel(response);
        }

        /// <summary>
        /// update department api
        /// </summary>
        /// <param name="identity"> id or slug </param>
        /// <param name="model"> the instance of <see cref="DepartmentRequestModel" />. </param>
        /// <returns> the instance of <see cref="DepartmentResponseModel" /> .</returns>
        [HttpPut("{identity}")]
        public async Task<DepartmentResponseModel> UpdateAsync(string identity, DepartmentRequestModel model)
        {
            if (CurrentUser.Role != UserRole.Admin)
            {
                _logger.LogWarning("User with Id : {userId} is not allowed to update department with identity : {identity} and having user role : {role}", CurrentUser.Id, identity, CurrentUser.Role.ToString());
                throw new ForbiddenException("Only user with admin role is allowed to update department");
            }
            await _validator.ValidateAsync(model, options => options.ThrowOnFailures()).ConfigureAwait(false);
            var existing = await _departmentService.GetByIdOrSlugAsync(identity, CurrentUser.Id).ConfigureAwait(false);
            var currentTimeStamp = DateTime.UtcNow;

            existing.Id = existing.Id;
            existing.Name = model.Name;
            existing.IsActive = model.IsActive;
            existing.UpdatedBy = CurrentUser.Id;
            existing.UpdatedOn = currentTimeStamp;

            var savedEntity = await _departmentService.UpdateAsync(existing).ConfigureAwait(false);
            return new DepartmentResponseModel(savedEntity);
        }

        /// <summary>
        /// delete department api
        /// </summary>
        /// <param name="identity"> id or slug </param>
        /// <returns> the task complete </returns>
        [HttpDelete("{identity}")]
        public async Task<CommonResponseModel> DeletAsync(string identity)
        {
            if (CurrentUser.Role != UserRole.Admin)
            {
                _logger.LogWarning("User with Id : {userId} is not allowed to delete department having user role : {role}", CurrentUser.Id, CurrentUser.Role.ToString());
                throw new ForbiddenException("Only user with admin role is allowed to delete department");
            }
            await _departmentService.DeleteAsync(identity, CurrentUser.Id).ConfigureAwait(false);
            return new CommonResponseModel() { Success = true, Message = "Department removed successfully." };
        }

        /// <summary>
        /// change department status api
        /// </summary>
        /// <param name="identity">the department id or slug</param>
        /// <param name="enabled">the boolean</param>
        /// <returns>the instance of <see cref="DepartmentResponseModel"/></returns>
        [HttpPatch("{identity}/status")]
        public async Task<DepartmentResponseModel> ChangeStatus(string identity, [FromQuery] bool enabled)
        {
            var existing = await _departmentService.GetByIdOrSlugAsync(identity, CurrentUser.Id).ConfigureAwait(false);

            existing.Id = existing.Id;
            existing.IsActive = enabled;
            existing.UpdatedBy = CurrentUser.Id;
            existing.UpdatedOn = DateTime.UtcNow;

            var savedEntity = await _departmentService.UpdateAsync(existing).ConfigureAwait(false);
            return new DepartmentResponseModel(savedEntity);
        }
    }
}
