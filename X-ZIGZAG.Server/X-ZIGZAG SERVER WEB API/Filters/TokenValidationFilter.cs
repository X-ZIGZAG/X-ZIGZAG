using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using X_ZIGZAG_SERVER_WEB_API.Data;
using Microsoft.EntityFrameworkCore;

namespace X_ZIGZAG_SERVER_WEB_API.Filters
{
    public class TokenValidationFilter : IAsyncActionFilter
    {
        private readonly MyDbContext _dbContext;

        public TokenValidationFilter(MyDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var token = context.HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (string.IsNullOrEmpty(token))
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            var isValidToken = await _dbContext.Admins.AnyAsync(u => u.Id == token);

            if (!isValidToken)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            await next();
        }
    }
}
