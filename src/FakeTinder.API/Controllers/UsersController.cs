using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using FakeTinder.API.Data;
using FakeTinder.API.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FakeTinder.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IDatingRepository _repo;
        private readonly IMapper _mapper;
        public UsersController(IDatingRepository repo, IMapper mapper)
        {
            this._repo = repo;
            this._mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var users = await this._repo.GetUsers();

            var usersToReturn = this._mapper.Map<IEnumerable<UserForListDto>>(users);

            return Ok(usersToReturn);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await this._repo.GetUser(id);

            var userToReturn = this._mapper.Map<UserForDetailsDto>(user);

            return Ok(userToReturn);
        }
    }
}