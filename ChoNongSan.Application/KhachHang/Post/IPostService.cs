﻿using BingMapsRESTToolkit;
using ChoNongSan.Application.Common.Files;
using ChoNongSan.Application.KhachHang.PostImages;
using ChoNongSan.Data.Models;
using ChoNongSan.Utilities.Exceptions;
using ChoNongSan.ViewModels.Common;
using ChoNongSan.ViewModels.Requests.Common;
using ChoNongSan.ViewModels.Requests.DanhMuc;
using ChoNongSan.ViewModels.Requests.TinDang;
using ChoNongSan.ViewModels.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ChoNongSan.Application.KhachHang.Posts
{
    public interface IPostService
    {
        Task<int> CreatePost(CreatePostRequest request);

        Task<bool> HiddenPost(int PostID);

        Task<PageResult<PostVmTongQuat>> GetAllByStatusPaging(GetPagingCommonRequest request);

        Task<int> AddImage(int postId, PostImageCreateRequest request);

        Task<int> UpdateImage(int imageId, PostImageUpdateRequest request);

        Task<int> RemoveImage(int imageId);

        Task<ImagePostVm> GetImageById(int imageId);

        Task<PostVmChiTiet> GetPostById(int postID);

        Task AddViewcount(int postID);

        Task<List<PostVmTongQuat>> GetListPostForApp();

        Task<PageResult<PostVmTongQuat>> GetAllPostsViewHome(GetPagingCommonRequest request);
    }

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

        public async Task<PostVmChiTiet> GetPostById(int postID)
        {
            var post = await _context.Posts.FindAsync(postID);
            var lsImage = await _context.ImagePosts.AsNoTracking().Where(p => p.PostId == post.PostId).ToListAsync();
            var viewModel = new PostVmChiTiet()
            {
                PostID = post.PostId,
                Title = post.Title,
                ProductName = post.ProductName,
                NameAccount = _context.Accounts.AsNoTracking().FirstOrDefault(x => x.AccountId == post.AccountId).FullName,
                Price = post.Price,
                WeightName = _context.WeightTypes.AsNoTracking().FirstOrDefault(x => x.WeightId == post.WeightId).WeightName,
                WeightNumber = post.WeightNumber,
                Address = post.Address,
                Phone = post.PhoneNumber,
                CatName = _context.Categories.AsNoTracking().FirstOrDefault(x => x.CategoryId == post.CategoryId).CateName,
                ViewCount = post.ViewCount,
                Description = post.Description,
                Quality = post.Quality,
                IsDeliver = post.IsDeliver,
                StatusPost = post.StatusPost,
                TimePost = post.PostTime,
                Lat = _context.Locations.AsNoTracking().FirstOrDefault(p => p.LocationId == post.LocationId).Lat,
                Lng = _context.Locations.AsNoTracking().FirstOrDefault(p => p.LocationId == post.LocationId).Lng,
                ListImage = lsImage,
                Expiry = post.Expiry
            };

            return viewModel;
        }

        public async Task<PageResult<PostVmTongQuat>> GetAllByStatusPaging(GetPagingCommonRequest request)
        {
            var lsPost = await _context.Posts.ToListAsync();
            if ((request.ById) != null)
            {
                lsPost = lsPost.Where(x => x.StatusPost == (request.ById)).ToList();
            }

            var totalRow = lsPost.Count;

            var data = lsPost.Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => new PostVmTongQuat()
                {
                    PostID = x.PostId,
                    Title = x.Title,
                    ProductName = x.ProductName,
                    NameAccount = _context.Accounts.AsNoTracking().FirstOrDefault(p => p.AccountId == x.AccountId).FullName,
                    Price = x.Price,
                    WeightName = _context.WeightTypes.AsNoTracking().FirstOrDefault(p => p.WeightId == x.WeightId).WeightName,
                    WeightNumber = x.WeightNumber,
                    ViewCount = x.ViewCount,
                    Description = x.Description,
                    StatusPost = x.StatusPost,
                    TimePost = x.PostTime,
                    ImageDefault = _context.ImagePosts.AsNoTracking().FirstOrDefault(p => p.PostId == x.PostId && p.IsDefault == true).ImagePath,
                    Lat = _context.Locations.AsNoTracking().FirstOrDefault(p => p.LocationId == x.LocationId).Lat,
                    Lng = _context.Locations.AsNoTracking().FirstOrDefault(p => p.LocationId == x.LocationId).Lng,
                }).ToList();

            var pageResult = new PageResult<PostVmTongQuat>()
            {
                TotalRecords = totalRow,
                Items = data,
                PageIndex = request.PageIndex,
                PageSize = request.PageSize
            };
            return pageResult;
        }

        public async Task<PageResult<PostVmTongQuat>> GetAllPostsViewHome(GetPagingCommonRequest request)
        {
            var lsPost = await _context.Posts.Where(x => x.IsHidden == false && x.StatusPost == 2).ToListAsync();
            if (!String.IsNullOrEmpty(request.Keyword))
            {
                request.Keyword = request.Keyword.ToLower();
                lsPost = lsPost.Where(x => x.ProductName.ToLower().Contains(request.Keyword)
                 || x.Address.ToLower().Contains(request.Keyword) || x.Title.ToLower().Contains(request.Keyword)).ToList();
            }

            if (request.ById != 0)
            {
                lsPost = lsPost.Where(x => x.CategoryId == request.ById).ToList();
            }

            var totalRow = lsPost.Count;

            var data = lsPost.Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => new PostVmTongQuat()
                {
                    PostID = x.PostId,
                    Title = x.Title,
                    ProductName = x.ProductName,
                    NameAccount = _context.Accounts.AsNoTracking().FirstOrDefault(p => p.AccountId == x.AccountId).FullName,
                    Price = x.Price,
                    WeightName = _context.WeightTypes.AsNoTracking().FirstOrDefault(p => p.WeightId == x.WeightId).WeightName,
                    WeightNumber = x.WeightNumber,
                    ViewCount = x.ViewCount,
                    Description = x.Description,
                    StatusPost = x.StatusPost,
                    TimePost = x.PostTime,
                    ImageDefault = _context.ImagePosts.AsNoTracking().FirstOrDefault(p => p.PostId == x.PostId && p.IsDefault == true).ImagePath,
                    Lat = _context.Locations.AsNoTracking().FirstOrDefault(p => p.LocationId == x.LocationId).Lat,
                    Lng = _context.Locations.AsNoTracking().FirstOrDefault(p => p.LocationId == x.LocationId).Lng,
                }).ToList();

            var pageResult = new PageResult<PostVmTongQuat>()
            {
                TotalRecords = totalRow,
                Items = data,
                PageIndex = request.PageIndex,
                PageSize = request.PageSize
            };
            return pageResult;
        }

        public async Task<List<PostVmTongQuat>> GetListPostForApp()
        {
            var lsPost = await _context.Posts.AsNoTracking().Where(x => x.StatusPost == 2).ToListAsync();
            var data = lsPost.Select(x => new PostVmTongQuat()
            {
                PostID = x.PostId,
                Title = x.Title,
                ImageDefault = _context.ImagePosts.AsNoTracking().FirstOrDefault(p => p.PostId == x.PostId && p.IsDefault == true).ImagePath,
                ViewCount = x.ViewCount,
                StatusPost = x.StatusPost,
                Description = x.Description,
                TimePost = x.PostTime,
                NameAccount = _context.Accounts.AsNoTracking().FirstOrDefault(p => p.AccountId == x.AccountId).FullName,
                ProductName = x.ProductName,
                Price = x.Price,
                WeightNumber = x.WeightNumber,
                WeightName = _context.WeightTypes.AsNoTracking().FirstOrDefault(p => p.WeightId == x.WeightId).WeightName,
                Lat = _context.Locations.AsNoTracking().FirstOrDefault(p => p.LocationId == x.LocationId).Lat,
                Lng = _context.Locations.AsNoTracking().FirstOrDefault(p => p.LocationId == x.LocationId).Lng,
            }).ToList();
            return data;
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
                AccountId = (int)request.AccountID,
                CategoryId = request.CategoryID,
                WeightId = request.WeightId,
                Expiry = request.Expiry
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
                //new ImagePost()
                //{
                //    ImagePath = await this.SaveFile(item),
                //    IsDefault = true,
                //}
                var lsImg = new List<ImagePost>();
                for (var i = 0; i < request.ThumbnailImage.Count; i++)
                {
                    if (i == 0)
                    {
                        lsImg.Add(new ImagePost()
                        {
                            ImagePath = await this.SaveFile(request.ThumbnailImage[i]),
                            IsDefault = true,
                        });
                    }
                    else
                    {
                        lsImg.Add(new ImagePost()
                        {
                            ImagePath = await this.SaveFile(request.ThumbnailImage[i]),
                            IsDefault = false,
                        });
                    }
                }
                post.ImagePosts = lsImg;
                //post.ImagePosts = new List<ImagePost>()
                //{
                //    new ImagePost()
                //    {
                //        ImagePath = await this.SaveFile(item),
                //        IsDefault = true,
                //    }
                //};
            }

            _context.Posts.Add(post);
            return await _context.SaveChangesAsync();
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

        public async Task AddViewcount(int postID)
        {
            var post = await _context.Posts.FindAsync(postID);
            post.ViewCount += 1;
            await _context.SaveChangesAsync();
        }

        private async Task<string> SaveFile(IFormFile file)
        {
            var originalFileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(originalFileName)}";
            await _storageService.SaveFileAsync(file.OpenReadStream(), fileName, "post");
            return "/" + POST_CONTENT_FOLDER_NAME + "/" + fileName;
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

        public async Task<ImagePostVm> GetImageById(int imageId)
        {
            var image = await _context.ImagePosts.FindAsync(imageId);
            if (image == null) throw new ChoNSException($"Không tìm ảnh với mã {imageId}");
            var viewModel = new ImagePostVm()
            {
                ImageID = image.ImageId,
                ImagePath = image.ImagePath,
                PostID = image.PostId,
                IsDefault = image.IsDefault,
            };
            return viewModel;
        }
    }
}