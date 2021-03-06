using BingMapsRESTToolkit;
using ChoNongSan.Application.Common.Files;
using ChoNongSan.Application.KhachHang.PostImages;
using ChoNongSan.Data.Models;
using ChoNongSan.Utilities.Exceptions;
using ChoNongSan.ViewModels.Common;
using ChoNongSan.ViewModels.Requests;
using ChoNongSan.ViewModels.Requests.Common;
using ChoNongSan.ViewModels.Requests.DanhMuc;
using ChoNongSan.ViewModels.Requests.TinDang;
using ChoNongSan.ViewModels.Responses;
using ChoNongSan.ViewModels.Responses.TinDang;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ChoNongSan.Application.KhachHang.Posts
{
	public interface IPostService
	{
		Task<bool> CreatePost(CreatePostRequest request);

		Task<bool> HiddenPost(int PostID);

		Task<PageResult<PostVmTongQuat>> GetAllByStatusPaging(int accountId, GetPagingCommonRequest request);

		Task<int> AddImage(int postId, PostImageCreateRequest request);

		Task<int> UpdateImage(int imageId, PostImageUpdateRequest request);

		Task<int> RemoveImage(int imageId);

		Task<ImagePostVm> GetImageById(int imageId);

		Task<PostVmChiTiet> GetPostById(int postID);

		Task<List<PostVmTongQuat>> GetListPostForApp();

		Task<List<PostVmTongQuat>> GetListPostBySearch(string keyword);

		Task<PageResult<PostVmTongQuat>> GetAllPostsViewHome(FilterPostRequest request);

		Task<string> AddLovePost(LoveRequest request);

		Task<PageResult<PostVmTongQuat>> GetAllLoveByAccountId(GetPagingCommonRequest request);

		Task<List<PostVmTongQuat>> GetAllManyViews(int number);

		Task<List<PostVmTongQuat>> PostNew(int number);

		Task<List<PostVmTongQuat>> GetAllPostByAccountId(int accountId);
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
			var lsImage = await _context.ImagePosts.AsNoTracking().Where(p => p.PostId == post.PostId).Select(y => y.ImagePath).ToListAsync();
			var danhgia = await _context.Reviews.Where(x => x.PostId == postID).ToListAsync();
			var sosao = 0.0;
			if (danhgia.Count() != 0)
			{
				sosao = (double)danhgia.Average(x => x.NumberOfReviews);
			}
			
			var viewModel = new PostVmChiTiet()
			{
				PostID = post.PostId,
				SoSao = sosao,
				Title = post.Title,
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
				StatusPost = (int)post.StatusPost,
				TimePost = post.PostTime,
				Lat = _context.Locations.AsNoTracking().FirstOrDefault(p => p.LocationId == post.LocationId).Lat,
				Lng = _context.Locations.AsNoTracking().FirstOrDefault(p => p.LocationId == post.LocationId).Lng,
				ListImage = lsImage,
				Expiry = post.Expiry,
				AccountId = post.AccountId,
				Reason = post.Reason,
				Avatar = _context.Accounts.AsNoTracking().FirstOrDefault(x => x.AccountId == post.AccountId).Avatar,
			};
			post.ViewCount += 1;
			_context.Posts.Update(post);
			_context.SaveChanges();
			return viewModel;
		}

		public async Task<PageResult<PostVmTongQuat>> GetAllByStatusPaging(int accountId, GetPagingCommonRequest request)
		{
			var lsPost = await _context.Posts.Where(x => x.AccountId == accountId).ToListAsync();
			var pageResult = new PageResult<PostVmTongQuat>();
			pageResult.Stt0 = lsPost.Where(x => x.StatusPost == 0).ToList().Count;
			pageResult.Stt1 = lsPost.Where(x => x.StatusPost == 1).ToList().Count;
			pageResult.Stt2 = lsPost.Where(x => x.StatusPost == 2).ToList().Count;
			pageResult.Stt3 = lsPost.Where(x => x.StatusPost == 3).ToList().Count;

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
					NumImg = _context.ImagePosts.AsNoTracking().Where(y => y.PostId == x.PostId).ToList().Count,
					NameAccount = _context.Accounts.AsNoTracking().FirstOrDefault(p => p.AccountId == x.AccountId).FullName,
					Price = x.Price,
					WeightName = _context.WeightTypes.AsNoTracking().FirstOrDefault(p => p.WeightId == x.WeightId).WeightName,
					WeightNumber = x.WeightNumber,
					ViewCount = x.ViewCount,
					Description = x.Description,
					StatusPost = x.StatusPost,
					TimePost = x.PostTime,
					Reason = x.Reason,
					Address = x.Address,
					ImageDefault = _context.ImagePosts.AsNoTracking().FirstOrDefault(p => p.PostId == x.PostId && p.IsDefault == true).ImagePath,
					Lat = _context.Locations.AsNoTracking().FirstOrDefault(p => p.LocationId == x.LocationId).Lat,
					Lng = _context.Locations.AsNoTracking().FirstOrDefault(p => p.LocationId == x.LocationId).Lng,
				}).ToList();

			pageResult.TotalRecords = totalRow;
			pageResult.Items = data;
			pageResult.PageIndex = request.PageIndex;
			pageResult.PageSize = request.PageSize;

			return pageResult;
		}

		public async Task<PageResult<PostVmTongQuat>> GetAllPostsViewHome(FilterPostRequest request)
		{
			List<Post> lsPost;
			var pageResult = new PageResult<PostVmTongQuat>();
			if (request.Roles == 3) //Phân biệt trang KH với trang CTV (3: KH, còn lại là ctv)
			{
				lsPost = await (from p in _context.Posts.AsNoTracking()
								where p.IsHidden == false && p.StatusPost == 2
								select p
						  ).ToListAsync();
				//lsPost = await _context.Posts.AsNoTracking().Where(x => x.IsHidden == false && x.StatusPost == 2).ToListAsync();
				pageResult.MaxPrice = (decimal)lsPost.Max(p => p.Price);
			}
			else
			{
				lsPost = await _context.Posts.AsNoTracking().Where(x => x.IsHidden == false && x.StatusPost == 0).ToListAsync();
			}

			if (!String.IsNullOrEmpty(request.Keyword))
			{
				request.Keyword = request.Keyword.ToLower();
				lsPost = lsPost.Where(x => x.Address.ToLower().Contains(request.Keyword) ||
					x.Title.ToLower().Contains(request.Keyword) ||
					_context.Categories.FirstOrDefault(y => y.CategoryId == x.CategoryId).CateName.ToLower().Contains(request.Keyword)
					).ToList();
			}

			if (request.ById != null && request.ById != 0)
			{
				lsPost = lsPost.Where(x => x.CategoryId == request.ById).ToList();
			}

			if (request != null)
			{
				if (request.CategoryId != 0)
				{
					lsPost = lsPost.Where(x => x.CategoryId == request.CategoryId).ToList();
				}

				if (!string.IsNullOrEmpty(request.SortPost))
				{
					//if (request.RequestFilterPost.SortPost.Contains("MacDinh")) thì cứ để y như vậy

					if (request.SortPost.Contains("TinMoi"))
						lsPost = (from p in lsPost
								  orderby p.PostTime descending
								  select p
								  ).ToList();
					else if (request.SortPost.Contains("TinCu"))
						lsPost = (from p in lsPost
								  orderby p.PostTime ascending
								  select p
								  ).ToList();
					else if (request.SortPost.Contains("GiaCao"))
						lsPost = (from p in lsPost
								  orderby p.Price descending
								  select p
								  ).ToList();
					else if (request.SortPost.Contains("GiaThap"))
						lsPost = (from p in lsPost
								  orderby p.Price ascending
								  select p
								  ).ToList();
				}

				request.Quality = request.Quality == null ? "TatCa" : request.Quality;
				request.IsDeliver = request.IsDeliver == null ? "TatCa" : request.IsDeliver;

				if (!request.Quality.Contains("TatCa"))
				{
					lsPost = (from p in lsPost
							  where p.Quality.Contains(request.Quality)
							  select p).ToList();
				}

				if (!request.IsDeliver.Contains("TatCa") && request.IsDeliver != null)
				{
					lsPost = (from p in lsPost
							  where p.IsDeliver == (request.IsDeliver.Contains("Co") ? true : false)
							  select p).ToList();
				}
				if (request.MinPrice != 0 && request.MaxPrice != 0)
				{
					lsPost = (from p in lsPost
							  where p.Price >= request.MinPrice
								&& p.Price <= request.MaxPrice
							  select p).ToList();
				}
			}

			var totalRow = lsPost.Count;
			var data = lsPost.Skip((request.PageIndex - 1) * request.PageSize)
				.Take(request.PageSize)
				.Select(x => new PostVmTongQuat()
				{
					PostID = x.PostId,
					//CategoryName = _context.Categories.AsNoTracking().SingleOrDefault(p => p.CategoryId == x.CategoryId).CateName,
					Title = x.Title,
					Address = x.Address,
					NumImg = _context.ImagePosts.AsNoTracking().Where(y => y.PostId == x.PostId).Count(),
					NameAccount = _context.Accounts.AsNoTracking().SingleOrDefault(p => p.AccountId == x.AccountId).FullName,
					Price = x.Price,
					WeightName = _context.WeightTypes.AsNoTracking().SingleOrDefault(p => p.WeightId == x.WeightId).WeightName,
					WeightNumber = x.WeightNumber,
					ViewCount = x.ViewCount,
					Description = x.Description,
					StatusPost = x.StatusPost,
					TimePost = x.PostTime,
					ImageDefault = _context.ImagePosts.AsNoTracking().SingleOrDefault(p => p.PostId == x.PostId && p.IsDefault == true).ImagePath,
				}).ToList();

			pageResult.TotalRecords = totalRow;
			pageResult.Items = data;
			pageResult.PageIndex = request.PageIndex;
			pageResult.PageSize = request.PageSize;

			return pageResult;
		}

		public async Task<List<PostVmTongQuat>> GetListPostForApp()
		{
			var lsPost = await _context.Posts.AsNoTracking().Where(x => x.StatusPost == 2).ToListAsync();
			var data = lsPost.Select(x => new PostVmTongQuat()
			{
				PostID = x.PostId,
				Title = x.Title,
				NumImg = _context.ImagePosts.AsNoTracking().Where(p => p.PostId == x.PostId).ToList().Count,
				CategoryName = _context.Categories.AsNoTracking().FirstOrDefault(p => p.CategoryId == p.CategoryId).CateName,
				ImageDefault = _context.ImagePosts.AsNoTracking().FirstOrDefault(p => p.PostId == x.PostId && p.IsDefault == true).ImagePath,
				ViewCount = x.ViewCount,
				StatusPost = x.StatusPost,
				Description = x.Description,
				TimePost = x.PostTime,
				NameAccount = _context.Accounts.AsNoTracking().FirstOrDefault(p => p.AccountId == x.AccountId).FullName,
				Price = x.Price,
				WeightNumber = x.WeightNumber,
				Address = x.Address,
				WeightName = _context.WeightTypes.AsNoTracking().FirstOrDefault(p => p.WeightId == x.WeightId).WeightName,
				Lat = _context.Locations.AsNoTracking().FirstOrDefault(p => p.LocationId == x.LocationId).Lat,
				Lng = _context.Locations.AsNoTracking().FirstOrDefault(p => p.LocationId == x.LocationId).Lng,
			}).ToList();
			return data;
		}

		public async Task<List<PostVmTongQuat>> GetListPostBySearch(string keyword)
		{
			var lsPost = await _context.Posts.AsNoTracking().Where(x => x.StatusPost == 2).ToListAsync();
			if (!string.IsNullOrEmpty(keyword))
			{
				lsPost = (from p in lsPost
						  where _context.Categories.AsNoTracking().FirstOrDefault(x => x.CategoryId == p.CategoryId).CateName.ToLower().Contains(keyword.ToLower())
							|| p.Title.ToLower().Contains(keyword.ToLower())
						  select p
						  ).ToList();
			}
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
				Price = x.Price,
				WeightNumber = x.WeightNumber,
				CategoryName = _context.Categories.AsNoTracking().FirstOrDefault(p => p.CategoryId == x.CategoryId).CateName,
				NumImg = _context.ImagePosts.AsNoTracking().Where(p => p.PostId == x.PostId).ToList().Count,
				Address = x.Address,
				WeightName = _context.WeightTypes.AsNoTracking().FirstOrDefault(p => p.WeightId == x.WeightId).WeightName,
				Lat = _context.Locations.AsNoTracking().FirstOrDefault(p => p.LocationId == x.LocationId).Lat,
				Lng = _context.Locations.AsNoTracking().FirstOrDefault(p => p.LocationId == x.LocationId).Lng,
			}).ToList();
			return data;
		}

		public async Task<bool> CreatePost(CreatePostRequest request)
		{
			try
			{
				var post = new Post()
				{
					Title = request.Title,
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
					CategoryId = (int)request.CategoryID,
					WeightId = (int)request.WeightId,
					Expiry = request.Expiry
				};
				PlatformEnum check = PlatformEnum.Web;
				if (request.PlatForm.Equals("App"))
					check = PlatformEnum.App;

				if (PlatformEnum.Web == check)
				{
					if (!string.IsNullOrEmpty(request.Address) && request.Province != 0 && request.District != 0 && request.SubDistrict != 0)
					{
						//lấy ra tên Tỉnh, Huyện, Xã
						string urlAdd = $"https://provinces.open-api.vn/api/?depth=3";
						string tinh = "";
						string huyen = "";
						string xa = "";
						using (var webClient = new System.Net.WebClient())
						{
							var json = webClient.DownloadString(urlAdd);

							var dataTinh = (JArray)JsonConvert.DeserializeObject(json);
							foreach (var province in dataTinh)
							{
								var codePro = Convert.ToInt32(province.Children<JProperty>().FirstOrDefault(x => x.Name == "code").Value);
								if (codePro == request.Province)
								{
									tinh = Convert.ToString(province.Children<JProperty>().FirstOrDefault(x => x.Name == "name").Value);

									var dtHuyen = (JArray)JsonConvert.DeserializeObject(Convert.ToString(province["districts"]));
									foreach (var district in dtHuyen)
									{
										var codeDis = Convert.ToInt32(district.Children<JProperty>().FirstOrDefault(p => p.Name == "code").Value);
										if (codeDis == request.District)
										{
											huyen = Convert.ToString(district.Children<JProperty>().FirstOrDefault(p => p.Name == "name").Value);

											var dtxa = (JArray)JsonConvert.DeserializeObject(Convert.ToString(district["wards"]));
											foreach (var subdistrict in dtxa)
											{
												var codeSubDis = Convert.ToInt32(subdistrict.Children<JProperty>().FirstOrDefault(p => p.Name == "code").Value);
												if (codeSubDis == request.SubDistrict)
												{
													xa = Convert.ToString(subdistrict.Children<JProperty>().FirstOrDefault(p => p.Name == "name").Value);
													break;
												}
											}
											break;
										}
									}
									break;
								}
							}
						}

						if (!string.IsNullOrEmpty(xa) && !string.IsNullOrEmpty(huyen) && !string.IsNullOrEmpty(tinh))
						{
							request.Address = request.Address + ", " + xa + ", " + huyen + ", " + tinh;
						}

						string url = $"https://api.map4d.vn/sdk/autosuggest?text={request.Address}&Key=acaa76a4fa1828592ffb38d431b75aea";
						using (var webClient = new System.Net.WebClient())
						{
							var json = webClient.DownloadString(url);
							// Now parse with JSON.Net
							var datax = (JObject)JsonConvert.DeserializeObject(json);

							post.Location = new Data.Models.Location()
							{
								Lat = Convert.ToString(datax["result"][0]["location"]["lat"]),
								Lng = Convert.ToString(datax["result"][0]["location"]["lng"]),
							};
						}
					}
				}
				else
				{
					post.Location = new Data.Models.Location()
					{
						Lat = request.Lat,
						Lng = request.Lng,
					};
				}

				if (request.ThumbnailImage != null)
				{
					//post.ImagePosts = new List<ImagePost>()
					//{
					//    new ImagePost()
					//    {
					//        ImagePath = await this.SaveFile(request.ThumbnailImage),
					//        IsDefault = true,
					//    }
					//};

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
				}
				var user = await _context.Accounts.FindAsync(request.AccountID);
				user.NumberOfPost = user.NumberOfPost + 1;

				_context.Posts.Add(post);
				_context.Accounts.Update(user);
				await _context.SaveChangesAsync();
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		public async Task<bool> HiddenPost(int PostID)
		{
			try
			{
				var post = await _context.Posts.FindAsync(PostID);
				post.IsHidden = true;
				post.StatusPost = 3;
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

		public async Task<string> AddLovePost(LoveRequest request)
		{
			try
			{
				var love = new Love()
				{
					AccountId = request.accountId,
					PostId = request.postId
				};
				_context.Add(love);
				await _context.SaveChangesAsync();
				return "Tin đã được lưu vào danh sách yêu thích";
			}
			catch (Exception)
			{
				return "Không thể thêm tin vào yêu thích";
			}
		}

		public async Task<PageResult<PostVmTongQuat>> GetAllLoveByAccountId(GetPagingCommonRequest request)
		{
			var lsPostLove = (from p in _context.Posts
							  join lo in _context.Loves on p.PostId equals lo.PostId
							  where lo.AccountId == request.ById
							  select p);
			var totalRow = lsPostLove.Count();

			var data = await lsPostLove.Skip((request.PageIndex - 1) * request.PageSize)
				.Take(request.PageSize).Select(x => new PostVmTongQuat()
				{
					Title = x.Title,
					Description = x.Description,
					NameAccount = _context.Accounts.AsNoTracking().FirstOrDefault(y => y.AccountId == x.AccountId).FullName,
					ImageDefault = _context.ImagePosts.AsNoTracking().FirstOrDefault(y => y.PostId == x.PostId && y.IsDefault == true).ImagePath,
					Price = x.Price,
					PostID = x.PostId,
					Lat = _context.Locations.AsNoTracking().FirstOrDefault(y => y.LocationId == x.LocationId).Lat,
					Lng = _context.Locations.AsNoTracking().FirstOrDefault(y => y.LocationId == x.LocationId).Lng,
					StatusPost = x.StatusPost,
					TimePost = x.PostTime,
					NumImg = _context.ImagePosts.AsNoTracking().Where(y => y.PostId == x.PostId).ToList().Count,
					ViewCount = x.ViewCount,
					WeightName = _context.WeightTypes.AsNoTracking().FirstOrDefault(y => y.WeightId == x.WeightId).WeightName,
					WeightNumber = x.WeightNumber
				}).ToListAsync();

			var pageResult = new PageResult<PostVmTongQuat>()
			{
				TotalRecords = totalRow,
				Items = data,
				PageIndex = request.PageIndex,
				PageSize = request.PageSize,
			};

			return pageResult;
		}

		public async Task<List<PostVmTongQuat>> GetAllManyViews(int number)
		{
			var kq = await (from p in _context.Posts.AsNoTracking()
							where p.StatusPost == 2
							orderby p.ViewCount descending
							select p).Take(number).ToListAsync();
			var data = kq.Select(x => new PostVmTongQuat()
			{
				Description = x.Description,
				Address = x.Address,
				ImageDefault = _context.ImagePosts.AsNoTracking().FirstOrDefault(y => y.PostId == x.PostId && y.IsDefault == true).ImagePath,
				//Lat = _context.Locations.AsNoTracking().FirstOrDefault(y => y.LocationId == x.LocationId).Lat,
				//Lng = _context.Locations.AsNoTracking().FirstOrDefault(y => y.LocationId == x.LocationId).Lng,
				//NameAccount = _context.Accounts.AsNoTracking().FirstOrDefault(y => y.AccountId == x.AccountId).FullName,
				PostID = x.PostId,
				Price = x.Price,
				Reason = x.Reason,
				StatusPost = x.StatusPost,
				TimePost = x.PostTime,
				NumImg = _context.ImagePosts.AsNoTracking().Where(y => y.PostId == x.PostId).Count(),
				Title = x.Title,
				ViewCount = x.ViewCount,
				WeightNumber = x.WeightNumber,
				WeightName = _context.WeightTypes.AsNoTracking().FirstOrDefault(y => y.WeightId == x.WeightId).WeightName,
			}).ToList();
			return data;
		}

		public async Task<List<PostVmTongQuat>> PostNew(int number)
		{
			var kq = await (from p in _context.Posts.AsNoTracking()
							where p.StatusPost == 2
							orderby p.PostId descending
							select p).Take(number).ToListAsync();

			var data = kq.Select(x => new PostVmTongQuat()
			{
				Description = x.Description,
				ImageDefault = _context.ImagePosts.AsNoTracking().FirstOrDefault(y => y.PostId == x.PostId && y.IsDefault == true).ImagePath,
				//Lat = _context.Locations.AsNoTracking().FirstOrDefault(y => y.LocationId == x.LocationId).Lat,
				//Lng = _context.Locations.AsNoTracking().FirstOrDefault(y => y.LocationId == x.LocationId).Lng,
				//NameAccount = _context.Accounts.AsNoTracking().FirstOrDefault(y => y.AccountId == x.AccountId).FullName,
				NumImg = _context.ImagePosts.AsNoTracking().Where(y => y.PostId == x.PostId).Count(),
				PostID = x.PostId,
				Price = x.Price,
				Reason = x.Reason,
				StatusPost = x.StatusPost,
				TimePost = x.PostTime,
				Title = x.Title,
				Address = x.Address,
				ViewCount = x.ViewCount,
				WeightNumber = x.WeightNumber,
				//WeightName = _context.WeightTypes.AsNoTracking().FirstOrDefault(y => y.WeightId == x.WeightId).WeightName,
			}).ToList();
			return data;
		}

		public async Task<List<PostVmTongQuat>> GetAllPostByAccountId(int accountId)
		{
			var lsPost = await _context.Posts.AsNoTracking().Where(x => x.AccountId == accountId).ToListAsync();
			var data = lsPost.Select(x => new PostVmTongQuat()
			{
				Title = x.Title,
				PostID = x.PostId,
				Address = x.Address,
				Description = x.Description,
				Price = x.Price,
				Reason = x.Reason,
				TimePost = x.PostTime,
				StatusPost = x.StatusPost,
				ViewCount = x.ViewCount,
				WeightNumber = x.WeightNumber,
				ImageDefault = _context.ImagePosts.AsNoTracking().FirstOrDefault(p => p.PostId == x.PostId && p.IsDefault == true).ImagePath,
				NameAccount = _context.Accounts.AsNoTracking().FirstOrDefault(p => p.AccountId == x.AccountId).FullName,
				Lat = _context.Locations.AsNoTracking().FirstOrDefault(p => p.LocationId == x.LocationId).Lat,
				Lng = _context.Locations.AsNoTracking().FirstOrDefault(p => p.LocationId == x.LocationId).Lng,
				NumImg = _context.ImagePosts.AsNoTracking().Where(p => p.PostId == x.PostId).ToList().Count,
				WeightName = _context.WeightTypes.AsNoTracking().FirstOrDefault(p => p.WeightId == x.WeightId).WeightName,
			}).ToList();
			return data;
		}
	}
}