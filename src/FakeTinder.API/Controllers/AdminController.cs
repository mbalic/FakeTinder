using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using FakeTinder.API.Data;
using FakeTinder.API.Dtos;
using FakeTinder.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace FakeTinder.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        [Authorize(Policy = "RequireAdminRole")]
        [HttpGet("usersWithRoles")]
        public IActionResult GetUsersWithRoles()
        {
            return Ok("Only admins can see this");
        }

        [Authorize(Policy = "ModeratePhotoRole")]
        [HttpGet("photosForModeration")]
        public IActionResult GetPhotosFormoderation()
        {
            return Ok("Admins or moderators can see this");
        }
    }
}