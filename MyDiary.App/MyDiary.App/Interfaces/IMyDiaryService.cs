using MyDiary.App.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MyDiary.App.Interfaces
{
    public interface IMyDiaryService
    {
        Task InitializeAsync();
        Task LoginAsync();
        Task<IEnumerable<DiaryEntry>> GetAllEntry();
        Task PostEntry(DiaryEntry response);
        Task PatchEntry(string Id, DiaryEntry entry);
        Task<DiaryEntry> GetEntry(string Id);
        Task DeleteEntry(DiaryEntry entry);
        Task LogOffAsync();
        Task ReSyncAllEntires();
        Task<User> GetUser();
        Task<IEnumerable<DiaryEntry>> Search(string searchText, object cancellationToken);
    }
}
