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

        public Task<PagedList<Message>> GetMessagesForUser()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Message>> GetMessagesThread(int userId, int recipientId)
        {
            throw new NotImplementedException();
        }

        public async Task<Photo> GetPhoto(int id)
        {
            return await this._context.Photos
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<User> GetUser(int id)
        {
            return await this._context.Users
                .Include(p => p.Photos)
                .FirstOrDefaultAsync(p => p.Id == id);
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