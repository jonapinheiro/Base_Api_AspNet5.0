using AutoMapper;
using MkW_Models.Dto;
using MkW_Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MkW_Models.Profiles
{
    public class ProfilesDto : Profile
    {
        public ProfilesDto()
        {
            CreateMap<UserDto, EntityUserLogin>();
           
        }
    }
}
