﻿using System.ComponentModel;
using TvShowsCatalog.Web.Models.ApiModels;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Scoping;
using Umbraco.Cms.Core.Services;

namespace TvShowsCatalog.Web.Services
{
    public class ImportContentService : IImportContentService
    {
        private readonly IContentService _contentService;
        private readonly ITvMazeService _tvMazeService;
        private readonly IImportMediaService _importMediaService;
        private readonly ICoreScopeProvider _coreScopeProvider;

        public ImportContentService(IContentService contentService, ITvMazeService tVMazeService, IImportMediaService importMediaService, ICoreScopeProvider coreScopeProvider)
        {
            _contentService = contentService;
            _tvMazeService = tVMazeService;
            _importMediaService = importMediaService;
            _coreScopeProvider = coreScopeProvider;
		}

        // TODO: Task<IEnumerable<TvMazeModel>> UpdateImportedContent? In case there is new tv shows added to the list since the last import.
        // TODO: For creating media in Umbraco via code, use the Media Service

        public IEnumerable<TvMazeModel> ImportContent(int rootContentId)
        {
            var allTvShows = _tvMazeService.GetAllAsync().GetAwaiter().GetResult();
            var cultures = Array.Empty<string>();

			using ICoreScope scope = _coreScopeProvider.CreateCoreScope();
            _importMediaService.ImportBulkMedia(allTvShows);
			foreach (var show in allTvShows)
            {
				// TODO: Use publishedcontent instead, it's more efficient.
				// contentservice goes to the database, publishedcontent will use the cache.
                var tvshow = _contentService.Create($"{show.Name}", rootContentId, "tVShow");
                tvshow.SetValue("showSummary", $"{show.Summary}");
                // SetValue -> mediapicker image

				_contentService.Save(tvshow);
                _contentService.Publish(tvshow, cultures);
                
			}
			scope.Complete();
			return allTvShows;
        }

        public (bool, int) ShouldRunImport()
        {
            var rootContent = _contentService.GetRootContent().FirstOrDefault();

            if (rootContent == null)
            {
                throw new Exception("Root content node was not found");
            }
            var rootContentId = rootContent.Id;
            
            bool isThereContent = _contentService.HasChildren(rootContentId);

            return (isThereContent,rootContentId);
        }
    }
}
