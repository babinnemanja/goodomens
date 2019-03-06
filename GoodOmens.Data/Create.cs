using Raven.Client.Documents.Session;

namespace GoodOmens.Data
{
    public static class Create
    {
        public static void Omen()
        {
            using (IDocumentSession session = DocumentStoreHolder.Store.OpenSession())
            {
                // generate Id automatically
                session.Store(new Good
                {
                    Omen = "One good omen"
                });

                // send all pending operations to server, in this case only `Put` operation
                session.SaveChanges();
            }
        }
    }

    public class Good
    {
        public string Omen { get; set; }
    }
}
