using System;
using System.Collections.Generic;
using TheTechIdea.Beep.Addin;
using TheTechIdea.Beep.ConfigUtil;
using TheTechIdea.Beep.DataBase;
using TheTechIdea.Beep.DataView;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Utilities;
using TheTechIdea.Beep.Vis;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.TreeNodes.DataViews
{
    [AddinAttribute(Caption = "Data View Entity", BranchType = EnumPointType.Entity, Name = "DataViewEntityNode.Beep", misc = "Beep", iconimage = "entity.png", menu = "Beep", ObjectType = "Beep")]
    public class DataViewEntityNode : IBranch
    {
        public DataViewEntityNode() { }
        public DataViewEntityNode(ITree pTreeEditor, IDMEEditor pDMEEditor, IBranch pParentNode, string pBranchText, int pID, EnumPointType pBranchType, string pimagename, string dataViewName)
        {
            TreeEditor = pTreeEditor;
            DMEEditor = pDMEEditor;
            ParentBranchID = pParentNode != null ? pParentNode.ID : -1;
            BranchText = pBranchText;
            BranchType = pBranchType;
            IconImageName = pimagename;
            DataSourceName = dataViewName;
            
            // Get the DataSource
            ds = (DataViewDataSource)DMEEditor.GetDataSource(DataSourceName);

            if (pID != 0) { ID = pID; BranchID = pID; }
        }

        public string MenuID { get; set; }
        public bool Visible { get; set; } = true;
        public bool IsDataSourceNode { get; set; } = true;
        public string GuidID { get; set; } = Guid.NewGuid().ToString();
        public string ParentGuidID { get; set; }
        public string DataSourceConnectionGuidID { get; set; }
        public string EntityGuidID { get; set; }
        public string MiscStringID { get; set; }
        public IBranch ParentBranch { get; set; }
        public string ObjectType { get; set; } = "Beep";
        public int ID { get; set; }
        public EntityStructure EntityStructure { get; set; } = new EntityStructure();
        public string Name { get; set; }
        public string BranchText { get; set; } = "Entity";
        public IDMEEditor DMEEditor { get; set; }
        public IDataSource DataSource { get; set; }
        public string DataSourceName { get; set; }
        public int Level { get; set; }
        public EnumPointType BranchType { get; set; } = EnumPointType.Entity;
        public int BranchID { get; set; }
        public string IconImageName { get; set; } = "entity.png";
        public string BranchStatus { get; set; }
        public int ParentBranchID { get; set; }
        public string BranchDescription { get; set; }
        public string BranchClass { get; set; } = "DATAVIEW";
        public List<IBranch> ChildBranchs { get; set; } = new List<IBranch>();
        public ITree TreeEditor { get; set; }
        public List<string> BranchActions { get; set; } = new List<string>();
        public object TreeStrucure { get; set; }
        public IAppManager Visutil { get; set; }
        public int MiscID { get; set; }

        private DataViewDataSource ds;

        public IBranch CreateCategoryNode(CategoryFolder p)
        {
            throw new NotImplementedException();
        }

        public IErrorsInfo CreateChildNodes()
        {
            try
            {
                // If this is a JoinLeaf, maybe show Left/Right entities as children
                if (BranchType == EnumPointType.JoinLeaf && ds != null)
                {
                    // For now, no recursive expansion of joins to avoid cycles
                    // But we could show the joined columns here
                }

                DMEEditor.AddLogMessage("Success", "Created DataView entity node", DateTime.Now, 0, null, Errors.Ok);
            }
            catch (Exception ex)
            {
                DMEEditor.AddLogMessage(ex.Message, "Could not create DataView entity node", DateTime.Now, -1, null, Errors.Failed);
            }
            return DMEEditor.ErrorObject;
        }

        public IErrorsInfo ExecuteBranchAction(string ActionName) => DMEEditor.ErrorObject;
        public IErrorsInfo MenuItemClicked(string ActionNam) => DMEEditor.ErrorObject;
        public IErrorsInfo RemoveChildNodes() => DMEEditor.ErrorObject;

        [CommandAttribute(Caption = "Preview Data", iconimage = "preview.png", ObjectType = "Beep")]
        public IErrorsInfo PreviewData()
        {
            try
            {
                if (ds == null) ds = (DataViewDataSource)DMEEditor.GetDataSource(DataSourceName);
                if (ds != null)
                {
                    ds.Openconnection();
                    var data = ds.GetEntityPreview(BranchText, 50);
                    var ob = new List<ObjectItem>
                    {
                        new ObjectItem { Name = "DataSource", obj = data },
                        new ObjectItem { Name = "TitleText", obj = $"Preview — {BranchText}" }
                    };
                    var args = new PassedArgs
                    {
                        DMView = ds.DataView, CurrentEntity = BranchText,
                        ObjectType = "PREVIEWENTITY", EventType = "PREVIEWENTITY",
                        DatasourceName = DataSourceName, Objects = ob
                    };
                    Visutil.ShowPage("uc_DataViewer", args);
                }
            }
            catch (Exception ex)
            {
                DMEEditor.AddLogMessage(ex.Message, "PreviewData failed.", DateTime.Now, -1, null, Errors.Failed);
            }
            return DMEEditor.ErrorObject;
        }

        [CommandAttribute(Caption = "Set Filter", iconimage = "filter.png", ObjectType = "Beep")]
        public IErrorsInfo SetFilter()
        {
            try
            {
                if (ds != null)
                {
                    string current = ds.GetEntityFilter(BranchText) ?? string.Empty;
                    DialogReturn input = Visutil.DialogManager.InputBoxAsync("Set Filter",
                        $"WHERE clause for '{BranchText}' (current: {current}):\n(leave empty to clear)").GetAwaiter().GetResult();
                    if (input.Result != BeepDialogResult.OK) return DMEEditor.ErrorObject;
                    string expr = input.Value;

                    if (string.IsNullOrWhiteSpace(expr))
                        ds.ClearEntityFilter(BranchText);
                    else
                        ds.SetEntityFilter(BranchText, expr);
                    
                    ds.WriteDataViewFile(DataSourceName);
                }
            }
            catch (Exception ex)
            {
                DMEEditor.AddLogMessage(ex.Message, "SetFilter failed.", DateTime.Now, -1, null, Errors.Failed);
            }
            return DMEEditor.ErrorObject;
        }

        [CommandAttribute(Caption = "Refresh Schema", iconimage = "refresh.png", ObjectType = "Beep")]
        public IErrorsInfo RefreshSchema()
        {
            try
            {
                if (ds != null)
                {
                    var changes = ds.RefreshEntitySchema(BranchText);
                    Visutil.DialogManager.MsgBoxAsync("Refresh Schema",
                        changes == null || changes.Count == 0
                            ? "\u2714 Schema is up to date."
                            : string.Join(Environment.NewLine, changes)).GetAwaiter().GetResult();
                }
            }
            catch (Exception ex)
            {
                DMEEditor.AddLogMessage(ex.Message, "RefreshSchema failed.", DateTime.Now, -1, null, Errors.Failed);
            }
            return DMEEditor.ErrorObject;
        }

        public IErrorsInfo SetConfig(ITree pTreeEditor, IDMEEditor pDMEEditor, IBranch pParentNode, string pBranchText, int pID, EnumPointType pBranchType, string pimagename)
        {
            try
            {
                TreeEditor = pTreeEditor;
                DMEEditor = pDMEEditor;
                ParentBranchID = pParentNode != null ? pParentNode.ID : -1;
                BranchText = pBranchText;
                BranchType = pBranchType;
                IconImageName = pimagename;
                if (pID != 0) ID = pID;
                ds = (DataViewDataSource)DMEEditor.GetDataSource(DataSourceName);
            }
            catch (Exception ex)
            {
                DMEEditor.AddLogMessage(ex.Message, "Could not Set Config", DateTime.Now, -1, null, Errors.Failed);
            }
            return DMEEditor.ErrorObject;
        }
    }
}
