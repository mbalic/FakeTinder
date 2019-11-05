using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using FakeTinder.API.Data;
using FakeTinder.API.Dtos;
using FakeTinder.API.Helpers;
using FakeTinder.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace FakeTinder.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly UserManager<User> _userManager;
        private readonly IOptions<CloudinarySettings> _cloudinaryConfig;
        private Cloudinary _cloudinary;

        public AdminController(DataContext context, UserManager<User> userManager, IOptions<CloudinarySettings> cloudinaryConfig)
        {
            this._cloudinaryConfig = cloudinaryConfig;
            this._context = context;
            this._userManager = userManager;

            var account = new Account(
               this._cloudinaryConfig.Value.CloudName,
               this._cloudinaryConfig.Value.ApiKey,
               this._cloudinaryConfig.Value.ApiSecret
           );

            this._cloudinary = new Cloudinary(account);
        }

        [Authorize(Policy = "RequireAdminRole")]
        [HttpGet("usersWithRoles")]
        public async Task<IActionResult> GetUsersWithRoles()
        {
            var userList = await this._context.Users
                .Select(u => new
                {
                    Id = u.Id,
                    UserName = u.UserName,
                    Roles = u.UserRoles
                        .Select(r => r.Role.Name)
                        .ToList()
                })
                .OrderBy(u => u.UserName)
                .ToListAsync();

            return Ok(userList);
        }

        [Authorize(Policy = "RequireAdminRole")]
        [HttpPost("editRoles/{userName}")]
        public async Task<IActionResult> EditRoles(string userName, RoleEditDto roleEditDto)
        {
            var user = await this._userManager.FindByNameAsync(userName);
            var userRoles = await this._userManager.GetRolesAsync(user);
            var selectedRoles = roleEditDto.RoleNames;
            selectedRoles = selectedRoles ?? new string[] { };

            var result = await this._userManager.AddToRolesAsync(user, selectedRoles.Except(userRoles));

            if (!result.Succeeded)
            {
                return BadRequest("Failed to add to roles");
            }

            result = await this._userManager.RemoveFromRolesAsync(user, userRoles.Except(selectedRoles));

            if (!result.Succeeded)
            {
                return BadRequest("Failed to remove roles");
            }

            return Ok(await this._userManager.GetRolesAsync(user));
        }

        [Authorize(Policy = "ModeratePhotoRole")]
        [HttpGet("photosForModeration")]
        public async Task<IActionResult> GetPhotosForModeration()
        {
            var photos = await this._context.Photos
            .Include(u => u.User)
            .IgnoreQueryFilters()
            .Where(p => p.IsApproved == false)
            .Select(u => new
            {
                Id = u.Id,
                userName = u.User.UserName,
                Url = u.Url,
                IsApproved = u.IsApproved
            }).ToListAsync();

            return Ok(photos);
        }

        [Authorize(Policy = "ModeratePhotoRole")]
        [HttpPost("approvePhoto/{photoId}")]
        public async Task<IActionResult> ApprovePhoto(int photoId)
        {
            var photo = await this._context.Photos
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(p => p.Id == photoId);

            photo.IsApproved = true;

            await this._context.SaveChangesAsync();

            return Ok();
        }

        [Authorize(Policy = "ModeratePhotoRole")]
        [HttpPost("rejectPhoto/{photoId}")]
        public async Task<IActionResult> RejectPhoto(int photoId)
        {
            var photo = await this._context.Photos
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(p => p.Id == photoId);

            if (photo.IsMain)
            {
                return BadRequest("You cannot reject the main photo");
            }

            if (photo.PublicId != null)
            {
                var deleteParams = new DeletionParams(photo.PublicId);

                var result = this._cloudinary.Destroy(deleteParams);

                if (result.Result == "ok")
                {
                    this._context.Photos.Remove(photo);
                }
            }

            if (photo.PublicId == null)
            {
                this._context.Photos.Remove(photo);
            }

            await this._context.SaveChangesAsync();

            return Ok();
        }
    }
}