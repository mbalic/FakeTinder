using System;
using Microsoft.AspNetCore.Http;

namespace FakeTinder.API.Dtos
{
    public class RoleEditDto
    {
        public string[] RoleNames { get; set; }
    }
}