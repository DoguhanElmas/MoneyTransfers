using Core;
using Core.Dto.Request;
using Core.Dto.Response;
using Core.Models;
using Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Services.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;

        public UserService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<UserDetailResponse> GetUserDetail(UserDetailRequest userDetailRequest)
        {
            try
            {
                var user = await _unitOfWork.Users.GetByIdAsync(userDetailRequest.UserId);
                var response = new UserDetailResponse();
                if (user != null)
                {
                    response.Success = true;
                    response.Balance = user.Balance;
                }
                return response;
            }
            catch (Exception e)
            {

                var log = new Log();
                log.StackTrace = e.StackTrace;
                log.Message = e.Message;
                log.Request = JsonSerializer.Serialize(userDetailRequest);
                log.FunctionName = "Service-GetUserDetail";
                log.DateTime = DateTime.Now;
                await _unitOfWork.Logs.AddAsync(log);
                await _unitOfWork.Logs.SaveChangesAsync();
                return new UserDetailResponse();
            }

        }
    }
}
