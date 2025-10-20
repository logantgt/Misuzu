using Misuzu.Database.Models;
using Realms;

namespace Misuzu.Database;

public class RealmDbContext
{
    private readonly Realm _realm;

    public RealmDbContext(Realm realm)
    {
        _realm = realm;
    }

    public IQueryable<ImmersionLog> GetImmersionLogs() =>
        _realm.All<ImmersionLog>();
    public IQueryable<LibraryEntry> GetLibraryEntries() =>
    _realm.All<LibraryEntry>();

    public void AddImmersionLog(ImmersionLog log)
    {
        _realm.Write(() =>
        {
            _realm.Add(log);
        });
    }

    public void RemoveImmersionLog(ImmersionLog log)
    {
        _realm.Write(() =>
        {
            _realm.Remove(log);
        });
    }

    public void AddLibraryEntry(LibraryEntry entry)
    {
        _realm.Write(() =>
        {
            _realm.Add(entry);
        });
    }
}
