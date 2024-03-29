﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HR_Project.Domain.Entities.Concrete;
using HR_Project.Domain.Entities.Concrete.FileEntities;
using HR_Project.Domain.Enum;
using HR_Project.Domain.Repositories;
using HR_Project.Domain.Services.StorageService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;
using F = HR_Project.Domain.Entities.Concrete.FileEntities;

namespace HR_Project.Application.Services.FileService
{
	public class ProfileImageService : IProfileImageService
	{
		private readonly IStorage _azureStorage;
		private readonly IFileRepository<PersonnelPicture> _fileRepository;
		private readonly IPersonelRepository _personnelRepository;
		private readonly IConfiguration _configuration;

		public ProfileImageService(IStorage azureStorage, IFileRepository<PersonnelPicture> fileRepository, IPersonelRepository personnelRepository, IConfiguration configuration)
		{
			_azureStorage = azureStorage;
			_fileRepository = fileRepository;
			_personnelRepository = personnelRepository;
			_configuration = configuration;
		}

		public async Task<bool> DeleteFile(string personnelId)
		{
			try
			{
				Personnel personnel = await _personnelRepository.GetDefault(x => x.Id == Guid.Parse(personnelId));


				PersonnelPicture picture = await _fileRepository.GetDefault(x => x.Id == personnel.ImageId);

				picture.DeletedDate = DateTime.Now;
				picture.Status = Status.Deleted;

				personnel.ImageId = null;


				await _fileRepository.Delete(picture);

				return true;
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		public async Task<string> GetFileById(string personnelId)
		{

			PersonnelPicture picture = await _fileRepository.GetDefault(x => x.PersonnelId == Guid.Parse(personnelId)&&x.Status!=Status.Deleted);

			return $"{_configuration["BaseStorageUrl"]}/{picture.FilePath}";

		}
		public IFormFile ConvertImageToFormFile(Image image, string fileName)
		{
			using (var memoryStream = new MemoryStream())
			{
				image.Save(memoryStream, new JpegEncoder());
				return new FormFile(memoryStream, 0, memoryStream.Length, "image", fileName);
			}
		}
		public async Task<bool> UploadFile(string personnelId, IFormFile file)
		{
			try
			{
				Personnel personnel = await _personnelRepository.GetDefault(x => x.Id == Guid.Parse(personnelId));

				if (file != null)
				{

					//using (var image = Image.Load(file.OpenReadStream()))
					//{
					//	image.Mutate(x => x.Resize(256, 256));

						//IFormFile formFile = ConvertImageToFormFile(fi, file.FileName);

						List<(string fileName, string pathOrContainerName)> result = await _azureStorage.UploadAsync("profile-photos", file);

						if (personnel.ImageId != null)
						{
							var pic = await _fileRepository.GetDefault(x => x.Id == personnel.ImageId);
							pic.Status = Status.Deleted;
							pic.DeletedDate = DateTime.Now;

							await _fileRepository.Delete(pic);
						}

						PersonnelPicture picture = new PersonnelPicture()
						{
							FileName = result[0].fileName,
							FilePath = result[0].pathOrContainerName,
							Status = Status.Inserted,
							CreatedDate = DateTime.Now,
							PersonnelId = Guid.Parse(personnelId)
						};

						personnel.PersonnelPicture = picture;

						await _fileRepository.Create(picture);


						return true;

					//}
				}
				else if (personnel.ImageId == null)
				{

					PersonnelPicture picture = new PersonnelPicture()
					{
						FileName = personnel.Name + personnel.Id,
						FilePath = "default-photos/" + (personnel.Gender == Gender.Female ? "default_pic_woman.png" : "default_pic_man.png"),
						PersonnelId = Guid.Parse(personnelId)
					};

					await _fileRepository.Create(picture);

					personnel.PersonnelPicture = picture;

					await _personnelRepository.Delete(personnel);
					return true;
				}

				return true;

			}
			catch (Exception ex)
			{
				throw ex;
			}

		}
	}
}
