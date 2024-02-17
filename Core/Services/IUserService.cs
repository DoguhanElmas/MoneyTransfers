using Core.Dto.Request;
using Core.Dto.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Services
{
    public interface IUserService
    {
        Task<UserDetailResponse> GetUserDetail(UserDetailRequest userDetailRequest);
    }
}
