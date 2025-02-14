using System;
using AuctionService.DTOs;
using AuctionService.Entities;
using AutoMapper;
using Contracts;

namespace AuctionService.RequestHelpers;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        //map auction properties to auctiondto
        //map Auction to AuctionDto and include properties from Item
        CreateMap<Auction, AuctionDto>().IncludeMembers(x => x.Item);
        CreateMap<Item, AuctionDto>();

        //for creating
        CreateMap<CreateAuctionDto, Auction>().ForMember(d => d.Item, o => o.MapFrom(s => s));
        CreateMap<CreateAuctionDto, Item>();

        //map local dto to service bus dto/ local dto to contracts, AUction created is like the middleman
        CreateMap<AuctionDto, AuctionCreated>();
        CreateMap<AuctionDto, AuctionUpdated>();
    }
}
