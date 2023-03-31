namespace Lingtren.Api.Controllers
{
    using FluentValidation;
    using Lingtren.Api.Common;
    using Lingtren.Application.Common.Dtos;
    using Lingtren.Application.Common.Interfaces;
    using Lingtren.Application.Common.Models.RequestModels;
    using Lingtren.Application.Common.Models.ResponseModels;
    using Lingtren.Domain.Entities;
    using LinqKit;
    using Microsoft.AspNetCore.Mvc;

    public class DepartmentController : BaseApiController
    {
        private readonly IDepartmentService _departmentService;
        private readonly IValidator<DepartmentRequestModel> _validator;
        public DepartmentController(
            IDepartmentService departmentService,
            IValidator<DepartmentRequestModel> validator)
        {
            _departmentService = departmentService;
            _validator = validator;
        }

        /// <summary>
        /// get department api
        /// </summary>
        /// <returns> the list of <see cref="DepartmentResponseModel" /> .</returns>
        [HttpGet]
        public async Task<SearchResult<DepartmentResponseModel>> SearchAsync([FromQuery] DepartmentBaseSearchCriteria searchCriteria )
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
            IsSuperAdminOrAdmin(CurrentUser.Role);

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
        /// get department by id or slug
        /// </summary>
        /// <param name="identity"> the department id or slug</param>
        /// <returns> the instance of <see cref="DepartmentResponseModel" /> .</returns>
        [HttpGet("{identity}")]
        public async Task<DepartmentResponseModel> Get(string identity)
        {
            var model = await _departmentService.GetByIdOrSlugAsync(identity).ConfigureAwait(false);
            return new DepartmentResponseModel(model);
        }

        /// <summary>
        /// Get Department 
        /// </summary>
        /// <param name ="departmentName">the group id or slug</param>
        /// <returns>the instance of <see cref="UserResponseModel"/></returns>

        [HttpGet("{departmentName}/identity")]
        public async Task<List<UserResponseModel>> GetUserBuDepartmentName(string departmentName)
        {
            return await _departmentService.GetUserByDepartmentName(CurrentUser.Id, departmentName).ConfigureAwait(false);
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
            IsSuperAdminOrAdmin(CurrentUser.Role);

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
        public async Task<IActionResult> DeleteAsync(string identity)
        {
            IsSuperAdminOrAdmin(CurrentUser.Role);

            await _departmentService.DeleteAsync(identity, CurrentUser.Id).ConfigureAwait(false);
            return Ok(new CommonResponseModel() { Success = true, Message = "Department removed successfully." });
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
            IsSuperAdminOrAdmin(CurrentUser.Role);

            var existing = await _departmentService.GetByIdOrSlugAsync(identity, CurrentUser.Id).ConfigureAwait(false);

            existing.Id = existing.Id;
            existing.IsActive = enabled;
            existing.UpdatedBy = CurrentUser.Id;
            existing.UpdatedOn = DateTime.UtcNow;

            var savedEntity = await _departmentService.UpdateAsync(existing).ConfigureAwait(false);
            return new DepartmentResponseModel(savedEntity);
        }

        /// <summary>
        /// get deparment users api
        /// </summary>
        /// <param name="identity">the department id or slug</param>
        /// <returns>the instance of <see cref="UserResponseModel"/></returns>
        [HttpGet("{identity}/users")]
        public async Task<SearchResult<UserResponseModel>> GetUsers(string identity, BaseSearchCriteria searchCriteria)
        {
            IsSuperAdminOrAdmin(CurrentUser.Role);
            return await _departmentService.GetUsers(identity, searchCriteria, CurrentUser.Id);
        }
    }
}
