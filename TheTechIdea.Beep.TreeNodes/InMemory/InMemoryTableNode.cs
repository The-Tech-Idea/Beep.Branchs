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
    /// Table node for in-memory database tables
    /// </summary>
    [AddinAttribute(Caption = "In-Memory Table", misc = "Beep", BranchType = EnumPointType.Entity, 
                    FileType = "Beep", iconimage = "memorytable.png", menu = "InMemory", 
                    ObjectType = "Beep", ClassType = "TABLE")]
    [AddinVisSchema(BranchType = EnumPointType.Entity, BranchClass = "INMEMORY.TABLE")]
    public class InMemoryTableNode : IBranch
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
        public int Order { get; set; } = 8;
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
        public string BranchText { get; set; } = "Table";
        public int Level { get; set; }
        public EnumPointType BranchType { get; set; } = EnumPointType.Entity;
        public int BranchID { get; set; }
        public string IconImageName { get; set; } = "memorytable.png";
        public string BranchStatus { get; set; }
        public int ParentBranchID { get; set; }
        public string BranchDescription { get; set; }
        public string BranchClass { get; set; } = "INMEMORY.TABLE";
        public IBranch ParentBranch { get; set; }
        #endregion

        #region Constructors
        public InMemoryTableNode(ITree pTreeEditor, IDMEEditor pDMEEditor, IBranch pParentNode, 
                                string pBranchText, int pID, EnumPointType pBranchType, string pimagename)
        {
            TreeEditor = pTreeEditor;
            DMEEditor = pDMEEditor;
            ParentBranch = pParentNode;
            BranchText = pBranchText;
            BranchClass = "INMEMORY.TABLE";
            IconImageName = "memorytable.png";
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

        public InMemoryTableNode()
        {
            BranchText = "Table";
            BranchClass = "INMEMORY.TABLE";
            IconImageName = "memorytable.png";
            BranchType = EnumPointType.Entity;
            ID = -1;
        }
        #endregion

        #region IBranch Implementation
        public IBranch CreateCategoryNode(CategoryFolder p)
        {
            // Tables don't create categories
            return null;
        }

        public IErrorsInfo CreateChildNodes()
        {
            try
            {
                ChildBranchs.Clear();
                // In-memory tables typically don't have child nodes, but could show columns/indexes
                // This could be extended to show column details if needed
                
                DMEEditor.AddLogMessage("Success", $"Created child nodes for table: {BranchText}", 
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
                        return ViewTableData();
                    case "EDITDATA":
                        return EditTableData();
                    case "DELETEDATA":
                        return DeleteTableData();
                    case "INSERTDATA":
                        return InsertTableData();
                    case "EXPORTDATA":
                        return ExportTableData();
                    case "CLEARDATA":
                        return ClearTableData();
                    case "DROPTABLE":
                        return DropTable();
                    case "GETSTRUCTURE":
                        return GetTableStructure();
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

                DMEEditor.AddLogMessage("Success", $"Removed child nodes from table: {BranchText}", 
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

                DMEEditor.AddLogMessage("Success", $"In-memory table configuration set: {BranchText}", 
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
        public IErrorsInfo ViewTableData()
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

                        DMEEditor.AddLogMessage("Success", $"Table {BranchText} has {rowCount} rows", 
                                               DateTime.Now, 0, null, Errors.Ok);
                    }
                    else
                    {
                        DMEEditor.AddLogMessage("Warning", $"No data found in table: {BranchText}", 
                                               DateTime.Now, 0, null, Errors.Ok);
                    }
                }
                else
                {
                    DMEEditor.AddLogMessage("Error", "Data source or table name not available", 
                                           DateTime.Now, -1, null, Errors.Failed);
                }
            }
            catch (Exception ex)
            {
                DMEEditor.AddLogMessage("Error", $"Failed to view table data: {ex.Message}", 
                                       DateTime.Now, -1, null, Errors.Failed);
            }

            return DMEEditor.ErrorObject;
        }

        [CommandAttribute(Caption = "Edit Data", iconimage = "edit.png", ObjectType = "Beep")]
        public IErrorsInfo EditTableData()
        {
            try
            {
                // This would typically open a data editor
                // Implementation would depend on the UI framework being used
                DMEEditor.AddLogMessage("Info", $"Edit data functionality for table: {BranchText}", 
                                       DateTime.Now, 0, null, Errors.Ok);
            }
            catch (Exception ex)
            {
                DMEEditor.AddLogMessage("Error", $"Failed to edit table data: {ex.Message}", 
                                       DateTime.Now, -1, null, Errors.Failed);
            }

            return DMEEditor.ErrorObject;
        }

        [CommandAttribute(Caption = "Insert Data", iconimage = "insert.png", ObjectType = "Beep")]
        public IErrorsInfo InsertTableData()
        {
            try
            {
                // This would typically open an insert dialog
                DMEEditor.AddLogMessage("Info", $"Insert data functionality for table: {BranchText}", 
                                       DateTime.Now, 0, null, Errors.Ok);
            }
            catch (Exception ex)
            {
                DMEEditor.AddLogMessage("Error", $"Failed to insert table data: {ex.Message}", 
                                       DateTime.Now, -1, null, Errors.Failed);
            }

            return DMEEditor.ErrorObject;
        }

        [CommandAttribute(Caption = "Delete Data", iconimage = "delete.png", ObjectType = "Beep")]
        public IErrorsInfo DeleteTableData()
        {
            try
            {
                // This would typically show delete options
                DMEEditor.AddLogMessage("Info", $"Delete data functionality for table: {BranchText}", 
                                       DateTime.Now, 0, null, Errors.Ok);
            }
            catch (Exception ex)
            {
                DMEEditor.AddLogMessage("Error", $"Failed to delete table data: {ex.Message}", 
                                       DateTime.Now, -1, null, Errors.Failed);
            }

            return DMEEditor.ErrorObject;
        }

        [CommandAttribute(Caption = "Export Data", iconimage = "export.png", ObjectType = "Beep")]
        public IErrorsInfo ExportTableData()
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
                        DMEEditor.AddLogMessage("Success", $"Export functionality available for table: {BranchText}", 
                                               DateTime.Now, 0, null, Errors.Ok);
                    }
                    else
                    {
                        DMEEditor.AddLogMessage("Warning", $"No data to export from table: {BranchText}", 
                                               DateTime.Now, 0, null, Errors.Ok);
                    }
                }
            }
            catch (Exception ex)
            {
                DMEEditor.AddLogMessage("Error", $"Failed to export table data: {ex.Message}", 
                                       DateTime.Now, -1, null, Errors.Failed);
            }

            return DMEEditor.ErrorObject;
        }

        [CommandAttribute(Caption = "Clear Data", iconimage = "clear.png", ObjectType = "Beep")]
        public IErrorsInfo ClearTableData()
        {
            try
            {
                if (DataSource != null && !string.IsNullOrEmpty(BranchText))
                {
                    // Execute DELETE statement to clear all data
                    var sql = $"DELETE FROM {BranchText}";
                    if (DataSource is IRDBSource rdbSource)
                    {
                        rdbSource.ExecuteSql(sql);
                        DMEEditor.AddLogMessage("Success", $"Cleared all data from table: {BranchText}", 
                                               DateTime.Now, 0, null, Errors.Ok);
                    }
                    else
                    {
                        DMEEditor.AddLogMessage("Warning", $"Clear data not supported for this data source type", 
                                               DateTime.Now, 0, null, Errors.Ok);
                    }
                }
            }
            catch (Exception ex)
            {
                DMEEditor.AddLogMessage("Error", $"Failed to clear table data: {ex.Message}", 
                                       DateTime.Now, -1, null, Errors.Failed);
            }

            return DMEEditor.ErrorObject;
        }

        [CommandAttribute(Caption = "Drop Table", iconimage = "droptable.png", ObjectType = "Beep")]
        public IErrorsInfo DropTable()
        {
            try
            {
                if (DataSource != null && !string.IsNullOrEmpty(BranchText))
                {
                    // Execute DROP TABLE statement
                    var sql = $"DROP TABLE {BranchText}";
                    if (DataSource is IRDBSource rdbSource)
                    {
                        rdbSource.ExecuteSql(sql);
                        
                        // Remove this node from the tree
                        if (ParentBranch != null)
                        {
                            TreeEditor.Treebranchhandler.RemoveBranch(this);
                        }
                        
                        DMEEditor.AddLogMessage("Success", $"Dropped table: {BranchText}", 
                                               DateTime.Now, 0, null, Errors.Ok);
                    }
                    else
                    {
                        DMEEditor.AddLogMessage("Warning", $"Drop table not supported for this data source type", 
                                               DateTime.Now, 0, null, Errors.Ok);
                    }
                }
            }
            catch (Exception ex)
            {
                DMEEditor.AddLogMessage("Error", $"Failed to drop table: {ex.Message}", 
                                       DateTime.Now, -1, null, Errors.Failed);
            }

            return DMEEditor.ErrorObject;
        }

        [CommandAttribute(Caption = "Get Structure", iconimage = "structure.png", ObjectType = "Beep")]
        public IErrorsInfo GetTableStructure()
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
                                               $"Table {BranchText} has {fieldCount} fields", 
                                               DateTime.Now, 0, null, Errors.Ok);
                        
                        // Optionally log field details
                        if (EntityStructure.Fields != null)
                        {
                            foreach (var field in EntityStructure.Fields.Take(5)) // Show first 5 fields
                            {
                                DMEEditor.AddLogMessage("Info", 
                                                       $"  Field: {field.FieldName} ({field.Fieldtype})", 
                                                       DateTime.Now, 0, null, Errors.Ok);
                            }
                        }
                    }
                    else
                    {
                        DMEEditor.AddLogMessage("Warning", $"Could not get structure for table: {BranchText}", 
                                               DateTime.Now, 0, null, Errors.Ok);
                    }
                }
            }
            catch (Exception ex)
            {
                DMEEditor.AddLogMessage("Error", $"Failed to get table structure: {ex.Message}", 
                                       DateTime.Now, -1, null, Errors.Failed);
            }

            return DMEEditor.ErrorObject;
        }
        #endregion
    }
}