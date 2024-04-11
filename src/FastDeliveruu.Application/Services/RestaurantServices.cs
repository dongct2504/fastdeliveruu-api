using System.Data;
using Dapper;
using FastDeliveruu.Application.Dtos.RestaurantDtos;
using FastDeliveruu.Application.Interfaces;
using FastDeliveruu.Domain.Constants;
using FastDeliveruu.Domain.Entities;
using FastDeliveruu.Domain.Interfaces;

namespace FastDeliveruu.Application.Services;

public class RestaurantServices : IRestaurantServices
{
    private readonly ISP_Call _sP_Call;

    public RestaurantServices(ISP_Call sP_Call)
    {
        _sP_Call = sP_Call;
    }

    public async Task<IEnumerable<Restaurant>> GetAllRestaurantsAsync(int page)
    {
        string procedureName = "spGetAllRestaurantsPaging";

        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("@RowOffSet", PagingConstants.PageSize * (page - 1));
        parameters.Add("@FetchNextRow", PagingConstants.PageSize);

        return await _sP_Call.ListAsync<Restaurant>(procedureName, parameters);
    }

    public async Task<Restaurant?> GetRestaurantByIdAsync(int id)
    {
        string procedureName = "spGetRestaurantById";

        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("@RestaurantId", id);

        return await _sP_Call.OneRecordAsync<Restaurant>(procedureName, parameters);
    }

    public async Task<RestaurantWithMenuItemsDto?> GetRestaurantWithMenuItemsByIdAsync(int id)
    {
        string procedureName = "spGetRestaurantWithMenuItemsById";

        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("@RestaurantId", id);

        return await _sP_Call.OneRecordAsync<RestaurantWithMenuItemsDto>(procedureName, parameters);        
    }

    public async Task<Restaurant?> GetRestaurantByNameAsync(string name)
    {
        string procedureName = "spGetRestaurantByName";

        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("@Name", name);

        return await _sP_Call.OneRecordAsync<Restaurant>(procedureName, parameters);
    }

    public async Task<Restaurant?> GetRestaurantByPhoneNumberAsync(string phoneNumber)
    {
        string procedureName = "spGetRestaurantByName";

        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("@PhoneNumber", phoneNumber);

        return await _sP_Call.OneRecordAsync<Restaurant>(procedureName, parameters);
    }

    public async Task<int> CreateRestaurantAsync(Restaurant restaurant)
    {
        string procedureName = "spCreateRestaurant";

        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("@Name", restaurant.Name);
        parameters.Add("@Description", restaurant.Description);
        parameters.Add("@PhoneNumber", restaurant.PhoneNumber);
        parameters.Add("@IsVerify", restaurant.IsVerify);
        parameters.Add("@ImageUrl", restaurant.ImageUrl);
        parameters.Add("@Address", restaurant.Address);
        parameters.Add("@Ward", restaurant.Ward);
        parameters.Add("@District", restaurant.District);
        parameters.Add("@City", restaurant.City);
        parameters.Add("@CreatedAt", restaurant.CreatedAt);
        parameters.Add("@UpdatedAt", restaurant.UpdatedAt);
        parameters.Add("@RestaurantId", DbType.Int32, direction: ParameterDirection.Output);

        await _sP_Call.ExecuteAsync(procedureName, parameters);

        return parameters.Get<int>("@RestaurantId");
    }

    public async Task UpdateRestaurantAsync(Restaurant restaurant)
    {
        string procedureName = "spUpdateRestaurant";

        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("@RestaurantId", restaurant.RestaurantId);
        parameters.Add("@Name", restaurant.Name);
        parameters.Add("@Description", restaurant.Description);
        parameters.Add("@PhoneNumber", restaurant.PhoneNumber);
        parameters.Add("@IsVerify", restaurant.IsVerify);
        parameters.Add("@ImageUrl", restaurant.ImageUrl);
        parameters.Add("@Address", restaurant.Address);
        parameters.Add("@Ward", restaurant.Ward);
        parameters.Add("@District", restaurant.District);
        parameters.Add("@City", restaurant.City);
        parameters.Add("@UpdatedAt", restaurant.UpdatedAt);

        await _sP_Call.ExecuteAsync(procedureName, parameters);
    }

    public async Task DeleteRestaurantAsync(int id)
    {
        string procedureName = "spDeleteRestaurant";

        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("@RestaurantId", id);

        await _sP_Call.ExecuteAsync(procedureName, parameters);
    }
}
