﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Dictionary.Services.Models.Word;
using Dictionary.Shared.Filters.Word;

namespace Dictionary.Services.Services.Word
{
    public interface IWordService
    {
        Task<IList<WordListServiceModel>> ListAsync(WordListFilter filter);
        
        Task CreateAsync(WordCreateServiceModel word);

        Task ImportAsync(IList<WordImportServiceModel> words);
    }
}
