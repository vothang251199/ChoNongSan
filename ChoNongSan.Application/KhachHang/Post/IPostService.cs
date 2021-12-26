using ChoNongSan.Application.KhachHang.PostImages;
using ChoNongSan.Data.Models;
using ChoNongSan.ViewModels.Common;
using ChoNongSan.ViewModels.Requests.KhachHang.Posts;
using ChoNongSan.ViewModels.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChoNongSan.Application.KhachHang.Posts
{
    public interface IPostService
    {
        Task<int> CreatePost(CreatePostRequest request);

        Task<bool> HiddenPost(int PostID);

        Task<PageResult<ListPostVm>> GetAllBySearchPaging(GetSearchPostPagingRequest request);

        Task<PageResult<ListPostVm>> GetAllByCategoryID(GetCatIDPostPagingRequest request);

        Task<List<ListPostVm>> GetAllPostBySearch(string keyword);

        Task<int> AddImage(int postId, PostImageCreateRequest request);

        Task<int> UpdateImage(int imageId, PostImageUpdateRequest request);

        Task<int> RemoveImage(int imageId);

        Task<ImagePostViewModel> GetImageById(int imageId);

        Task<PostViewModel> GetPostById(int postID);

        Task AddViewcount(int postID);

        Task<List<ListPostVm>> GetListPost();
    }
}