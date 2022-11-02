namespace Lingtren.Api.Controllers
{
    using Lingtren.Application.Common.Models.RequestModels;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    public class AccountController : BaseApiController
    {
        [HttpPost("Login")]
        [AllowAnonymous]
        public async Task<IActionResult> LoginAsync([FromBody] LoginRequestModel model)
        {
            if (!ModelState.IsValid)
            {
                string validationErrors = string.Join(",<br/>",
                                                        ModelState.Values.Where(E => E.Errors.Count > 0)
                                                        .SelectMany(E => E.Errors)
                                                        .Select(E => E.ErrorMessage)
                                                        .ToArray());
                return BadRequest(validationErrors);
            }
           
            return Ok();
        }
    }
}
