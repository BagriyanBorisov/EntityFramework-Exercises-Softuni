using Footballers.Data.Models;
using Footballers.DataProcessor.ExportDto;

namespace Footballers
{
    using AutoMapper;

    // Configure your AutoMapper here if you wish to use it. If not, DO NOT DELETE OR RENAME THIS CLASS
    public class FootballersProfile : Profile
    {
        public FootballersProfile()
        {
            CreateMap<Footballer, ExportFootballerDto>();
            CreateMap<Coach, ExportCoachDto>();
        }
    }
}
