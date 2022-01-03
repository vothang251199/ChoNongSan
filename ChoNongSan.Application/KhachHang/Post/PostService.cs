using ChoNongSan.ViewModels.Requests.KhachHang.Posts;
using ChoNongSan.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChoNongSan.ViewModels.Common;
using ChoNongSan.ViewModels.Responses;
using Microsoft.EntityFrameworkCore;
using ChoNongSan.Application.Common.Files;
using Microsoft.AspNetCore.Http;
using System.Net.Http.Headers;
using System.IO;
using ChoNongSan.Application.KhachHang.PostImages;
using ChoNongSan.Utilities.Exceptions;
using BingMapsRESTToolkit;

namespace ChoNongSan.Application.KhachHang.Posts
{
    public class PostService : IPostService
    {
        private readonly ChoNongSanContext _context;
        private readonly IStorageService _storageService;
        private const string POST_CONTENT_FOLDER_NAME = "post-content";

        public PostService(ChoNongSanContext context, IStorageService storageService)
        {
            _storageService = storageService;
            _context = context;
        }

        public async Task<int> CreatePost(CreatePostRequest request)
        {
            var post = new Post()
            {
                Title = request.Title,
                ProductName = request.ProductName,
                Description = request.Description,
                WeightNumber = request.WeightNumber,
                Price = request.Price,
                Quality = request.Quality,
                Address = request.Address,
                PhoneNumber = request.PhoneNumber,
                IsDeliver = request.IsDeliver,
                StatusPost = 0,
                ViewCount = 0,
                IsHidden = false,
                PostTime = DateTime.Now,
                AccountId = request.AccountID,
                CategoryId = request.CategoryID,
                WeightId = request.WeightID,
            };

            if (!string.IsNullOrEmpty(request.Address))
            {
                var geocdoe = new GeocodeRequest();
                geocdoe.BingMapsKey = "AhccP2WRstNnaiP--eT_K3M0IH7WfyRvzfYzPebH6WxqFPfmRouhpObDoXCfMBly";
                if (!request.Address.ToLower().Contains("việt nam"))
                    geocdoe.Query = request.Address + ", Việt Nam";
                else
                    geocdoe.Query = request.Address;
                var result = await geocdoe.Execute();
                if (result.StatusCode == 200)
                {
                    var toolkitLocation = (result?.ResourceSets?.FirstOrDefault())
                        ?.Resources?.FirstOrDefault() as BingMapsRESTToolkit.Location;
                    //var latitude = toolkitLocation.Point.Coordinates[0];
                    //var longitude = toolkitLocation.Point.Coordinates[1];
                    post.Location = new Data.Models.Location()
                    {
                        Lat = Convert.ToString(toolkitLocation.Point.Coordinates[0]),
                        Lng = Convert.ToString(toolkitLocation.Point.Coordinates[1]),
                    };
                }
            }

            if (request.ThumbnailImage != null)
            {
                post.ImagePosts = new List<ImagePost>()
                {
                    new ImagePost()
                    {
                        ImagePath = await this.SaveFile(request.ThumbnailImage),
                        IsDefault = true,
                    }
                };
            }

            _context.Posts.Add(post);
            return await _context.SaveChangesAsync();
        }

        public async Task<PageResult<PostVm>> GetAllByCategoryID(GetCatIDPostPagingRequest request)
        {
            var lsPost = await _context.Posts.AsNoTracking().Where(x => x.CategoryId == request.CategoryID).ToListAsync();
            var totalRow = lsPost.Count();
            var data = lsPost.Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => new PostVm()
                {
                    PostID = x.PostId,
                    Title = x.Title,
                    ImageDefault = _context.ImagePosts.AsNoTracking().FirstOrDefault(p => p.PostId == x.PostId && p.IsDefault == true).ImagePath,
                    ProductName = x.ProductName,
                    NameAccount = _context.Accounts.AsNoTracking().FirstOrDefault(p => p.AccountId == x.AccountId).FullName,
                    Price = x.Price,
                    WeightName = _context.WeightTypes.AsNoTracking().FirstOrDefault(p => p.WeightId == x.WeightId).WeightName,
                    WeightNumber = x.WeightNumber,
                    ViewCount = x.ViewCount,
                    CatName = _context.Categories.AsNoTracking().FirstOrDefault(p => p.CategoryId == x.CategoryId).CateName,
                    Lat = _context.Locations.AsNoTracking().FirstOrDefault(p => p.LocationId == x.LocationId).Lat,
                    Lng = _context.Locations.AsNoTracking().FirstOrDefault(p => p.LocationId == x.LocationId).Lng,
                }).ToList();

            var pageResult = new PageResult<PostVm>()
            {
                TotalRecords = totalRow,
                Items = data,
                PageIndex = request.PageIndex,
                PageSize = request.PageSize
            };
            return pageResult;
        }

        public async Task<PageResult<PostVm>> GetAllBySearchAndCatIdPaging(GetSearchPostPagingRequest request)
        {
            var lsPost = await _context.Posts.Where(x => x.IsHidden == false && x.StatusPost == 2).ToListAsync();
            if (!String.IsNullOrEmpty(request.KeyWord))
            {
                request.KeyWord = request.KeyWord.ToLower();
                lsPost = lsPost.Where(x => x.ProductName.ToLower().Contains(request.KeyWord)
                 || x.Address.ToLower().Contains(request.KeyWord) || x.Title.ToLower().Contains(request.KeyWord)).ToList();
            }

            if (request.CategoryID != null)
            {
                lsPost = lsPost.Where(x => x.CategoryId == request.CategoryID).ToList();
            }

            var totalRow = lsPost.Count();

            var data = lsPost.Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => new PostVm()
                {
                    PostID = x.PostId,
                    Title = x.Title,
                    Address = x.Address,
                    ImageDefault = _context.ImagePosts.AsNoTracking().FirstOrDefault(p => p.PostId == x.PostId && p.IsDefault == true).ImagePath,
                    ProductName = x.ProductName,
                    NameAccount = _context.Accounts.AsNoTracking().FirstOrDefault(p => p.AccountId == x.AccountId).FullName,
                    Price = x.Price,
                    WeightName = _context.WeightTypes.AsNoTracking().FirstOrDefault(p => p.WeightId == x.WeightId).WeightName,
                    WeightNumber = x.WeightNumber,
                    ViewCount = x.ViewCount,
                    CatName = _context.Categories.AsNoTracking().FirstOrDefault(p => p.CategoryId == x.CategoryId).CateName,
                    Lat = _context.Locations.AsNoTracking().FirstOrDefault(p => p.LocationId == x.LocationId).Lat,
                    Lng = _context.Locations.AsNoTracking().FirstOrDefault(p => p.LocationId == x.LocationId).Lng,
                }).ToList();

            var pageResult = new PageResult<PostVm>()
            {
                TotalRecords = totalRow,
                Items = data,
                PageIndex = request.PageIndex,
                PageSize = request.PageSize
            };
            return pageResult;
        }

        public async Task<List<PostVm>> GetAllPostBySearch(string keyword)
        {
            var lsPost = await _context.Posts.Where(x => x.ProductName.Contains(keyword)).ToListAsync();
            var data = lsPost.Select(x => new PostVm()
            {
                PostID = x.PostId,
                Title = x.Title,
                ImageDefault = _context.ImagePosts.AsNoTracking().FirstOrDefault(p => p.PostId == x.PostId && p.IsDefault == true).ImagePath,
                ProductName = x.ProductName,
                NameAccount = _context.Accounts.AsNoTracking().FirstOrDefault(p => p.AccountId == x.AccountId).FullName,
                Price = x.Price,
                WeightName = _context.WeightTypes.AsNoTracking().FirstOrDefault(p => p.WeightId == x.WeightId).WeightName,
                WeightNumber = x.WeightNumber,
                ViewCount = x.ViewCount,
                CatName = _context.Categories.AsNoTracking().FirstOrDefault(p => p.CategoryId == x.CategoryId).CateName,
                Lat = _context.Locations.AsNoTracking().FirstOrDefault(p => p.LocationId == x.LocationId).Lat,
                Lng = _context.Locations.AsNoTracking().FirstOrDefault(p => p.LocationId == x.LocationId).Lng,
            }).ToList();
            return data;
        }

        public async Task<bool> HiddenPost(int PostID)
        {
            try
            {
                var post = await _context.Posts.FindAsync(PostID);
                post.IsHidden = true;
                _context.Posts.Update(post);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private async Task<string> SaveFile(IFormFile file)
        {
            var originalFileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
            //var fileName = $"{Guid.NewGuid()}{Path.GetExtension(originalFileName)}";
            await _storageService.SaveFileAsync(file.OpenReadStream(), originalFileName, "post");
            return "/" + POST_CONTENT_FOLDER_NAME + "/" + originalFileName;
        }

        public async Task<int> AddImage(int postId, PostImageCreateRequest request)
        {
            var postImage = new ImagePost()
            {
                PostId = postId,
                IsDefault = false
            };
            if (request.ImagePath != null)
                postImage.ImagePath = await this.SaveFile(request.ImagePath);

            _context.ImagePosts.Add(postImage);
            await _context.SaveChangesAsync();
            return postImage.ImageId;
        }

        public async Task<int> UpdateImage(int imageId, PostImageUpdateRequest request)
        {
            var postImage = await _context.ImagePosts.FindAsync(imageId);
            if (postImage == null)
                throw new ChoNSException($"Không tìm thấy ảnh với id: {imageId}");

            if (request.ImagePath != null)
            {
                postImage.ImagePath = await this.SaveFile(request.ImagePath);
            }
            _context.ImagePosts.Update(postImage);
            return await _context.SaveChangesAsync();
        }

        public async Task<int> RemoveImage(int imageId)
        {
            var postImage = await _context.ImagePosts.FindAsync(imageId);
            if (postImage == null)
                throw new ChoNSException($"Không tìm thấy ảnh với id: {imageId}");
            _context.ImagePosts.Remove(postImage);
            return await _context.SaveChangesAsync();
        }

        public async Task<ImagePostViewModel> GetImageById(int imageId)
        {
            var image = await _context.ImagePosts.FindAsync(imageId);
            if (image == null) throw new ChoNSException($"Không tìm ảnh với mã {imageId}");
            var viewModel = new ImagePostViewModel()
            {
                ImageID = image.ImageId,
                ImagePath = image.ImagePath,
                PostID = image.PostId,
                IsDefault = image.IsDefault,
            };
            return viewModel;
        }

        public async Task AddViewcount(int postID)
        {
            var post = await _context.Posts.FindAsync(postID);
            post.ViewCount += 1;
            await _context.SaveChangesAsync();
        }

        public async Task<List<PostVm>> GetListPost()
        {
            var lsPost = await _context.Posts.ToListAsync();
            var data = lsPost.Select(x => new PostVm()
            {
                PostID = x.PostId,
                Title = x.Title,
                ImageDefault = _context.ImagePosts.AsNoTracking().FirstOrDefault(p => p.PostId == x.PostId && p.IsDefault == true).ImagePath,
                ProductName = x.ProductName,
                NameAccount = _context.Accounts.AsNoTracking().FirstOrDefault(p => p.AccountId == x.AccountId).FullName,
                Price = x.Price,
                WeightName = _context.WeightTypes.AsNoTracking().FirstOrDefault(p => p.WeightId == x.WeightId).WeightName,
                WeightNumber = x.WeightNumber,
                ViewCount = x.ViewCount,
                CatName = _context.Categories.AsNoTracking().FirstOrDefault(p => p.CategoryId == x.CategoryId).CateName,
                Lat = _context.Locations.AsNoTracking().FirstOrDefault(p => p.LocationId == x.LocationId).Lat,
                Lng = _context.Locations.AsNoTracking().FirstOrDefault(p => p.LocationId == x.LocationId).Lng,
            }).ToList();
            return data;
        }

        public async Task<PostViewModel> GetPostById(int postID)
        {
            var post = await _context.Posts.FindAsync(postID);
            var lsImage = await _context.ImagePosts.AsNoTracking().Where(p => p.PostId == post.PostId).ToListAsync();
            var viewModel = new PostViewModel()
            {
                PostID = post.PostId,
                Title = post.Title,
                ProductName = post.ProductName,
                NameAccount = _context.Accounts.AsNoTracking().FirstOrDefault(x => x.AccountId == post.AccountId).FullName,
                Price = post.Price,
                WeightName = _context.WeightTypes.AsNoTracking().FirstOrDefault(x => x.WeightId == post.WeightId).WeightName,
                WeightNumber = post.WeightNumber,
                Address = post.Address,
                PhoneNumber = post.PhoneNumber,
                CatName = _context.Categories.AsNoTracking().FirstOrDefault(x => x.CategoryId == post.CategoryId).CateName,
                ViewCount = post.ViewCount,
                Description = post.Description,
                Quality = post.Quality,
                IsDeliver = post.IsDeliver,
                Lat = _context.Locations.AsNoTracking().FirstOrDefault(p => p.LocationId == post.LocationId).Lat,
                Lng = _context.Locations.AsNoTracking().FirstOrDefault(p => p.LocationId == post.LocationId).Lng,
                ListImage = lsImage,
            };

            return viewModel;
        }

        public async Task<PageResult<PostVm>> GetAllByStatusPaging(GetPostByStatusRequest request)
        {
            var lsPost = await _context.Posts.ToListAsync();
            if ((request.Status) != null)
            {
                lsPost = lsPost.Where(x => x.StatusPost == (request.Status)).ToList();
            }

            var totalRow = lsPost.Count();

            var data = lsPost.Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => new PostVm()
                {
                    PostID = x.PostId,
                    Title = x.Title,
                    Address = x.Address,
                    ImageDefault = _context.ImagePosts.AsNoTracking().FirstOrDefault(p => p.PostId == x.PostId && p.IsDefault == true).ImagePath,
                    ProductName = x.ProductName,
                    NameAccount = _context.Accounts.AsNoTracking().FirstOrDefault(p => p.AccountId == x.AccountId).FullName,
                    Price = x.Price,
                    WeightName = _context.WeightTypes.AsNoTracking().FirstOrDefault(p => p.WeightId == x.WeightId).WeightName,
                    WeightNumber = x.WeightNumber,
                    ViewCount = x.ViewCount,
                    CatName = _context.Categories.AsNoTracking().FirstOrDefault(p => p.CategoryId == x.CategoryId).CateName,
                    Lat = _context.Locations.AsNoTracking().FirstOrDefault(p => p.LocationId == x.LocationId).Lat,
                    Lng = _context.Locations.AsNoTracking().FirstOrDefault(p => p.LocationId == x.LocationId).Lng,
                }).ToList();

            var pageResult = new PageResult<PostVm>()
            {
                TotalRecords = totalRow,
                Items = data,
                PageIndex = request.PageIndex,
                PageSize = request.PageSize
            };
            return pageResult;
        }
    }
}