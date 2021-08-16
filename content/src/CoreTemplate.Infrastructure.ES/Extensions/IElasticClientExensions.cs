using Nest;
using System;
using System.Threading.Tasks;

namespace CoreTemplate.Infrastructure.ES.Extensions
{
    /// <summary>
    /// 
    /// </summary>
    public static class IElasticClientExensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="indexName"></param>
        public static void Run<T>(this IElasticClient client, string indexName) where T : class
        {
            try
            {
                var index = client.IndexExists(indexName);
                client.UpdateIndexSettings(Indices.Index(indexName), setting => setting.IndexSettings(isetting => isetting.Setting("max_result_window", 1000000)));
                if (!index.Exists)
                {
                    CreateIndexDescriptor createIndex = new CreateIndexDescriptor(indexName)
                    .Settings(s =>
                        s.NumberOfShards(1).NumberOfReplicas(1)
                        .Analysis(a => a.Analyzers(
                            aa => aa.Language("standard_listing", sa => sa.Language(Language.Chinese))))
                     )
                    .Mappings(ms =>
                        ms.Map<T>(m =>
                            m.AutoMap()
                        )
                    );
                    ICreateIndexResponse create = client.CreateIndex(createIndex);
                }
                else
                {
                    Console.WriteLine($"{indexName}索引已存在。");

                    IPutMappingResponse putMap = client.Map<T>(m => m.AutoMap());
                    if (putMap.ApiCall.Success)
                    {
                        Console.WriteLine($"索引已更新。");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="id"></param>
        /// <param name="updateObject"></param>
        /// <returns></returns>
        public static async Task UpdateAsyncEx<T>(this IElasticClient client, Guid id, object updateObject) where T : class
        {
            var docPath = new DocumentPath<T>(id);
            var response = await client.DocumentExistsAsync(docPath);
            if (response.Exists)
            {
                var updResponse = await client.UpdateAsync<T, object>(docPath, p => p.Doc(updateObject));
                if (!updResponse.IsValid)
                {
                    throw new Exception(updResponse.DebugInformation);
                }
            }
        }
    }
}
