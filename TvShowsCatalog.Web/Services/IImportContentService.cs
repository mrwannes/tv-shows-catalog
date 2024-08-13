﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TvShowsCatalog.Web.Models.ApiModels;
using Umbraco.Cms.Core.Models;

namespace TvShowsCatalog.Web.Services
{
    public interface IImportContentService
    {
        IEnumerable<TvMazeModel> ImportContent(int parentKey);
        void CreateContent(TvMazeModel tvshow, int rootContentId, string[] cultures);
        (bool, int) ShouldRunImport();
	}
}
