using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.EntityClient;
using System.Linq;
using System.Text;

namespace UmbracoEx
{
    public sealed class UmbracoContext
    {
        //    System.Data.SqlClient
        //        System.Data.SqlServerCe.4.0
        const string EntityConnectionFormat = "metadata=res://*/{0}.csdl|res://*/{0}l.ssdl|res://*/{0}.msl;provider={1};provider connection string=\"{2}\"";
        //   metadata=res://*/Model1.csdl|res://*/Model1.ssdl|res://*/Model1.msl;provider=;provider connection string="data source=E:\projects\NarnooSDK\umbraco-media-library\UmbracoCms.4.7.2.497\App_Data\Umbraco.sdf"


        public UmbracoContext()
            : this(UmbracoContext.GetEntityConnection("EntityContext"))
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
        public static EntityConnection GetEntityConnection(string edmx)
        {
            ConnectionStringSettings conn = ConfigurationManager.ConnectionStrings["umbracoDbDSN"];


            if (conn != null)
            {
                return new EntityConnection(string.Format(EntityConnectionFormat, edmx, conn.ProviderName, conn.ConnectionString));
            }
            else
            {
                var settings = ConfigurationManager.AppSettings["umbracoDbDSN"];
                if (settings != null)
                {
                    return new EntityConnection(string.Format(EntityConnectionFormat, edmx, "System.Data.SqlClient", conn.ConnectionString));
                }
                else
                {
                    throw new MissingSettingException("umbracoDbDSN is missing.");
                }
            }
        }

        UmbracoEntities UmbracoEntities { get; set; }

        public IQueryable<Document> SearchDocuments()
        {
            var query =
                    from c in this.UmbracoEntities.cmsContents
                    join t in this.UmbracoEntities.cmsContentTypes on c.contentType equals t.nodeId into types
                    join n in this.UmbracoEntities.umbracoNodes on c.nodeId equals n.id into nodes
                    join d in this.UmbracoEntities.cmsDocuments on c.nodeId equals d.nodeId into docs
                    join x in this.UmbracoEntities.cmsContentXmls on c.nodeId equals x.nodeId into xmls

                    from type in types.DefaultIfEmpty()
                    from node in nodes.DefaultIfEmpty()
                    from doc in docs.DefaultIfEmpty()
                    from xml in xmls.DefaultIfEmpty()

                    where (doc.expireDate == null || ( doc.expireDate.HasValue && doc.expireDate.Value <= DateTime.Now  && doc.releaseDate >= DateTime.Now))

                    select new Document
                    {
                        NodeId = c.nodeId,
                        NodeTypeId = type.nodeId,
                        NodeTypeAlias = type.alias,
                        IsTrashed = node.trashed,
                        SortOrder = node.sortOrder,
                        Name = doc.text,
                        ContentXml = xml.xml
                    };

            return query;

        }
    }
}
