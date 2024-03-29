﻿using HR_Project.Common.Models.Abstract;
using HR_Project.Common.Models.DTOs;
using HR_Project.Presentation.Models;

namespace HR_Project.Presentation.APIService
{
	public interface IAPIService
	{
		Task<T> GetAsync<T>(string endpoint, string token);
		Task<TResponse> PostAsync<TRequest, TResponse>(string endpoint, TRequest data, string token);
		Task<TResponse> PostWithImageAsync<TRequest, TResponse>(string endpoint, TRequest data, string token) where TRequest : class, IMasterExpense;
		Task<TResponse> PutWithImageAsync<TRequest, TResponse>(string endpoint, TRequest data, string token) where TRequest : class, IMasterExpense;

		//getbyid
		Task<T> GetByIdAsync<T>(string endpoint, string id, string token);

		//delete
		Task<T> DeleteAsync<T>(string endpoint, int id, string token);
        Task<T> DeleteAsync<T>(string endpoint, string id, string token);

        //update
        Task<T> UpdateAsync<T>(string endpoint, T data, string token);

		//get created model
		Task<T> GetCreateModelAsync<T>(string endpoint, string token);
		// login
		Task<TokenResponse> LoginAsync(LoginDTO loginModel);
		Task<TokenResponse> RefreshToken(string token);

        Task<T> GetAsyncWoToken<T>(string endpoint);

        //register
        Task<RegisterResponse> RegisterAsync(RegisterDTO registerModel);
		Task ConfirmAsync(string endpoint, MailConfirmDTO model);
		Task<TResponse> PostAsyncWoToken<TRequest, TResponse>(string endpoint, TRequest data);
		Task PostFileAsync(string endpoint, IFormFile file, string token);
    }
}
