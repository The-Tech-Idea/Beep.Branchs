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
    /// Category node for organizing in-memory databases by type or purpose
    /// </summary>
    [AddinAttribute(Caption = "In-Memory Category", misc = "Beep", BranchType = EnumPointType.Category, 
                    FileType = "Beep", iconimage = "inmemorycategory.png", menu = "InMemory", 
                    ObjectType = "Beep", ClassType = "CATEGORY")]
    [AddinVisSchema(BranchType = EnumPointType.Category, BranchClass = "INMEMORY.CATEGORY")]
    public class InMemoryCategoryNode : IBranch
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
        public int Order { get; set; } = 6;
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
        public string BranchText { get; set; } = "Category";
        public int Level { get; set; }
        public EnumPointType BranchType { get; set; } = EnumPointType.Category;
        public int BranchID { get; set; }
        public string IconImageName { get; set; } = "inmemorycategory.png";
        public string BranchStatus { get; set; }
        public int ParentBranchID { get; set; }
        public string BranchDescription { get; set; }
        public string BranchClass { get; set; } = "INMEMORY.CATEGORY";
        public IBranch ParentBranch { get; set; }
        #endregion

        #region Constructors
        public InMemoryCategoryNode(ITree pTreeEditor, IDMEEditor pDMEEditor, IBranch pParentNode, 
                                  string pBranchText, int pID, EnumPointType pBranchType, string pimagename)
        {
            TreeEditor = pTreeEditor;
            DMEEditor = pDMEEditor;
            ParentBranch = pParentNode;
            BranchText = pBranchText;
            BranchClass = "INMEMORY.CATEGORY";
            IconImageName = "inmemorycategory.png";
            BranchType = EnumPointType.Category;
            ID = pID;
        }

        public InMemoryCategoryNode()
        {
            BranchText = "Category";
            BranchClass = "INMEMORY.CATEGORY";
            IconImageName = "inmemorycategory.png";
            BranchType = EnumPointType.Category;
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
                DMEEditor.AddLogMessage("Error", $"Failed to create sub-category node: {ex.Message}", 
                                       DateTime.Now, -1, null, Errors.Failed);
                return null;
            }
        }

        public IErrorsInfo CreateChildNodes()
        {
            try
            {
                ChildBranchs.Clear();
                LoadCategorizedInMemoryDataSources();
                LoadSubCategories();
                DMEEditor.AddLogMessage("Success", $"Created child nodes for category: {BranchText}", 
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
                        return RefreshCategory();
                    case "CREATEDATABASE":
                        return CreateCategorizedDatabase();
                    case "CREATESUBCATEGORY":
                        return CreateSubCategory();
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

                DMEEditor.AddLogMessage("Success", $"Removed child nodes from category: {BranchText}", 
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
                ID = pID;

                DMEEditor.AddLogMessage("Success", $"In-memory category configuration set: {BranchText}", 
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
        [CommandAttribute(Caption = "Create In-Memory DB", iconimage = "createentity.png", ObjectType = "Beep")]
        public IErrorsInfo CreateCategorizedDatabase()
        {
            try
            {
                var dbName = $"InMemoryDB_{BranchText}_{DateTime.Now:yyyyMMdd_HHmmss}";
                
                var connectionProps = new ConnectionProperties
                {
                    ConnectionName = dbName,
                    DatabaseType = DataSourceType.SqlLite, // Changed from DataSourceType.InMemory which doesn't exist
                    ConnectionString = ":memory:",
                    Category = DatasourceCategory.INMEMORY,
                    IsInMemory = true
                };

                DMEEditor.ConfigEditor.AddDataConnection(connectionProps);

                // Create the database node directly in this category
                var dbNode = new InMemoryDatabaseNode(TreeEditor, DMEEditor, this, 
                                                    dbName, TreeEditor.SeqID, 
                                                    EnumPointType.DataPoint, "inmemorydatabase.png");
                TreeEditor.Treebranchhandler.AddBranch(this, dbNode);

                DMEEditor.AddLogMessage("Success", $"Created categorized in-memory database: {dbName}", 
                                       DateTime.Now, 0, null, Errors.Ok);
            }
            catch (Exception ex)
            {
                DMEEditor.AddLogMessage("Error", $"Failed to create categorized database: {ex.Message}", 
                                       DateTime.Now, -1, null, Errors.Failed);
            }

            return DMEEditor.ErrorObject;
        }

        [CommandAttribute(Caption = "Create Sub-Category", iconimage = "createfolder.png", ObjectType = "Beep")]
        public IErrorsInfo CreateSubCategory()
        {
            try
            {
                var subCategoryName = $"SubCategory_{DateTime.Now:HHmmss}";
                
                var categoryFolder = new CategoryFolder
                {
                    FolderName = subCategoryName,
                    RootName = "INMEMORY",
                    ParentName = BranchText,
                    IsParentRoot = false,
                    IsParentFolder = true
                };

                DMEEditor.ConfigEditor.CategoryFolders.Add(categoryFolder);
                CreateCategoryNode(categoryFolder);

                DMEEditor.AddLogMessage("Success", $"Created sub-category: {subCategoryName}", 
                                       DateTime.Now, 0, null, Errors.Ok);
            }
            catch (Exception ex)
            {
                DMEEditor.AddLogMessage("Error", $"Failed to create sub-category: {ex.Message}", 
                                       DateTime.Now, -1, null, Errors.Failed);
            }

            return DMEEditor.ErrorObject;
        }

        [CommandAttribute(Caption = "Refresh Category", iconimage = "refresh.png", ObjectType = "Beep")]
        public IErrorsInfo RefreshCategory()
        {
            try
            {
                RemoveChildNodes();
                CreateChildNodes();

                DMEEditor.AddLogMessage("Success", $"Refreshed category: {BranchText}", 
                                       DateTime.Now, 0, null, Errors.Ok);
            }
            catch (Exception ex)
            {
                DMEEditor.AddLogMessage("Error", $"Failed to refresh category: {ex.Message}", 
                                       DateTime.Now, -1, null, Errors.Failed);
            }

            return DMEEditor.ErrorObject;
        }
        #endregion

        #region Private Methods
        private void LoadCategorizedInMemoryDataSources()
        {
            try
            {
                // Load in-memory databases that might belong to this category
                var inMemoryConnections = DMEEditor.ConfigEditor.DataConnections
                    .Where(c => c.Category == DatasourceCategory.INMEMORY || 
                               c.IsInMemory == true ||
                               c.ConnectionString.Contains(":memory:"))
                    .Where(c => string.IsNullOrEmpty(c.ConnectionName) || c.ConnectionName.Equals(BranchText, StringComparison.OrdinalIgnoreCase))
                    .ToList();

                foreach (var conn in inMemoryConnections)
                {
                    if (!ChildBranchs.Any(p => p.BranchText.Equals(conn.ConnectionName, StringComparison.InvariantCultureIgnoreCase)))
                    {
                        var dbNode = new InMemoryDatabaseNode(TreeEditor, DMEEditor, this, 
                                                            conn.ConnectionName, TreeEditor.SeqID, 
                                                            EnumPointType.DataPoint, "inmemorydatabase.png");
                        TreeEditor.Treebranchhandler.AddBranch(this, dbNode);
                    }
                }

                DMEEditor.AddLogMessage("Success", $"Loaded {inMemoryConnections.Count} categorized in-memory databases", 
                                       DateTime.Now, 0, null, Errors.Ok);
            }
            catch (Exception ex)
            {
                DMEEditor.AddLogMessage("Error", $"Failed to load categorized databases: {ex.Message}", 
                                       DateTime.Now, -1, null, Errors.Failed);
            }
        }

        private void LoadSubCategories()
        {
            try
            {
                foreach (CategoryFolder folder in DMEEditor.ConfigEditor.CategoryFolders
                    .Where(x => x.RootName.Equals("INMEMORY", StringComparison.InvariantCultureIgnoreCase) &&
                               x.ParentName.Equals(BranchText, StringComparison.InvariantCultureIgnoreCase)))
                {
                    if (!ChildBranchs.Any(p => p.BranchText == folder.FolderName && p.BranchClass.Contains("INMEMORY")))
                    {
                        CreateCategoryNode(folder);
                    }
                }
            }
            catch (Exception ex)
            {
                DMEEditor.AddLogMessage("Error", $"Failed to load sub-categories: {ex.Message}", 
                                       DateTime.Now, -1, null, Errors.Failed);
            }
        }
        #endregion
    }
}