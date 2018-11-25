using AutoMapper;

namespace RunTogether.ApplicationServices.Mapping.Dto
{
	// При запуске приложения "AutoMapper" осуществляет поиск классов-наследников
	// от "Profile" и вызывает их конструкторы.
	// В данном случае мы прописали варианты преобразования доменных сущностей
	// в DTO-объекты (DTO = Data Transfer Object).
	public class AutoMapperProfile : Profile
	{
		public AutoMapperProfile()
		{
			CreateMap<RunTogether.Dal.Model.User, Dto.UserDto>().ReverseMap();
			CreateMap<RunTogether.Dal.Model.User, Dto.UserModelDto>().ReverseMap();
			CreateMap<RunTogether.Dal.Model.User, Dto.LoginModelDto>().ReverseMap();
			CreateMap<RunTogether.Dal.Model.User, Dto.ParticipantModelDto>().ReverseMap();
			CreateMap<RunTogether.Dal.Model.Marathon, Dto.MarathonDto>().ReverseMap();
			CreateMap<RunTogether.Dal.Model.WaypointInfo, Dto.WaypointInfoDto>().ReverseMap();
			
			CreateMap<Dto.UserModelDto, Dto.LoginModelDto>().ReverseMap();
			CreateMap<Dto.UserDto, Dto.UserModelDto>().ReverseMap();
			CreateMap<Dto.UserDto, Dto.LoginModelDto>().ReverseMap();
		}
	}
}
