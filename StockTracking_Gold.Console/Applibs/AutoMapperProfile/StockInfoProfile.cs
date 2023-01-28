using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AutoMapper;
using StockTracking_Gold.Ap.Dto;
using StockTracking_Gold.Ap.Model;

namespace StockTracking_Gold.Ap.Applibs.AutoMapperProfile
{
    // AutoMapper Model資料對應設定
    class StockInfoProfile : Profile
    {
        public StockInfoProfile()
        {
            {
                CreateMap<StockInfo, StockViewModelDtoV2>()
                    //黃金交叉Dto
                    .ForMember(p => p.Date,
                        o => o.MapFrom(p => p.Gold.Date))
                    .ForMember(p => p.Market,
                        o => o.MapFrom(p => p.Gold.Market))
                    .ForMember(p => p.Code,
                        o => o.MapFrom(p => p.Gold.Code))
                    .ForMember(p => p.Name,
                        o => o.MapFrom(p => p.Gold.Name))
                    .ForMember(p => p.Price,
                        o => o.MapFrom(p => p.Gold.Price))
                    .ForMember(p => p.MA5,
                        o => o.MapFrom(p => p.Gold.MA5))
                    .ForMember(p => p.MA20,
                        o => o.MapFrom(p => p.Gold.MA20))
                    .ForMember(p => p.MA60,
                        o => o.MapFrom(p => p.Gold.MA60))
                     .ForMember(p => p.MA120,
                        o => o.MapFrom(p => p.Gold.MA120))
                     .ForMember(p => p.Indicators,
                        o => o.MapFrom(p => p.Gold.Indicators))
                     //本益比本淨比河流Dto
                     .ForMember(p => p.FlowDate,
                        o => o.MapFrom(p => p.Pbr.Date))
                     .ForMember(p => p.Transaction,
                        o => o.MapFrom(p => p.Pbr.Transaction))
                     .ForMember(p => p.Volume,
                        o => o.MapFrom(p => p.Pbr.Volume))
                     .ForMember(p => p.Total_amount,
                        o => o.MapFrom(p => p.Pbr.Total_amount))
                     .ForMember(p => p.Bps,
                        o => o.MapFrom(p => p.Pbr.Bps_price))
                     .ForMember(p => p.Eps,
                        o => o.MapFrom(p => p.Per.Eps_price))
                     .ForMember(p => p.PbrLevel,
                        o => o.MapFrom(p => p.Pbr.Flow_Level))
                     .ForMember(p => p.PerLevel,
                        o => o.MapFrom(p => p.Per.Flow_Level))
                     .ForMember(p => p.Url,
                        o => o.MapFrom(p => p.Pbr.url));
            }

            {
                CreateMap<Gold, StockViewModelDtoV2>()
                    //黃金交叉Dto
                    .ForMember(p => p.Date,
                        o => o.MapFrom(p => p.Date))
                    .ForMember(p => p.Market,
                        o => o.MapFrom(p => p.Market))
                    .ForMember(p => p.Code,
                        o => o.MapFrom(p => p.Code))
                    .ForMember(p => p.Name,
                        o => o.MapFrom(p => p.Name))
                    .ForMember(p => p.Price,
                        o => o.MapFrom(p => p.Price))
                    .ForMember(p => p.MA5,
                        o => o.MapFrom(p => p.MA5))
                    .ForMember(p => p.MA20,
                        o => o.MapFrom(p => p.MA20))
                    .ForMember(p => p.MA60,
                        o => o.MapFrom(p => p.MA60))
                     .ForMember(p => p.MA120,
                        o => o.MapFrom(p => p.MA120))
                     .ForMember(p => p.Indicators,
                        o => o.MapFrom(p => p.Indicators));
            }
        }
    }
}
