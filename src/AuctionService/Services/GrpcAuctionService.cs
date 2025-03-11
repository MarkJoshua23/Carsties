using System;
using AuctionService.Data;
using Grpc.Core;

namespace AuctionService.Services;

//GrpcAuction is created when dotnet build
public class GrpcAuctionService : GrpcAuction.GrpcAuctionBase
{
    private readonly AuctionDbContext _dbContext;

    public GrpcAuctionService(AuctionDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public override async Task<GrpcAuctionResponse> GetAuction(GetAuctionRequest request, ServerCallContext context)
    {
        Console.WriteLine("==> Received GRPC request for auction");
        var auction = await _dbContext.Auctions.FindAsync(Guid.Parse(request.Id)) ?? throw new RpcException(new Status(StatusCode.NotFound, "Not Found"));
        var response = new GrpcAuctionResponse
        {
            Auction = new GrpcAuctionModel
            {
                AuctionEnd = auction.AuctionEnd.ToString(),
                Id = auction.Id.ToString(),
                ReservePrice = auction.ReservePrice,
                Seller = auction.Seller,
            }
        };

        return response;
    }

}
