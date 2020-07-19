﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Dictionary.Database.Models;
using Dictionary.Database.Repositories.Word;
using Dictionary.Services.Models.Word;
using Dictionary.Shared.Filters;

namespace Dictionary.Services.Services.Word
{
    public class WordService : IWordService
    {
        private readonly IWordRepository _wordRepository;
        private readonly IMapper _mapper;

        public WordService(IWordRepository wordRepository, IMapper mapper)
        {
            _wordRepository = wordRepository;
            _mapper = mapper;
        }

        public async Task<int> CreateAsync(WordCreateServiceModel model)
        {
            WordDto word = _mapper.Map<WordDto>(model);
            word.Created = DateTime.Now;

            await _wordRepository.CreateAsync(word);
            await _wordRepository.SaveChangesAsync();

            return word.WordId;
        }

        public async Task UpdateAsync(WordUpdateServiceModel model)
        {
            WordDto word = _mapper.Map<WordDto>(model);

            await _wordRepository.UpdateAsync(word);
            await _wordRepository.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            await _wordRepository.DeleteAsync(id);
            await _wordRepository.SaveChangesAsync();
        }

        public async Task<bool> WordExists(string name, int languageId)
        {
            WordDto word = await _wordRepository.GetByNameAsync(name, languageId);

            return word != null;
        }
        public async Task<IList<WordListServiceModel>> ListAsync(WordListFilter filter)
        {
            IList<WordDto> result = await _wordRepository.ListAsync(filter);

            return _mapper.Map<IList<WordListServiceModel>>(result);
        }
        public async Task<IList<WordListServiceModel>> SearchAsync(WordSearchFilter filter)
        {
            IList<WordDto> result = await _wordRepository.SearchAsync(filter);

            return _mapper.Map<IList<WordListServiceModel>>(result);
        }
    }
}
