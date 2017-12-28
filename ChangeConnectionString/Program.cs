using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Xml;
using System.Data.Entity.Core.EntityClient;
using System.Web;
using System.Configuration;
using System.Collections.Specialized;
using static System.Net.Mime.MediaTypeNames;
using System.IO;
using System.Reflection;
using Oracle.ManagedDataAccess.Client;

namespace ChangeConnectionString
{
    class Program
    {

        static void Main(string[] args)
        {
            Console.WriteLine("Nom du fichier cible:");
            string fileConfig = Console.ReadLine();

            Console.WriteLine("Nom de l'attribut name dans le connectionStrings:");
            string name = Console.ReadLine();


            // Configuration des paramètres du noeud connectionStrings
            Console.WriteLine("Configuration des paramètres du noeud connectionStrings");
            Console.WriteLine("InitialCatalog :");
            string InitialCatalog = Console.ReadLine();
            Console.WriteLine("DataSource :");
            string DataSource = Console.ReadLine();
            Console.WriteLine("UserID:");
            string UserID = Console.ReadLine();
            Console.WriteLine("Password:");
            string Password = Console.ReadLine();

            

            string ApplicationPath = @"C:\Users\User\Documents\Visual Studio 2015\Projects\ChangeConnectionString\ChangeConnectionString";
            string YourPath = Path.GetDirectoryName(ApplicationPath);

            // Affichage des chemins
            Console.WriteLine("ApplicationPath :" + ApplicationPath);
            Console.WriteLine("YourPath :" + YourPath);
            Console.ReadKey();

            bool isNewName = false;
            bool isNewAlias = false;

            string path = Path.GetDirectoryName(YourPath) + "\\ChangeConnectionString\\ChangeConnectionString\\"+fileConfig+"";
            XmlDocument doc = new XmlDocument();
            doc.Load(path);
            XmlNodeList listName = doc.DocumentElement.SelectNodes(string.Format("connectionStrings/add[@name='{0}']", name));
            
            XmlNode nodeName;
            XmlNode nodeAlias;

            isNewName = listName.Count == 0;
            
            Console.WriteLine("Nombre d'occurence trouvé pour l'attribut name :" + listName.Count);
            Console.ReadKey();

            if (isNewName)
            {
                // connectionStrings
                Console.WriteLine("is new name");
                Console.ReadKey();
                nodeName = doc.CreateNode(XmlNodeType.Element, "add", null);
                XmlAttribute attributeName = doc.CreateAttribute("name");
                attributeName.Value = name;
                nodeName.Attributes.Append(attributeName);

                attributeName = doc.CreateAttribute("connectionString");
                attributeName.Value = name;
                nodeName.Attributes.Append(attributeName);

                attributeName = doc.CreateAttribute("providerName");
                attributeName.Value = "System.Data.SqlClient";
                nodeName.Attributes.Append(attributeName);
                
            }
            else
            {
                nodeName = listName[0];
            }

            string conString = nodeName.Attributes["connectionString"].Value;
            SqlConnectionStringBuilder conStringBuilder = new SqlConnectionStringBuilder(conString);
            conStringBuilder.InitialCatalog = InitialCatalog;
            conStringBuilder.DataSource = DataSource;
            conStringBuilder.IntegratedSecurity = false;
            conStringBuilder.UserID = UserID;
            conStringBuilder.Password = Password;
            nodeName.Attributes["connectionString"].Value = conStringBuilder.ConnectionString;

            if (isNewName)
            {
                doc.DocumentElement.SelectNodes("connectionStrings")[0].AppendChild(nodeName);
            }




            Console.WriteLine("Nom de l'attribut alias:");
            string alias = Console.ReadLine();

            // Configuration des paramètres du noeud oracle.manageddataaccess.client
            Console.WriteLine("Configuration des paramètres du noeud oracle.manageddataacess.client");
            Console.WriteLine("Protocol :");
            string protocol = Console.ReadLine();
            Console.WriteLine("Host :");
            string host = Console.ReadLine();
            Console.WriteLine("Port :");
            string port = Console.ReadLine();
            Console.WriteLine("Service name :");
            string serviceName = Console.ReadLine();

            XmlNodeList listAlias = doc.DocumentElement.SelectNodes(string.Format("oracle.manageddataaccess.client/version/dataSources/dataSource[@alias='{0}']", alias));

            isNewAlias = listAlias.Count == 0;
            Console.WriteLine("Nombre d'occurence trouvé pour alias:" + listAlias.Count);
            Console.ReadKey();

            if (isNewAlias)
            {
                //oracle managed data access client
                Console.WriteLine("is new alias");
                nodeAlias = doc.CreateNode(XmlNodeType.Element, "dataSource", null);
                XmlAttribute attributeAlias = doc.CreateAttribute("alias");
                attributeAlias.Value = alias;
                nodeAlias.Attributes.Append(attributeAlias);

                attributeAlias = doc.CreateAttribute("dataSource");
                attributeAlias.Value = alias;
                nodeAlias.Attributes.Append(attributeAlias);

                attributeAlias = doc.CreateAttribute("descriptor");
                attributeAlias.Value = "(DESCRIPTION = (ADDRESS = (PROTOCOL = " + protocol + ")(HOST = " + host + ")(PORT = " + port + "))(CONNECT_DATA=(SERVICE_NAME=" + serviceName + ")))";
                nodeAlias.Attributes.Append(attributeAlias);
            }
            else
            {
                nodeAlias = listAlias[0];
            }



            // Affichage de la valeur actuelle
            Console.WriteLine("Valeur actuelle du noeud dataSource :" + nodeAlias.Attributes["descriptor"].Value);
            Console.ReadKey();

            // Modification du noeud datasource
            nodeAlias.Attributes["descriptor"].Value = "(DESCRIPTION = (ADDRESS = (PROTOCOL = " + protocol + ")(HOST = " + host + ")(PORT = " + port + "))(CONNECT_DATA=(SERVICE_NAME=" + serviceName + ")))";

            // Affichage de la valeur modifiée
            Console.WriteLine("Nouvelle valeur modifiée du noeud dataSource :" + nodeAlias.Attributes["descriptor"].Value);
            Console.ReadKey();

            if (isNewAlias)
            {
                doc.DocumentElement.SelectNodes("connectionStrings")[0].AppendChild(nodeAlias);
            }

            doc.Save(path);
            
        }
    }
}



