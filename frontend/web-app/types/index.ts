//its result array since it will get all the items and their properties
// T is a placeholder not a return type, it can be Auction so the results is array of Auction
export type PagedResult<T> = {
    results : T[]
    pageCount: number
    totalCount: number
}

export type Auction = {
    reservePrice: number
    seller: string
    winner: any
    soldAmount: number
    currentHighBid: number
    createdAt: string
    updatedAt: string
    auctionEnd: string
    status: string
    make: string
    model: string
    year: number
    color: string
    mileAge: number
    imageUrl: string
    id: string
  }
  

