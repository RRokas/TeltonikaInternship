using AutoMapper;
using Core.DTOs;

namespace Core
{
    public class CoreMapper
    {
        public IMapper CreateMapper()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<DtoMapProfile>();
            });

            return config.CreateMapper();
        }
    }
}