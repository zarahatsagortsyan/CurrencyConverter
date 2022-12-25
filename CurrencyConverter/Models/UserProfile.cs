using CurrencyConverter.ViewModels;
using AutoMapper;

namespace CurrencyConverter.Models
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<Users, RegisterViewModel>();
            CreateMap<RegisterViewModel, Users>();
        }
    }
}
