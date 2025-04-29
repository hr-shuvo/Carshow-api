using AuctionService.DTOs;
using AuctionService.Entities;
using AutoMapper;
using Contracts;
using Microsoft.AspNetCore.Http.HttpResults;

namespace AuctionService.RequestHelpers;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        CreateMap<Auction, AuctionDto>().IncludeMembers(x => x.Item);
        CreateMap<Item, AuctionDto>();
        CreateMap<AuctionCreateDto, Auction>()
            .ForMember(dest => dest.Item, opt => opt.MapFrom(src => src));
        
        CreateMap<AuctionCreateDto, Item>();
        CreateMap<AuctionDto, AuctionCreated>();

    }
}