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
    /// View node for in-memory database views
    /// </summary>
    [AddinAttribute(Caption = "In-Memory View", misc = "Beep", BranchType = EnumPointType.Entity, 
                    FileType = "Beep", iconimage = "memoryview.png", menu = "InMemory", 
                    ObjectType = "Beep", ClassType = "VIEW")]
    [AddinVisSchema(BranchType = EnumPointType.Entity, BranchClass = "INMEMORY.VIEW")]
    public class InMemoryViewNode : IBranch
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
        public int Order { get; set; } = 9;
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
        public string BranchText { get; set; } = "View";
        public int Level { get; set; }
        public EnumPointType BranchType { get; set; } = EnumPointType.Entity;
        public int BranchID { get; set; }
        public string IconImageName { get; set; } = "memoryview.png";
        public string BranchStatus { get; set; }
        public int ParentBranchID { get; set; }
        public string BranchDescription { get; set; }
        public string BranchClass { get; set; } = "INMEMORY.VIEW";
        public IBranch ParentBranch { get; set; }
        #endregion

        #region Constructors
        public InMemoryViewNode(ITree pTreeEditor, IDMEEditor pDMEEditor, IBranch pParentNode, 
                               string pBranchText, int pID, EnumPointType pBranchType, string pimagename)
        {
            TreeEditor = pTreeEditor;
            DMEEditor = pDMEEditor;
            ParentBranch = pParentNode;
            BranchText = pBranchText;
            BranchClass = "INMEMORY.VIEW";
            IconImageName = "memoryview.png";
            BranchType = EnumPointType.Entity;
            ID = pID;
            
            // Get data source from parent
            if (pParentNode is InMemoryDatabaseNode dbNode)
            {
                DataSource = dbNode.DataSource;
                DataSourceName = dbNode.DataSourceName;
            }
            
            // Try to get entity structure
            if (DataSource != null)
            {
                EntityStructure = DataSource.GetEntityStructure(BranchText, false);
            }
        }

        public InMemoryViewNode()
        {
            BranchText = "View";
            BranchClass = "INMEMORY.VIEW";
            IconImageName = "memoryview.png";
            BranchType = EnumPointType.Entity;
            ID = -1;
        }
        #endregion

        #region IBranch Implementation
        public IBranch CreateCategoryNode(CategoryFolder p)
        {
            // Views don't create categories
            return null;
        }

        public IErrorsInfo CreateChildNodes()
        {
            try
            {
                ChildBranchs.Clear();
                // In-memory views typically don't have child nodes, but could show columns
                // This could be extended to show column details if needed
                
                DMEEditor.AddLogMessage("Success", $"Created child nodes for view: {BranchText}", 
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
                    case "VIEWDATA":
                        return ViewData();
                    case "EXPORTDATA":
                        return ExportViewData();
                    case "DROPVIEW":
                        return DropView();
                    case "GETSTRUCTURE":
                        return GetViewStructure();
                    case "SHOWDEFINITION":
                        return ShowViewDefinition();
                    case "REFRESHVIEW":
                        return RefreshView();
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

                DMEEditor.AddLogMessage("Success", $"Removed child nodes from view: {BranchText}", 
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

                // Get data source from parent
                if (pParentNode is InMemoryDatabaseNode dbNode)
                {
                    DataSource = dbNode.DataSource;
                    DataSourceName = dbNode.DataSourceName;
                }

                DMEEditor.AddLogMessage("Success", $"In-memory view configuration set: {BranchText}", 
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
        [CommandAttribute(Caption = "View Data", iconimage = "view.png", ObjectType = "Beep")]
        public IErrorsInfo ViewData()
        {
            try
            {
                if (DataSource != null && !string.IsNullOrEmpty(BranchText))
                {
                    // This would typically open a data viewer
                    // For now, just get count and log it
                    var data = DataSource.GetEntity(BranchText, null);
                    if (data != null)
                    {
                        int rowCount = 0;
                        if (data is System.Data.DataTable dt)
                        {
                            rowCount = dt.Rows.Count;
                        }
                        else if (data is System.Collections.ICollection collection)
                        {
                            rowCount = collection.Count;
                        }

                        DMEEditor.AddLogMessage("Success", $"View {BranchText} returns {rowCount} rows", 
                                               DateTime.Now, 0, null, Errors.Ok);
                    }
                    else
                    {
                        DMEEditor.AddLogMessage("Warning", $"No data returned from view: {BranchText}", 
                                               DateTime.Now, 0, null, Errors.Ok);
                    }
                }
                else
                {
                    DMEEditor.AddLogMessage("Error", "Data source or view name not available", 
                                           DateTime.Now, -1, null, Errors.Failed);
                }
            }
            catch (Exception ex)
            {
                DMEEditor.AddLogMessage("Error", $"Failed to view data: {ex.Message}", 
                                       DateTime.Now, -1, null, Errors.Failed);
            }

            return DMEEditor.ErrorObject;
        }

        [CommandAttribute(Caption = "Export Data", iconimage = "export.png", ObjectType = "Beep")]
        public IErrorsInfo ExportViewData()
        {
            try
            {
                if (DataSource != null && !string.IsNullOrEmpty(BranchText))
                {
                    var data = DataSource.GetEntity(BranchText, null);
                    if (data != null)
                    {
                        // This would typically open an export dialog
                        // For now, just log the action
                        DMEEditor.AddLogMessage("Success", $"Export functionality available for view: {BranchText}", 
                                               DateTime.Now, 0, null, Errors.Ok);
                    }
                    else
                    {
                        DMEEditor.AddLogMessage("Warning", $"No data to export from view: {BranchText}", 
                                               DateTime.Now, 0, null, Errors.Ok);
                    }
                }
            }
            catch (Exception ex)
            {
                DMEEditor.AddLogMessage("Error", $"Failed to export view data: {ex.Message}", 
                                       DateTime.Now, -1, null, Errors.Failed);
            }

            return DMEEditor.ErrorObject;
        }

        [CommandAttribute(Caption = "Drop View", iconimage = "dropview.png", ObjectType = "Beep")]
        public IErrorsInfo DropView()
        {
            try
            {
                if (DataSource != null && !string.IsNullOrEmpty(BranchText))
                {
                    // Execute DROP VIEW statement
                    var sql = $"DROP VIEW {BranchText}";
                    if (DataSource is IRDBSource rdbSource)
                    {
                        rdbSource.ExecuteSql(sql);
                        
                        // Remove this node from the tree
                        if (ParentBranch != null)
                        {
                            TreeEditor.Treebranchhandler.RemoveBranch(this);
                        }
                        
                        DMEEditor.AddLogMessage("Success", $"Dropped view: {BranchText}", 
                                               DateTime.Now, 0, null, Errors.Ok);
                    }
                    else
                    {
                        DMEEditor.AddLogMessage("Warning", $"Drop view not supported for this data source type", 
                                               DateTime.Now, 0, null, Errors.Ok);
                    }
                }
            }
            catch (Exception ex)
            {
                DMEEditor.AddLogMessage("Error", $"Failed to drop view: {ex.Message}", 
                                       DateTime.Now, -1, null, Errors.Failed);
            }

            return DMEEditor.ErrorObject;
        }

        [CommandAttribute(Caption = "Get Structure", iconimage = "structure.png", ObjectType = "Beep")]
        public IErrorsInfo GetViewStructure()
        {
            try
            {
                if (DataSource != null && !string.IsNullOrEmpty(BranchText))
                {
                    EntityStructure = DataSource.GetEntityStructure(BranchText, true);
                    
                    if (EntityStructure != null)
                    {
                        var fieldCount = EntityStructure.Fields?.Count ?? 0;
                        DMEEditor.AddLogMessage("Success", 
                                               $"View {BranchText} has {fieldCount} fields", 
                                               DateTime.Now, 0, null, Errors.Ok);
                        
                        // Optionally log field details
                        if (EntityStructure.Fields != null)
                        {
                            foreach (var field in EntityStructure.Fields.Take(5)) // Show first 5 fields
                            {
                                DMEEditor.AddLogMessage("Info", 
                                                       $"  Field: {field.fieldname} ({field.fieldtype})", 
                                                       DateTime.Now, 0, null, Errors.Ok);
                            }
                        }
                    }
                    else
                    {
                        DMEEditor.AddLogMessage("Warning", $"Could not get structure for view: {BranchText}", 
                                               DateTime.Now, 0, null, Errors.Ok);
                    }
                }
            }
            catch (Exception ex)
            {
                DMEEditor.AddLogMessage("Error", $"Failed to get view structure: {ex.Message}", 
                                       DateTime.Now, -1, null, Errors.Failed);
            }

            return DMEEditor.ErrorObject;
        }

        [CommandAttribute(Caption = "Show Definition", iconimage = "definition.png", ObjectType = "Beep")]
        public IErrorsInfo ShowViewDefinition()
        {
            try
            {
                if (DataSource != null && !string.IsNullOrEmpty(BranchText))
                {
                    // Try to get view definition/SQL
                    if (DataSource is IRDBSource rdbSource)
                    {
                        // This would depend on the specific database implementation
                        // For now, just indicate the functionality is available
                        DMEEditor.AddLogMessage("Info", $"View definition functionality for: {BranchText}", 
                                               DateTime.Now, 0, null, Errors.Ok);
                    }
                    else
                    {
                        DMEEditor.AddLogMessage("Warning", $"View definition not supported for this data source type", 
                                               DateTime.Now, 0, null, Errors.Ok);
                    }
                }
            }
            catch (Exception ex)
            {
                DMEEditor.AddLogMessage("Error", $"Failed to show view definition: {ex.Message}", 
                                       DateTime.Now, -1, null, Errors.Failed);
            }

            return DMEEditor.ErrorObject;
        }

        [CommandAttribute(Caption = "Refresh View", iconimage = "refresh.png", ObjectType = "Beep")]
        public IErrorsInfo RefreshView()
        {
            try
            {
                // Refresh view metadata and structure
                if (DataSource != null && !string.IsNullOrEmpty(BranchText))
                {
                    EntityStructure = DataSource.GetEntityStructure(BranchText, true);
                    DMEEditor.AddLogMessage("Success", $"Refreshed view: {BranchText}", 
                                           DateTime.Now, 0, null, Errors.Ok);
                }
            }
            catch (Exception ex)
            {
                DMEEditor.AddLogMessage("Error", $"Failed to refresh view: {ex.Message}", 
                                       DateTime.Now, -1, null, Errors.Failed);
            }

            return DMEEditor.ErrorObject;
        }
        #endregion
    }
}