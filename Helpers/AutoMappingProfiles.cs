using AutoMapper;
using ChatApp.DTOs;
using ChatApp.Entities;
using ChatApp.Extensions;

namespace ChatApp.Helpers
{
    public class AutoMappingProfiles : Profile
    {
        public AutoMappingProfiles()
{
    CreateMap<AppUser, MemberDto>()
        .ForMember(
            dest => dest.PhotoUrl,
            opt => opt.MapFrom(src =>
                src.Photos.Where(x => x.IsMain).Select(x => x.Url).FirstOrDefault()))
        .ForMember(
            dest => dest.Age,
            opt => opt.MapFrom(src => src.DateOfBirth.CalculateAge()));

    CreateMap<Photo, PhotoDto>();

    CreateMap<AppUser, LikeDto>()
        .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName))
        .ForMember(dest => dest.PhotoUrl, opt => opt.MapFrom(src =>
            src.Photos.Where(x => x.IsMain).Select(x => x.Url).FirstOrDefault()))
        .ForMember(dest => dest.Age, opt => opt.MapFrom(src => src.DateOfBirth.CalculateAge()));

    CreateMap<UpdateMemberDto, AppUser>();

    CreateMap<RegisterDto, AppUser>();

    CreateMap<Message, MessageDto>()
        .ForMember(
            dest => dest.SenderPhotoUrl,
            opt => opt.MapFrom(src =>
                src.Sender.Photos.Where(x => x.IsMain).Select(x => x.Url).FirstOrDefault()))
        .ForMember(
            dest => dest.RecipientPhotoUrl,
            opt => opt.MapFrom(src =>
                src.Recipient.Photos.Where(x => x.IsMain).Select(x => x.Url).FirstOrDefault()));

    CreateMap<DateTime, DateTime>()
        .ConvertUsing(src =>
            DateTime.SpecifyKind(src, DateTimeKind.Utc));

    CreateMap<DateTime?, DateTime?>()
        .ConvertUsing(src =>
            src.HasValue
                ? DateTime.SpecifyKind(src.Value, DateTimeKind.Utc)
                : null);
}
    }
}
