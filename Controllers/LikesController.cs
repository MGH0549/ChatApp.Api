using ChatApp.DTOs;
using ChatApp.Entities;
using ChatApp.Extensions;
using ChatApp.Helpers;
using ChatApp.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.Controllers
{
    public class LikesController : BaseApiController
    {
        private readonly IUnitOfWork _unitofwork;

        public LikesController(IUnitOfWork unitofwork)
        {
            _unitofwork = unitofwork;
        }

        [HttpPost("{username}")]
        public async Task<ActionResult> AddLike(string username)
        {
            var sourceUserId = User.GetUserId();
            var likedUser = await _unitofwork.UserRepository.GetUserByUsernameAsync(username);
            var sourceUser = await _unitofwork.LikeRepository.GetUserWithLikes(sourceUserId);

            if (likedUser == null) return NotFound();

            if (sourceUser.UserName == username) return BadRequest("You cannot like yourself");

            var userLike = await _unitofwork.LikeRepository.GetUserLike(sourceUserId, likedUser.Id);

            if (userLike != null) return BadRequest("You already like this user");

            userLike = new UserLike
            {
                SourceUserId = sourceUserId,
                TargetUserId = likedUser.Id
            };

            sourceUser.LikedUsers.Add(userLike);

            if (await _unitofwork.Complete()) return Ok();

            return BadRequest("Failed to like user");
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<LikeDto>>> GetUserLikes(
            [FromQuery] LikesParams likesParams)
        {
            likesParams.UserId = User.GetUserId();

            var users = await _unitofwork.LikeRepository.GetUserLikes(likesParams);

            Response.AddPaginationHeader(
                new PaginationHeader(
                    users.CurrentPage,
                    users.PageSize,
                    users.TotalCount,
                    users.TotalPages));

            return Ok(users);
        }
    }
}