using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        public async Task<Photo> GetMainPhotoForUser(int userId)
        {
            return await this._context.Photos
                .Where(p => p.UserId == userId)
                .FirstOrDefaultAsync(p => p.IsMain);
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

        public async Task<IEnumerable<User>> GetUsers()
        {
            return await this._context.Users
                .Include(p => p.Photos)
                .ToListAsync();
        }

        public async Task<bool> SaveAll()
        {
            // returns changes saved to db
            return await this._context.SaveChangesAsync() > 0;
        }
    }
}