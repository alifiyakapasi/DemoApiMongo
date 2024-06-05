using DemoApiMongo.Entities.DataModels;
using DemoApiMongo.Entities.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoApiMongo.Repository
{
    public interface IUserRepo
    {
        bool IsUserUnique (string username);

        Task<LoginResponseDto> GetLogin (LoginRequestDto login);

        Task<User> Register(RegistrationRequestDto registration);
    }
}
