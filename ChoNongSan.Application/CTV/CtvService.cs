using ChoNongSan.Data.Models;
using ChoNongSan.ViewModels.Requests.CTV;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChoNongSan.Application.CTV
{
    public class CtvService : ICtvService
    {
        private readonly ChoNongSanContext _context;

        public CtvService(ChoNongSanContext context)
        {
            _context = context;
        }

        public async Task<int> AcceptPost(AcceptPostRequest request)
        {
            var post = await _context.Posts.FindAsync(request.PostID);
            post.StatusPost = request.StatustPost;
            post.Reason = request.Reason;
            _context.Posts.Update(post);
            return await _context.SaveChangesAsync();
        }
    }
}