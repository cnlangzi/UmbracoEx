using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.EntityClient;
using System.Data.Metadata.Edm;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;

namespace UmbracoEx
{
    public sealed class UmbracoContext
    {
        //    System.Data.SqlClient
        //        System.Data.SqlServerCe.4.0
        const string EntityConnectionFormat = "metadata=res://*/{0}.csdl|res://*/{0}.ssdl|res://*/{0}.msl;provider={1};provider connection string=&quot;data source={2};initial catalog={3};integrated security=True;multipleactiveresultsets=True;application name=EntityFramework&quot;";
        //   metadata=res://*/Model1.csdl|res://*/Model1.ssdl|res://*/Model1.msl;provider=;provider connection string="data source=E:\projects\NarnooSDK\umbraco-media-library\UmbracoCms.4.7.2.497\App_Data\Umbraco.sdf"


        public UmbracoContext()
            : this(UmbracoContext.GetEntityConnection("EntityContext",typeof(UmbracoContext).Assembly))
        {

        }

        public UmbracoContext(EntityConnection entityConnection)
        {
            this.UmbracoEntities = new UmbracoEntities(entityConnection);
        }


        /// <summary>
        ///
        /// </summary>
        /// <param name="edmx">the file name of .edmx</param>
        /// <returns></returns>
        public static EntityConnection GetEntityConnection(string edmx,Assembly assembly)
        {
            var builder = new EntityConnectionStringBuilder();

            DbConnection dbConnection ;


            ConnectionStringSettings conn = ConfigurationManager.ConnectionStrings["umbracoDbDSN"];

            if (conn != null)
            {
                var factory = DbProviderFactories.GetFactory(conn.ProviderName);
                dbConnection = factory.CreateConnection();
                dbConnection.ConnectionString = conn.ConnectionString;
            }
            else
            {
                var settings = ConfigurationManager.AppSettings["umbracoDbDSN"];
                if (settings != null)
                {
                  
                    var factory = DbProviderFactories.GetFactory("System.Data.SqlClient");
                    dbConnection = factory.CreateConnection();
                    dbConnection.ConnectionString = settings;
                }
                else
                {
                    throw new MissingSettingException("umbracoDbDSN is missing.");
                }
            }

            return new EntityConnection(new MetadataWorkspace(new string[] { "res://*/" }, new Assembly[] { assembly }), dbConnection);
        }

        UmbracoEntities UmbracoEntities { get; set; }

        public IQueryable<Document> GetDocuments()
        {
            var query =
                    from c in this.UmbracoEntities.cmsContents
                    join t in this.UmbracoEntities.cmsContentTypes on c.contentType equals t.nodeId into types
                    join n in this.UmbracoEntities.umbracoNodes on c.nodeId equals n.id into nodes
                    join d in this.UmbracoEntities.cmsDocuments.Where(i=>i.published) on c.nodeId equals d.nodeId into docs
                    join x in this.UmbracoEntities.cmsContentXmls on c.nodeId equals x.nodeId into xmls

                    from type in types.DefaultIfEmpty()
                    from node in nodes.DefaultIfEmpty()
                    from doc in docs.DefaultIfEmpty()
                    from xml in xmls.DefaultIfEmpty()

                    where (doc.expireDate == null || (doc.expireDate.HasValue && doc.expireDate.Value <= DateTime.Now && doc.releaseDate >= DateTime.Now))

                    select new Document
                    {
                        NodeId = c.nodeId,
                        NodeTypeId = type.nodeId,
                        NodeTypeAlias = type.alias,
                        IsTrashed = node.trashed,
                        SortOrder = node.sortOrder,
                        Name = node.text,
                        ContentXml = xml.xml
                    };

            return query;

        }

        public IQueryable<Document> GetLiveDocuments()
        {
            return this.GetDocuments().Where(i => i.IsTrashed == false);
        }
    }
}
