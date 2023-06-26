using AutoMapper;
using OWP.SumReport.LAReport.Models;

namespace OWP.SumReport
{
    public class AutoMapperProfile: Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Assignment, Assignment>();
        }
    }
}
