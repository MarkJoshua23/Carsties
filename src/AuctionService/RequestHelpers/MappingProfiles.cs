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

        //tells automapper that it should also map Item but it doesnt know the properties of the Item
        CreateMap<Auction, AuctionUpdated>().IncludeMembers(a => a.Item);
        //this tells the automapper what are the properties of the Item
        CreateMap<Item, AuctionUpdated>();
    }
}
