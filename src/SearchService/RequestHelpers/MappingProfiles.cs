using System;
using AutoMapper;
using Contracts;
using SearchService.Models;

namespace SearchService.RequestHelpers;

public class MappingProfiles : Profile
{
    //this will map the auctioncreated from the service bus to item that can store items in mongodb
    public MappingProfiles()
    {
        CreateMap<AuctionCreated, Item>();
        CreateMap<AuctionCreated, Item>();
    }
}
