using BlazorVideoIndexerUploader.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PTI.Microservices.Library.Services;

namespace BlazorVideoIndexerUploader.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class VideoIndexerUploaderController : ControllerBase
    {
        private ILogger<VideoIndexerUploaderController> Logger { get; }
        private AzureBingSearchService AzureBingSearchService { get; }
        private AzureVideoIndexerService AzureVideoIndexerService { get; }
        public VideoIndexerUploaderController(ILogger<VideoIndexerUploaderController> logger,
            AzureBingSearchService azureBingSearchService, AzureVideoIndexerService azureVideoIndexerService)
        {
            this.Logger = logger;
            this.AzureBingSearchService = azureBingSearchService;
            this.AzureVideoIndexerService = azureVideoIndexerService;

        }

        [HttpGet("[action]")]
        public async Task<IActionResult> Search(string searchTerm, int page)
        {
            int itemsToRetrieve = 60;
            int offset = (page - 1) * itemsToRetrieve;
            var searchResponse = await this.AzureBingSearchService.SearchImagesAsync(searchTerm,
                AzureBingSearchService.SafeSearchMode.Strict, itemsToRetrieve: itemsToRetrieve, offset: offset);
            var result = searchResponse.value.Select(p => new SearchResultItem()
            {
                ImageUrl = p.contentUrl
            }).ToList();
            return Ok(result);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> UploadFaces([FromBody] UploadFacesModel model)
        {
            try
            {
                var person = await this.AzureVideoIndexerService.CreatePersonAsync(Guid.Parse(model.PersonModelId), model.PersonName);
                foreach (var singlePersonImage in model.PersonImages.Where(p => p.IsSelected == true))
                {
                    await AddCustomFace(model, person, singlePersonImage);
                }
            }
            catch (Exception ex)
            {
                this.Logger.LogError(ex, ex.Message);
            }
            return Ok();
        }

        private async Task AddCustomFace(UploadFacesModel model, PTI.Microservices.Library.Models.AzureVideoIndexerService.CreatePerson.CreatePersonResponse person, PersonImage singlePersonImage)
        {
            try
            {
                await this.AzureVideoIndexerService.CreateCustomFaceAsync(Guid.Parse(model.PersonModelId),
                    Guid.Parse(person.id), new Uri(singlePersonImage.ImageUrl));
            }
            catch (Exception ex)
            {
                this.Logger.LogError(ex, ex.Message);
            }
        }
    }
}
