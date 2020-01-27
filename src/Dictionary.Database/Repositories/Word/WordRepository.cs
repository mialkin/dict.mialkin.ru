﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dictionary.Database.Models;
using Dictionary.Shared.ExtensionMethods;
using Dictionary.Shared.Filters.Word;
using Microsoft.EntityFrameworkCore;

namespace Dictionary.Database.Repositories.Word
{
    public class WordRepository : IWordRepository
    {
        public WordRepository(DictionaryDb db)
        {
            Db = db;
        }
        public DictionaryDb Db { get; }

        // Подумать над вынесением в RepositoryBase.
        public async Task CreateAsync(WordDto word)
        {
            Db.Words.Add(word);
            await Db.SaveChangesAsync();
        }

        public async Task CreateAsync(IEnumerable<WordDto> words)
        {
            // Грузить порциями по 1000 слов. Report back progress.
            Db.Words.AddRange(words);
            await Db.SaveChangesAsync();
        }

        // Подумать над вынесением в RepositoryBase. Создать return ListAsync(x => x.Words)

        public async Task<IList<WordDto>> ListAsync(WordListFilter filter)
        {
            var query = Db.Words.AsQueryable();

            if (filter.LanguageId != null)
            {
                query = query.Where(x => x.LanguageId == filter.LanguageId);
            }

            if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
            {
                query = query.Where(x => x.Name.StartsWith(filter.SearchTerm));
            }

            if (!string.IsNullOrWhiteSpace(filter.OrderByPropertyName))
            {
                query = filter.OrderByDescending ?
                    query.OrderByPropertyNameDescending(filter.OrderByPropertyName) :
                    query.OrderByPropertyName(filter.OrderByPropertyName);
            }

            var result = await query.Take(filter.Take).ToListAsync();

            return result;
        }
    }
}
