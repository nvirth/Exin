using System;
using System.Collections.Generic;
using AutoMapper;
using AutoMapper.Mappers;

namespace Common.Utils
{
	public static class AutoMapperInitializer<TSource, TDestination>
	{
		private static readonly Dictionary<Tuple<Type, Type>, IMappingExpression> Dictionary = new Dictionary<Tuple<Type, Type>, IMappingExpression>();

		/// <summary>
		/// Initializes AutoMapper (for runtime proxys too) by calling Mapper.CreateMap method properly
		/// </summary>
		/// <param name="wasNew">If the mapping between source and destination type is already done, and come from cache, this param will be False. Otherwise True (if there are more init operations, they were nor processed yet)</param>
		/// <param name="sourceProxy">If the source of the mapping is a runtime generated proxy class (e.g.: Entity Framework lazy loading entity classes), give here an instance from it to make mapping work properly</param>
		/// <param name="destinationProxy">If the destination of the mapping is a runtime generated proxy class (e.g.: Entity Framework lazy loading entity classes), give here an instance from it to make mapping work properly</param>
		/// <returns></returns>
		public static IMappingExpression InitializeIfNeeded(out bool wasNew, TSource sourceProxy = default(TSource), TDestination destinationProxy = default(TDestination))
		{
			var isSourceProxy = !object.Equals(sourceProxy, default(TSource));
			var sourceType = isSourceProxy ? sourceProxy.GetType() : typeof(TSource);

			var isDestinationProxy = !object.Equals(destinationProxy, default(TDestination));
			var destinationType = isDestinationProxy ? destinationProxy.GetType() : typeof(TDestination);

			var key = new Tuple<Type, Type>(sourceType, destinationType);
			var isInitializedThisType = Dictionary.ContainsKey(key);
			if(!isInitializedThisType)
				Dictionary[key] = Mapper.CreateMap(sourceType, destinationType);

			wasNew = !isInitializedThisType;
			return Dictionary[key];
		}

		/// <summary>
		/// Initializes AutoMapper (for runtime proxys too) by calling Mapper.CreateMap method properly
		/// </summary>
		/// <param name="sourceProxy">If the source of the mapping is a runtime generated proxy class (e.g.: Entity Framework lazy loading entity classes), give here an instance from it to make mapping work properly</param>
		/// <param name="destinationProxy">If the destination of the mapping is a runtime generated proxy class (e.g.: Entity Framework lazy loading entity classes), give here an instance from it to make mapping work properly</param>
		/// <returns></returns>
		public static IMappingExpression InitializeIfNeeded(TSource sourceProxy = default(TSource), TDestination destinationProxy = default(TDestination))
		{
			bool wasNew;
			return InitializeIfNeeded(out wasNew, sourceProxy, destinationProxy);
		}
	}

	public static class AutoMapperHelper
	{

		/// <summary>
		/// We had the same issue on our build server. 
		/// MsTest seemed to remove DLLs it deemed unnecessary 
		/// (note : this claim is only an educated guess). 
		/// Anyways, to fix it add an explicit call to something in AutoMapper.Net4.dll. 
		/// For instance, with the class ListSourceMapper :
		/// 
		/// var useless = new ListSourceMapper()
		/// 
		/// Adding this to a project which is shared by all other projects fixed it for us. 
		/// Where to put this line of code may vary.
		/// </summary>
		private static void DoNothing()
		{
			var useless = new ListSourceMapper();
		}

		/// <summary>
		/// Delegates AutoMapper.IMappingExpression.ForMember method, with the option for skipping the (full) operation. 
		/// Original description: 
		/// Customize individual members
		/// </summary>
		/// <param name="mappingExpression"></param>
		/// <param name="needInitialize">If it's True, ForMember will be called. If it's False, nothing will be happened </param>
		/// <param name="name">Name of the member</param><param name="memberOptions">Callback for configuring member</param>
		/// <returns>Itself</returns>
		public static IMappingExpression ForMemberIfNeeded(this IMappingExpression mappingExpression, bool needInitialize, string name, Action<IMemberConfigurationExpression> memberOptions)
		{
			if(needInitialize)
				mappingExpression.ForMember(name, memberOptions);

			return mappingExpression;
		}
	}
}
