using BlogsAPI.Constants;
using BlogsAPI.DataManagers;
using BlogsAPI.Helpers;
using BlogsAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlogsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogsController : ControllerBase
    {
        private readonly AppSettings _appSettings;

        public BlogsController(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }

        [Authorize(Roles = Role.Admin)]
        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            var blogs = await BlogsDataManager.GetAsync(_appSettings.BlogsConnectionString);
            return Ok(blogs);
        }

        [Authorize(Roles = Role.Admin)]
        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] Blog model)
        {
            var response = await BlogsDataManager.CreateAsync(model, _appSettings.BlogsConnectionString);
            return Ok(response);
        }
    }
}
