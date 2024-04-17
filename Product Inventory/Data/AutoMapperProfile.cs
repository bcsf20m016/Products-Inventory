using System;
using AutoMapper;
using Product_Inventory.Models;
using Product_Inventory.Models.ViewModels;

namespace Product_Inventory.Data
{
	public class AutoMapperProfile : Profile
	{
		public AutoMapperProfile()
		{
			CreateMap<Product, ProductViewModel>();
            CreateMap<ProductViewModel, Product>();
        }
	}
}

