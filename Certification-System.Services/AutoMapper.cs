using AutoMapper;
using Certification_System.DTOViewModels;
using Certification_System.Entities;

namespace Certification_System.Services
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Branch,DisplayBranchViewModel>();
            CreateMap<DisplayBranchViewModel,Branch>();

            CreateMap<EditBranchViewModel, Branch>().ForMember(dest => dest.BranchIdentificator, opts => opts.Ignore());
            CreateMap<Branch, EditBranchViewModel>();

            CreateMap<AddBranchViewModel, Branch>();
            CreateMap<Branch, AddBranchViewModel>();
        }
    }
}
