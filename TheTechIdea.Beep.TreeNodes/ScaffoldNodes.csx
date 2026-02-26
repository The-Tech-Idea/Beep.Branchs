using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections.Generic;

// Note: This script assumes to be executed in Beep.TreeNodes directory.
string basePath = @"c:\Users\f_ald\source\repos\The-Tech-Idea\Beep.Branchs\TheTechIdea.Beep.TreeNodes";
            
            string[] directoriesToProcess = new string[] {
                "QUEUE", "VectorDB", "STREAM", "MLModel", "IoT", "Blockchain", "Connector", "WebAPI", "NoSQL"
            };

            foreach (var dir in directoriesToProcess)
            {
                string folderPath = Path.Combine(basePath, dir);
                if (!Directory.Exists(folderPath)) continue;

                // Determine the prefix (e.g. QUEUE, NoSql from NoSqlRootNode)
                var rootNodeFile = Directory.GetFiles(folderPath, "*RootNode.cs").FirstOrDefault();
                if (rootNodeFile == null) continue;

                string rootNodeContent = File.ReadAllText(rootNodeFile);
                
                // Regex to find "public class [Prefix]RootNode"
                var match = Regex.Match(rootNodeContent, @"public class (\w+)RootNode");
                if (!match.Success) {
                     Console.WriteLine($"Skipping {dir} as no clear prefix was inferred");
                     continue;
                }
                
                string prefix = match.Groups[1].Value;
                
                // Get the DatasourceCategory enum value
                // e.g. Category = DatasourceCategory.QUEUE
                var categoryMatch = Regex.Match(rootNodeContent, @"Category\s*=\s*DatasourceCategory\.(\w+)");
                string categoryName = categoryMatch.Success ? categoryMatch.Groups[1].Value : dir.ToUpper();
                
                 // get exact namespace
                 var nsMatch = Regex.Match(rootNodeContent, @"namespace\s+([\w\.]+)");
                 string namespaceName = nsMatch.Success ? nsMatch.Groups[1].Value : $"TheTechIdea.Beep.TreeNodes.{dir}";

                // 1. Generate CategoryNode.cs
                string categoryNodeName = $"{prefix}CategoryNode";
                string categoryNodePath = Path.Combine(folderPath, $"{categoryNodeName}.cs");
                string categoryNodeTemplate = GetCategoryNodeTemplate(namespaceName, categoryNodeName, prefix, categoryName, dir);
                
                if (true)
                {
                    File.WriteAllText(categoryNodePath, categoryNodeTemplate);
                    Console.WriteLine($"Created {categoryNodePath}");
                }

                // 2. Generate EntitiesNode.cs
                string entitiesNodeName = $"{prefix}EntitiesNode";
                string entitiesNodePath = Path.Combine(folderPath, $"{entitiesNodeName}.cs");
                string entitiesNodeTemplate = GetEntitiesNodeTemplate(namespaceName, entitiesNodeName, prefix, categoryName);
                
                if (true)
                {
                    File.WriteAllText(entitiesNodePath, entitiesNodeTemplate);
                    Console.WriteLine($"Created {entitiesNodePath}");
                }

            }

        static void ModifyRootNode(string file, string content, string prefix, string categoryEnum, string categoryNodeName)
        {
            bool modified = false;
            // First we need to make sure CreateCategoryNode exists and return not implemented is replaced.
            string createCatRegex = @"public\s*(?:virtual\s*)?IBranch\s*CreateCategoryNode\s*\(\s*CategoryFolder\s+p\s*\)\s*\{\s*(throw\s+new\s+NotImplementedException\(\)\s*;|return\s+null\s*;|//[\s\w]*throw\s+new\s+NotImplementedException\(\)\s*;)\s*\}";
            
            string newCreateCat = 
$@"public IBranch CreateCategoryNode(CategoryFolder p)
        {{
            {categoryNodeName} categoryBranch = null;
            try
            {{
                IBranch parent = this;
                categoryBranch = new {categoryNodeName}(TreeEditor, DMEEditor, parent, p.FolderName, TreeEditor.SeqID, EnumPointType.Category, """"{categoryEnum.ToLower()}.svg"""");
                TreeEditor.Treebranchhandler.AddBranch(parent, categoryBranch);
                categoryBranch.CreateChildNodes();
            }}
            catch (Exception ex)
            {{
                DMEEditor.Logger.WriteLog($""Error Creating Category Node ({{ex.Message}}) "");
                DMEEditor.ErrorObject.Flag = Errors.Failed;
                DMEEditor.ErrorObject.Ex = ex;
            }}
            return categoryBranch;
        }}";

            if (Regex.IsMatch(content, createCatRegex))
            {
                content = Regex.Replace(content, createCatRegex, newCreateCat);
                modified = true;
            }

            // Now, we need to add the Category loading loop in CreateChildNodes if it's missing.
            // Replace CreateChildNodes completely by finding it
            string createChildRegex = @"(?s)(public\s+(?:virtual\s*)?IErrorsInfo\s+CreateChildNodes\s*\(\)\s*\{)(.*?)(return\s+DMEEditor\.ErrorObject\s*;\s*\})";
            var m = Regex.Match(content, createChildRegex);
            
            
            if (m.Success)
            {
                string prefixBlock = m.Groups[1].Value; // "public IErrorsInfo CreateChildNodes() {"
                string bodyBlock = m.Groups[2].Value;
                string returnBlock = m.Groups[3].Value; // "return DMEEditor.ErrorObject; }"
                 
                if (bodyBlock.Contains("throw new NotImplementedException();"))
                {
                       string fullRepl = prefixBlock + @"
            try
            {
                foreach (ConnectionProperties i in DMEEditor.ConfigEditor.DataConnections.Where(c => c.Category == DatasourceCategory." + categoryEnum + @" && c.IsComposite == false))
                {
                    if (TreeEditor.Treebranchhandler.CheckifBranchExistinCategory(i.ConnectionName, """ + categoryEnum + @""") == null)
                    {
                        if (!ChildBranchs.Any(p => p.GuidID.Equals(i.GuidID, StringComparison.InvariantCultureIgnoreCase)))
                        {
                            CreateDBNode(i);
                            i.Drawn = true;
                        }
                    }
                }
                foreach (CategoryFolder i in DMEEditor.ConfigEditor.CategoryFolders.Where(x => x.RootName.Equals(""" + categoryEnum + @""", StringComparison.InvariantCultureIgnoreCase)))
                {
                    if (!ChildBranchs.Where(p => p.BranchText == i.FolderName).Any())
                    {
                        CreateCategoryNode(i);
                    }
                }
            }
            catch (Exception ex)
            {
                DMEEditor.Logger.WriteLog($""Error Creating Child Nodes ({ex.Message})"");
            }
            " + returnBlock;
                     content = content.Replace(m.Value, fullRepl);
                     modified = true;
                }
                else if (bodyBlock.Contains("DMEEditor.ConfigEditor.DataConnections.Where") && !bodyBlock.Contains("DMEEditor.ConfigEditor.CategoryFolders.Where"))
                {
                    // It has the connection loop, but not category loop
                    string catLoop = $@"
                foreach (CategoryFolder i in DMEEditor.ConfigEditor.CategoryFolders.Where(x => x.RootName.Equals(""{categoryEnum}"", StringComparison.InvariantCultureIgnoreCase)))
                {{
                    if (!ChildBranchs.Where(p => p.BranchText == i.FolderName).Any())
                    {{
                        CreateCategoryNode(i);
                    }}
                }}
";
                    // Append before return Block
                    string newBody = bodyBlock + catLoop;
                    string fullRepl = prefixBlock + newBody + returnBlock;
                    content = content.Replace(m.Value, fullRepl);
                    modified = true;
                }
            }


            if (modified)
            {
                File.WriteAllText(file, content);
                Console.WriteLine($"Modified {file}");
            }
        }

        static string GetCategoryNodeTemplate(string ns, string className, string prefix, string categoryEnum, string dir)
        {
            string sourceNodeName = prefix == "NoSql" ? "NoSqlSourceNode" : $"{prefix}Node";
            return $@"using System;
using System.Collections.Generic;
using System.Linq;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea;
using TheTechIdea.Beep;
using TheTechIdea.Beep.DataBase;
using TheTechIdea.Beep.Vis;
using TheTechIdea.Beep.ConfigUtil;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.DriversConfigurations;

namespace {ns}
{{
    [AddinAttribute(Caption = ""{dir}"", BranchType = EnumPointType.Category, Name = ""{className}.Beep"", misc = ""Beep"", iconimage = ""category.png"", menu = ""Beep"", ObjectType = ""Beep"")]
    public class {className} : IBranch 
    {{
        public {className}() {{ }}

        public {className}(ITree pTreeEditor, IDMEEditor pDMEEditor, IBranch pParentNode, string pBranchText, int pID, EnumPointType pBranchType, string pimagename)
        {{
            TreeEditor = pTreeEditor;
            DMEEditor = pDMEEditor;
            ParentBranchID = pParentNode != null ? pParentNode.ID : -1;
            BranchText = pBranchText;
            BranchType = pBranchType;
            IconImageName = pimagename;
            if (pID != 0)
            {{
                ID = pID;
                BranchID = ID;
            }}
        }}

        public IErrorsInfo SetConfig(ITree pTreeEditor, IDMEEditor pDMEEditor, IBranch pParentNode, string pBranchText, int pID, EnumPointType pBranchType, string pimagename)
        {{
            try
            {{
                TreeEditor = pTreeEditor;
                DMEEditor = pDMEEditor;
                ParentBranchID = pParentNode != null ? pParentNode.ID : -1;
                BranchText = pBranchText;
                BranchType = pBranchType;
                IconImageName = pimagename;
                if (pID != 0)
                {{
                    ID = pID;
                    BranchID = ID;
                }}
            }}
            catch (Exception ex)
            {{
                string mes = ""Could not Set Config"";
                DMEEditor.AddLogMessage(ex.Message, mes, DateTime.Now, -1, mes, Errors.Failed);
            }}
            return DMEEditor.ErrorObject;
        }}

        public string MenuID {{ get; set; }}
        public bool Visible {{ get; set; }} = true;
        public bool IsDataSourceNode {{ get; set; }} = true;
        public string GuidID {{ get; set; }} = Guid.NewGuid().ToString();
        public string ParentGuidID {{ get; set; }}
        public string DataSourceConnectionGuidID {{ get; set; }}
        public string EntityGuidID {{ get; set; }}
        public string MiscStringID {{ get; set; }}
        public IBranch ParentBranch {{ get; set; }}
        public string Name {{ get; set; }}
        public EntityStructure EntityStructure {{ get; set; }}
        public int ID {{ get; set; }}
        public string BranchText {{ get; set; }}
        public IDMEEditor DMEEditor {{ get; set; }}
        public IDataSource DataSource {{ get; set; }}
        public string DataSourceName {{ get; set; }}
        public int Level {{ get; set; }}
        public EnumPointType BranchType {{ get; set; }} = EnumPointType.Category;
        public int BranchID {{ get; set; }}
        public string IconImageName {{ get; set; }} = ""category.png"";
        public string BranchStatus {{ get; set; }}
        public int ParentBranchID {{ get; set; }}
        public string BranchDescription {{ get; set; }}
        public string BranchClass {{ get; set; }} = ""{dir}"";
        public List<IBranch> ChildBranchs {{ get; set; }} = new List<IBranch>();
        public ITree TreeEditor {{ get; set; }}
        public List<string> BranchActions {{ get; set; }}
        public List<Delegate> Delegates {{ get; set; }}
        public object TreeStrucure {{ get; set; }}
        public IAppManager Visutil {{ get; set; }}
        public int MiscID {{ get; set; }}
        public string ObjectType {{ get; set; }} = ""Beep"";

        public IErrorsInfo CreateChildNodes()
        {{
            try
            {{
                TreeEditor.Treebranchhandler.RemoveChildBranchs(this);
                foreach (CategoryFolder p in DMEEditor.ConfigEditor.CategoryFolders.Where(x => x.RootName.Equals(""{categoryEnum}"", StringComparison.InvariantCultureIgnoreCase) && x.FolderName == BranchText))
                {{
                    foreach (string item in p.items)
                    {{
                        ConnectionProperties i = DMEEditor.ConfigEditor.DataConnections.Where(x => x.ConnectionName == item).FirstOrDefault();
                        if (i != null)
                        {{
                            CreateDBNode(i);
                            i.Drawn = true;
                        }}
                    }}
                }}
            }}
            catch (Exception ex)
            {{
                string mes = ""Could not Add Connection"";
                DMEEditor.AddLogMessage(ex.Message, mes, DateTime.Now, -1, mes, Errors.Failed);
            }}
            return DMEEditor.ErrorObject;
        }}

        public IErrorsInfo CreateDBNode(ConnectionProperties i)
        {{
            try
            {{
                ConnectionDriversConfig drv = DMEEditor.ConfigEditor.DataDriversClasses.Where(p => p.PackageName == i.DriverName).FirstOrDefault();
                string icon = drv is null ? ""unknowndatasource.svg"" : drv.iconname;
                {sourceNodeName} database = new {sourceNodeName}(i, TreeEditor, DMEEditor, this, i.ConnectionName, TreeEditor.SeqID, EnumPointType.DataPoint, icon);
                database.DataSource = DataSource;
                database.DataSourceName = i.ConnectionName;
                database.DataSourceConnectionGuidID = i.GuidID;
                database.GuidID = i.GuidID;
                database.IconImageName = icon;

                TreeEditor.Treebranchhandler.AddBranch(this, database);
            }}
            catch (Exception ex)
            {{
                string mes = ""Could not Add Database Connection"";
                DMEEditor.AddLogMessage(ex.Message, mes, DateTime.Now, -1, mes, Errors.Failed);
            }}

            return DMEEditor.ErrorObject;
        }}

        public IBranch CreateCategoryNode(CategoryFolder p)
        {{
            throw new NotImplementedException();
        }}

        public IErrorsInfo ExecuteBranchAction(string ActionName) {{ throw new NotImplementedException(); }}
        public IErrorsInfo MenuItemClicked(string ActionNam) {{ throw new NotImplementedException(); }}
        public IErrorsInfo RemoveChildNodes() {{ throw new NotImplementedException(); }}
    }}
}}";
        }

        static string GetEntitiesNodeTemplate(string ns, string className, string prefix, string categoryEnum)
        {
            return $@"using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea;
using TheTechIdea.Beep;
using TheTechIdea.Beep.DataBase;
using TheTechIdea.Beep.Vis;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.ConfigUtil;
using TheTechIdea.Beep.Utilities;

namespace {ns}
{{
    [AddinAttribute(Caption = ""{prefix}"", BranchType = EnumPointType.Entity, Name = ""{className}.Beep"", misc = ""Beep"", iconimage = ""database.png"", menu = ""Beep"", ObjectType = ""Beep"")]
    public class {className} : IBranch 
    {{
        public {className}() {{ }}

        public {className}(ITree pTreeEditor, IDMEEditor pDMEEditor, IBranch pParentNode, string pBranchText, int pID, EnumPointType pBranchType, string pimagename, IDataSource ds)
        {{
            DataSource = ds;
            TreeEditor = pTreeEditor;
            DMEEditor = pDMEEditor;
            ParentBranchID = pParentNode != null ? pParentNode.ID : -1;
            BranchText = pBranchText;
            BranchType = EnumPointType.Entity;
            IconImageName = pimagename;
            EntityStructure = new EntityStructure();
            EntityStructure.DataSourceID = ds.DatasourceName;
            EntityStructure.Viewtype = ViewType.Table;
            EntityStructure.EntityName = pBranchText;
            DataSourceName = ds.DatasourceName;

            if (pID != 0)
            {{
                ID = pID;
                BranchID = ID;
            }}
        }}

        public string MenuID {{ get; set; }}
        public bool Visible {{ get; set; }} = true;
        public bool IsDataSourceNode {{ get; set; }} = true;
        public string GuidID {{ get; set; }} = Guid.NewGuid().ToString();
        public string ParentGuidID {{ get; set; }}
        public string DataSourceConnectionGuidID {{ get; set; }}
        public string EntityGuidID {{ get; set; }}
        public string MiscStringID {{ get; set; }}
        public IBranch ParentBranch {{ get; set; }}
        public int ID {{ get; set; }}
        public EntityStructure EntityStructure {{ get; set; }}
        public int Order {{ get; set; }}
        public string Name {{ get; set; }} = """";
        public string BranchText {{ get; set; }}
        public IDMEEditor DMEEditor {{ get; set; }}
        public IDataSource DataSource {{ get; set; }}
        public string DataSourceName {{ get; set; }}
        public int Level {{ get; set; }}
        public EnumPointType BranchType {{ get; set; }} = EnumPointType.Entity;
        public int BranchID {{ get; set; }}
        public string IconImageName {{ get; set; }}
        public string BranchStatus {{ get; set; }}
        public int ParentBranchID {{ get; set; }}
        public string BranchDescription {{ get; set; }}
        public string BranchClass {{ get; set; }} = ""{categoryEnum}"";
        public List<IBranch> ChildBranchs {{ get; set; }}
        public ITree TreeEditor {{ get; set; }}
        public List<string> BranchActions {{ get; set; }}
        public object TreeStrucure {{ get; set; }}
        public IAppManager Visutil {{ get; set; }}
        public string ObjectType {{ get; set; }} = ""Beep"";
        public int MiscID {{ get; set; }}
       
        public IErrorsInfo CreateChildNodes()
        {{
            throw new NotImplementedException();
        }}

        public IErrorsInfo ExecuteBranchAction(string ActionName)
        {{
            throw new NotImplementedException();
        }}

        public IErrorsInfo MenuItemClicked(string ActionNam)
        {{
            throw new NotImplementedException();
        }}

        public IErrorsInfo RemoveChildNodes()
        {{
            throw new NotImplementedException();
        }}

        public IErrorsInfo SetConfig(ITree pTreeEditor, IDMEEditor pDMEEditor, IBranch pParentNode, string pBranchText, int pID, EnumPointType pBranchType, string pimagename)
        {{
            try
            {{
                TreeEditor = pTreeEditor;
                DMEEditor = pDMEEditor;
                ParentBranchID = pParentNode != null ? pParentNode.ID : -1;
                BranchText = pBranchText;
                BranchType = pBranchType;
                IconImageName = pimagename;
                if (pID != 0)
                {{
                    ID = pID;
                }}
            }}
            catch (Exception ex)
            {{
                string mes = ""Could not Set Config"";
                DMEEditor.AddLogMessage(ex.Message, mes, DateTime.Now, -1, mes, Errors.Failed);
            }}
            return DMEEditor.ErrorObject;
        }}

        public IBranch CreateCategoryNode(CategoryFolder p)
        {{
            throw new NotImplementedException();
        }}
    }}
}}";
}
