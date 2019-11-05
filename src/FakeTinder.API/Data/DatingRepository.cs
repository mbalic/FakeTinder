using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FakeTinder.API.Helpers;
using FakeTinder.API.Models;
using Microsoft.EntityFrameworkCore;

namespace FakeTinder.API.Data
{
    public class DatingRepository : IDatingRepository
    {
        private readonly DataContext _context;
        public DatingRepository(DataContext context)
        {
            this._context = context;
        }
        public void Add<T>(T entity) where T : class
        {
            this._context.Add(entity);
        }

        public void Delete<T>(T entity) where T : class
        {
            this._context.Remove(entity);
        }

        public async Task<Like> GetLike(int userId, int recipientId)
        {
            return await this._context.Likes
                .FirstOrDefaultAsync(u => u.LikerId == userId && u.LikeeId == recipientId);
        }

        public async Task<Photo> GetMainPhotoForUser(int userId)
        {
            return await this._context.Photos
                .Where(p => p.UserId == userId)
                .FirstOrDefaultAsync(p => p.IsMain);
        }

        public async Task<Message> GetMessage(int id)
        {
            return await this._context.Messages.FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<PagedList<Message>> GetMessagesForUser(MessageParams messageParams)
        {
            var messages = this._context.Messages
                .Include(m => m.Sender).ThenInclude(m => m.Photos)
                .Include(m => m.Recipient).ThenInclude(m => m.Photos)
                .AsQueryable();

            switch (messageParams.MessageContainer)
            {
                case "Inbox":
                    messages = messages.Where(m => m.RecipientId == messageParams.UserId && !m.RecipientDeleted);
                    break;
                case "Outbox":
                    messages = messages.Where(m => m.SenderId == messageParams.UserId && !m.SenderDeleted);
                    break;
                default:
                    messages = messages.Where(m => m.RecipientId == messageParams.UserId && !m.IsRead && !m.RecipientDeleted);
                    break;
            }

            messages = messages.OrderByDescending(m => m.DateCreated);

            return await PagedList<Message>.CreateAsync(messages, messageParams.PageNumber, messageParams.PageSize);
        }

        public async Task<IEnumerable<Message>> GetMessagesThread(int userId, int recipientId)
        {
            return await this._context.Messages
                .Include(m => m.Sender).ThenInclude(m => m.Photos)
                .Include(m => m.Recipient).ThenInclude(m => m.Photos)
                .Where(m => m.RecipientId == userId && m.SenderId == recipientId && !m.RecipientDeleted
                    || m.RecipientId == recipientId && m.SenderId == userId && !m.SenderDeleted)
                .OrderByDescending(m => m.DateCreated)
                .ToListAsync();
        }

        public async Task<Photo> GetPhoto(int id)
        {
            return await this._context.Photos
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<User> GetUser(int id, bool isCurrentUser)
        {
            var query = this._context.Users
                .Include(u => u.Photos)
                .AsQueryable();

            // Fo
            if (isCurrentUser)
            {
                query = query.IgnoreQueryFilters();
            }

            var user = await query.FirstOrDefaultAsync(u => u.Id == id);

            return user;
        }

        public async Task<PagedList<User>> GetUsers(UserParams userParams)
        {
            var users = this._context.Users.Include(p => p.Photos)
                .OrderByDescending(p => p.LastActive)
                .AsQueryable()
                .Where(p => p.Id != userParams.UserId);

            if (userParams.Gender != "all")
            {
                users = users.Where(p => p.Gender == userParams.Gender);
            }

            if (userParams.Likers)
            {
                var userLikers = await this.GetUserLikes(userParams.UserId, userParams.Likers);
                users = users.Where(u => userLikers.Contains(u.Id));
            }

            if (userParams.Likees)
            {
                var userLikees = await this.GetUserLikes(userParams.UserId, userParams.Likers);
                users = users.Where(u => userLikees.Contains(u.Id));
            }

            if (userParams.MinAge != 18 || userParams.MaxAge != 99)
            {
                var minDateOfBirth = DateTime.Today.AddYears(-userParams.MaxAge - 1);
                var maxDateOfBirth = DateTime.Today.AddYears(-userParams.MinAge);

                users = users.Where(p => p.DateOfBirth >= minDateOfBirth && p.DateOfBirth <= maxDateOfBirth);
            }

            if (!string.IsNullOrEmpty(userParams.OrderBy))
            {
                switch (userParams.OrderBy)
                {
                    case "created":
                        users = users.OrderByDescending(p => p.Created);
                        break;
                    default:
                        users = users.OrderByDescending(p => p.LastActive);
                        break;

                }
            }

            return await PagedList<User>.CreateAsync(users, userParams.PageNumber, userParams.PageSize);
        }

        public async Task<bool> SaveAll()
        {
            // returns changes saved to db
            return await this._context.SaveChangesAsync() > 0;
        }

        private async Task<IEnumerable<int>> GetUserLikes(int id, bool returnLikers)
        {
            var user = await this._context.Users
                .Include(u => u.Likers)
                .Include(u => u.Likees)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (returnLikers)
            {
                return user.Likers
                    .Where(u => u.LikeeId == id)
                    .Select(u => u.LikerId);
            }
            else
            {
                return user.Likees
                    .Where(u => u.LikerId == id)
                    .Select(u => u.LikeeId);
            }
        }
    }
}