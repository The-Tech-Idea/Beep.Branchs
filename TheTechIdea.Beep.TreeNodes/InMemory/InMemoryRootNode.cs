using TheTechIdea.Beep.Vis.Modules;
using System;
using System.Collections.Generic;
using TheTechIdea.Beep.Addin;
using TheTechIdea;
using TheTechIdea.Beep;
using TheTechIdea.Beep.DataBase;
using TheTechIdea.Beep.Vis;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.ConfigUtil;
using System.Linq;
using TheTechIdea.Beep.Utilities;

namespace TheTechIdea.Beep.TreeNodes.InMemory
{
    /// <summary>
    /// Root node for in-memory database management in the Beep framework.
    /// Supports DuckDB, SQLite in-memory, and other in-memory data stores.
    /// </summary>
    [AddinAttribute(Caption = "In-Memory Databases", misc = "Beep", BranchType = EnumPointType.Genre, 
                    FileType = "Beep", iconimage = "inmemoryroot.png", menu = "InMemory", 
                    ObjectType = "Beep", ClassType = "ROOT")]
    [AddinVisSchema(BranchType = EnumPointType.Genre, BranchClass = "INMEMORY")]
    public class InMemoryRootNode : IBranch
    {
        #region Properties
        public string GuidID { get; set; } = Guid.NewGuid().ToString();
        public string ParentGuidID { get; set; }
        public string DataSourceConnectionGuidID { get; set; }
        public string EntityGuidID { get; set; }
        public bool Visible { get; set; } = true;
        public string MenuID { get; set; }
        public string MiscStringID { get; set; }
        public bool IsDataSourceNode { get; set; } = false;
        public string ObjectType { get; set; } = "Beep";
        public int Order { get; set; } = 5;
        public object TreeStrucure { get; set; }
        public IAppManager Visutil { get; set; }
        public int ID { get; set; } = -1;
        public IDMEEditor DMEEditor { get; set; }
        public IDataSource DataSource { get; set; }
        public string DataSourceName { get; set; }
        public List<IBranch> ChildBranchs { get; set; } = new List<IBranch>();
        public ITree TreeEditor { get; set; }
        public List<string> BranchActions { get; set; } = new List<string>();
        public EntityStructure EntityStructure { get; set; }
        public int MiscID { get; set; }
        public string Name { get; set; }
        public string BranchText { get; set; } = "In-Memory Databases";
        public int Level { get; set; }
        public EnumPointType BranchType { get; set; } = EnumPointType.Genre;
        public int BranchID { get; set; }
        public string IconImageName { get; set; } = "inmemoryroot.png";
        public string BranchStatus { get; set; }
        public int ParentBranchID { get; set; }
        public string BranchDescription { get; set; }
        public string BranchClass { get; set; } = "INMEMORY.ROOT";
        public IBranch ParentBranch { get; set; }
        #endregion

        #region Constructors
        public InMemoryRootNode(ITree pTreeEditor, IDMEEditor pDMEEditor, IBranch pParentNode, 
                               string pBranchText, int pID, EnumPointType pBranchType, string pimagename)
        {
            TreeEditor = pTreeEditor;
            DMEEditor = pDMEEditor;
            ParentBranch = pParentNode;
            BranchText = "In-Memory Databases";
            BranchClass = "INMEMORY.ROOT";
            IconImageName = "inmemoryroot.png";
            BranchType = EnumPointType.Genre;
            ID = pID;
        }

        public InMemoryRootNode()
        {
            BranchText = "In-Memory Databases";
            BranchClass = "INMEMORY.ROOT";
            IconImageName = "inmemoryroot.png";
            BranchType = EnumPointType.Genre;
            ID = -1;
        }
        #endregion

        #region IBranch Implementation
        public IBranch CreateCategoryNode(CategoryFolder p)
        {
            try
            {
                var categoryNode = new InMemoryCategoryNode(TreeEditor, DMEEditor, this, p.FolderName, 
                                                          TreeEditor.SeqID, EnumPointType.Category, 
                                                          TreeEditor.CategoryIcon);
                TreeEditor.Treebranchhandler.AddBranch(this, categoryNode);
                categoryNode.CreateChildNodes();
                return categoryNode;
            }
            catch (Exception ex)
            {
                DMEEditor.AddLogMessage("Error", $"Failed to create category node: {ex.Message}", 
                                       DateTime.Now, -1, null, Errors.Failed);
                return null;
            }
        }

        public IErrorsInfo CreateChildNodes()
        {
            try
            {
                ChildBranchs.Clear();
                LoadInMemoryDataSources();
                LoadCategoryFolders();
                DMEEditor.AddLogMessage("Success", "Created in-memory database child nodes", 
                                       DateTime.Now, 0, null, Errors.Ok);
            }
            catch (Exception ex)
            {
                DMEEditor.AddLogMessage("Error", $"Failed to create in-memory child nodes: {ex.Message}", 
                                       DateTime.Now, -1, null, Errors.Failed);
            }

            return DMEEditor.ErrorObject;
        }

        public IErrorsInfo ExecuteBranchAction(string ActionName)
        {
            try
            {
                switch (ActionName.ToUpper())
                {
                    case "REFRESH":
                        return RefreshInMemoryDatabases();
                    case "CREATEDATABASE":
                        return CreateNewInMemoryDatabase();
                    case "CREATEDUCKDB":
                        return CreateDuckDbDatabase();
                    case "CREATESQLITEMEMORY":
                        return CreateSQLiteInMemoryDatabase();
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                DMEEditor.AddLogMessage("Error", $"Action execution failed: {ex.Message}", 
                                       DateTime.Now, -1, null, Errors.Failed);
            }

            return DMEEditor.ErrorObject;
        }

        public IErrorsInfo MenuItemClicked(string ActionName)
        {
            return ExecuteBranchAction(ActionName);
        }

        public IErrorsInfo RemoveChildNodes()
        {
            try
            {
                foreach (IBranch child in ChildBranchs.ToArray())
                {
                    TreeEditor.Treebranchhandler.RemoveBranch(child);
                }
                ChildBranchs.Clear();

                DMEEditor.AddLogMessage("Success", "Removed in-memory database child nodes", 
                                       DateTime.Now, 0, null, Errors.Ok);
            }
            catch (Exception ex)
            {
                DMEEditor.AddLogMessage("Error", $"Failed to remove child nodes: {ex.Message}", 
                                       DateTime.Now, -1, null, Errors.Failed);
            }

            return DMEEditor.ErrorObject;
        }

        public IErrorsInfo SetConfig(ITree pTreeEditor, IDMEEditor pDMEEditor, IBranch pParentNode, 
                                   string pBranchText, int pID, EnumPointType pBranchType, string pimagename)
        {
            try
            {
                TreeEditor = pTreeEditor;
                DMEEditor = pDMEEditor;
                ParentBranch = pParentNode;
                ID = pID;

                DMEEditor.AddLogMessage("Success", "In-memory database root configuration set", 
                                       DateTime.Now, 0, null, Errors.Ok);
            }
            catch (Exception ex)
            {
                DMEEditor.AddLogMessage("Error", $"Configuration failed: {ex.Message}", 
                                       DateTime.Now, -1, null, Errors.Failed);
            }

            return DMEEditor.ErrorObject;
        }
        #endregion

        #region Context Menu Commands
        [CommandAttribute(Caption = "Create DuckDB Database", iconimage = "createentity.png", ObjectType = "Beep")]
        public IErrorsInfo CreateDuckDbDatabase()
        {
            try
            {
                var dbName = $"DuckDB_{DateTime.Now:yyyyMMdd_HHmmss}";
                
                var connectionProps = new ConnectionProperties
                {
                    ConnectionName = dbName,
                    DatabaseType = DataSourceType.DuckDB,
                    ConnectionString = ":memory:",
                    Category = DatasourceCategory.INMEMORY,
                    IsInMemory = true
                };

                DMEEditor.ConfigEditor.AddDataConnection(connectionProps);

                DMEEditor.AddLogMessage("Success", $"Created DuckDB in-memory database: {dbName}", 
                                       DateTime.Now, 0, null, Errors.Ok);
                
                RefreshInMemoryDatabases();
            }
            catch (Exception ex)
            {
                DMEEditor.AddLogMessage("Error", $"Failed to create DuckDB database: {ex.Message}", 
                                       DateTime.Now, -1, null, Errors.Failed);
            }

            return DMEEditor.ErrorObject;
        }

        [CommandAttribute(Caption = "Create SQLite In-Memory DB", iconimage = "createentity.png", ObjectType = "Beep")]
        public IErrorsInfo CreateSQLiteInMemoryDatabase()
        {
            try
            {
                var dbName = $"SQLiteMemory_{DateTime.Now:yyyyMMdd_HHmmss}";
                
                var connectionProps = new ConnectionProperties
                {
                    ConnectionName = dbName,
                    DatabaseType = DataSourceType.SqlLite,
                    ConnectionString = "Data Source=:memory:;Version=3;",
                    Category = DatasourceCategory.INMEMORY,
                    IsInMemory = true
                };

                DMEEditor.ConfigEditor.AddDataConnection(connectionProps);

                DMEEditor.AddLogMessage("Success", $"Created SQLite in-memory database: {dbName}", 
                                       DateTime.Now, 0, null, Errors.Ok);
                
                RefreshInMemoryDatabases();
            }
            catch (Exception ex)
            {
                DMEEditor.AddLogMessage("Error", $"Failed to create SQLite in-memory database: {ex.Message}", 
                                       DateTime.Now, -1, null, Errors.Failed);
            }

            return DMEEditor.ErrorObject;
        }

        [CommandAttribute(Caption = "Create In-Memory DB", iconimage = "createentity.png", ObjectType = "Beep")]
        public IErrorsInfo CreateNewInMemoryDatabase()
        {
            try
            {
                var dbName = $"InMemoryDB_{DateTime.Now:yyyyMMdd_HHmmss}";
                
                // Use a generic memory database type - could be DuckDB or other
                var connectionProps = new ConnectionProperties
                {
                    ConnectionName = dbName,
                    DatabaseType = DataSourceType.DuckDB, // Default to DuckDB for in-memory
                    ConnectionString = ":memory:",
                    Category = DatasourceCategory.INMEMORY,
                    IsInMemory = true
                };

                DMEEditor.ConfigEditor.AddDataConnection(connectionProps);

                DMEEditor.AddLogMessage("Success", $"Created in-memory database: {dbName}", 
                                       DateTime.Now, 0, null, Errors.Ok);
                
                RefreshInMemoryDatabases();
            }
            catch (Exception ex)
            {
                DMEEditor.AddLogMessage("Error", $"Failed to create in-memory database: {ex.Message}", 
                                       DateTime.Now, -1, null, Errors.Failed);
            }

            return DMEEditor.ErrorObject;
        }

        [CommandAttribute(Caption = "Refresh Databases", iconimage = "refresh.png", ObjectType = "Beep")]
        public IErrorsInfo RefreshInMemoryDatabases()
        {
            try
            {
                RemoveChildNodes();
                CreateChildNodes();

                DMEEditor.AddLogMessage("Success", "Refreshed in-memory databases", 
                                       DateTime.Now, 0, null, Errors.Ok);
            }
            catch (Exception ex)
            {
                DMEEditor.AddLogMessage("Error", $"Failed to refresh databases: {ex.Message}", 
                                       DateTime.Now, -1, null, Errors.Failed);
            }

            return DMEEditor.ErrorObject;
        }
        #endregion

        #region Private Methods
        private void LoadInMemoryDataSources()
        {
            try
            {
                var inMemoryConnections = DMEEditor.ConfigEditor.DataConnections
                    .Where(c => c.Category == DatasourceCategory.INMEMORY || 
                               c.IsInMemory == true ||
                               c.ConnectionString.Contains(":memory:") ||
                               c.DatabaseType == DataSourceType.DuckDB).ToList();

                foreach (var conn in inMemoryConnections)
                {
                    // Create in-memory database node
                    var dbNode = new InMemoryDatabaseNode(TreeEditor, DMEEditor, this, 
                                                        conn.ConnectionName, TreeEditor.SeqID, 
                                                        EnumPointType.DataPoint, "inmemorydatabase.png");
                    TreeEditor.Treebranchhandler.AddBranch(this, dbNode);
                }

                DMEEditor.AddLogMessage("Success", $"Loaded {inMemoryConnections.Count} in-memory databases", 
                                       DateTime.Now, 0, null, Errors.Ok);
            }
            catch (Exception ex)
            {
                DMEEditor.AddLogMessage("Error", $"Failed to load in-memory databases: {ex.Message}", 
                                       DateTime.Now, -1, null, Errors.Failed);
            }
        }

        private void LoadCategoryFolders()
        {
            try
            {
                foreach (CategoryFolder folder in DMEEditor.ConfigEditor.CategoryFolders
                    .Where(x => x.RootName.Equals("INMEMORY", StringComparison.InvariantCultureIgnoreCase)))
                {
                    if (!ChildBranchs.Any(p => p.BranchText == folder.FolderName && p.BranchClass.Contains("INMEMORY")))
                    {
                        CreateCategoryNode(folder);
                    }
                }
            }
            catch (Exception ex)
            {
                DMEEditor.AddLogMessage("Error", $"Failed to load category folders: {ex.Message}", 
                                       DateTime.Now, -1, null, Errors.Failed);
            }
        }
        #endregion
    }
}