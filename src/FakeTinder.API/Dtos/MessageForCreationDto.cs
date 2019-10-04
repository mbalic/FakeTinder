using System;
using Microsoft.AspNetCore.Http;

namespace FakeTinder.API.Dtos
{
    public class MessageForCreationDto
    {
        public MessageForCreationDto()
        {
            this.DateCreated = DateTime.Now;
        }
        public int SenderId { get; set; }
        public int RecipientId { get; set; }
        public string Content { get; set; }
        public DateTime DateCreated { get; set; }
    }
}