using CurrencyConverter.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
namespace CurrencyConverter.Models
{
    public class UserProfile:Profile
    {
        public UserProfile()
        {
            CreateMap<Users, RegisterViewModel>();
            CreateMap<RegisterViewModel, Users>();
        }
    }
}
