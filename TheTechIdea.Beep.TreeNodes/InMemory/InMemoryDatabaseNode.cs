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
    /// Database node for individual in-memory database instances
    /// </summary>
    [AddinAttribute(Caption = "In-Memory Database", misc = "Beep", BranchType = EnumPointType.DataPoint, 
                    FileType = "Beep", iconimage = "inmemorydatabase.png", menu = "InMemory", 
                    ObjectType = "Beep", ClassType = "DATABASE")]
    [AddinVisSchema(BranchType = EnumPointType.DataPoint, BranchClass = "INMEMORY.DATABASE")]
    public class InMemoryDatabaseNode : IBranch
    {
        #region Properties
        public string GuidID { get; set; } = Guid.NewGuid().ToString();
        public string ParentGuidID { get; set; }
        public string DataSourceConnectionGuidID { get; set; }
        public string EntityGuidID { get; set; }
        public bool Visible { get; set; } = true;
        public string MenuID { get; set; }
        public string MiscStringID { get; set; }
        public bool IsDataSourceNode { get; set; } = true;
        public string ObjectType { get; set; } = "Beep";
        public int Order { get; set; } = 7;
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
        public string BranchText { get; set; } = "Database";
        public int Level { get; set; }
        public EnumPointType BranchType { get; set; } = EnumPointType.DataPoint;
        public int BranchID { get; set; }
        public string IconImageName { get; set; } = "inmemorydatabase.png";
        public string BranchStatus { get; set; }
        public int ParentBranchID { get; set; }
        public string BranchDescription { get; set; }
        public string BranchClass { get; set; } = "INMEMORY.DATABASE";
        public IBranch ParentBranch { get; set; }
        #endregion

        #region Constructors
        public InMemoryDatabaseNode(ITree pTreeEditor, IDMEEditor pDMEEditor, IBranch pParentNode, 
                                  string pBranchText, int pID, EnumPointType pBranchType, string pimagename)
        {
            TreeEditor = pTreeEditor;
            DMEEditor = pDMEEditor;
            ParentBranch = pParentNode;
            BranchText = pBranchText;
            DataSourceName = pBranchText;
            BranchClass = "INMEMORY.DATABASE";
            IconImageName = "inmemorydatabase.png";
            BranchType = EnumPointType.DataPoint;
            ID = pID;
            
            // Try to get the data source
            DataSource = DMEEditor.GetDataSource(DataSourceName);
        }

        public InMemoryDatabaseNode()
        {
            BranchText = "Database";
            BranchClass = "INMEMORY.DATABASE";
            IconImageName = "inmemorydatabase.png";
            BranchType = EnumPointType.DataPoint;
            ID = -1;
        }
        #endregion

        #region IBranch Implementation
        public IBranch CreateCategoryNode(CategoryFolder p)
        {
            // In-memory database nodes don't typically create categories, but could organize tables
            return null;
        }

        public IErrorsInfo CreateChildNodes()
        {
            try
            {
                ChildBranchs.Clear();
                LoadTables();
                LoadViews();
                DMEEditor.AddLogMessage("Success", $"Created child nodes for database: {BranchText}", 
                                       DateTime.Now, 0, null, Errors.Ok);
            }
            catch (Exception ex)
            {
                DMEEditor.AddLogMessage("Error", $"Failed to create child nodes: {ex.Message}", 
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
                        return RefreshDatabase();
                    case "CONNECT":
                        return ConnectToDatabase();
                    case "DISCONNECT":
                        return DisconnectFromDatabase();
                    case "CREATETABLE":
                        return CreateNewTable();
                    case "LOADDATA":
                        return LoadDataIntoMemory();
                    case "CLEARDATA":
                        return ClearMemoryData();
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

                DMEEditor.AddLogMessage("Success", $"Removed child nodes from database: {BranchText}", 
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
                BranchText = pBranchText;
                DataSourceName = pBranchText;
                ID = pID;

                // Try to get the data source
                DataSource = DMEEditor.GetDataSource(DataSourceName);

                DMEEditor.AddLogMessage("Success", $"In-memory database configuration set: {BranchText}", 
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
        [CommandAttribute(Caption = "Connect", iconimage = "connect.png", ObjectType = "Beep")]
        public IErrorsInfo ConnectToDatabase()
        {
            try
            {
                if (DataSource == null)
                {
                    DataSource = DMEEditor.GetDataSource(DataSourceName);
                }

                if (DataSource != null)
                {
                    DMEEditor.OpenDataSource(DataSourceName);
                    
                    if (DataSource.ConnectionStatus == System.Data.ConnectionState.Open)
                    {
                        BranchStatus = "Connected";
                        DMEEditor.AddLogMessage("Success", $"Connected to in-memory database: {BranchText}", 
                                               DateTime.Now, 0, null, Errors.Ok);
                        
                        // Load entities after successful connection
                        CreateChildNodes();
                    }
                    else
                    {
                        BranchStatus = "Disconnected";
                        DMEEditor.AddLogMessage("Error", $"Failed to connect to database: {BranchText}", 
                                               DateTime.Now, -1, null, Errors.Failed);
                    }
                }
                else
                {
                    DMEEditor.AddLogMessage("Error", $"Data source not found: {DataSourceName}", 
                                           DateTime.Now, -1, null, Errors.Failed);
                }
            }
            catch (Exception ex)
            {
                DMEEditor.AddLogMessage("Error", $"Failed to connect to database: {ex.Message}", 
                                       DateTime.Now, -1, null, Errors.Failed);
            }

            return DMEEditor.ErrorObject;
        }

        [CommandAttribute(Caption = "Disconnect", iconimage = "disconnect.png", ObjectType = "Beep")]
        public IErrorsInfo DisconnectFromDatabase()
        {
            try
            {
                if (DataSource != null)
                {
                    DMEEditor.CloseDataSource(DataSourceName);
                    BranchStatus = "Disconnected";
                    RemoveChildNodes(); // Clear tables when disconnected
                    
                    DMEEditor.AddLogMessage("Success", $"Disconnected from in-memory database: {BranchText}", 
                                           DateTime.Now, 0, null, Errors.Ok);
                }
            }
            catch (Exception ex)
            {
                DMEEditor.AddLogMessage("Error", $"Failed to disconnect from database: {ex.Message}", 
                                       DateTime.Now, -1, null, Errors.Failed);
            }

            return DMEEditor.ErrorObject;
        }

        [CommandAttribute(Caption = "Create Table", iconimage = "createtable.png", ObjectType = "Beep")]
        public IErrorsInfo CreateNewTable()
        {
            try
            {
                var tableName = $"Table_{DateTime.Now:HHmmss}";
                
                // Create a simple entity structure for the new table
                var entityStructure = new EntityStructure
                {
                    EntityName = tableName,
                    DataSourceID = DataSourceName,
                    Viewtype = ViewType.Table,
                    Fields = new List<EntityField>
                    {
                        new EntityField
                        {
                            fieldname = "ID",
                            fieldtype = "int",
                            AllowDBNull = false,
                            IsKey = true,
                            IsAutoIncrement = true
                        },
                        new EntityField
                        {
                            fieldname = "Name",
                            fieldtype = "varchar",
                            Size1 = 255,
                            AllowDBNull = true
                        },
                        new EntityField
                        {
                            fieldname = "CreatedDate",
                            fieldtype = "datetime",
                            AllowDBNull = false
                        }
                    }
                };

                if (DataSource != null && DataSource.CreateEntityAs(entityStructure))
                {
                    // Create table node
                    var tableNode = new InMemoryTableNode(TreeEditor, DMEEditor, this, 
                                                        tableName, TreeEditor.SeqID, 
                                                        EnumPointType.Entity, "memorytable.png");
                    TreeEditor.Treebranchhandler.AddBranch(this, tableNode);

                    DMEEditor.AddLogMessage("Success", $"Created table: {tableName}", 
                                           DateTime.Now, 0, null, Errors.Ok);
                }
                else
                {
                    DMEEditor.AddLogMessage("Error", $"Failed to create table: {tableName}", 
                                           DateTime.Now, -1, null, Errors.Failed);
                }
            }
            catch (Exception ex)
            {
                DMEEditor.AddLogMessage("Error", $"Failed to create table: {ex.Message}", 
                                       DateTime.Now, -1, null, Errors.Failed);
            }

            return DMEEditor.ErrorObject;
        }

        [CommandAttribute(Caption = "Load Data", iconimage = "import.png", ObjectType = "Beep")]
        public IErrorsInfo LoadDataIntoMemory()
        {
            try
            {
                if (DataSource is IInMemoryDB memoryDB)
                {
                    // Try to load data structure and data
                    if (!memoryDB.IsStructureCreated)
                    {
                        var progress = new Progress<PassedArgs>(args => 
                        {
                            DMEEditor.AddLogMessage("Info", args.Messege ?? "Loading...", 
                                                   DateTime.Now, 0, null, Errors.Ok);
                        });
                        
                        var token = new System.Threading.CancellationToken();
                        memoryDB.LoadStructure(progress, token);
                        memoryDB.CreateStructure(progress, token);
                    }

                    if (!memoryDB.IsLoaded)
                    {
                        var progress = new Progress<PassedArgs>(args => 
                        {
                            DMEEditor.AddLogMessage("Info", args.Messege ?? "Loading data...", 
                                                   DateTime.Now, 0, null, Errors.Ok);
                        });
                        
                        var token = new System.Threading.CancellationToken();
                        memoryDB.LoadData(progress, token);
                    }

                    RefreshDatabase();
                    DMEEditor.AddLogMessage("Success", $"Loaded data into memory for: {BranchText}", 
                                           DateTime.Now, 0, null, Errors.Ok);
                }
                else
                {
                    DMEEditor.AddLogMessage("Warning", $"Database does not support in-memory loading: {BranchText}", 
                                           DateTime.Now, 0, null, Errors.Ok);
                }
            }
            catch (Exception ex)
            {
                DMEEditor.AddLogMessage("Error", $"Failed to load data into memory: {ex.Message}", 
                                       DateTime.Now, -1, null, Errors.Failed);
            }

            return DMEEditor.ErrorObject;
        }

        [CommandAttribute(Caption = "Clear Memory", iconimage = "clear.png", ObjectType = "Beep")]
        public IErrorsInfo ClearMemoryData()
        {
            try
            {
                if (DataSource is IInMemoryDB memoryDB)
                {
                    // Clear in-memory data while keeping structure
                    var progress = new Progress<PassedArgs>(args => 
                    {
                        DMEEditor.AddLogMessage("Info", args.Messege ?? "Clearing...", 
                                               DateTime.Now, 0, null, Errors.Ok);
                    });
                    
                    var token = new System.Threading.CancellationToken();
                    memoryDB.RefreshData(progress, token);

                    DMEEditor.AddLogMessage("Success", $"Cleared memory data for: {BranchText}", 
                                           DateTime.Now, 0, null, Errors.Ok);
                }
                else
                {
                    DMEEditor.AddLogMessage("Warning", $"Database does not support memory clearing: {BranchText}", 
                                           DateTime.Now, 0, null, Errors.Ok);
                }
            }
            catch (Exception ex)
            {
                DMEEditor.AddLogMessage("Error", $"Failed to clear memory data: {ex.Message}", 
                                       DateTime.Now, -1, null, Errors.Failed);
            }

            return DMEEditor.ErrorObject;
        }

        [CommandAttribute(Caption = "Refresh", iconimage = "refresh.png", ObjectType = "Beep")]
        public IErrorsInfo RefreshDatabase()
        {
            try
            {
                RemoveChildNodes();
                CreateChildNodes();

                DMEEditor.AddLogMessage("Success", $"Refreshed database: {BranchText}", 
                                       DateTime.Now, 0, null, Errors.Ok);
            }
            catch (Exception ex)
            {
                DMEEditor.AddLogMessage("Error", $"Failed to refresh database: {ex.Message}", 
                                       DateTime.Now, -1, null, Errors.Failed);
            }

            return DMEEditor.ErrorObject;
        }
        #endregion

        #region Private Methods
        private void LoadTables()
        {
            try
            {
                if (DataSource == null)
                {
                    DataSource = DMEEditor.GetDataSource(DataSourceName);
                }

                if (DataSource != null && DataSource.ConnectionStatus == System.Data.ConnectionState.Open)
                {
                    DataSource.GetEntitesList();
                    
                    foreach (EntityStructure entity in DataSource.Entities.Where(e => e.Viewtype == ViewType.Table))
                    {
                        if (!ChildBranchs.Any(p => p.BranchText.Equals(entity.EntityName, StringComparison.InvariantCultureIgnoreCase)))
                        {
                            var tableNode = new InMemoryTableNode(TreeEditor, DMEEditor, this, 
                                                                entity.EntityName, TreeEditor.SeqID, 
                                                                EnumPointType.Entity, "memorytable.png");
                            TreeEditor.Treebranchhandler.AddBranch(this, tableNode);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                DMEEditor.AddLogMessage("Error", $"Failed to load tables: {ex.Message}", 
                                       DateTime.Now, -1, null, Errors.Failed);
            }
        }

        private void LoadViews()
        {
            try
            {
                if (DataSource != null && DataSource.ConnectionStatus == System.Data.ConnectionState.Open)
                {
                    foreach (EntityStructure entity in DataSource.Entities.Where(e => e.Viewtype == ViewType.View))
                    {
                        if (!ChildBranchs.Any(p => p.BranchText.Equals(entity.EntityName, StringComparison.InvariantCultureIgnoreCase)))
                        {
                            var viewNode = new InMemoryViewNode(TreeEditor, DMEEditor, this, 
                                                              entity.EntityName, TreeEditor.SeqID, 
                                                              EnumPointType.Entity, "memoryview.png");
                            TreeEditor.Treebranchhandler.AddBranch(this, viewNode);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                DMEEditor.AddLogMessage("Error", $"Failed to load views: {ex.Message}", 
                                       DateTime.Now, -1, null, Errors.Failed);
            }
        }
        #endregion
    }
}